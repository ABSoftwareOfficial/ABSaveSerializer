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
using ABSoftware.ABSave.Deserialization;
using ABSoftware.ABSave.Exceptions;
using ABSoftware.ABSave.Exceptions.Base;
using ABSoftware.ABSave.Serialization;
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
    // \u0002 - The NULL character
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
        /// The main parser which is used over and over again to parse strings.
        /// </summary>
        //internal static ABSaveParser<T> MainParser;

        /// <summary>
        /// Turns a C# object into an ABSave string.
        /// </summary>
        /// <param name="obj">The object to "serialize".</param>
        /// <param name="version">Sticks a version into the header, if wanted.</param>
        /// <param name="type">The way of serializing this ABSave (unnamed/named).</param>
        /// <param name="writeHeader">Whether we should write the header in or not.</param>
        /// <param name="showTypes">Whether we should show types.</param>
        /// <param name="errorHandler">The way of handling errors through the process.</param>
        /// <returns>The ABSave string.</returns>
        public static string SerializeABSave(object obj, ABSaveType type, bool showTypes = true, int version = 0, bool writeHeader = true, ABSaveErrorHandler errorHandler = null)
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

            SerializeABSaveToStringBuilder(obj, type, ret, showTypes, writeHeader, version, errorHandler);
            return ret.ToString();
        }

        /// <summary>
        /// Turns a C# object into ABSave and places the result in a StringBuilder.
        /// </summary>
        /// <param name="obj">The object to "serialize".</param>
        /// <param name="type">The type of the ABSave Document.</param>
        /// <param name="sb">The StringBuilder to put the result into.</param>
        /// <param name="errorHandler">The way of handling errors through the process.</param>
        public static void SerializeABSaveToStringBuilder(object obj, ABSaveType type, StringBuilder sb, bool showTypes = true, bool writeHeader = true, int version = 0, ABSaveErrorHandler errorHandler = null)
        {
            // Make sure that the error handler isn't null.
            errorHandler = ABSaveErrorHandler.EnsureNotNull(errorHandler);

            // If the type is invalid, throw an exception.
            if (type == ABSaveType.Infer)
            {
                errorHandler.InferTypeWhenSerializing();
                return;
            }

            // If the object is null, don't bother with anything else.
            if (obj == null)
                return;

            // This gets all the variables.
            var members = ABSaveUtils.GetFieldsAndPropertiesWithValues(obj);

            // If we should write the header, write it.
            if (writeHeader)
            {
                // Insert header ("U" for unnamed and "N" for named)
                sb.Append(type == ABSaveType.WithOutNames ? 'U' : 'N');

                // Insert show types
                ABSaveSerializer.SerializeBool(showTypes, false, true, sb);

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
            for (int i = 0; i < members.Count; i++)
            {
                // If we're doing it named - write the name.
                if (type == ABSaveType.WithNames)
                {
                    // Write the name out, don't write the Next Instruction character if it's the first item or the last item had a "lowerInnerlevel" sign after it.
                    ABSaveWriter.WriteString(members.Items[i].Info.Name, ABSaveUtils.RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, true, sb);

                    // Since we've written the name out... And the "notFirst" variable is used to determine whether to write the next instruction symbol or not... Set "notFirst" to true since it will HAVE to have the next instruction symbol now.
                    notFirst = true;
                }

                // Serialize each variable, to the StringBuilder. If the last type was an array or "Unknown"... That means it 
                ABSaveSerializer.Serialize(members.Items[i].Value, type, out lastType, showTypes, true, sb, ABSaveUtils.RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, i == members.Count - 1, errorHandler);

                // Update the "notFirst" variable if it's false and we've gone through one item.
                if (!notFirst)
                    notFirst = true;
            }
        }

        /// <summary>
        /// Deserializes an ABSave document into an ABSave object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="str">The string to deserialize from.</param>
        /// <param name="type">The type of the ABSave Document.</param>
        /// <param name="errorHandler">The way of handling errors through the process.</param>
        /// <returns></returns>
        public static T DeserializeABSave<T>(string str, ABSaveType type, ABSaveErrorHandler errorHandler = null)
        {
            // Create a new parser.
            var parser = new ABSaveParser<T>(type, errorHandler);

            // Start the new parser.
            parser.Start(str);

            // Return the result.
            return parser.Result;
        }
    }
}
