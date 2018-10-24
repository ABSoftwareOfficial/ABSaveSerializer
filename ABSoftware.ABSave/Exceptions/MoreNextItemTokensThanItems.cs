using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions
{
    /// <summary>
    /// When there are too many "Next Item" tokens in a single object.
    /// </summary>
    public class MoreNextItemTokensThanActualItems : Exception
    {
        public MoreNextItemTokensThanActualItems(string message) : base(message) { }
    }
}
