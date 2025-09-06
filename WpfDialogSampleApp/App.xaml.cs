using System.Windows;
using WpfDialogSampleApp.Services;
using WpfDialogSampleApp.ViewModels;
using WpfDialogSampleApp.Views;

namespace WpfDialogSampleApp
{
    /// <summary>
    /// WPFダイアログサンプルアプリケーションのメインアプリケーションクラス
    /// アプリケーションの初期化とサービスの設定を行います
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// アプリケーション全体で使用するダイアログサービス
        /// </summary>
        private static IDialogService? _dialogService;

        /// <summary>
        /// アプリケーションの開始時に呼び出されるメソッド
        /// </summary>
        /// <param name="e">スタートアップイベント引数</param>
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

        /// <summary>
        /// アプリケーションの終了時に呼び出されるメソッド
        /// </summary>
        /// <param name="e">終了イベント引数</param>
        protected override void OnExit(ExitEventArgs e)
        {
            // リソースのクリーンアップ
            _dialogService?.CloseAllDialogs();
            base.OnExit(e);
        }

        /// <summary>
        /// テスト用途でのダイアログサービス取得メソッド
        /// </summary>
        /// <returns>ダイアログサービスのインスタンス（またはnull）</returns>
        public static IDialogService? GetDialogService() => _dialogService;
    }
}
