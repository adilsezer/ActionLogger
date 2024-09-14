using ActionLogger.Views;
using System.Security.Principal;
using System.Windows;

namespace ActionLogger
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check if the application is running as administrator
            if (!IsRunningAsAdministrator())
            {
                // Show a message box to inform the user that admin privileges are required
                MessageBox.Show("The application requires administrator privileges to run correctly. Please restart the application as an administrator.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Exit the application gracefully
                Application.Current.Shutdown();
                return;
            }

            // Now that we are sure admin privileges are present, manually create and show MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        /// <summary>
        /// Determines if the current application is running with administrator privileges.
        /// </summary>
        /// <returns>True if the app is running as an administrator, otherwise false.</returns>
        private bool IsRunningAsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
