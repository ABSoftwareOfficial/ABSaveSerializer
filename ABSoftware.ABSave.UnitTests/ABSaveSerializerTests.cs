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

            Assert.AreEqual("\u0001T", result);
        }

        [TestMethod]
        public void Serialize_StringObject_ReturnsString()
        {
            // Arrange

            var obj = "Hello world!";

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual("\u0001Hello world!", result);
        }

        [TestMethod]
        public void Serialize_IntegerObject_ReturnsString()
        {
            // Arrange

            var obj = 7;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual("\u00017", result);
        }

        [TestMethod]
        public void Serialize_LongObject_ReturnsString()
        {
            // Arrange

            var obj = 7L;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual("\u00017", result);
        }

        [TestMethod]
        public void Serialize_DoubleObject_ReturnsString()
        {
            // Arrange

            var obj = 7.23d;

            // Act

            var result = ABSaveSerializer.Serialize(obj);

            // Assert

            Assert.AreEqual("\u00017.23", result);
        }

        [TestMethod]
        public void SerializeBool_TrueValue_ReturnsString()
        {
            // Act

            var result = ABSaveSerializer.SerializeBool(true);

            // Assert

            Assert.AreEqual("\u0001T", result);
        }

        [TestMethod]
        public void SerializeBool_FalseValue_ReturnsString()
        {
            // Act

            var result = ABSaveSerializer.SerializeBool(false);

            // Assert

            Assert.AreEqual("\u0001F", result);
        }

        [TestMethod]
        public void SerializeObject_SaveObject_ReturnsString()
        {
            // Arrange

            var test = new TestClass();

            // Act

            var result = ABSaveSerializer.SerializeObject(test, test.GetType());

            // Assert

            System.IO.File.WriteAllText("expected.txt", ABSaveWriter.WriteType(typeof(TestClass)) + "\u0003Oh, Hello!\u0001365" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003F\u0005\u0004FirstStr\u0001SecondStr\u0005");
            System.IO.File.WriteAllText("result.txt", result);
            Assert.AreEqual(ABSaveWriter.WriteType(typeof(TestClass)) + "\u0003Oh, Hello!\u0001365" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003F\u0005\u0004FirstStr\u0001SecondStr\u0005", result);
        }

        [TestMethod]
        public void SerializeArray_SingleItemArray_ReturnsString()
        {
            // Arrange

            var test = new List<string>() { "cool1" };

            // Act

            var result = ABSaveSerializer.SerializeArray(test);

            // Assert

            System.IO.File.WriteAllText("SingleItemArrayResult.txt", result);
            Assert.AreEqual("\u0004cool1\u0005", result);        
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
