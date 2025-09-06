using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfDialogSampleApp.Services;
using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp.ViewModels
{
    /// <summary>
    /// メインウィンドウのViewModelクラス
    /// ダイアログ表示機能とユーザー情報管理を提供します
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// ウェルカムメッセージを取得または設定します
        /// </summary>
        [ObservableProperty]
        private string _welcomeMessage = "WPF MVVM ダイアログサンプルアプリケーションへようこそ！";

        /// <summary>
        /// ユーザー情報の表示テキストを取得または設定します
        /// </summary>
        [ObservableProperty]
        private string _userInfoDisplay = "ユーザー情報がまだ入力されていません。";

        /// <summary>
        /// ダイアログサービスのインスタンス
        /// </summary>
        private readonly IDialogService _dialogService;

        /// <summary>
        /// MainWindowViewModelの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="dialogService">ダイアログサービス</param>
        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        /// <summary>
        /// デザイナー用のデフォルトコンストラクター
        /// </summary>
        public MainWindowViewModel() : this(CreateDefaultDialogService())
        {
        }

        /// <summary>
        /// デフォルトのダイアログサービスを作成します
        /// </summary>
        /// <returns>設定済みのダイアログサービス</returns>
        private static IDialogService CreateDefaultDialogService()
        {
            var dialogService = new DialogService();
            dialogService.RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();
            return dialogService;
        }

        /// <summary>
        /// ユーザー情報ダイアログを非モーダルで表示します
        /// </summary>
        [RelayCommand]
        private void ShowUserInfoDialog()
        {
            // 改善された型安全なダイアログ表示
            var handle = _dialogService.Show<UserInfoDialogViewModel>();
            
            // ダイアログが閉じられた時の処理（イベントベース）
            handle.Closed += OnUserInfoDialogClosed;
        }

        /// <summary>
        /// ユーザー情報ダイアログが閉じられた時の処理
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">ダイアログクローズイベント引数</param>
        private void OnUserInfoDialogClosed(object? sender, DialogClosedEventArgs e)
        {
            if (e.Result == true && e.ViewModel is UserInfoDialogViewModel viewModel && viewModel.IsValid)
            {
                UserInfoDisplay = $"名前: {viewModel.UserInfo.Name}, メール: {viewModel.UserInfo.Email}, 年齢: {viewModel.UserInfo.Age}";
            }
        }

        /// <summary>
        /// ユーザー情報ダイアログをモーダルで表示します
        /// </summary>
        [RelayCommand]
        private void ShowUserInfoDialogModal()
        {
            // シンプルなモーダルダイアログ
            var viewModel = new UserInfoDialogViewModel();
            viewModel.UserInfo.Name = "モーダル"; // 初期設定
            
            var result = _dialogService.ShowModal(viewModel);

            if (result == true && viewModel.IsValid)
            {
                UserInfoDisplay = $"[モーダル] 名前: {viewModel.UserInfo.Name}, メール: {viewModel.UserInfo.Email}, 年齢: {viewModel.UserInfo.Age}";
            }
        }

        /// <summary>
        /// シンプルなメッセージダイアログを表示します
        /// </summary>
        [RelayCommand]
        private void ShowMessageDialog()
        {
            _dialogService.ShowMessageBox("これはシンプルなメッセージダイアログです。", "メッセージ", MessageBoxType.Information);
        }

        /// <summary>
        /// 確認ダイアログを表示します
        /// </summary>
        [RelayCommand]
        private void ShowConfirmationDialog()
        {
            if (_dialogService.ShowConfirmation("この操作を実行しますか？", "確認"))
            {
                _dialogService.ShowMessageBox("操作が実行されました。", "結果", MessageBoxType.Information);
            }
        }
    }
}
