using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// An exception for when the header for an ABSave document is invalid when parsing.
    /// </summary>
    public class InvalidHeaderWhenParsing : Exception
    {
        public InvalidHeaderWhenParsing(string message) : base(message) { }
    }
}
