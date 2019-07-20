using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Tests.UnitTests.TestObjects
{
    public class ComplexSerializeTestClass
    {
        public List<Solar_System> SolarSystems;

        public static ComplexSerializeTestClass TestInstance = new ComplexSerializeTestClass()
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
    }

    public class Planet
    {
        public string Name;
        public List<Animal> Animals;
        public List<Building> Buildings;

        public Dictionary<WorkSite, Building> Worksites;
    }

    public class Animal
    {
        public string Name;
        public string Description;
        public int IQ;
    }

    public class Building
    {
        public string Name;
        public string Address;
        public Company Owner;
        public double Height;
        public DateTime CreationTime;
    }

    public class WorkSite
    {
        public string Name;
        public double CurrentInvestedMoney
        {
            get
            {
                return MaterialCost + BuildingWorkCost;
            }
        }
        public Company Owner;

        public double MaterialCost;
        public double BuildingWorkCost;
    }

    public class Company
    {
        public string Name;
        public Int64 Value;
        public int Workers;
        public DateTime CreationTime;
    }

    public class Solar_System
    {
        public string Name;
        public List<Planet> Planets;
    }
}
