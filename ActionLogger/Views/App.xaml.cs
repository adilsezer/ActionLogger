using ActionLogger.Views;
using System.Windows;

namespace ActionLogger
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Manually create and show MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
