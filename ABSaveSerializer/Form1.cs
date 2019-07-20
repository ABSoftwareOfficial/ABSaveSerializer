using ABSoftware.ABSave;
using ABSoftware.ABSave.Deserialization;
using ABSoftware.ABSave.Exceptions.Base;
using ABSoftware.ABSave.Serialization;
using ABSoftware.ABSave.Tests.UnitTests.TestObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace ABSaveSerializer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            var dtimeobj = new DateTime(2024, 4, 21, 7, 54, 32, 242);
            var dtimeres = ABSoftware.ABSave.Serialization.ABSaveSerializer.SerializeDateTime(dtimeobj);

            var testDict = new Dictionary<string, string>()
            {
                { "FirstKey", "FirstValue" },
                { "SecondKey", "SecondValue" }
            };

            // Act

            var result = ABSoftware.ABSave.Serialization.ABSaveSerializer.SerializeDictionary(testDict, ABSaveType.NoNames, new ABSaveSettings());

            TestClass test = new TestClass();
            textBox1.Text = ABSaveConvert.ObjectToABSave(test, ABSaveType.NoNames, new ABSaveSettings());

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

            textBox1.Text = ABSaveConvert.ObjectToABSave(testClass, ABSaveType.NoNames, new ABSaveSettings());

            //IDictionary<string, string> dict = new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2"} };
            //textBox1.Text = ABSoftware.ABSave.Serialization.ABSaveSerializer.Serialize(dict as IDictionary<string, string>, ABSaveType.WithNames);

            ////textBox1.Text = ABSaveConvert.SerializeABSave(new TestClass(), ABSaveType.WithOutNames);

            //// EXCEPTION TESTING:
            //var errHandler = new ABSoftware.ABSave.Exceptions.Base.ABSaveErrorHandler(ABSoftware.ABSave.Exceptions.Base.ABSaveError.InferTypeWhenSerializing);
            //errHandler.ErrorEncountered += (e2) =>
            //{
            //    MessageBox.Show(e2.ToString());
            //};

            //var maybe = ABSaveConvert.DeserializeABSave<UltimateTestClass>("UF\u0001Great\u00014727\u0001Str\u0001YES!YES!YES!", ABSaveType.WithNames);

            //var result2 = ABSaveDeserializer.Deserialize(ABSaveWriter.WriteType(typeof(TestClass)), typeof(TestClass), out ABSavePrimitiveType type, out bool parse);
            //MessageBox.Show(ABSoftware.ABSave.Serialization.ABSaveSerializer.Serialize(result2, ABSaveType.WithOutNames));

            //textBox1.Text = ABSaveConvert.SerializeABSave(new MultiObjectTest(), ABSaveType.WithNames);
            //var instance = ABSaveUtils.CreateInstance<UltimateTestClass>(errHandler, new Dictionary<string, object>() { { "Great", 4 } });
            //MessageBox.Show(instance.ToString());

            //var atest = new Dictionary<string, string>()
            //{
            //    { "FirstKey", "FirstValue" },
            //    { "SecondKey", "SecondValue" }
            //};

            //// Act

            //var aresult = ABSoftware.ABSave.ABSaveSerializer.SerializeDictionary(atest, ABSaveSerializeType.WithOutNames);

            //// Assert

            //textBox1.Text = aresult;

            DebugParser();

            NumberExperimenting();

        }

        public void DebugParser()
        {
            var headerTest = new char[] { 'u', 'n', 'v', 'm' };

            for (int i = 0; i < headerTest.Length; i++)
            {
                var parser = new ABSaveParser<TestClass>(ABSaveType.WithNames, new ABSaveSettings(new ABSaveErrorHandler(ABSoftware.ABSave.Exceptions.Base.ABSaveError.InvalidValueInABSaveWhenParsing, 
                    (e) => MessageBox.Show("ERROR: " + e.Message))));

                parser.Start(headerTest[i] + "\u0001str\u0001heyhey\u0001i\u00011617");

                Console.WriteLine("debug");
            }
        }

        private static void NumberExperimenting()
        {
            var num = 216161;
            var bytes = BitConverter.GetBytes(num);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Console.ReadLine();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            var num_recreate = BitConverter.ToInt32(bytes, 0);

            // Convert a decimal to bytes 
            Stopwatch watch = Stopwatch.StartNew();

            decimal dec = 261663061631136316003160.1616754194237272794327232324747478442882884282m;

            var decBytes = decimal.GetBits(dec);

            var final = new byte[16];

            BitConverter.GetBytes(decBytes[0]).CopyTo(final, 0);
            BitConverter.GetBytes(decBytes[1]).CopyTo(final, 4);
            BitConverter.GetBytes(decBytes[2]).CopyTo(final, 8);
            BitConverter.GetBytes(decBytes[3]).CopyTo(final, 12);

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + ": " + watch.ElapsedTicks);

            // Convert back.
            Stopwatch watch2 = Stopwatch.StartNew();

            var recreate_arr = new int[4];

            recreate_arr[0] = BitConverter.ToInt32(final, 0);
            recreate_arr[1] = BitConverter.ToInt32(final, 4);
            recreate_arr[2] = BitConverter.ToInt32(final, 8);
            recreate_arr[3] = BitConverter.ToInt32(final, 12);

            var finalDec = new decimal(recreate_arr);

            watch2.Stop();


            // Test byte lengths.
            var single = BitConverter.GetBytes(612316f);
            var dou = BitConverter.GetBytes(362361d);

            while (true)
            {
                var current = ABSaveWriter.WriteNumerical(626136617721, TypeCode.Int64, false);

                Console.WriteLine(watch2.ElapsedMilliseconds + ": " + watch2.ElapsedTicks);
            }


            //BitConverter.
        }
    }

    public class TestClass
    {
        public string str = "Oh, Hello!";
        public int i = 365;

        public NextClass nextCl = new NextClass();

        public List<string> lstOfStr = new List<string>()
        {
            "FirstStr",
            "SecondStr"
        };        
    }

    public class NextClass
    {
        public bool yoy = false;
    }

    public class UltimateTestClass
    {
        public int Great;
        public string Str;

        public UltimateTestClass(int great)
        {
            Great = great + 3;
        }

        public UltimateTestClass(bool great)
        {
            
        }

        public UltimateTestClass(int great, string str, bool k)
        {
            Great = great + 9;
            Str = str + "YEET";
        }
    }
}
