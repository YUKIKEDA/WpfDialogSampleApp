using CommunityToolkit.Mvvm.ComponentModel;
using WpfDialogSampleApp.Services;

namespace WpfDialogSampleApp.ViewModels
{
    /// <summary>
    /// ダイアログ用ViewModelの基底クラス
    /// </summary>
    public abstract partial class DialogViewModelBase : ObservableObject, IDialogAware, IDialogResult
    {
        [ObservableProperty]
        private string _title = "ダイアログ";

        [ObservableProperty]
        private bool _isDialogOpen;

        private IDialogService? _dialogService;
        private Action<bool?>? _closeAction;

        private bool? _dialogResult;

        /// <summary>
        /// ダイアログの結果
        /// </summary>
        public bool? DialogResult 
        { 
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        protected DialogViewModelBase(string title = "ダイアログ")
        {
            Title = title;
        }

        // IDialogAware implementation
        public virtual void SetDialogService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public virtual void SetCloseAction(Action<bool?> closeAction)
        {
            _closeAction = closeAction;
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        /// <param name="result">ダイアログの結果</param>
        protected void CloseDialog(bool? result = null)
        {
            _closeAction?.Invoke(result);
        }

        /// <summary>
        /// メッセージボックスを表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageType">メッセージの種類</param>
        /// <returns>ユーザーの選択結果</returns>
        protected MessageBoxResult ShowMessageBox(string message, string title = "メッセージ", MessageBoxType messageType = MessageBoxType.Information)
        {
            return _dialogService?.ShowMessageBox(message, title, messageType) ?? MessageBoxResult.None;
        }

        /// <summary>
        /// 確認ダイアログを表示
        /// </summary>
        /// <param name="message">確認メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>ユーザーの選択結果（Yes/No）</returns>
        protected bool ShowConfirmation(string message, string title = "確認")
        {
            return _dialogService?.ShowConfirmation(message, title) ?? false;
        }
    }
}
