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
            DataContext = new MainWindowViewModel();
        }
    }
}