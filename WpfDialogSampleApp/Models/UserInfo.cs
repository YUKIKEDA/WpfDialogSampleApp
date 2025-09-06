using System.ComponentModel;

namespace WpfDialogSampleApp.Models
{
    /// <summary>
    /// ユーザー情報を表すモデルクラス
    /// INotifyPropertyChangedを実装してプロパティ変更通知を提供します
    /// </summary>
    public class UserInfo : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _email = string.Empty;
        private int _age;

        /// <summary>
        /// ユーザーの名前を取得または設定します
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// ユーザーのメールアドレスを取得または設定します
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        /// <summary>
        /// ユーザーの年齢を取得または設定します
        /// </summary>
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        /// <summary>
        /// プロパティ値が変更されたときに発生するイベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティ変更通知を発生させます
        /// </summary>
        /// <param name="propertyName">変更されたプロパティの名前</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
