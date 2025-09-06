using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfDialogSampleApp.Services;

namespace WpfDialogSampleApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _welcomeMessage = "WPF MVVM ダイアログサンプルアプリケーションへようこそ！";

        [ObservableProperty]
        private string _userInfoDisplay = "ユーザー情報がまだ入力されていません。";

        private readonly IDialogService _dialogService;

        public MainWindowViewModel()
        {
            _dialogService = new DialogService();
        }

        [RelayCommand]
        private void ShowUserInfoDialog()
        {
            var viewModel = new UserInfoDialogViewModel();
            
            // 非モーダルで表示（既に開いている場合は何もしない）
            _dialogService.Show(viewModel);
            
            // ダイアログが閉じられた時の処理（PropertyChangedでDialogResultを監視）
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.DialogResult) && viewModel.DialogResult == true && viewModel.IsValid)
                {
                    UserInfoDisplay = $"名前: {viewModel.UserInfo.Name}, メール: {viewModel.UserInfo.Email}, 年齢: {viewModel.UserInfo.Age}";
                }
            };
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
