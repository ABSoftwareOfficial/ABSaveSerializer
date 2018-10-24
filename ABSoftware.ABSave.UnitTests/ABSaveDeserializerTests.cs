using System;
using System.Collections.Generic;
using ABSoftware.ABSave.Deserialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABSoftware.ABSave.UnitTests
{
    [TestClass]
    public class ABSaveDeserializerTests
    {
        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_StringObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("abcdefg", typeof(string));

            // Assert
            Assert.AreEqual("abcdefg", result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_LongObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("36263", typeof(long));

            // Assert
            Assert.AreEqual((long)36263, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_ArrayObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("\u0004First,Second", typeof(string[]), out ABSavePrimitiveType typ, out bool manuallyParse);

            // Assert
            Assert.AreEqual(ABSavePrimitiveType.Array, typ);
            Assert.AreEqual(true, manuallyParse);
            Assert.AreEqual(null, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_DictionaryObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("\u0006FirstItem,FirstValue,SecondItem,SecondValue", typeof(Dictionary<string, string>), out ABSavePrimitiveType typ, out bool manuallyParse);

            // Assert
            Assert.AreEqual(ABSavePrimitiveType.Dictionary, typ);
            Assert.AreEqual(true, manuallyParse);
            Assert.AreEqual(null, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_DateTimeObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("3616316", typeof(DateTime));

            // Assert
            Assert.AreEqual(new DateTime(3616316), result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_FalseBoolObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("F", typeof(bool));

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void Deserialize_TrueBoolObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.Deserialize("T", typeof(bool));

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void DeserializeBool_FalseObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.DeserializeBool("F");

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void DeserializeBool_TrueObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.DeserializeBool("T");

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void DeserializeNumber_IntObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.DeserializeNumber<int>("15136");

            // Assert
            Assert.AreEqual(15136, result);
        }

        [TestCategory("Deserialization, Components"), TestMethod]
        public void DeserializeDateTime_DateTimeObject_ReturnsCertainObject()
        {
            // Act
            var result = ABSaveDeserializer.DeserializeDateTime("15136");

            // Assert
            Assert.AreEqual(new DateTime(15136), result);
        }
    }
}
