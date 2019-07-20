using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSoftware.ABSave.Serialization;
using ABSoftware.ABSave.Tests.UnitTests.TestObjects;
using ABSoftware.ABSave.Tests.VersionAssembly;
using ABSoftware.ABSave.Tests.PKeyAndVersionAssembly;

namespace ABSoftware.ABSave.Tests.UnitTests
{
    [TestClass]
    public class ABSaveWriterTests
    {

        [TestCategory("Writing"), TestMethod]
        public void Escape_NoNeedToEscapeString()
        {
            // Arrange
            var str = "ab as+_Ag[ag;g[sa]gas'#g;sg['asgas[]lfawa.";

            // Act
            var result = ABSaveWriter.Escape(str);

            // Assert
            Assert.AreEqual(result, str);
        }

        [TestCategory("Writing"), TestMethod]
        public void Escape_OneReasonToEscapeString()
        {
            // Arrange
            var str = "ab as+_Ag[ag;g\u0001[sa]gas'#g;sg['asgas[]lfawa.";

            // Act
            var result = ABSaveWriter.Escape(str);

            // Assert
            Assert.AreEqual(result, "ab as+_Ag[ag;g\\\u0001[sa]gas'#g;sg['asgas[]lfawa.");
        }

        [TestCategory("Writing"), TestMethod]
        public void Escape_ReasonsToEscapeString()
        {
            // Arrange
            var str = "ab as+_Ag[ag;g\u0001[sa]g\u0004as\u0005'#g;sg['\u0003asga\u0005s[]lfa\u0006wa.";

            // Act
            var result = ABSaveWriter.Escape(str);

            // Assert
            Assert.AreEqual(result, "ab as+_Ag[ag;g\\\u0001[sa]g\\\u0004as\\\u0005'#g;sg['\\\u0003asga\\\u0005s[]lfa\\\u0006wa.");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNextItem_ShouldNotWriteNextItem()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNextItem(false), "");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNextItem_ShouldWriteNextItem()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNextItem(true), "\u0001");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteString_ShouldNotWriteNextItem()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteString("cool", false), "cool");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteString_ShouldWriteNextItem()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteString("cool", true), "\u0001cool");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Int16ToSB()
        {
            // Act
            StringBuilder sb = new StringBuilder();
            ABSaveWriter.WriteNumerical(243, TypeCode.Int16, true, true, sb);

            // Act/Assert
            Assert.AreEqual(sb.ToString(), "\u0001ó");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Int16()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(243, TypeCode.Int16, true), "\u0001ó");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Int32()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(6261, TypeCode.Int32, true), "\u0007u\u0018");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Int64()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(626136617721, TypeCode.Int64, true), "\b\u0005ù\u0092¦È\u0091");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Float()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(62613661.36161f, TypeCode.Single, true), "\b\u0004'ÚnL");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Double()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteNumerical(6261366.36161d, TypeCode.Double, true), "\b\bE\u009e$\u0097\u009dâWA");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_Decimal()
        {
            var val = ABSaveWriter.WriteNumerical(616136316631790.13613137137177m, TypeCode.Decimal, true);
            // Act/Assert
            Assert.AreEqual(val, "\b\u000f\u0019\u008e[Òr½o2\u0001\u009a\u0015Ç\0\0\u000e");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteNumerical_DecimalToSB()
        {
            // Act
            var sb = new StringBuilder();
            ABSaveWriter.WriteNumerical(616136316631790.13613137137177m, TypeCode.Decimal, true, true, sb);

            // Act/Assert
            Assert.AreEqual(sb.ToString(), "\b\u000f\u0019\u008e[Òr½o2\u0001\u009a\u0015Ç\0\0\u000e");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteObjectOpen()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteObjectOpen(), "\u0003");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteObjectClose()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteObjectClose(), "\u0005");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteArrayOpening()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteArrayOpening(), "\u0004");
        }

        [TestCategory("Writing"), TestMethod]
        public void WriteDictionaryOpening()
        {
            // Act/Assert
            Assert.AreEqual(ABSaveWriter.WriteDictionaryOpening(), "\u0006");
        }
    }
}
