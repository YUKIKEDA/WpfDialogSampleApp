namespace WpfDialogSampleApp.Services
{
    /// <summary>
    /// ダイアログ表示を抽象化するサービスインターフェース
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// モーダルダイアログを表示
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="viewModel">ViewModel インスタンス</param>
        /// <returns>ダイアログの結果</returns>
        bool? ShowModal<TViewModel>(TViewModel viewModel) where TViewModel : class;

        /// <summary>
        /// 非モーダルダイアログを表示
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="viewModel">ViewModel インスタンス</param>
        void Show<TViewModel>(TViewModel viewModel) where TViewModel : class;

        /// <summary>
        /// 非同期でモーダルダイアログを表示
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="viewModel">ViewModel インスタンス</param>
        /// <returns>ダイアログの結果</returns>
        Task<bool?> ShowModalAsync<TViewModel>(TViewModel viewModel) where TViewModel : class;

        /// <summary>
        /// メッセージボックスを表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="messageType">メッセージの種類</param>
        /// <returns>ユーザーの選択結果</returns>
        MessageBoxResult ShowMessageBox(string message, string title = "メッセージ", MessageBoxType messageType = MessageBoxType.Information);

        /// <summary>
        /// 確認ダイアログを表示
        /// </summary>
        /// <param name="message">確認メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>ユーザーの選択結果（Yes/No）</returns>
        bool ShowConfirmation(string message, string title = "確認");
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
