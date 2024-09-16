using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActionLogger.ViewModels
{
    [TestClass]
    public class BaseViewModelTests
    {
        private class TestViewModel : BaseViewModel
        {
            private string _testProperty;
            public string TestProperty
            {
                get { return _testProperty; }
                set
                {
                    _testProperty = value;
                    OnPropertyChanged(nameof(TestProperty));
                }
            }
        }

        [TestMethod]
        public void BaseViewModel_OnPropertyChanged_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            var viewModel = new TestViewModel();
            string propertyName = null;
            viewModel.PropertyChanged += (sender, args) =>
            {
                propertyName = args.PropertyName;
            };

            // Act
            viewModel.TestProperty = "New Value";

            // Assert
            Assert.AreEqual("TestProperty", propertyName, "PropertyChanged event was not raised correctly.");
        }
    }
}
