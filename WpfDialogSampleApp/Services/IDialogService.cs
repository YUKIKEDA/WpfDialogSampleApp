using System.Windows;

namespace WpfDialogSampleApp.Services
{
    /// <summary>
    /// ダイアログ表示を抽象化するサービスインターフェース
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// ViewModelとViewの対応を登録
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <typeparam name="TView">Viewの型</typeparam>
        void RegisterDialog<TViewModel, TView>()
            where TViewModel : class
            where TView : Window, new();

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
        /// <returns>ダイアログのライフサイクル管理オブジェクト</returns>
        IDialogHandle Show<TViewModel>(TViewModel viewModel) where TViewModel : class;


        /// <summary>
        /// 型安全なダイアログファクトリー
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="configure">ViewModelの設定アクション</param>
        /// <returns>ダイアログの結果</returns>
        bool? ShowModal<TViewModel>(Action<TViewModel>? configure = null) 
            where TViewModel : class, new();

        /// <summary>
        /// 型安全な非モーダルダイアログファクトリー
        /// </summary>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <param name="configure">ViewModelの設定アクション</param>
        /// <returns>ダイアログのライフサイクル管理オブジェクト</returns>
        IDialogHandle Show<TViewModel>(Action<TViewModel>? configure = null) 
            where TViewModel : class, new();

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

        /// <summary>
        /// すべてのダイアログを閉じる
        /// </summary>
        void CloseAllDialogs();
    }

    /// <summary>
    /// ダイアログのライフサイクルを管理するインターフェース
    /// </summary>
    public interface IDialogHandle
    {
        /// <summary>
        /// ダイアログのViewModel
        /// </summary>
        object ViewModel { get; }

        /// <summary>
        /// ダイアログが閉じられた時のイベント
        /// </summary>
        event EventHandler<DialogClosedEventArgs> Closed;

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        /// <param name="result">ダイアログの結果</param>
        void Close(bool? result = null);

        /// <summary>
        /// ダイアログがアクティブかどうか
        /// </summary>
        bool IsActive { get; }
    }

    /// <summary>
    /// ダイアログが閉じられた時のイベント引数
    /// </summary>
    public class DialogClosedEventArgs : EventArgs
    {
        /// <summary>
        /// ダイアログの結果を取得します
        /// </summary>
        public bool? Result { get; }
        
        /// <summary>
        /// ダイアログのViewModelを取得します
        /// </summary>
        public object ViewModel { get; }

        /// <summary>
        /// DialogClosedEventArgsの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="result">ダイアログの結果</param>
        /// <param name="viewModel">ダイアログのViewModel</param>
        public DialogClosedEventArgs(bool? result, object viewModel)
        {
            Result = result;
            ViewModel = viewModel;
        }
    }

}
