using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// An exception for when <see cref="ABSaveType."/>
    /// </summary>
    public class InvalidABSaveTypeWhenSerializing : Exception
    {
        public InvalidABSaveTypeWhenSerializing(string message) : base(message) { }
    }
}
