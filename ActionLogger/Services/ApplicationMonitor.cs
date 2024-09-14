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
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string activeAppName = GetActiveWindowTitle();
                if (!string.IsNullOrEmpty(activeAppName))
                {
                    ApplicationStarted(null, new ApplicationEventArgs { ProcessName = activeAppName });
                }
            });
        }

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
