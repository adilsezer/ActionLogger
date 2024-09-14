using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ActionLogger.Services
{
    public static class ApplicationMonitor
    {
        public static event EventHandler<ApplicationEventArgs> ApplicationStarted = delegate { };

        public static void Start()
        {
            // Use DispatcherTimer to ensure STA thread compliance
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Poll every 5 seconds
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            // Ensure that the GetActiveWindowTitle method runs on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                string activeAppName = GetActiveWindowTitle();
                if (!string.IsNullOrEmpty(activeAppName))
                {
                    ApplicationStarted(null, new ApplicationEventArgs { ProcessName = activeAppName });
                }
            });
        }

        // Change this method to public so it can be accessed in MainViewModel
        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }

    public class ApplicationEventArgs : EventArgs
    {
        public string ProcessName { get; set; }
    }
}
