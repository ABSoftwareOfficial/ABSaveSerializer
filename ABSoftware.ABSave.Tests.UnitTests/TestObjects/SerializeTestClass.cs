using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Tests.UnitTests.TestObjects
{
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
}
