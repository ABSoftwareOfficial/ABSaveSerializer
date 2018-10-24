using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// When there are too many constructors with a different case to the fields in a class.
    /// </summary>
    public class TooManyConstructorsWithDifferentCase : Exception
    {
        public TooManyConstructorsWithDifferentCase(string message) : base(message) { }
    }
}
