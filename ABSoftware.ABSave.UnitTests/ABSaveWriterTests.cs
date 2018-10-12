using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSoftware.ABSave;
using ABSoftware.ABSave.Serialization;

namespace ABSoftware.ABSave.UnitTests
{
    [TestClass]
    public class ABSaveWriterTests
    {
        [TestCategory("Writing"), TestMethod]
        public void WriteType_DifferentAssemblyType_ReturnsCertainString()
        {
            // Arrange

            var testClass = new TestClass();

            // Act

            var result = ABSaveWriter.WriteType(testClass.GetType());

            // Assert

            Assert.AreEqual(result, testClass.GetType().FullName);
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteType_SameAssemblyType_ReturnsCertainString()
        {
            // Act

            var result = ABSaveWriter.WriteType(typeof(ABSaveSerializer));

            // Assert

            Assert.AreEqual(result, typeof(ABSaveSerializer).FullName);
        }

        [TestCategory("Writing"), TestMethod]
        public void Escape_NoNeedToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "ab as+_Ag[ag;g[sa]gas'#g;sg['asgas[]lfawa.";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, str);
        }

        [TestCategory("Writing"), TestMethod]
        public void Escape_OneReasonToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "ab as+_Ag[ag;g\u0001[sa]gas'#g;sg['asgas[]lfawa.";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, "ab as+_Ag[ag;g\\\u0001[sa]gas'#g;sg['asgas[]lfawa.");
        }

        [TestCategory("Writing"), TestMethod]
        public void Escape_ReasonsToEscapeString_ReturnsCertainString()
        {
            // Arrange

            var str = "ab as+_Ag[ag;g\u0001[sa]g\u0004as\u0005'#g;sg['\u0003asga\u0005s[]lfa\u0006wa.";

            // Act

            var result = ABSaveWriter.Escape(str);

            // Assert

            Assert.AreEqual(result, "ab as+_Ag[ag;g\\\u0001[sa]g\\\u0004as\\\u0005'#g;sg['\\\u0003asga\\\u0005s[]lfa\\\u0006wa.");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNextItem_ShouldNotWriteNextItem_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNextItem(false), "");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNextItem_ShouldWriteNextItem_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNextItem(true), "\u0001");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteString_ShouldNotWriteNextItem_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteString("cool", false), "cool");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteString_ShouldWriteNextItem_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteString("cool", true), "\u0001cool");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_ShouldNotWriteNextItem_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(6261, false), "6261");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_ShouldWriteNextItem_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(6261, true), "\u00016261");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteObjectOpen_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteObjectOpen(typeof(TestClass)), ABSaveWriter.WriteType(typeof(TestClass)) + "\u0003");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteObjectClose_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteObjectClose(), "\u0005");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteArrayOpening_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteArrayOpening(), "\u0004");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteDictionaryOpening_ReturnsCertainString()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteDictionaryOpening(), "\u0006");
        }
    }
}
