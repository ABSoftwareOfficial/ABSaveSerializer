using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave
{
    /// <summary>
    /// Handles general writing utilities for ABSave, such as escaping things.
    /// </summary>
    public static class ABSaveWriter
    {
        public static string WriteType(Type typ)
        {
            return "\u0002" + typ.FullName;
        }

        public static string Escape(string str)
        {
            return str;
        }
    }
}
