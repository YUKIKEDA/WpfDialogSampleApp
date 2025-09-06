using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Linq;
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
                // ダイアログの結果をtrueに設定して閉じる
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            // ダイアログの結果をfalseに設定して閉じる
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
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
