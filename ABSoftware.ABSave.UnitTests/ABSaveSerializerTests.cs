using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ABSoftware.ABSave.UnitTests
{
    [TestClass]
    public class ABSaveSerializerTests
    {
        [TestMethod]
        public void Serialize_BooleanObject_ReturnsString()
        {
            // Arrange

            var obj = true;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual(result, "\u0001T");
        }

        [TestMethod]
        public void Serialize_StringObject_ReturnsString()
        {
            // Arrange

            var obj = "Hello world!";

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual(result, "\u0001Hello world!");
        }

        [TestMethod]
        public void Serialize_IntegerObject_ReturnsString()
        {
            // Arrange

            var obj = 7;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual(result, "\u00017");
        }

        [TestMethod]
        public void Serialize_LongObject_ReturnsString()
        {
            // Arrange

            var obj = 7L;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual(result, "\u00017");
        }

        [TestMethod]
        public void Serialize_DoubleObject_ReturnsString()
        {
            // Arrange

            var obj = 7.23d;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual(result, "\u00017.23");
        }

        [TestMethod]
        public void SerializeBool_TrueValue_ReturnsString()
        {
            // Act

            var result = ABSaveSerializer.SerializeBool(true);

            // Assert

            Assert.AreEqual(result, "T");
        }

        [TestMethod]
        public void SerializeBool_FalseValue_ReturnsString()
        {
            // Act

            var result = ABSaveSerializer.SerializeBool(false);

            // Assert

            Assert.AreEqual(result, "F");
        }

        [TestMethod]
        public void SerializeObject_SaveObject_ReturnsString()
        {
            // Arrange

            var test = new TestClass();

            // Act

            var result = ABSaveSerializer.SerializeObject(test);

            // Assert

            Assert.AreEqual("\u0002" + typeof(TestClass).FullName + "\u0003Oh, Hello!\u0001365\u0002" + typeof(NextClass).FullName + "\u0003F\u0005\u0004\u0001FirstStr\u0001SecondStr\u0005", result);
        }

        [TestMethod]
        public void SerializeDictionary_StringStringDictionary_ReturnsString()
        {
            // Arrange

            var test = new Dictionary<string, string>()
            {
                { "FirstKey", "FirstValue" },
                { "SecondKey", "SecondValue" }
            };

            // Act

            var result = ABSaveSerializer.SerializeDictionary(test);

            // Assert

            Assert.AreEqual("\u0006FirstKey\u0001FirstValue\u0001SecondKey\u0001SecondValue\u0005", result);
        }
    }
}
