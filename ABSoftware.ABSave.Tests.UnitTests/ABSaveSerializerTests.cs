using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ABSoftware.ABSave.Serialization;
using ABSoftware.ABSave.Tests.UnitTests.TestObjects;
using ABSoftware.ABSave.Tests.VersionAssembly;
using ABSoftware.ABSave.Tests.PKeyAndVersionAssembly;

namespace ABSoftware.ABSave.Tests.UnitTests
{
    [TestClass]
    public class ABSaveSerializerTests
    {
        const string USerializationToString = "Serialization, Unnamed, With Types";
        #region Unnamed, With Types, To String

        [TestCategory(USerializationToString), TestMethod]
        public void Serialize_DateTimeObject()
        {
            // Arrange
            var obj = new DateTime(25325);

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0007íb", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void Serialize_BooleanObject()
        {
            // Arrange
            var obj = true;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0001T", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void Serialize_StringObject()
        {
            // Arrange
            var obj = "Hello world!";

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0001Hello world!", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void Serialize_IntegerObject()
        {
            // Arrange
            var obj = 7;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0001\a", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void Serialize_LongObject()
        {
            // Arrange
            var obj = 7L;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0001\a", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void Serialize_DoubleObject()
        {
            // Arrange
            var obj = 7.23d;

            // Act
            var result = ABSaveSerializer.Serialize(obj, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\b\bìQ¸\u001e\u0085ë\u001c@", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeBool_TrueValue()
        {
            // Arrange
            var obj = true;

            // Act
            var result = ABSaveSerializer.SerializeBool(obj);

            // Assert
            Assert.AreEqual("\u0001T", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeBool_FalseValue()
        {
            // Arrange
            var obj = false;

            // Act
            var result = ABSaveSerializer.SerializeBool(obj);

            // Assert
            Assert.AreEqual("\u0001F", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeObject_SaveObject()
        {
            // Arrange
            var test = new TestClass();

            // Act
            var result = ABSaveSerializer.SerializeObject(test, ABSaveType.NoNames, test.GetType(), new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.TestClass,ABSoftware.ABSave.Tests.UnitTests\0\0\u0003Oh, Hello!\am\u0001\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.NextClass,ABSoftware.ABSave.Tests.UnitTests\u0001\0\u0003F\u0005\u0004FirstStr\u0001SecondStr\u0005", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeArray_SingleItemArray()
        {
            // Arrange
            var test = new List<string>() { "cool1" };

            // Act
            var result = ABSaveSerializer.SerializeArray(test, typeof(List<string>), ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0004cool1\u0005", result);        
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeDictionary_StringStringDictionary()
        {
            // Arrange
            var test = new Dictionary<string, string>()
            {
                { "FirstKey", "FirstValue" },
                { "SecondKey", "SecondValue" }
            };

            // Act
            var result = ABSaveSerializer.SerializeDictionary(test, ABSaveType.NoNames, new ABSaveSettings());

            // Assert
            Assert.AreEqual("\u0006FirstKey\u0001FirstValue\u0001SecondKey\u0001SecondValue\u0005", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeDateTime_DateTimeObject()
        {
            // Arrange
            DateTime obj = new DateTime(253253526);

            // Act
            var result = ABSaveSerializer.SerializeDateTime(obj);

            // Assert
            Assert.AreEqual("\b\u0004\u0096W\u0018\u000f", result);
        }
        #endregion

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeType_NoVersion_NoPublicKey()
        {
            // Act
            var result = ABSaveSerializer.SerializeType(typeof(ABSaveSerializerTests));

            // Assert
            Assert.AreEqual("ABSoftware.ABSave.Tests.UnitTests.ABSaveSerializerTests,ABSoftware.ABSave.Tests.UnitTests", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeType_NoVersion_PublicKey()
        {
            // Act
            var result = ABSaveSerializer.SerializeType(typeof(ABSaveSerializer));

            // Assert
            Assert.AreEqual("ABSoftware.ABSave.Serialization.ABSaveSerializer,ABSoftware.ABSave,,,õ=\u009b\u0006~ò®\u0098", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeType_Version_NoPublicKey()
        {
            // Act
            var result = ABSaveSerializer.SerializeType(typeof(VersionClass));

            // Assert
            Assert.AreEqual("ABSoftware.ABSave.Tests.VersionAssembly.VersionClass,ABSoftware.ABSave.Tests.VersionAssembly,\u0009\0\0\0.\u0005\0\0\0.\u0003\0\0\0.\u0007\0\0\0", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeType_Version_PublicKey()
        {
            // Act
            var result = ABSaveSerializer.SerializeType(typeof(PublicKeyAndVersionClass));

            // Assert
            Assert.AreEqual("ABSoftware.ABSave.Tests.PKeyAndVersionAssembly.PublicKeyAndVersionClass,ABSoftware.ABSave.Tests.PKeyAndVersionAssembly,\u0009\0\0\0.\u0005\0\0\0.\u0003\0\0\0.\u0007\0\0\0,,R\tG4 \u0083;¬", result);
        }

        [TestCategory(USerializationToString), TestMethod]
        public void SerializeTypeBeforeObject_NoVersion_PublicKey()
        {
            // Act
            var result = ABSaveSerializer.SerializeTypeBeforeObject(typeof(ABSaveSerializer), new ABSaveSettings());

            // Assert
            Assert.AreEqual("ABSoftware.ABSave.Serialization.ABSaveSerializer,ABSoftware.ABSave,,,õ=\u009b\u0006~ò®\u0098\0\0", result);
        }
    }
}
