using System;
using System.Windows;
using System.Windows.Threading;

namespace ActionLogger.Services
{
    public static class ClipboardMonitor
    {
        private static string _lastClipboardText = string.Empty;
        private static bool _isFirstRead = true; // Track whether this is the first clipboard read

        public static event EventHandler<ClipboardEventArgs> ClipboardChanged = delegate { };

        public static void StartMonitoring()
        {
            // Use DispatcherTimer to ensure STA thread compliance and reduce performance impact
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Adjust polling interval for better performance
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            // Ensure that clipboard access is on the UI thread (STA)
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        string clipboardText = Clipboard.GetText();

                        if (_isFirstRead)
                        {
                            // Skip the first clipboard content after the app starts
                            _lastClipboardText = clipboardText; // Just set it, don't log
                            _isFirstRead = false;
                        }
                        else if (clipboardText != _lastClipboardText)
                        {
                            _lastClipboardText = clipboardText;
                            ClipboardChanged(null, new ClipboardEventArgs { ClipboardContent = $"You copied the following text: '{TrimText(clipboardText)}'." });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (like access violations) gracefully without freezing the app
                    Console.WriteLine($"Clipboard access error: {ex.Message}");
                }
            });
        }

        private static string TrimText(string text)
        {
            return text.Length > 50 ? text.Substring(0, 50) + "..." : text;
        }
    }

    public class ClipboardEventArgs : EventArgs
    {
        public string ClipboardContent { get; set; }
    }
}
