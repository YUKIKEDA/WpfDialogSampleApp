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

        [RelayCommand]
        private void ShowUserInfoDialog()
        {
            var dialog = new UserInfoDialog();
            var viewModel = new UserInfoDialogViewModel();
            dialog.DataContext = viewModel;
            
            viewModel.IsDialogOpen = true;
            
            if (dialog.ShowDialog() == true)
            {
                // ダイアログが正常に閉じられた場合の処理
                UserInfoDisplay = $"名前: {viewModel.UserInfo.Name}, メール: {viewModel.UserInfo.Email}, 年齢: {viewModel.UserInfo.Age}";
            }
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
