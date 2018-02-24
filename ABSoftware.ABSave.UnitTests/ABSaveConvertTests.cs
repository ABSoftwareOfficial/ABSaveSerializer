using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABSoftware.ABSave.UnitTests
{
    [TestClass]
    public class ABSaveConvertTests
    {
        [TestMethod]
        public void SerializeABSave_SaveObject_ReturnsString()
        {
            // Arrange

            var testClass = new TestClass();

            // Act

            var result = ABSaveConvert.SerializeABSave(testClass);

            // Assert

            Assert.AreEqual("Oh, Hello!\u0001365\u0002" + typeof(NextClass).FullName + "\u0003F\u0005\u0004\u0001FirstStr\u0001SecondStr", result);
        }
    }
}
