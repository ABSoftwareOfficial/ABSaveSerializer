using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSoftware.ABSave;

namespace ABSoftware.ABSave.UnitTests
{
    [TestClass]
    public class ABSaveWriterTests
    {
        [TestMethod]
        public void WriteType_DifferentAssemblyType_ReturnsCertainString()
        {
            // Arrange

            var testClass = new TestClass();

            // Act

            var result = ABSaveWriter.WriteType(testClass.GetType());

            // Assert

            Assert.AreEqual(result, "\u0002" + testClass.GetType().FullName);
        }

        [TestMethod]
        public void WriteType_SameAssemblyType_ReturnsCertainString()
        {
            // Act

            var result = ABSaveWriter.WriteType(typeof(ABSaveSerializer));

            // Assert

            Assert.AreEqual(result, "\u0002" + typeof(ABSaveSerializer).FullName);
        }

        [TestMethod]
        public void WriteType_NoNeedToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "a";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, str);
        }
    }
}
