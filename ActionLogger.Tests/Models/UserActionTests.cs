using ActionLogger.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActionLogger.Tests.Models
{
    [TestClass]
    public class UserActionTests
    {
        [TestMethod]
        public void UserAction_Property_SetAndGet_ShouldWorkCorrectly()
        {
            // Arrange
            var userAction = new UserAction
            {
                Timestamp = "2024-09-16 12:00:00",
                ActionType = "Test Action",
                Description = "This is a test description."
            };

            // Act
            var timestamp = userAction.Timestamp;
            var actionType = userAction.ActionType;
            var description = userAction.Description;

            // Assert
            Assert.AreEqual("2024-09-16 12:00:00", timestamp);
            Assert.AreEqual("Test Action", actionType);
            Assert.AreEqual("This is a test description.", description);
        }
    }
}
