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
            return str.Replace("\u0001", "\\\u0001").Replace("\u0002", "\\\u0002").Replace("\u0003", "\\\u0003").Replace("\u0004", "\\\u0004").Replace("\u0005", "\\\u0005").Replace("\u0006", "\\\u0006");
        }
    }
}
