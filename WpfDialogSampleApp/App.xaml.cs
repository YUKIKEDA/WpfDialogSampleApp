using System.Windows;
using WpfDialogSampleApp.Core.Dialogs.Services;
using WpfDialogSampleApp.Core.Dialogs.ViewModels;
using WpfDialogSampleApp.ViewModels;
using WpfDialogSampleApp.Dialogs.UserInfoDialog;
using WpfDialogSampleApp.Dialogs.ConfirmDialog;

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
        /// メインViewModelの参照（Dispose用）
        /// </summary>
        private static MainWindowViewModel? _mainViewModel;

        /// <summary>
        /// アプリケーションの開始時に呼び出されるメソッド
        /// </summary>
        /// <param name="e">スタートアップイベント引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // シンプルなサービス設定
            var dialogViewModel = new DialogViewModel();
            _dialogService = new DialogService(dialogViewModel);
            
            // ダイアログの登録
            _dialogService.RegisterDialog<UserInfoDialogView, UserInfoDialogViewModel>();
            _dialogService.RegisterDialog<ConfirmDialogView, ConfirmDialogViewModel>();
            
            // UserInfoDialogViewModelにDialogServiceを渡すためのファクトリ登録
            _dialogService.RegisterViewModelFactory<UserInfoDialogViewModel>(() => new UserInfoDialogViewModel(_dialogService));

            // メインウィンドウの作成
            var mainWindow = new MainWindow();
            _mainViewModel = new MainWindowViewModel(_dialogService)
            {
                DialogViewModel = dialogViewModel
            };
            mainWindow.DataContext = _mainViewModel;

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
            _mainViewModel?.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// テスト用途でのダイアログサービス取得メソッド
        /// </summary>
        /// <returns>ダイアログサービスのインスタンス（またはnull）</returns>
        public static IDialogService? GetDialogService() => _dialogService;
    }
}
