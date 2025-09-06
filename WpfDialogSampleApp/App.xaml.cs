using System.Windows;
using WpfDialogSampleApp.Services;
using WpfDialogSampleApp.ViewModels;
using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IDialogService? _dialogService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // シンプルなサービス設定
            _dialogService = new DialogService();
            _dialogService.RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();

            // メインウィンドウの作成
            var mainWindow = new MainWindow();
            var mainViewModel = new MainWindowViewModel(_dialogService);
            mainWindow.DataContext = mainViewModel;

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // リソースのクリーンアップ
            _dialogService?.CloseAllDialogs();
            base.OnExit(e);
        }

        /// <summary>
        /// テスト用途でのダイアログサービス取得
        /// </summary>
        public static IDialogService? GetDialogService() => _dialogService;
    }
}
