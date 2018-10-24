// ***********************************************************************
// Assembly         : ABSoftware.ABSave
// Author           : Alex
// Created          : 02-24-2018
//
// Last Modified By : Alex
// Last Modified On : 02-28-2018
// ***********************************************************************
// <copyright file="ABSaveWriter.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Serialization
{
    /// <summary>
    /// Handles general writing utilities for ABSave, such as escaping things.
    /// </summary>
    public static class ABSaveWriter
    {
        /// <summary>
        /// Writes the type, as a string, of a type.
        /// </summary>
        /// <param name="typ">The type to write as a string.</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>Returns the type as a string.</returns>
        public static string WriteType(Type typ, bool writeTypes = true, bool useSB = false, StringBuilder sb = null)
        {
            // Don't do anything if we're not meant to.
            if (!writeTypes)
                return "";

            // If we're using the StringBuilder - write to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append(typ.AssemblyQualifiedName);
            else
                return typ.AssemblyQualifiedName;

            // If we made it here - that means we were using a StringBuilder... So, just return nothing since it won't be used.
            return "";
        }

        /// <summary>
        /// Escapes a string - by escaping (with a "\") the characters: \u0001, \u0003, \u0004, \u0005, \u0006. Which are rarely used anyway.
        /// </summary>
        /// <param name="str">The string to escape</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>The escaped string... If we weren't writing it to a StringBuilder.</returns>
        public static string Escape(string str, bool useSB = false, StringBuilder sb = null)
        {
            // If we aren't doing it to a StringBuilder... Just do this basic logic:
            if (!useSB)
                return str.Replace("\u0001", "\\\u0001").Replace("\u0003", "\\\u0003").Replace("\u0004", "\\\u0004").Replace("\u0005", "\\\u0005").Replace("\u0006", "\\\u0006");

            // Otherwise, go through each character and append them to the StringBuilder... If they need to be escaped append a "\" before it.
            for (int i = 0; i < str.Length; i++)
            {
                // If it's one of the character which need to be escaped... add a "\".
                if (str[i] == '\u0001' || str[i] == '\u0003' || str[i] == '\u0004' || str[i] == '\u0005' || str[i] == '\u0006')
                    sb.Append('\\');

                // Add the character.
                sb.Append(str[i]);
            }

            // If we got here, we were writing to a StringBuilder - so, return nothing.
            return "";
        }

        /// <summary>
        /// Writes the "next item" character to either a string or StringBuilder.
        /// </summary>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteNextItem(bool writeNextInstructionSymbol = true, bool useSB = false, StringBuilder sb = null)
        {
            // Don't do anything if we're not meant to.
            if (!writeNextInstructionSymbol)
                return "";

            // If we're using the StringBuilder - write to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append('\u0001');
            else
                return "\u0001";

            // If we made it here - that means we were using a StringBuilder... So, just return nothing since it won't be used.
            return "";
        }

        /// <summary>
        /// Writes a string, and either returns the result or writes it to a StringBuilder.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it is serializing the first object in a class.</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteString(string str, bool writeNextInstructionSymbol, bool useSB = false, StringBuilder sb = null)
        {
            // Create a variable to store what we'll return.
            var ret = "";

            // Write the next item character.
            ret += WriteNextItem(writeNextInstructionSymbol, useSB, sb);

            // Write the escaped string.
            ret += Escape(str, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        /// <summary>
        /// Writes a number, and either returns the result or writes it to a StringBuilder.
        /// </summary>
        /// <param name="str">The number to write.</param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it is serializing the first object in a class.</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteNumerical(dynamic num, bool writeNextInstructionSymbol, bool useSB = false, StringBuilder sb = null)
        {
            // Create a variable to store what we'll return.
            var ret = "";

            // Write the next item character.
            ret += WriteNextItem(writeNextInstructionSymbol, useSB, sb);

            // Write the number - for a StringBuilder, we can just pass the number in.
            if (useSB)
                sb.Append(num);
            else
                ret += num;

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        /// <summary>
        /// Writes the opening for an object either to a string or StringBuilder.
        /// </summary>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteObjectOpen(Type objType, bool showTypes = true, bool useSB = false, StringBuilder sb = null)
        {
            // Create a variable to store what we'll return.
            var ret = "";

            // Write the type.
            ret += WriteType(objType, showTypes, useSB, sb);

            // If we're using the StringBuilder - write the open object character to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append('\u0003');
            else
                ret += "\u0003";

            // Return the string result - this would be empty if we are using a StringBuilder though.
            return ret;
        }

        /// <summary>
        /// Writes the closing for an object or array either to a string or StringBuilder.
        /// </summary>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteObjectClose(bool dnWriteEndLevel = false, bool useSB = false, StringBuilder sb = null)
        {
            // Don't do anything if we're not meant to write anything.
            if (dnWriteEndLevel)
                return "";

            // If we're using the StringBuilder - write to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append('\u0005');
            else
                return "\u0005";

            // If we made it here - that means we were using a StringBuilder... So, just return nothing since it won't be used.
            return "";
        }

        /// <summary>
        /// Writes the opening for an array.
        /// </summary>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteArrayOpening(bool useSB = false, StringBuilder sb = null)
        {
            // If we're using the StringBuilder - write to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append('\u0004');
            else
                return "\u0004";

            // If we made it here - that means we were using a StringBuilder... So, just return nothing since it won't be used.
            return "";
        }

        /// <summary>
        /// Writes the opening for an array.
        /// </summary>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteDictionaryOpening(bool useSB = false, StringBuilder sb = null)
        {
            // If we're using the StringBuilder - write to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append('\u0006');
            else
                return "\u0006";

            // If we made it here - that means we were using a StringBuilder... So, just return nothing since it won't be used.
            return "";
        }

        /// <summary>
        /// Writes the "NULL" character for a null object.
        /// </summary>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteNullItem(bool useSB = false, StringBuilder sb = null)
        {
            // If we're using the StringBuilder - write to it... Otherwise, return the correct string.
            if (useSB)
                sb.Append('\u0002');
            else
                return "\u0002";

            // If we made it here - that means we were using a StringBuilder... So, just return nothing since it won't be used.
            return "";
        }
    }
}
