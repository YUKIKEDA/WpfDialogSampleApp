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
        private readonly Dictionary<object, DialogBase> _openDialogs = new();

        public DialogService()
        {
            // ViewModelとViewの対応を登録
            RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();
        }

        /// <summary>
        /// ViewModelとViewの対応を登録
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <typeparam name="TView">Viewの型</typeparam>
        public void RegisterDialog<TViewModel, TView>()
            where TViewModel : class
            where TView : DialogBase, new()
        {
            _dialogMappings[typeof(TViewModel)] = typeof(TView);
        }

        public bool? ShowModal<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var dialog = CreateDialog(viewModel);
            if (dialog == null) return null;

            SetupDialog(dialog, viewModel);
            return dialog.ShowDialog();
        }

        public void Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            // 既に同じViewModelでダイアログが開いている場合は何もしない
            if (_openDialogs.ContainsKey(viewModel))
                return;

            var dialog = CreateDialog(viewModel);
            if (dialog == null) return;

            SetupDialog(dialog, viewModel);
            
            // ダイアログが閉じられたときに辞書から削除
            dialog.Closed += (s, e) => _openDialogs.Remove(viewModel);
            
            _openDialogs[viewModel] = dialog;
            dialog.Show();
        }

        public async Task<bool?> ShowModalAsync<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            return await Task.Run(() => ShowModal(viewModel));
        }

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

        public bool ShowConfirmation(string message, string title = "確認")
        {
            var result = ShowMessageBox(message, title, MessageBoxType.Question);
            return result == MessageBoxResult.Yes;
        }

        private DialogBase? CreateDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            if (!_dialogMappings.TryGetValue(typeof(TViewModel), out var dialogType))
            {
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} に対応するDialogが登録されていません。");
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
}
