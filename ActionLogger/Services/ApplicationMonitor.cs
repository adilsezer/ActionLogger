using System;
using System.Management;

namespace ActionLogger.Services
{
    public static class ApplicationMonitor
    {
        public static event EventHandler<ApplicationEventArgs> ApplicationStarted = delegate { };

        private static ManagementEventWatcher startWatch;

        public static void Start()
        {
            // Watch for process start
            WqlEventQuery startQuery = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace");
            startWatch = new ManagementEventWatcher(startQuery);
            startWatch.EventArrived += OnProcessStarted;
            startWatch.Start();
        }

        public static void Stop()
        {
            if (startWatch != null)
            {
                startWatch.Stop();
                startWatch.Dispose();
                startWatch = null;
            }
        }

        private static void OnProcessStarted(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            uint processId = (uint)e.NewEvent.Properties["ProcessID"].Value;

            if (IsUserProcess(processId))
            {
                ApplicationStarted(null, new ApplicationEventArgs { ProcessName = processName });
            }
        }

        /// <summary>
        /// Determines if the process with the given ID is initiated by a user.
        /// Excludes system processes.
        /// </summary>
        /// <param name="processId">The ID of the process.</param>
        /// <returns>True if user-initiated, otherwise false.</returns>
        private static bool IsUserProcess(uint processId)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    $"SELECT * FROM Win32_Process WHERE ProcessId = {processId}"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string[] ownerInfo = new string[2];
                        obj.InvokeMethod("GetOwner", ownerInfo);

                        string user = ownerInfo[0];
                        string domain = ownerInfo[1];

                        if (!string.IsNullOrEmpty(user) &&
                            !string.Equals(user, "SYSTEM", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if necessary
                Console.WriteLine($"Error determining process owner: {ex.Message}");
            }

            return false;
        }
    }

    public class ApplicationEventArgs : EventArgs
    {
        public string ProcessName { get; set; }
    }
}
