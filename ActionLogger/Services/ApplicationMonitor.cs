using System;
using System.Management;

namespace ActionLogger.Services
{
    public static class ApplicationMonitor
    {
        public static event EventHandler<ApplicationEventArgs> ApplicationStarted = delegate { };
        public static event EventHandler<ApplicationEventArgs> ApplicationStopped = delegate { };

        private static ManagementEventWatcher startWatch;
        private static ManagementEventWatcher stopWatch;

        public static void Start()
        {
            // Watch for process start
            WqlEventQuery startQuery = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace");
            startWatch = new ManagementEventWatcher(startQuery);
            startWatch.EventArrived += (sender, args) =>
            {
                string processName = args.NewEvent.Properties["ProcessName"].Value.ToString();
                ApplicationStarted(null, new ApplicationEventArgs { ProcessName = processName });
            };
            startWatch.Start();

            // Watch for process stop
            WqlEventQuery stopQuery = new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace");
            stopWatch = new ManagementEventWatcher(stopQuery);
            stopWatch.EventArrived += (sender, args) =>
            {
                string processName = args.NewEvent.Properties["ProcessName"].Value.ToString();
                ApplicationStopped(null, new ApplicationEventArgs { ProcessName = processName });
            };
            stopWatch.Start();
        }

        public static void Stop()
        {
            if (startWatch != null)
            {
                startWatch.Stop();
                startWatch.Dispose();
            }

            if (stopWatch != null)
            {
                stopWatch.Stop();
                stopWatch.Dispose();
            }
        }
    }

    public class ApplicationEventArgs : EventArgs
    {
        public string ProcessName { get; set; }
    }
}
