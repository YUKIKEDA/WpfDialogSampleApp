using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows;
using WpfDialogSampleApp.Models;

namespace WpfDialogSampleApp.ViewModels
{
    public partial class UserInfoDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserInfo _userInfo = new();

        [ObservableProperty]
        private bool _isDialogOpen;

        [ObservableProperty]
        private string _title = "ユーザー情報入力";

        [ObservableProperty]
        private bool _isValid;

        public UserInfoDialogViewModel()
        {
            UserInfo.PropertyChanged += OnUserInfoPropertyChanged;
            ValidateInput();
        }

        private void OnUserInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ValidateInput();
        }

        private void ValidateInput()
        {
            IsValid = !string.IsNullOrWhiteSpace(UserInfo.Name) &&
                     !string.IsNullOrWhiteSpace(UserInfo.Email) &&
                     UserInfo.Age > 0;
        }

        [RelayCommand]
        private void Save()
        {
            if (IsValid)
            {
                IsDialogOpen = false;
                MessageBox.Show($"ユーザー情報が保存されました:\n名前: {UserInfo.Name}\nメール: {UserInfo.Email}\n年齢: {UserInfo.Age}", 
                    "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            IsDialogOpen = false;
        }

        [RelayCommand]
        private void Reset()
        {
            UserInfo.Name = string.Empty;
            UserInfo.Email = string.Empty;
            UserInfo.Age = 0;
        }
    }
}
