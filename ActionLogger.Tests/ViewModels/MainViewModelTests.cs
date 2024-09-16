using ActionLogger.Models;
using ActionLogger.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ActionLogger.Tests.ViewModels
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void MainViewModel_AddUserAction_ShouldAddToCollection()
        {
            // Arrange
            var viewModel = new MainViewModel();
            int initialCount = viewModel.UserActions.Count;

            // Act
            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:00:00",
                ActionType = "Test Action",
                Description = "This is a test description."
            });

            // Assert
            Assert.AreEqual(initialCount + 1, viewModel.UserActions.Count, "UserAction was not added to the collection.");
            var addedAction = viewModel.UserActions.First();
            Assert.AreEqual("2024-09-16 12:00:00", addedAction.Timestamp);
            Assert.AreEqual("Test Action", addedAction.ActionType);
            Assert.AreEqual("This is a test description.", addedAction.Description);
        }

        [TestMethod]
        public void MainViewModel_FilterText_ShouldFilterUserActions()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Add sample data
            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:00:00",
                ActionType = "File Access",
                Description = "You created a new file 'test.txt' in the Documents folder."
            });

            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:05:00",
                ActionType = "Clipboard",
                Description = "You copied the following text: 'Sample Clipboard Text'."
            });

            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:10:00",
                ActionType = "Keyboard Input",
                Description = "You typed the word 'Hello' in the 'Notepad' window."
            });

            // Act
            viewModel.FilterText = "Clipboard";

            // Assert
            var filteredActions = viewModel.UserActionsView.Cast<UserAction>().ToList();
            Assert.AreEqual(1, filteredActions.Count, "Filter did not return the expected number of UserActions.");
            Assert.AreEqual("Clipboard", filteredActions[0].ActionType);
        }

        [TestMethod]
        public void MainViewModel_FilterText_ShouldBeCaseInsensitive()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Add sample data
            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:00:00",
                ActionType = "File Access",
                Description = "You created a new file 'test.txt' in the Documents folder."
            });

            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:05:00",
                ActionType = "Clipboard",
                Description = "You copied the following text: 'Sample Clipboard Text'."
            });

            // Act
            viewModel.FilterText = "clipboard"; // Lowercase

            // Assert
            var filteredActions = viewModel.UserActionsView.Cast<UserAction>().ToList();
            Assert.AreEqual(1, filteredActions.Count, "Filter should be case-insensitive.");
            Assert.AreEqual("Clipboard", filteredActions[0].ActionType);
        }

        [TestMethod]
        public void MainViewModel_FilterText_Empty_ShouldReturnAllActions()
        {
            // Arrange
            var viewModel = new MainViewModel();

            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:00:00",
                ActionType = "File Access",
                Description = "You created a new file 'test.txt' in the Documents folder."
            });

            viewModel.UserActions.Add(new UserAction
            {
                Timestamp = "2024-09-16 12:05:00",
                ActionType = "Clipboard",
                Description = "You copied the following text: 'Sample Clipboard Text'."
            });

            // Act
            viewModel.FilterText = string.Empty;

            // Assert
            var filteredActions = viewModel.UserActionsView.Cast<UserAction>().ToList();
            Assert.AreEqual(2, filteredActions.Count, "FilterText empty should return all UserActions.");
        }
    }
}
