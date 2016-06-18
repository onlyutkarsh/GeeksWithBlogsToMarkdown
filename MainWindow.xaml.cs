using GeeksWithBlogsToMarkdown.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GeeksWithBlogsToMarkdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel(DialogCoordinator.Instance);
            DataContext = viewModel;
        }
    }
}