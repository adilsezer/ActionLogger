using ActionLogger.Models;
using ActionLogger.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace ActionLogger.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<UserAction> UserActions { get; private set; }
        public ICollectionView UserActionsView { get; private set; }

        private string _filterText;
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
            UserActionsView.Filter = FilterActions;

            // Subscribe to mouse events
            MouseHook.MouseClicked += OnMouseClicked;

            // Subscribe to keyboard events
            KeyboardHook.KeyPressed += OnKeyPressed;

            // Subscribe to application events
            ApplicationMonitor.ApplicationStarted += OnApplicationStarted;
            ApplicationMonitor.ApplicationStopped += OnApplicationStopped;

            // Start the hooks and monitors
            MouseHook.Start();
            KeyboardHook.Start();
            ApplicationMonitor.Start();
        }

        private bool FilterActions(object obj)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;

            var action = obj as UserAction;
            return action.Description.IndexOf(FilterText, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                   action.ActionType.IndexOf(FilterText, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void OnMouseClicked(object sender, MouseEventArgs e)
        {
            string button = e.Button == MouseButtons.Left ? "left" : "right";
            string description = $"User performed a {button} mouse click at coordinates ({e.X}, {e.Y}).";

            AddUserAction("Mouse Click", description);
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            string description = $"User pressed the '{e.Key}' key.";

            AddUserAction("Keyboard Input", description);
        }

        private void OnApplicationStarted(object sender, ApplicationEventArgs e)
        {
            string description = $"User started the application '{e.ProcessName}'.";

            AddUserAction("Application Launch", description);
        }

        private void OnApplicationStopped(object sender, ApplicationEventArgs e)
        {
            string description = $"User closed the application '{e.ProcessName}'.";

            AddUserAction("Application Closure", description);
        }

        private void AddUserAction(string actionType, string description)
        {
            // Ensure thread-safe addition to the ObservableCollection
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

        public void StopLogging()
        {
            MouseHook.Stop();
            KeyboardHook.Stop();
            ApplicationMonitor.Stop();
        }
    }
}
