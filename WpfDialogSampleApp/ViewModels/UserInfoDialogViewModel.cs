using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfDialogSampleApp.Models;
using WpfDialogSampleApp.Services;

namespace WpfDialogSampleApp.ViewModels
{
    public partial class UserInfoDialogViewModel : DialogViewModelBase
    {
        [ObservableProperty]
        private UserInfo _userInfo = new();

        [ObservableProperty]
        private bool _isValid;

        public UserInfoDialogViewModel() : base("ユーザー情報入力")
        {
            InitializeViewModel();
        }

        // DI対応の追加コンストラクター（将来の拡張用）
        public UserInfoDialogViewModel(string title) : base(title)
        {
            InitializeViewModel();
        }

        private void InitializeViewModel()
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
                // ダイアログを閉じる（保存成功）
                CloseDialog(true);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            // ダイアログを閉じる（キャンセル）
            CloseDialog(false);
        }

        [RelayCommand]
        private void Reset()
        {
            // 確認ダイアログを表示してからリセット
            if (ShowConfirmation("入力した内容をリセットしますか？", "リセット確認"))
            {
                UserInfo.Name = string.Empty;
                UserInfo.Email = string.Empty;
                UserInfo.Age = 0;
                ShowMessageBox("入力内容をリセットしました。", "リセット完了", MessageBoxType.Information);
            }
        }
    }
}
