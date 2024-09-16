using ActionLogger.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ActionLogger.Tests.Utilities
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void RelayCommand_Execute_ShouldInvokeAction()
        {
            // Arrange
            bool wasExecuted = false;
            Action<object> execute = (param) => { wasExecuted = true; };
            var command = new RelayCommand(execute);

            // Act
            command.Execute(null);

            // Assert
            Assert.IsTrue(wasExecuted, "The execute action was not invoked.");
        }

        [TestMethod]
        public void RelayCommand_CanExecute_ShouldReturnTrue_WhenNoPredicate()
        {
            // Arrange
            Action<object> execute = (param) => { };
            var command = new RelayCommand(execute);

            // Act
            bool canExecute = command.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute, "CanExecute should return true when no predicate is provided.");
        }

        [TestMethod]
        public void RelayCommand_CanExecute_ShouldReturnExpectedValue()
        {
            // Arrange
            Action<object> execute = (param) => { };
            Predicate<object> canExecutePredicate = (param) => param != null;
            var command = new RelayCommand(execute, canExecutePredicate);

            // Act & Assert
            Assert.IsFalse(command.CanExecute(null), "CanExecute should return false when predicate returns false.");
            Assert.IsTrue(command.CanExecute("Test"), "CanExecute should return true when predicate returns true.");
        }
    }
}
