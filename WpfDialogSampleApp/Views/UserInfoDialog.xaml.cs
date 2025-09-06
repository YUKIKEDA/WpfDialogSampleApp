using System.ComponentModel;
using System.Windows;
using WpfDialogSampleApp.ViewModels;

namespace WpfDialogSampleApp.Views
{
    /// <summary>
    /// UserInfoDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class UserInfoDialog : Window
    {
        public UserInfoDialog()
        {
            InitializeComponent();
            Loaded += UserInfoDialog_Loaded;
        }

        private void UserInfoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // 親ウィンドウの位置変更を監視
            if (Owner != null)
            {
                Owner.LocationChanged += Owner_LocationChanged;
                Owner.SizeChanged += Owner_SizeChanged;
            }
        }

        private void Owner_LocationChanged(object? sender, EventArgs e)
        {
            // 親ウィンドウが移動したときにダイアログを中央に再配置
            CenterToOwner();
        }

        private void Owner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 親ウィンドウのサイズが変更されたときにダイアログを中央に再配置
            CenterToOwner();
        }

        private void CenterToOwner()
        {
            if (Owner != null)
            {
                Left = Owner.Left + (Owner.Width - Width) / 2;
                Top = Owner.Top + (Owner.Height - Height) / 2;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // イベントハンドラーを解除
            if (Owner != null)
            {
                Owner.LocationChanged -= Owner_LocationChanged;
                Owner.SizeChanged -= Owner_SizeChanged;
            }
            base.OnClosing(e);
        }
    }
}
