using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _welcomeMessage = "WPF MVVM ダイアログサンプルアプリケーションへようこそ！";

        [ObservableProperty]
        private string _userInfoDisplay = "ユーザー情報がまだ入力されていません。";

        private UserInfoDialog? _currentDialog;

        [RelayCommand]
        private void ShowUserInfoDialog()
        {
            // 既にダイアログが開いている場合は何もしない
            if (_currentDialog != null)
                return;

            _currentDialog = new UserInfoDialog();
            var viewModel = new UserInfoDialogViewModel();
            _currentDialog.DataContext = viewModel;
            
            // 親ウィンドウを設定して中央配置を確実にする
            _currentDialog.Owner = Application.Current.MainWindow;
            _currentDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            // ダイアログが閉じられたときの処理
            _currentDialog.Closed += (sender, e) =>
            {
                if (viewModel.IsValid && sender is UserInfoDialog dialog && dialog.DialogResult == true)
                {
                    UserInfoDisplay = $"名前: {viewModel.UserInfo.Name}, メール: {viewModel.UserInfo.Email}, 年齢: {viewModel.UserInfo.Age}";
                }
                _currentDialog = null;
            };

            viewModel.IsDialogOpen = true;
            
            // モーダレス（非モーダル）で表示
            _currentDialog.Show();
        }

        [RelayCommand]
        private void ShowMessageDialog()
        {
            MessageBox.Show("これはシンプルなメッセージダイアログです。", "メッセージ", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ShowConfirmationDialog()
        {
            var result = MessageBox.Show("この操作を実行しますか？", "確認", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("操作が実行されました。", "結果", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
