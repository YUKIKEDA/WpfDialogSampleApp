using System.Windows;
using WpfDialogSampleApp.Behaviors;

namespace WpfDialogSampleApp.Services
{
    /// <summary>
    /// ダイアログ表示サービスの実装
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// ViewModelとViewのマッピングを保持する辞書
        /// </summary>
        private readonly Dictionary<Type, Type> _dialogMappings = new();
        
        /// <summary>
        /// 開いているダイアログの管理用辞書
        /// </summary>
        private readonly Dictionary<object, DialogHandle> _openDialogs = new();
        
        /// <summary>
        /// DialogServiceの新しいインスタンスを初期化します
        /// </summary>
        public DialogService()
        {
        }

        /// <summary>
        /// ViewModelとViewの対応を登録
        /// </summary>
        public void RegisterDialog<TViewModel, TView>()
            where TViewModel : class
            where TView : Window, new()
        {
            _dialogMappings[typeof(TViewModel)] = typeof(TView);
        }

        /// <summary>
        /// モーダルダイアログを表示
        /// </summary>
        public bool? ShowModal<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var dialog = CreateDialog(viewModel);
            if (dialog == null) return null;

            SetupDialog(dialog, viewModel);
            return dialog.ShowDialog();
        }

        /// <summary>
        /// 非モーダルダイアログを表示
        /// </summary>
        public IDialogHandle Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            // 既に同じViewModelでダイアログが開いている場合は既存のハンドルを返す
            if (_openDialogs.TryGetValue(viewModel, out var existingHandle) && existingHandle.IsActive)
            {
                return existingHandle;
            }

            var dialog = CreateDialog(viewModel);
            if (dialog == null) 
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} に対応するDialogが作成できませんでした。");

            SetupDialog(dialog, viewModel);
            
            var handle = new DialogHandle(viewModel, dialog);
            _openDialogs[viewModel] = handle;

            // ダイアログが閉じられたときにクリーンアップ
            handle.Closed += (s, e) => _openDialogs.Remove(viewModel);
            
            dialog.Show();
            return handle;
        }


        /// <summary>
        /// 型安全なモーダルダイアログファクトリー
        /// </summary>
        public bool? ShowModal<TViewModel>(Action<TViewModel>? configure = null) 
            where TViewModel : class, new()
        {
            var viewModel = CreateViewModel<TViewModel>();
            configure?.Invoke(viewModel);
            return ShowModal(viewModel);
        }

        /// <summary>
        /// 型安全な非モーダルダイアログファクトリー
        /// </summary>
        public IDialogHandle Show<TViewModel>(Action<TViewModel>? configure = null) 
            where TViewModel : class, new()
        {
            var viewModel = CreateViewModel<TViewModel>();
            configure?.Invoke(viewModel);
            return Show(viewModel);
        }

        /// <summary>
        /// メッセージボックスを表示
        /// </summary>
        public MessageBoxResult ShowMessageBox(string message, string title = "メッセージ", MessageBoxType messageType = MessageBoxType.Information)
        {
            var wpfMessageBoxType = ConvertMessageBoxType(messageType);
            var wpfMessageBoxImage = ConvertMessageBoxImage(messageType);

            var result = System.Windows.MessageBox.Show(
                message, 
                title, 
                wpfMessageBoxType, 
                wpfMessageBoxImage);

            return ConvertMessageBoxResult(result);
        }

        /// <summary>
        /// 確認ダイアログを表示
        /// </summary>
        public bool ShowConfirmation(string message, string title = "確認")
        {
            var result = ShowMessageBox(message, title, MessageBoxType.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// すべてのダイアログを閉じる
        /// </summary>
        public void CloseAllDialogs()
        {
            var handles = _openDialogs.Values.ToList();
            foreach (var handle in handles)
            {
                handle.Close();
            }
            _openDialogs.Clear();
        }

        /// <summary>
        /// 指定された型のViewModelのインスタンスを作成します
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <returns>作成されたViewModelのインスタンス</returns>
        private TViewModel CreateViewModel<TViewModel>() where TViewModel : class, new()
        {
            return new TViewModel();
        }

        /// <summary>
        /// 指定されたViewModelに対応するダイアログウィンドウを作成します
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="viewModel">ViewModelのインスタンス</param>
        /// <returns>作成されたダイアログウィンドウ（またはnull）</returns>
        private Window? CreateDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            if (!_dialogMappings.TryGetValue(typeof(TViewModel), out var dialogType))
            {
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} に対応するDialogが登録されていません。");
            }

            var dialog = Activator.CreateInstance(dialogType) as Window;
            if (dialog != null)
            {
                SetupDialogBehavior(dialog);
            }
            return dialog;
        }

        /// <summary>
        /// ダイアログに必要なビヘイビアを設定します
        /// </summary>
        /// <param name="dialog">設定対象のダイアログ</param>
        private void SetupDialogBehavior(Window dialog)
        {
            // XAMLビヘイビアがアタッチされているかチェック
            var behaviors = Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(dialog);
            var hasDialogBehavior = behaviors.OfType<DialogBehavior>().Any();
            
            if (!hasDialogBehavior)
            {
                // ビヘイビアがない場合は新しく追加
                var behavior = new DialogBehavior();
                behaviors.Add(behavior);
            }
        }

        /// <summary>
        /// ダイアログのDataContextやイベントハンドラーを設定します
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="dialog">設定対象のダイアログ</param>
        /// <param name="viewModel">ViewModelのインスタンス</param>
        private void SetupDialog<TViewModel>(Window dialog, TViewModel viewModel) where TViewModel : class
        {
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;

            // ViewModelにダイアログ操作のインターフェースがある場合は設定
            if (viewModel is IDialogAware dialogAware)
            {
                dialogAware.SetDialogService(this);
                dialogAware.SetCloseAction((result) => {
                    // 結果を保存してからダイアログを閉じる
                    if (viewModel is IDialogResult dialogResult)
                    {
                        dialogResult.DialogResult = result;
                    }
                    
                    // DialogBehaviorがアタッチされているかチェック
                    var behaviors = Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(dialog);
                    var dialogBehavior = behaviors.OfType<DialogBehavior>().FirstOrDefault();
                    
                    if (dialogBehavior != null)
                    {
                        dialogBehavior.CloseDialog(result);
                    }
                    else
                    {
                        // ビヘイビアがない場合は直接閉じる
                        try
                        {
                            if (result.HasValue)
                            {
                                dialog.DialogResult = result;
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // 非モーダルダイアログの場合は無視
                        }
                        dialog.Close();
                    }
                });
            }
        }

        #region Helper Methods

        /// <summary>
        /// カスタムMessageBoxTypeをWPFのMessageBoxButtonに変換します
        /// </summary>
        /// <param name="messageType">変換元のメッセージボックスタイプ</param>
        /// <returns>変換後WPFのMessageBoxButton</returns>
        private static MessageBoxButton ConvertMessageBoxType(MessageBoxType messageType)
        {
            return messageType switch
            {
                MessageBoxType.Question => MessageBoxButton.YesNo,
                _ => MessageBoxButton.OK
            };
        }

        /// <summary>
        /// カスタムMessageBoxTypeをWPFのMessageBoxImageに変換します
        /// </summary>
        /// <param name="messageType">変換元のメッセージボックスタイプ</param>
        /// <returns>変換後WPFのMessageBoxImage</returns>
        private static MessageBoxImage ConvertMessageBoxImage(MessageBoxType messageType)
        {
            return messageType switch
            {
                MessageBoxType.Information => MessageBoxImage.Information,
                MessageBoxType.Warning => MessageBoxImage.Warning,
                MessageBoxType.Error => MessageBoxImage.Error,
                MessageBoxType.Question => MessageBoxImage.Question,
                _ => MessageBoxImage.Information
            };
        }

        /// <summary>
        /// WPFのMessageBoxResultをカスタムMessageBoxResultに変換します
        /// </summary>
        /// <param name="result">変換元のWPF MessageBoxResult</param>
        /// <returns>変換後のカスタムMessageBoxResult</returns>
        private static MessageBoxResult ConvertMessageBoxResult(System.Windows.MessageBoxResult result)
        {
            return result switch
            {
                System.Windows.MessageBoxResult.OK => MessageBoxResult.OK,
                System.Windows.MessageBoxResult.Cancel => MessageBoxResult.Cancel,
                System.Windows.MessageBoxResult.Yes => MessageBoxResult.Yes,
                System.Windows.MessageBoxResult.No => MessageBoxResult.No,
                _ => MessageBoxResult.None
            };
        }

        #endregion
    }

    /// <summary>
    /// ダイアログハンドルの実装
    /// </summary>
    internal class DialogHandle : IDialogHandle
    {
        /// <summary>
        /// 管理対象のダイアログウィンドウ
        /// </summary>
        private readonly Window _dialog;
        
        /// <summary>
        /// ダイアログがアクティブかどうかの内部状態
        /// </summary>
        private bool _isActive = true;

        /// <summary>
        /// ダイアログのViewModelを取得します
        /// </summary>
        public object ViewModel { get; }
        public bool IsActive 
        { 
            get
            {
                if (!_isActive || _dialog == null) return false;
                
                // DialogBehaviorがアタッチされているかチェック
                var behaviors = Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(_dialog);
                var dialogBehavior = behaviors.OfType<DialogBehavior>().FirstOrDefault();
                
                if (dialogBehavior != null)
                {
                    return !dialogBehavior.IsClosed;
                }
                
                // ビヘイビアがない場合は通常のロジック
                return _dialog.IsLoaded;
            }
        }

        public event EventHandler<DialogClosedEventArgs>? Closed;

        /// <summary>
        /// DialogHandleの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="viewModel">ViewModelのインスタンス</param>
        /// <param name="dialog">ダイアログウィンドウ</param>
        public DialogHandle(object viewModel, Window dialog)
        {
            ViewModel = viewModel;
            _dialog = dialog;

            // ダイアログが閉じられたときのイベントを監視
            _dialog.Closed += OnDialogClosed;
        }

        /// <summary>
        /// ダイアログが閉じられた時のイベントハンドラー
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント引数</param>
        private void OnDialogClosed(object? sender, EventArgs e)
        {
            _isActive = false;
            var result = _dialog.DialogResult;
            
            // ViewModelがIDialogResultを実装している場合、その結果を使用
            if (ViewModel is IDialogResult dialogResult)
            {
                result = dialogResult.DialogResult;
            }

            Closed?.Invoke(this, new DialogClosedEventArgs(result, ViewModel));
        }

        public void Close(bool? result = null)
        {
            if (IsActive)
            {
                // DialogBehaviorがアタッチされているかチェック
                var behaviors = Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(_dialog);
                var dialogBehavior = behaviors.OfType<DialogBehavior>().FirstOrDefault();
                
                if (dialogBehavior != null)
                {
                    dialogBehavior.CloseDialog(result);
                }
                else
                {
                    // ビヘイビアがない場合は直接閉じる
                    try
                    {
                        if (result.HasValue)
                        {
                            _dialog.DialogResult = result;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // 非モーダルダイアログの場合は無視
                    }
                    _dialog.Close();
                }
            }
        }
    }

    /// <summary>
    /// ダイアログ操作を可能にするインターフェース
    /// </summary>
    public interface IDialogAware
    {
        /// <summary>
        /// ダイアログサービスを設定
        /// </summary>
        /// <param name="dialogService">ダイアログサービス</param>
        void SetDialogService(IDialogService dialogService);

        /// <summary>
        /// ダイアログを閉じるアクションを設定
        /// </summary>
        /// <param name="closeAction">閉じるアクション</param>
        void SetCloseAction(Action<bool?> closeAction);
    }

    /// <summary>
    /// ダイアログの結果を保持するインターフェース
    /// </summary>
    public interface IDialogResult
    {
        /// <summary>
        /// ダイアログの結果
        /// </summary>
        bool? DialogResult { get; set; }
    }

    /// <summary>
    /// メッセージボックスの種類
    /// </summary>
    public enum MessageBoxType
    {
        Information,
        Warning,
        Error,
        Question
    }

    /// <summary>
    /// メッセージボックスの結果
    /// </summary>
    public enum MessageBoxResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }
}