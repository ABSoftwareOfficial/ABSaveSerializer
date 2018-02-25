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

            System.IO.File.WriteAllText("expected2.txt", "Oh, Hello!\u0001365\u0002" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003F\u0005\u0004FirstStr\u0001SecondStr");
            System.IO.File.WriteAllText("result2", result);
            Assert.AreEqual("Oh, Hello!\u0001365" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003F\u0005\u0004FirstStr\u0001SecondStr", result);
        }
    }
}
