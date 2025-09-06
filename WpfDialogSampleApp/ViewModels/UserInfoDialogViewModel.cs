using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfDialogSampleApp.Models;
using WpfDialogSampleApp.Services;

namespace WpfDialogSampleApp.ViewModels
{
    /// <summary>
    /// ユーザー情報入力ダイアログのViewModelクラス
    /// ユーザー情報の入力、検証、保存機能を提供します
    /// </summary>
    public partial class UserInfoDialogViewModel : DialogViewModelBase
    {
        /// <summary>
        /// ユーザー情報のインスタンスを取得または設定します
        /// </summary>
        [ObservableProperty]
        private UserInfo _userInfo = new();

        /// <summary>
        /// 入力内容が有効かどうかを示す値を取得または設定します
        /// </summary>
        [ObservableProperty]
        private bool _isValid;

        /// <summary>
        /// UserInfoDialogViewModelの新しいインスタンスを初期化します
        /// </summary>
        public UserInfoDialogViewModel() : base("ユーザー情報入力")
        {
            InitializeViewModel();
        }

        /// <summary>
        /// タイトルを指定してUserInfoDialogViewModelの新しいインスタンスを初期化します
        /// DI対応のためのコンストラクターです
        /// </summary>
        /// <param name="title">ダイアログのタイトル</param>
        public UserInfoDialogViewModel(string title) : base(title)
        {
            InitializeViewModel();
        }

        /// <summary>
        /// ViewModelの初期化処理を実行します
        /// </summary>
        private void InitializeViewModel()
        {
            UserInfo.PropertyChanged += OnUserInfoPropertyChanged;
            ValidateInput();
        }

        /// <summary>
        /// UserInfoプロパティが変更された時の処理
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">プロパティ変更イベント引数</param>
        private void OnUserInfoPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ValidateInput();
        }

        /// <summary>
        /// 入力内容の検証を実行します
        /// </summary>
        private void ValidateInput()
        {
            IsValid = !string.IsNullOrWhiteSpace(UserInfo.Name) &&
                     !string.IsNullOrWhiteSpace(UserInfo.Email) &&
                     UserInfo.Age > 0;
        }

        /// <summary>
        /// ユーザー情報を保存してダイアログを閉じます
        /// </summary>
        [RelayCommand]
        private void Save()
        {
            if (IsValid)
            {
                // ダイアログを閉じる（保存成功）
                CloseDialog(true);
            }
        }

        /// <summary>
        /// ダイアログをキャンセルして閉じます
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            // ダイアログを閉じる（キャンセル）
            CloseDialog(false);
        }

        /// <summary>
        /// 入力内容をリセットします
        /// </summary>
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
