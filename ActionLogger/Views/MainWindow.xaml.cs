using ActionLogger.ViewModels;
using System.Windows;

namespace ActionLogger.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = this.DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.StopLogging();
            }
            base.OnClosing(e);
        }
    }
}
