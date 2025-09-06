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
        private ServiceContainer? _serviceContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // DIコンテナーの設定
            ConfigureServices();

            // メインウィンドウの作成
            var mainWindow = new MainWindow();
            var mainViewModel = _serviceContainer?.GetService<MainWindowViewModel>();
            if (mainViewModel != null)
            {
                mainWindow.DataContext = mainViewModel;
            }

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void ConfigureServices()
        {
            _serviceContainer = new ServiceContainer();

            // サービスの登録
            _serviceContainer.RegisterSingleton<IDialogService, DialogService>(() => 
            {
                var dialogService = new DialogService(_serviceContainer);
                // ダイアログの登録
                dialogService.RegisterDialog<UserInfoDialogViewModel, UserInfoDialog>();
                return dialogService;
            });

            // ViewModelの登録
            _serviceContainer.RegisterTransient<MainWindowViewModel>(() => new MainWindowViewModel(_serviceContainer.GetService<IDialogService>()));
            _serviceContainer.RegisterTransient<UserInfoDialogViewModel>(() => new UserInfoDialogViewModel());

            // Viewの登録（必要に応じて）
            _serviceContainer.RegisterTransient<UserInfoDialog>(() => new UserInfoDialog());

            // 静的アクセス用に設定（テスト容易性のため）
            ServiceLocator.SetContainer(_serviceContainer);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // リソースのクリーンアップ
            _serviceContainer?.GetService<IDialogService>()?.CloseAllDialogs();
            base.OnExit(e);
        }
    }

    /// <summary>
    /// サービスロケーターパターン（テスト用途）
    /// </summary>
    public static class ServiceLocator
    {
        private static ServiceContainer? _container;

        public static void SetContainer(ServiceContainer container)
        {
            _container = container;
        }

        public static T GetService<T>() where T : class
        {
            return _container?.GetService<T>() ?? throw new InvalidOperationException("サービスコンテナーが設定されていません。");
        }

        public static T? TryGetService<T>() where T : class
        {
            return _container?.TryGetService<T>();
        }
    }
}
