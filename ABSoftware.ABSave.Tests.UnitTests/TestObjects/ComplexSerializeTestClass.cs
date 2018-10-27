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
