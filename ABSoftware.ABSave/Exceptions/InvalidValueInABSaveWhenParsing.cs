using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// When there is an invalid value in ABSave - such as a number being written as "32ha".
    /// </summary>
    public class InvalidValueInABSaveWhenParsing : Exception
    {
        public InvalidValueInABSaveWhenParsing(string message) : base(message) { }
    }
}
