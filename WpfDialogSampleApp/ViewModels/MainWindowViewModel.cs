using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfDialogSampleApp.Services;
using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _welcomeMessage = "WPF MVVM ダイアログサンプルアプリケーションへようこそ！";

        [ObservableProperty]
        private string _userInfoDisplay = "ユーザー情報がまだ入力されていません。";

        private readonly IDialogService _dialogService;

        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        // デザイナー用デフォルトコンストラクター
        public MainWindowViewModel() : this(CreateDefaultDialogService())
        {
        }

        private static IDialogService CreateDefaultDialogService()
        {
            var dialogService = new DialogService();
            dialogService.RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();
            return dialogService;
        }

        [RelayCommand]
        private void ShowUserInfoDialog()
        {
            // 改善された型安全なダイアログ表示
            var handle = _dialogService.Show<UserInfoDialogViewModel>();
            
            // ダイアログが閉じられた時の処理（イベントベース）
            handle.Closed += OnUserInfoDialogClosed;
        }

        private void OnUserInfoDialogClosed(object? sender, DialogClosedEventArgs e)
        {
            if (e.Result == true && e.ViewModel is UserInfoDialogViewModel viewModel && viewModel.IsValid)
            {
                UserInfoDisplay = $"名前: {viewModel.UserInfo.Name}, メール: {viewModel.UserInfo.Email}, 年齢: {viewModel.UserInfo.Age}";
            }
        }

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

        [RelayCommand]
        private void ShowMessageDialog()
        {
            _dialogService.ShowMessageBox("これはシンプルなメッセージダイアログです。", "メッセージ", MessageBoxType.Information);
        }

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
