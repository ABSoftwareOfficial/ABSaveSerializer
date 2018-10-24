using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// When there is no name to match with a value, when parsing.
    /// </summary>
    public class MissingNameToValueWhenParsing : Exception
    {
        public MissingNameToValueWhenParsing(string message) : base(message) { }
    }
}
