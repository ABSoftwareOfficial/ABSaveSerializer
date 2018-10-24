using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// When an unexpected token was found while parsing.
    /// </summary>
    public class UnexpectedTokenWhenParsing : Exception
    {
        public UnexpectedTokenWhenParsing(string message) : base(message) { }
    }
}
