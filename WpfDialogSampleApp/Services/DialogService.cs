using System.Windows;
using WpfDialogSampleApp.ViewModels;
using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp.Services
{
    /// <summary>
    /// ダイアログ表示サービスの実装
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> _dialogMappings = new();
        private readonly Dictionary<object, DialogHandle> _openDialogs = new();
        private readonly ServiceContainer? _serviceContainer;

        public DialogService(ServiceContainer? serviceContainer = null)
        {
            _serviceContainer = serviceContainer;
        }

        /// <summary>
        /// ViewModelとViewの対応を登録
        /// </summary>
        public void RegisterDialog<TViewModel, TView>()
            where TViewModel : class
            where TView : DialogBase
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
        /// 非同期でモーダルダイアログを表示
        /// </summary>
        public async Task<bool?> ShowModalAsync<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            return await Task.Run(() => ShowModal(viewModel));
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

        private TViewModel CreateViewModel<TViewModel>() where TViewModel : class, new()
        {
            // DIコンテナーが利用可能な場合は使用
            return _serviceContainer?.TryGetService<TViewModel>() ?? new TViewModel();
        }

        private DialogBase? CreateDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            if (!_dialogMappings.TryGetValue(typeof(TViewModel), out var dialogType))
            {
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} に対応するDialogが登録されていません。");
            }

            // DIコンテナーが利用可能な場合は使用を試行
            if (_serviceContainer != null)
            {
                var dialogFromDI = _serviceContainer.TryGetService(dialogType) as DialogBase;
                if (dialogFromDI != null) return dialogFromDI;
            }

            return Activator.CreateInstance(dialogType) as DialogBase;
        }

        private void SetupDialog<TViewModel>(DialogBase dialog, TViewModel viewModel) where TViewModel : class
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
                    dialog.CloseDialog(result);
                });
            }
        }

        #region Helper Methods

        private static MessageBoxButton ConvertMessageBoxType(MessageBoxType messageType)
        {
            return messageType switch
            {
                MessageBoxType.Question => MessageBoxButton.YesNo,
                _ => MessageBoxButton.OK
            };
        }

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
        private readonly DialogBase _dialog;
        private bool _isActive = true;

        public object ViewModel { get; }
        public bool IsActive => _isActive && !_dialog.IsClosed;

        public event EventHandler<DialogClosedEventArgs>? Closed;

        public DialogHandle(object viewModel, DialogBase dialog)
        {
            ViewModel = viewModel;
            _dialog = dialog;

            // ダイアログが閉じられたときのイベントを監視
            _dialog.Closed += OnDialogClosed;
        }

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
                _dialog.CloseDialog(result);
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