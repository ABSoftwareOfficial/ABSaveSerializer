using System;
using System.Collections.Generic;
using System.Diagnostics;
using ABSoftware.ABSave.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSoftware.ABSave.Tests.UnitTests.TestObjects;

namespace ABSoftware.ABSave.Tests.UnitTests
{
    [TestClass]
    public class ABSaveConvertTests
    {
        const string UConvert = "Full Conversion, Unnamed, With Types";
        const string NConvert = "Full Conversion, Named, With Types";
        const string MConvert = "Full Conversion, Named, Without Types";
        #region Unnamed
        [TestCategory(UConvert), TestMethod]
        public void SerializeABSave_SaveObject()
        {
            // Arrange
            var testClass = new TestClass();

            // Act
            var result = ABSaveConvert.ObjectToABSaveDocument(testClass, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("U\u0001Oh, Hello!\am\u0001\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.NextClass,ABSoftware.ABSave.Tests.UnitTests\0\0\u0003F\u0005\u0004FirstStr\u0001SecondStr", result);
        }

        [TestCategory(UConvert), TestMethod]
        public void SerializeABSave_ComplexObject()
        {

            // Act
            var result = ABSaveConvert.ObjectToABSaveDocument(ComplexSerializeTestClass.TestInstance, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("U\u0001\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Solar_System,ABSoftware.ABSave.Tests.UnitTests\0\0\u0003Milky Way\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Planet,ABSoftware.ABSave.Tests.UnitTests\u0001\0\u0003Mercury\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal,ABSoftware.ABSave.Tests.UnitTests\u0002\0\u0003Firefox\u0001Can survive ultra-high temperatures. Loves digging holes.\u0001\u0017\u0005\u0002\0\u0003AShift\u0001Shifts in and out of existence.\b\u0003 \u0086\u0001\u0005\u0002\0\u0003Unknown\u0001Unknown\0\u0005\u0002\0\u0003Unknown #2\u0001Unknown #2\0\u0005\u0005\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building,ABSoftware.ABSave.Tests.UnitTests\u0003\0\u0003Big Hole\u0001Somewhere\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company,ABSoftware.ABSave.Tests.UnitTests\u0004\0\u0003Unknown\0\0\0\u0005\b\b\0\0\0\0\0\u0088Ã@\b\a\0@,¯¯~\u0001\u0005\u0005\u0006ABSoftware.ABSave.Tests.UnitTests.TestObjects.WorkSite\u0001\u0003\0\u0003Biggest ever hole\u0001Somewhere Else\u0001\u0004\0\u0003Unknown\u0001\u0001\u0001\u0002\0\u0005\b\b\0\0\0\0\u0084×\u0097A\b\b\0@XÚyuÂ\b\u0001\0\u0003Earth\u0004\u0002\0\u0003Human\u0001Some mysterious creature that only humans (and maybe dolphins ;) ) know about.\b\u0004°·\u0094\u0003\u0005\u0002\0\u0003Cat\u0001Cat Videos.\u0001d\u0005\u0005\u0004\u0003\0\u0003The Shard\u000132 London Bridge Street, London SE1 9SG\u0001\u0004\0\u0003Stella Property\b\u0005\0\u0010¥Ôè\b\u0003 \u0086\u0001\b\b\0\00qJaÈ\b\u0005\b\bffff&\u0088Ã@\b\b\0@UA¡R¥\b\u0005\u0003\0\u0003Unnamed\u000125 London Bridge Street, London XYZ XYZ\u0001\u0004\0\u0003Elegantly Small\b\u0004­F\u0097>\u0001I\b\b\0\u0080ìÚÃ\bÅ\b\u0005\b\bo\u0012\u0083ÀÊ¡[@\b\b\0@#îßYÆ\b\u0005\u0003\0\u0003Simply Square\u0001176 XYZ Street, London XYZ ZYX\u0001\u0004\0\u0003SimplBuild\u0001\u0099\u0001\u0003\b\b\0À\u0097»#dÔ\b\u0005\b\bHáz\u0014®\an@\b\b\0ÀJ\n\u007f\u0013È\b\u0005\u0005\u0006ABSoftware.ABSave.Tests.UnitTests.TestObjects.WorkSite\u0001\u0003\0\u0003BrickShard\u00012521463 London Bridge Street, London XXZ XXZ\u0001\u0004\0\u0003Stella Property, Bigger, Better\b\b+\u0017îdxEc\u0001\b\u0003 \u0086\u0001\b\b\0À0¨Ô\u0097Ó\b\u0005\b\b33333Ø¢@\0\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.WorkSite\u0001\u0003\0\u0003SimplLarge HQ\u000154321 London Bridge Street, London ZYY XXZ\u0001\u0004\0\u0003Stella Property, Bigger, Better\b\b+\0d§³¶à\r\b\u0003 \u0086\u0001\b\b\0À0¨Ô\u0097Ó\b\u0005\b\b33333Ø¢@\0", result);
        }
        #endregion

        #region Named
        [TestCategory(UConvert), TestMethod]
        public void SerializeABSave_NAMEDSaveObject()
        {
            // Arrange
            var testClass = new TestClass();

            // Act
            var result = ABSaveConvert.ObjectToABSaveDocument(testClass, ABSaveType.WithNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("N\u0001str\u0001Oh, Hello!\u0001i\am\u0001\u0001nextCl\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.NextClass,ABSoftware.ABSave.Tests.UnitTests\0\0\u0003yoy\u0001F\u0005lstOfStr\u0004FirstStr\u0001SecondStr", result);
        }

        [TestCategory(NConvert), TestMethod]
        public void SerializeABSave_SaveObject_Named()
        {
            // Arrange
            var testClass = new MultiObjectTest();

            // Act
            var result = ABSaveConvert.ObjectToABSaveDocument(testClass, ABSaveType.WithNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("N\u0001Str\u0001hmmmm, ok\u0001InnerArr\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.MultiObjectInner,ABSoftware.ABSave.Tests.UnitTests\0\0\u0003OK\u0001$\u0005\0\0\u0003OK\u0001\u0019\u0005\u0005Inner\0\0\u0003OK\a\u001c\u000e", result);

        }

        [TestCategory(MConvert), TestMethod]
        public void SerializeABSave_SaveObject_Named_NoTypes()
        {
            // Arrange
            var testClass = new MultiObjectTest();

            // Act
            var result = ABSaveConvert.ObjectToABSaveDocument(testClass, ABSaveType.WithNames, new ABSaveSettings(false));

            // Assert
            Assert.AreEqual("M\u0001Str\u0001hmmmm, ok\u0001InnerArr\u0004\u0003OK\u0001$\u0005\u0003OK\u0001\u0019\u0005\u0005Inner\u0003OK\a\u001c\u000e", result);

        }
        #endregion
    }
}
