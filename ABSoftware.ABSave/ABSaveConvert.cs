using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave
{
    // Table of characters to use:
    // \u0001 - Represents "next value"
    // \u0002 - Represents "the type is" - it is ended with a "next object" sign
    // \u0003 - Represents "starting a object" - it is ended with a "lower innerLevel" sign
    // \u0004 - Represents "strating an array" - it is ended with a "lower innerLevel" sign
    // \u0005 - The "lower innerLevel sign"
    // \u0006 - The "DICTIONARY" starting marker
    /// <summary>
    /// Represents a class for converting to and from ABSave and C# objects.
    /// </summary>
    public class ABSaveConvert
    {
        /// <summary>
        /// Turns a C# object into an ABSave string.
        /// </summary>
        /// <param name="obj">The object to "serialize".</param>
        /// <returns>The ABSave string.</returns>
        public static string SerializeABSave(object obj)
        {
            string ret = "";

            if (obj != null)
            {       

                // Normal reflection stuff (the lower three variables)
                var bindingFlags = BindingFlags.Instance |
                       BindingFlags.NonPublic |
                       BindingFlags.Public;

                var fieldValues = obj.GetType()
                                     .GetFields(bindingFlags)
                                     .Select(field => field.GetValue(obj))
                                     .ToList();

                foreach (object val in fieldValues)
                    ret += ABSaveSerializer.Serialize(val);

                ret = ret.Trim('\u0001').TrimEnd('\u0005');
            }

            return ret;
        }
    }
}
