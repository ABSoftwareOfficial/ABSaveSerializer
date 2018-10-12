// ***********************************************************************
// Assembly         : ABSoftware.ABSave
// Author           : Alex
// Created          : 02-24-2018
//
// Last Modified By : Alex
// Last Modified On : 03-31-2018
// ***********************************************************************
// <copyright file="ABSaveConvert.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABSoftware.ABSave.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Serialization
{
    // Table of characters to use:
    // \u0001 - Represents "next value"
    // \u0003 - Represents "starting an object" - it is ended with a "lower innerLevel" sign
    // \u0004 - Represents "starting an array" - it is ended with a "lower innerLevel" sign
    // \u0005 - The "lower innerLevel sign"
    // \u0006 - The "DICTIONARY" starting marker
    /// <summary>
    /// Represents a class for converting to and from ABSave and C# objects.
    /// </summary>
    public static class ABSaveConvert
    {
        /// <summary>
        /// Turns a C# object into an ABSave string.
        /// </summary>
        /// <param name="obj">The object to "serialize".</param>
        /// <param name="version">Sticks a version into the header, if wanted.</param>
        /// <param name="type">The way of serializing this ABSave (unnamed/named).</param>
        /// <param name="writeHeader">Whether we should write the header in or not.</param>
        /// <returns>The ABSave string.</returns>
        public static string SerializeABSave(object obj, ABSaveType type, int version = 0, bool writeHeader = true)
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

            SerializeABSaveToStringBuilder(obj, type, ret, writeHeader, version);

            return ret.ToString();
        }

        /// <summary>
        /// Turns a C# object into ABSave and places the result in a StringBuilder.
        /// </summary>
        /// <param name="obj">The object to "serialize".</param>
        /// <param name=sb">The StringBuilder to put the result into.</param>
        public static void SerializeABSaveToStringBuilder(object obj, ABSaveType type, StringBuilder sb, bool writeHeader = true, int version = 0)
        {
            // If the object is null, don't bother.
            if (obj == null)
                return;

            // If the type is invalid, throw an exception.
            if (type == ABSaveType.Infer)
                throw new InvalidABSaveTypeWhenSerializing("You cannot use an 'infer' ABSaveType when serializing.");

            // Allows us to get all the variables - including public and non-public ones.
            var bindingFlags = BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public;

            // This gets all the fields - not the properties, since those have backing fields anyway.
            var fields = obj.GetType().GetFields(bindingFlags);

            // If we should write the header, write it.
            if (writeHeader)
            { 

                // Insert header ("U" for unnamed and "N" for named)
                sb.Append(type == ABSaveType.WithOutNames ? 'U' : 'N');

                // Insert version number - if needed.
                if (version != 0)
                    sb.Append(version);

                // End the header off.
                sb.Append('\u0001');
            }
            // This is a variable used across the whole process to decide whether this is the first one or not (to write the "Next Item" character or not).
            bool notFirst = false;

            // Keep track of what type the last property was - this allows us to decide whether to add the Next Item character to the next item.
            var lastType = ABSavePrimitiveType.Unknown;

            // Go through each variable.
            for (int i = 0; i < fields.Count(); i++)
            {
                // If we're doing it named - write the name.
                if (type == ABSaveType.WithNames)
                {
                    // Write the name out, don't write the Next Instruction character if it's the first item or the last item had a "lowerInnerlevel" sign after it.
                    ABSaveWriter.WriteString(fields[i].Name, ABSaveUtils.RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, true, sb);

                    // Since we've written the name out... And the "notFirst" variable is used to determine whether to write the next instruction symbol or not... Set "notFirst" to true since it will HAVE to have the next instruction symbol now.
                    notFirst = true;
                }

                // Serialize each variable, to the StringBuilder. If the last type was an array or "Unknown"... That means it 
                ABSaveSerializer.Serialize(fields[i].GetValue(obj), type, out lastType, true, sb, ABSaveUtils.RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, (i == fields.Count() - 1));

                // Update the "notFirst" variable if it's false and we've gone through one item.
                if (!notFirst)
                    notFirst = true;
            }
        }
    }
}
