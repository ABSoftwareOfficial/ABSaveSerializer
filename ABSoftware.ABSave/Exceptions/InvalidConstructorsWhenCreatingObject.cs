using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// When there aren't ANY useable constructors in a class when creating an instance of it.
    /// </summary>
    public class InvalidConstructorsWhenCreatingObject : Exception
    {
        public InvalidConstructorsWhenCreatingObject(string message) : base(message) { }
    }
}
