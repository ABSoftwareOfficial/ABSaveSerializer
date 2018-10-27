using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Tests.UnitTests.TestObjects.TestObjects
{
    /// <summary>
    /// A test class used to test multiple objects within each other - mainly used for testing "Show Types" and things like that.
    /// </summary>
    public class MultiObjectTest
    {
        public string Str = "hmmmm, ok";
        public List<MultiObjectInner> InnerArr = new List<MultiObjectInner>()
        {
            new MultiObjectInner(36),
            new MultiObjectInner(25)
        };

        public MultiObjectInner Inner = new MultiObjectInner(3612);
    }

    public class MultiObjectInner
    {
        public int OK = 7;

        public MultiObjectInner(int ok)
        {
            OK = ok;
        }
    }
}
