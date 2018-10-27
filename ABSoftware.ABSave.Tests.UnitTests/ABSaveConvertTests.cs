using System;
using System.Collections.Generic;
using System.Diagnostics;
using ABSoftware.ABSave.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABSoftware.ABSave.Tests.UnitTests.TestObjects.TestObjects;
using ABSoftware.ABSave.Tests.UnitTests.TestObjects;

namespace ABSoftware.ABSave.Tests.UnitTests
{
    [TestClass]
    public class ABSaveConvertTests
    {
        #region Unnamed
        [TestCategory("Serialization, Unnamed, With Types"), TestMethod]
        public void SerializeABSave_SaveObject_ReturnsString()
        {
            // Arrange
            var testClass = new TestClass();

            // Act
            var result = ABSaveConvert.SerializeABSave(testClass, ABSaveType.WithOutNames);

            // Assert
            Assert.AreEqual("UT\u0001Oh, Hello!\u0001365\u0001" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003F\u0005\u0004FirstStr\u0001SecondStr", result);
        }

        [TestCategory("Serialization, Unnamed, With Types"), TestMethod]
        public void SerializeABSave_ComplexObject_ReturnsString()
        {
            // Arrange
            var testClass = new ComplexSerializeTestClass()
            {
                SolarSystems = new List<Solar_System>()
                {
                    new Solar_System()
                    {
                        Name = "Milky Way",
                        Planets = new List<Planet>()
                        {
                            new Planet()
                            {
                                Name = "Mercury",
                                Animals = new List<Animal>()
                                {
                                    new Animal()
                                    {
                                        Name = "Firefox",
                                        Description = "Can survive ultra-high temperatures. Loves digging holes.",
                                        IQ = 23
                                    },
                                    new Animal()
                                    {
                                        Name = "AShift",
                                        Description = "Shifts in and out of existence.",
                                        IQ = 100000
                                    },
                                    new Animal()
                                    {
                                        Name = "Unknown",
                                        Description = "Unknown",
                                        IQ = 0
                                    },
                                    new Animal()
                                    {
                                        Name = "Unknown #2",
                                        Description = "Unknown #2",
                                        IQ = 0
                                    }
                                },
                                Buildings = new List<Building>()
                                {
                                    new Building()
                                    {
                                        Name = "Big Hole",
                                        Address = "Somewhere",
                                        CreationTime = new DateTime(2, 5, 3),
                                        Height = 10000d,
                                        Owner = new Company()
                                        {
                                            Name = "Unknown",
                                            Value = 0,
                                            CreationTime = new DateTime(0),
                                            Workers = 0
                                        }
                                    }
                                },
                                Worksites = new Dictionary<WorkSite, Building>()
                                {
                                    {
                                        new WorkSite()
                                        {
                                            Name = "Firefox + AShift Work",
                                            MaterialCost = 1d,
                                            BuildingWorkCost = 1d,
                                            Owner = new Company()
                                            {
                                                Name = "Firefox + AShift",
                                                CreationTime = new DateTime(1874, 3, 8),
                                                Value = 1,
                                                Workers = 2
                                            }
                                        },
                                        new Building()
                                        {
                                            Name = "Biggest ever hole",
                                            Height = 100000000d,
                                            Address = "Somewhere Else",
                                            CreationTime = new DateTime(2001, 3, 8),
                                            Owner = new Company()
                                            {
                                                Name = "Unknown",
                                                CreationTime = new DateTime(0),
                                                Value = 1,
                                                Workers = 2
                                            }
                                        }
                                    }
                                }
                            },
                            new Planet()
                            {
                                Name = "Earth",
                                Animals = new List<Animal>()
                                {
                                    new Animal()
                                    {
                                        Name = "Human",
                                        Description = "Some mysterious creature that only humans (and maybe dolphins ;) ) know about.",
                                        IQ = 60078000
                                    },
                                    new Animal()
                                    {
                                        Name = "Cat",
                                        Description = "Cat Videos.",
                                        IQ = 100
                                    }

                                },
                                Buildings = new List<Building>()
                                {
                                    new Building()
                                    {
                                        Name = "The Shard", // NOTE: The Creation time and height of this was made up.
                                        Address = "32 London Bridge Street, London SE1 9SG",
                                        Height = 10000.3d,
                                        Owner = new Company()
                                        {
                                            Name = "Stella Property",
                                            CreationTime = new DateTime(2006, 6, 19),
                                            Value = 1000000000000,
                                            Workers = 100000
                                        },
                                        CreationTime = new DateTime(1975, 3, 13)
                                    },
                                    new Building()
                                    {
                                        Name = "Unnamed",
                                        Address = "25 London Bridge Street, London XYZ XYZ",
                                        Height = 110.528d,
                                        Owner = new Company()
                                        {
                                            Name = "Elegantly Small",
                                            CreationTime = new DateTime(2003, 6, 25),
                                            Value = 1050101421,
                                            Workers = 73
                                        },
                                        CreationTime = new DateTime(2004, 8, 27)
                                    },
                                    new Building()
                                    {
                                        Name = "Simply Square",
                                        Address = "176 XYZ Street, London XYZ ZYX",
                                        Height = 240.24d,
                                        Owner = new Company()
                                        {
                                            Name = "SimplBuild",
                                            CreationTime = new DateTime(2017, 3, 6),
                                            Value = 153,
                                            Workers = 3
                                        },
                                        CreationTime = new DateTime(2006, 3, 12)
                                    }
                                },
                                Worksites = new Dictionary<WorkSite, Building>()
                                {
                                    {
                                        new WorkSite()
                                        {
                                            Name = "BrickBuild's Third Success",
                                            BuildingWorkCost = 96.2d,
                                            MaterialCost = 231d,
                                            Owner = new Company()
                                            {
                                                Name = "BrickBuild",
                                                CreationTime = new DateTime(2004, 2, 23),
                                                Value = 1328,
                                                Workers = 2
                                            }
                                        },
                                        new Building()
                                        {
                                            Name = "BrickShard",
                                            Address = "2521463 London Bridge Street, London XXZ XXZ",
                                            Height = 2412.1d,
                                            Owner = new Company()
                                            {
                                                Name = "Stella Property, Bigger, Better",
                                                CreationTime = new DateTime(2016, 6, 19),
                                                Value = 100000000124000043,
                                                Workers = 100000
                                            }
                                        }
                                    },
                                    {
                                        new WorkSite()
                                        {
                                            Name = "SimplLarge HQ BuildSite",
                                            BuildingWorkCost = 964367326.222d,
                                            MaterialCost = 2313623d,
                                            Owner = new Company()
                                            {
                                                Name = "SimplLarge",
                                                CreationTime = new DateTime(2006, 8, 20),
                                                Value = 132,
                                                Workers = 54
                                            }
                                        },
                                        new Building()
                                        {
                                            Name = "SimplLarge HQ",
                                            Address = "54321 London Bridge Street, London ZYY XXZ",
                                            Height = 2412.1d,
                                            Owner = new Company()
                                            {
                                                Name = "Stella Property, Bigger, Better",
                                                CreationTime = new DateTime(2016, 6, 19),
                                                Value = 1000000000000000043,
                                                Workers = 100000
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            Stopwatch tmr = Stopwatch.StartNew();
            var result = ABSaveConvert.SerializeABSave(testClass, ABSaveType.WithOutNames);
            tmr.Stop();
            Console.WriteLine(tmr.ElapsedMilliseconds);

            // Assert
            Assert.AreEqual("UT\u0001\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Solar_System, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Milky Way\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Planet, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Mercury\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Firefox\u0001Can survive ultra-high temperatures. Loves digging holes.\u000123\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003AShift\u0001Shifts in and out of existence.\u0001100000\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Unknown\u0001Unknown\u00010\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Unknown #2\u0001Unknown #2\u00010\u0005\u0005\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Big Hole\u0001Somewhere\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Unknown\u00010\u00010\u00010\u000510000\u0001420768000000000\u0005\u0005\u0006ABSoftware.ABSave.Tests.UnitTests.TestObjects.WorkSite\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Biggest ever hole\u0001Somewhere Else\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Unknown\u00011\u00012\u00010\u0005100000000\u0001631196064000000000ABSoftware.ABSave.Tests.UnitTests.TestObjects.Planet, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Earth\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Human\u0001Some mysterious creature that only humans (and maybe dolphins ;) ) know about.\u000160078000\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.Animal, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Cat\u0001Cat Videos.\u0001100\u0005\u0005\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003The Shard\u000132 London Bridge Street, London SE1 9SG\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Stella Property\u00011000000000000\u0001100000\u0001632862720000000000\u000510000.3\u0001622994976000000000\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Unnamed\u000125 London Bridge Street, London XYZ XYZ\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Elegantly Small\u00011050101421\u000173\u0001631920960000000000\u0005110.528\u0001632291616000000000\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Simply Square\u0001176 XYZ Street, London XYZ ZYX\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003SimplBuild\u0001153\u00013\u0001636243552000000000\u0005240.24\u0001632777184000000000\u0005\u0005\u0006ABSoftware.ABSave.Tests.UnitTests.TestObjects.WorkSite\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003BrickShard\u00012521463 London Bridge Street, London XXZ XXZ\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Stella Property, Bigger, Better\u0001100000000124000043\u0001100000\u0001636018912000000000\u00052412.1\u00010\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.WorkSite\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Building, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003SimplLarge HQ\u000154321 London Bridge Street, London ZYY XXZ\u0001ABSoftware.ABSave.Tests.UnitTests.TestObjects.Company, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003Stella Property, Bigger, Better\u00011000000000000000043\u0001100000\u0001636018912000000000\u00052412.1\u00010", result);
        }
        #endregion

        #region Named
        [TestCategory("Serialization, Named, With Types"), TestMethod]
        public void SerializeABSave_NAMEDSaveObject_ReturnsString()
        {
            // Arrange
            var testClass = new TestClass();

            // Act
            var result = ABSaveConvert.SerializeABSave(testClass, ABSaveType.WithNames);

            // Assert
            Assert.AreEqual("NT\u0001str\u0001Oh, Hello!\u0001i\u0001365\u0001nextCl\u0001" + ABSaveWriter.WriteType(typeof(NextClass)) + "\u0003yoy\u0001F\u0005lstOfStr\u0004FirstStr\u0001SecondStr", result);
        }

        [TestCategory("Serialization, Named, With Types"), TestMethod]
        public void SerializeABSave_WITHTYPESSaveObject_ReturnsString()
        {
            // Arrange
            var testClass = new MultiObjectTest();

            // Act
            var result = ABSaveConvert.SerializeABSave(testClass, ABSaveType.WithNames);

            // Assert
            Assert.AreEqual("NT\u0001Str\u0001hmmmm, ok\u0001InnerArr\u0004ABSoftware.ABSave.Tests.UnitTests.TestObjects.TestObjects.MultiObjectInner, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003OK\u000136\u0005ABSoftware.ABSave.Tests.UnitTests.TestObjects.TestObjects.MultiObjectInner, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003OK\u000125\u0005\u0005InnerABSoftware.ABSave.Tests.UnitTests.TestObjects.TestObjects.MultiObjectInner, ABSoftware.ABSave.Tests.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\u0003OK\u00013612", result);

        }

        [TestCategory("Serialization, Named, Without Types"), TestMethod]
        public void SerializeABSave_NOTYPESSaveObject_ReturnsString()
        {
            // Arrange
            var testClass = new MultiObjectTest();

            // Act
            var result = ABSaveConvert.SerializeABSave(testClass, ABSaveType.WithNames, false);

            // Assert
            Assert.AreEqual("NF\u0001Str\u0001hmmmm, ok\u0001InnerArr\u0004\u0003OK\u000136\u0005\u0003OK\u000125\u0005\u0005Inner\u0003OK\u00013612", result);

        }
        #endregion
    }
}
