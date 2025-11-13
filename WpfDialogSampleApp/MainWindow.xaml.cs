using System.Windows;
using WpfDialogSampleApp.ViewModels;

namespace WpfDialogSampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// MainWindowの新しいインスタンスを初期化します
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenUserInfoDialog(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                await viewModel.ShowUserInfoDialogAsync();
            }
        }
    }
}