using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            StringBuilder ret = new StringBuilder();

            // NOTE: (tl;dr)
            // Why do we use a StringBuilder, and not just a string?
            // Well, there's one word to explain that:
            // Performance.
            // The ONLY reason we are using a StringBuilder instead of a string is because it's much faster.
            // HOWEVER, we don't want to just support StringBuilder - we want to allow users to use strings as well.
            // And THAT'S why we have a string mode (just set UseSB to false on most methods) and a StringBuilder mode!
            // StringBuilder is faster, but less flexible - I mean, you can do basically anything with a string, because it's a standard type.

            SerializeABSaveToStringBuilder(obj, ret);

            return ret.ToString();
        }

        /// <summary>
        /// Turns a C# object into ABSave and places the result in a StringBuilder.
        /// </summary>
        /// <param name="obj">The object to "serialize".</param>
        /// <returns>The ABSave string.</returns>
        public static void SerializeABSaveToStringBuilder(object obj, StringBuilder sb = null)
        {
            if (obj != null)
            {
                Stopwatch testReflection = Stopwatch.StartNew();
                // Normal reflection stuff (the lower three variables)
                var bindingFlags = BindingFlags.Instance |
                       BindingFlags.NonPublic |
                       BindingFlags.Public;

                var fieldValues = obj.GetType()
                                     .GetFields(bindingFlags)
                                     .Select(field => field.GetValue(obj))
                                     .ToList();

                testReflection.Stop();
                Console.WriteLine("TIME TAKEN FOR REFLECTION: " + testReflection.Elapsed);
                bool notfirst = false;

                Stopwatch testLoop = Stopwatch.StartNew();
                for (int i = 0; i < fieldValues.Count; i++)
                {
                    ABSaveSerializer.Serialize(fieldValues[i], true, sb, notfirst, (i == fieldValues.Count - 1) ? true : false);
                    if (!notfirst)
                        notfirst = true;
                }
                testLoop.Stop();
                Console.WriteLine("TIME TAKEN FOR LOOP: " + testReflection.Elapsed);

                Stopwatch testTrim = Stopwatch.StartNew();
                //ret = ret.TrimEnd('\u0005');
                testTrim.Stop();
                Console.WriteLine("TIME TAKEN FOR TRIM: " + testTrim.Elapsed);
            }
        }
    }
}
