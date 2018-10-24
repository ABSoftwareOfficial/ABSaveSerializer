using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// An exception for when <see cref="ABSaveType.Infer"/> is used on a serialize method.
    /// </summary>
    public class InferABSaveTypeWhenSerializing : Exception
    {
        public InferABSaveTypeWhenSerializing(string message) : base(message) { }
    }
}
