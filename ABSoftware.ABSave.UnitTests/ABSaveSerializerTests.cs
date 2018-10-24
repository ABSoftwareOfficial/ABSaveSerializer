using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ABSoftware.ABSave.Serialization;

namespace ABSoftware.ABSave.UnitTests
{
    [TestClass]
    public class ABSaveSerializerTests
    {
        #region Unnamed
        [TestMethod]
        public void IGNORE_THIS_TEST()
        {
            // For some reason the first test that is run has an inaccurate time on it/runs really slow.
            // So, to make the times fairer - this test is just a dummy to essentially soak up the slow time on the first one. Seriously, Microsoft, fix it (I think THEY need better tests - pun intended).
            
            // Arrange
            DateTime obj = new DateTime(25325);

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u000125325", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void Serialize_DateTimeObject_ReturnsString()
        {
            // Arrange
            DateTime obj = new DateTime(25325);

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u000125325", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void Serialize_BooleanObject_ReturnsString()
        {
            // Arrange
            var obj = true;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u0001T", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void Serialize_StringObject_ReturnsString()
        {
            // Arrange
            var obj = "Hello world!";

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u0001Hello world!", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void Serialize_IntegerObject_ReturnsString()
        {
            // Arrange
            var obj = 7;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u00017", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void Serialize_LongObject_ReturnsString()
        {
            // Arrange
            var obj = 7L;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u00017", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void Serialize_DoubleObject_ReturnsString()
        {
            // Arrange
            var obj = 7.23d;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u00017.23", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void SerializeBool_TrueValue_ReturnsString()
        {
            // Arrange
            var obj = true;

            // Act
            var result = ABSaveSerializer.SerializeBool(obj);

            // Assert
            Assert.AreEqual("\u0001T", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void SerializeBool_FalseValue_ReturnsString()
        {
            // Arrange
            var obj = false;

            // Act
            var result = ABSaveSerializer.SerializeBool(obj);

            // Assert
            Assert.AreEqual("\u0001F", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void SerializeObject_SaveObject_ReturnsString()
        {
            // Arrange
            var test = new TestClass();

            // Act
            var result = ABSaveSerializer.SerializeObject(test, ABSaveType.WithOutNames, test.GetType());

            // Assert
            Assert.AreEqual("\u0001" + ABSaveWriter.WriteType(typeof(TestClass)) + "\u0003Oh, Hello!\u0001365\u0001" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003F\u0005\u0004FirstStr\u0001SecondStr\u0005", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void SerializeArray_SingleItemArray_ReturnsString()
        {
            // Arrange
            var test = new List<string>() { "cool1" };

            // Act
            var result = ABSaveSerializer.SerializeArray(test, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u0004cool1\u0005", result);        
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void SerializeDictionary_StringStringDictionary_ReturnsString()
        {
            // Arrange
            var test = new Dictionary<string, string>()
            {
                { "FirstKey", "FirstValue" },
                { "SecondKey", "SecondValue" }
            };

            // Act
            var result = ABSaveSerializer.SerializeDictionary(test, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("\u0006FirstKey\u0001FirstValue\u0001SecondKey\u0001SecondValue\u0005", result);
        }

        [TestCategory("Serialization, Unnamed"), TestMethod]
        public void SerializeDateTime_DateTimeObject_ReturnsString()
        {
            // Arrange
            DateTime obj = new DateTime(253253526);

            // Act
            var result = ABSaveSerializer.SerializeDateTime(obj);

            // Assert
            Assert.AreEqual("253253526", result);
        }
        #endregion
    }
}
