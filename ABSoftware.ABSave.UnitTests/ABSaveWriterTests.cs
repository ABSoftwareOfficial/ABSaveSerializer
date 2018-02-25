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
        public void Escape_NoNeedToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "ab as+_Ag[ag;g[sa]gas'#g;sg['asgas[]lfawa.";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, str);
        }

        [TestMethod]
        public void Escape_OneReasonToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "ab as+_Ag[ag;g\u0001[sa]gas'#g;sg['asgas[]lfawa.";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, "ab as+_Ag[ag;g\\\u0001[sa]gas'#g;sg['asgas[]lfawa.");
        }

        [TestMethod]
        public void Escape_ReasonsToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "ab as+_Ag[ag;g\u0001[sa]g\u0004as\u0002'#g;sg['\u0003asga\u0005s[]lfa\u0006wa.";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, "ab as+_Ag[ag;g\\\u0001[sa]g\\\u0004as\\\u0002'#g;sg['\\\u0003asga\\\u0005s[]lfa\\\u0006wa.");
        }
    }
}
