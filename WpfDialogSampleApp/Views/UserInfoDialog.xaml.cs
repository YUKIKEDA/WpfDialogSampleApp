using System.Windows;

namespace WpfDialogSampleApp.Views
{
    /// <summary>
    /// UserInfoDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class UserInfoDialog : Window
    {
        /// <summary>
        /// UserInfoDialogの新しいインスタンスを初期化します
        /// </summary>
        public UserInfoDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ダイアログのビヘイビアを取得します（XAMLで定義されたビヘイビア）
        /// </summary>
        /// <returns>ダイアログビヘイビアのインスタンス</returns>
        public Behaviors.DialogBehavior DialogBehavior => (Behaviors.DialogBehavior)((Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(this))[0]);

        /// <summary>
        /// ダイアログを閉じる（結果付き）
        /// </summary>
        /// <param name="result">ダイアログの結果</param>
        public void CloseDialog(bool? result = null)
        {
            DialogBehavior.CloseDialog(result);
        }
    }
}
