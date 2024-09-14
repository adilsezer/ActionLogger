using ActionLogger.Models;
using ActionLogger.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ActionLogger.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<UserAction> UserActions { get; private set; }
        public ICollectionView UserActionsView { get; private set; }

        private string _filterText;
        private string _currentActiveWindow = string.Empty;
        private readonly StringBuilder _typedBuffer = new StringBuilder();
        private readonly DispatcherTimer _typingTimer;
        private DateTime _lastKeyPressTime;
        private bool _isShiftPressed = false;

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                UserActionsView.Refresh();
            }
        }

        public MainViewModel()
        {
            UserActions = new ObservableCollection<UserAction>();
            UserActionsView = CollectionViewSource.GetDefaultView(UserActions);

            FileMonitor.FileAccessed += OnFileAccessed;
            ClipboardMonitor.ClipboardChanged += OnClipboardChanged;
            MouseHook.MouseClicked += OnMouseClicked;
            KeyboardHook.KeyPressed += OnKeyPressed;
            ApplicationMonitor.ApplicationStarted += OnApplicationStarted;

            FileMonitor.StartMonitoring(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            ClipboardMonitor.StartMonitoring();
            MouseHook.Start();
            KeyboardHook.Start();
            ApplicationMonitor.Start();

            _typingTimer = new DispatcherTimer();
            _typingTimer.Interval = TimeSpan.FromSeconds(2); // 2 second delay for grouping keystrokes
            _typingTimer.Tick += OnTypingTimerElapsed;
        }

        private bool FilterActions(object obj)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;

            var action = obj as UserAction;
            return action.Description.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   action.ActionType.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void OnFileAccessed(object sender, FileEventArgs e)
        {
            AddUserAction("File Access", e.Action);
        }

        private void OnClipboardChanged(object sender, ClipboardEventArgs e)
        {
            AddUserAction("Clipboard", e.ClipboardContent);
        }

        private void OnMouseClicked(object sender, ActionLogger.Services.MouseEventArgs e)
        {
            string button = e.Button == MouseButtons.Left ? "left" : "right";
            string activeWindow = ApplicationMonitor.GetActiveWindowTitle(); // Track active window
            string description = $"You clicked the {button} mouse button in the '{activeWindow}' window at coordinates ({e.X}, {e.Y}).";

            AddUserAction("Mouse Click", description);
        }

        private void OnKeyPressed(object sender, ActionLogger.Services.KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                _isShiftPressed = true;
                return;
            }

            if (e.Key == Key.CapsLock)
            {
                return;
            }

            if (IsControlKey(e.Key))
            {
                LogControlKey(e.Key);
            }
            else
            {
                string keyString = _isShiftPressed ? e.Key.ToString().ToUpper() : e.Key.ToString().ToLower();

                _typedBuffer.Append(keyString);
                _lastKeyPressTime = DateTime.Now;

                if (!_typingTimer.IsEnabled)
                {
                    _typingTimer.Start();
                }
            }

            _isShiftPressed = false;
        }

        private bool IsControlKey(Key key)
        {
            return key == Key.Return || key == Key.Back || key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.Tab || key == Key.Space;
        }

        private void LogControlKey(Key key)
        {
            string activeWindow = ApplicationMonitor.GetActiveWindowTitle();
            string description;

            if (key == Key.Return)
            {
                description = $"You pressed 'Enter' in the '{activeWindow}' window.";
            }
            else if (key == Key.Back)
            {
                description = $"You pressed 'Backspace' in the '{activeWindow}' window.";
            }
            else if (key == Key.Space)
            {
                description = $"You pressed 'Space' in the '{activeWindow}' window.";
                _typedBuffer.Append(" "); // Add a space to separate words
            }
            else
            {
                description = $"You pressed '{key}' in the '{activeWindow}' window.";
            }

            AddUserAction("Keyboard Input", description);
        }

        private void OnTypingTimerElapsed(object sender, EventArgs e)
        {
            if ((DateTime.Now - _lastKeyPressTime).TotalSeconds >= 2)
            {
                // Log the typed word if no key was pressed for 2 seconds
                string typedWord = _typedBuffer.ToString().Trim();
                if (!string.IsNullOrEmpty(typedWord))
                {
                    string activeWindow = ApplicationMonitor.GetActiveWindowTitle();
                    string description = $"You typed the word '{typedWord}' in the '{activeWindow}' window.";

                    AddUserAction("Keyboard Input", description);
                    _typedBuffer.Clear();
                }

                _typingTimer.Stop();
            }
        }

        private void OnApplicationStarted(object sender, ApplicationEventArgs e)
        {
            if (_currentActiveWindow != e.ProcessName)
            {
                string description = $"You started working in the application '{e.ProcessName}', used for {GetApplicationPurpose(e.ProcessName)}.";
                AddUserAction("Application Activity", description);
                _currentActiveWindow = e.ProcessName; // Update the current active window
            }
        }

        private void AddUserAction(string actionType, string description)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UserActions.Insert(0, new UserAction
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ActionType = actionType,
                    Description = description
                });
            });
        }

        private string GetApplicationPurpose(string processName)
        {
            switch (processName.ToLower())
            {
                case "word":
                    return "editing documents";
                case "excel":
                    return "working with spreadsheets";
                case "chrome":
                    return "browsing the internet";
                default:
                    return "general tasks";
            }
        }

        public void StopLogging()
        {
            MouseHook.Stop();
            KeyboardHook.Stop();
        }
    }
}
