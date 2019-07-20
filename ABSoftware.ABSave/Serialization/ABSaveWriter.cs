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
    /// Handles any "writing" for ABSave, this could be anything from writing a type, to escaping a string.
    /// </summary>
    public static class ABSaveWriter
    {

        public static string WriteDotAndInt32(int num, bool writeToSB, StringBuilder sb)
        {
            string ret = "";
            ret += WriteCharacter('.', writeToSB, sb);
            ret += WriteByteArray(ABSaveUtils.ConvertNumberToByteArray(num, TypeCode.Int32), writeToSB, sb);
            return ret;
        }

        public static string WriteCharacter(char ch, bool writeToSB, StringBuilder sb)
        {
            if (writeToSB)
                sb.Append(ch);
            else
                return new string(ch, 1);

            return "";
        }

        public static void WriteHeader(ABSaveType type, StringBuilder sb, ABSaveSettings settings, bool writeHeader)
        {
            // If we shouldn't write the header, then don't do it.
            if (!writeHeader)
                return;

            // Insert the type ("U" for unnamed and "N" for named and types, "V" for unnamed without types, "M" for named without types)
            if (settings.WithTypes)
                sb.Append(type == ABSaveType.NoNames ? 'U' : 'N');
            else
                sb.Append(type == ABSaveType.NoNames ? 'V' : 'M');

            // Insert version number - if needed.
            if (settings.HasVersion)
                sb.Append(settings.Version);

            // End the header off.
            sb.Append('\u0001');
        }

        private static unsafe string CharArrayToString(char[] ret, int significantCharacters = -1)
        {
            // If the significantCharacters given is -1, that means it's unknown, so, just set it to the amount of characters in "ret".
            if (significantCharacters == -1)
                significantCharacters = ret.Length;

            var finalStr = new string('\0', significantCharacters);
            fixed (char* fixedFinalPointer = finalStr, fixedRetPointer = ret)
            {
                // Create a non-fixed pointer for each.
                var finalPointer = fixedFinalPointer;
                var retPointer = fixedRetPointer;

                // Go through each character and add it to the string.
                for (int i = 0; i < significantCharacters; i++)
                    *finalPointer++ = *retPointer++;
            }

            // Now, return the finished string.
            return finalStr;
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
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="numType">The type that the number is.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteNumerical(dynamic num, TypeCode numType, bool writeLengthBytes = true, bool useSB = false, StringBuilder sb = null)
        {
            // NOTE: WE DON'T NEED THE "Next Instruction" CHARACTER BECAUSE \A AND \B ARE ACTUALLY ESCAPED!
            // Create a variable to store what we'll return.
            var ret = "";

            // Write the number as a byte array.
            byte[] numberBytes = ABSaveUtils.ConvertNumberToByteArray(num, numType);

            // Next, work out how many of those bytes actually contain something (we don't want to waste space on trailing "\0"s)
            numberBytes = RemoveTrailingZeroBytesInByteArray(numberBytes);

            // Then, if we're going to write the "length bytes", then do it.
            var lengthBytes = WriteNumericalLengthBytes(writeLengthBytes, numberBytes);

            // Write out those "length bytes" now.
            ret += WriteByteArray(lengthBytes, useSB, sb);

            // Write the actual number - as a byte array.
            ret += WriteByteArray(numberBytes, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        public static string WriteByteArray(byte[] arr, bool useSB = false, StringBuilder sb = null)
        {
            var ret = "";

            // Go through each item in the byte array and convert it into a character, then add it.
            for (int i = 0; i < arr.Length; i++)
                if (useSB)
                    sb.Append((char)arr[i]);
                else
                    ret += (char)arr[i];

            return ret;
        }

        private static byte[] RemoveTrailingZeroBytesInByteArray(byte[] numberBytes)
        {
            byte significantAmount = (byte)numberBytes.Length;

            // First, find out how many trailing "0"s there are.
            for (int i = numberBytes.Length - 1; i >= 0; i--)
                if (numberBytes[i] == 0)
                    significantAmount--;
                else
                    break;

            // Then, take the array and replace it with a non-trailing array.
            var newArray = new byte[significantAmount];
            Array.Copy(numberBytes, 0, newArray, 0, significantAmount);

            return newArray;
        }

        /// <summary>
        /// Writes out the "length bytes" for a numerical.
        /// </summary>
        /// <returns>The length bytes</returns>
        private static byte[] WriteNumericalLengthBytes(bool writeLengthBytes, byte[] number)
        {
            // If we're not writing the length bytes, then just write the number and don't do anything else.
            if (!writeLengthBytes)
                return new byte[0];

            // Then, figure out what the length bytes will be based on how many significant bytes there were.
            switch (number.Length)
            {
                // Write a null character if there was no number (or it was 0), and just stop there.
                case 0:
                    return new byte[] { 0 };

                // If there was only one byte, write a "next item" character.
                case 1:
                    return new byte[] { 1 };

                // Write the "Two Bytes" if there were two bytes.
                case 2:

                    return new byte[] { 7 };

                // If there were any more, we'll use "\u0008" followed by how many.
                default:

                    return new byte[] { 8, (byte)number.Length };
            }
        }

        /// <summary>
        /// Writes the opening for an object either to a string or StringBuilder.
        /// </summary>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string WriteObjectOpen(bool useSB = false, StringBuilder sb = null)
        {
            if (useSB)
                sb.Append('\u0003');
            else
                return "\u0003";

            return "";
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

        ///// <summary>
        ///// Converts a type to text.
        ///// Including: Assembly Name, Version, Culture, PublicKeyToken
        ///// </summary>
        ///// <param name="str">The type to shorten down.</param>
        ///// <param name="writeToSB">Whether we are writing to a StringBuilder or not.</param>
        ///// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        ///// <param name="writeToCharArray">Writes to a character array to return - it can do both a StringBuilder and character array at the same time.</param>
        ///// <returns></returns>
        //internal static unsafe char[] ShortenType(char[] str, bool writeToSB = false, StringBuilder sb = null, bool writeToCharArray = true)
        //{
        //    // For when writing to a string, we need to temporarly place all the characters into an array - 
        //    // and remember how many how many of those characters are used.
        //    char[] ret = null;
        //    var significantCharacters = 0;

        //    // If we're writing to a string, intialize the character array.
        //    if (writeToCharArray)
        //        ret = new char[str.Length];

        //    // Fix a pointer on the string we're escaping and possibly the string we need to write to.
        //    fixed (char* fixedCharPointer = str, fixedRetPointer = ret)
        //    {
        //        // Create a non-fixed pointer to move along both strings.
        //        var charPointer = fixedCharPointer;
        //        var retPointer = fixedRetPointer;

        //        // Now, move forward through the actual string.
        //        for (int i = 0; i < str.Length; i++)
        //        {
        //            var ch = *charPointer++;

        //            // Only write the non-space.
        //            if (ch != ' ')
        //            {
        //                if (writeToSB)
        //                    sb.Append(ch);
        //                if (writeToCharArray)
        //                    *retPointer++ = ch;

        //                // This counts as a significant character - we only care though if we're writing to a character array.
        //                if (writeToCharArray)
        //                    significantCharacters++;
        //            }
        //        }
        //    }

        //    // Strip out the unneeded characters at the end of the character array.
        //    if (writeToCharArray)
        //    {
        //        var finalArray = new char[significantCharacters];
        //        Array.Copy(ret, finalArray, significantCharacters);

        //        // Now, we can write the final, stripped down character array.
        //        return finalArray;
        //    }

        //    // Now, turn the character array into a string - if we need to.
        //    //if (!useSB)
        //    //    CharArrayToString(out ret, significantCharacters, out finalStr);

        //    // If "writeToCharArray" was false, then, of course, we didn't need to write to a character array, and so we'll just return nothing.
        //    return null;
        //}

        /// <summary>
        /// Escapes a string - by escaping (with a "\") the characters: \u0000, \u0001, \u0002, \u0003, \u0004, \u0005, \u0006. Which are rarely used anyway.
        /// </summary>
        /// <param name="str">The string to escape</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <returns>The escaped string... If we weren't writing it to a StringBuilder.</returns>
        public static unsafe string Escape(string str, bool useSB = false, StringBuilder sb = null)
        {
            // For when writing to a string, we need to temporarly place all the characters into an array - 
            // and remember how many how many of those characters are used.
            char[] ret = null;
            var significantCharacters = 0;

            // If we're writing to a string, initialize the character array with the double the correct number of characters,
            // so that every character can possibly be escaped.
            if (!useSB)
                ret = new char[str.Length * 2];

            // Fix a pointer on the string we're escaping and possibly the string we need to write to.
            fixed (char* fixedCharPointer = str, fixedRetPointer = ret)
            {
                // Create a pointer to move along both strings.
                var charPointer = fixedCharPointer;
                var retPointer = fixedRetPointer;

                // Go through each character and write them.
                for (int i = 0; i < str.Length; i++)
                {
                    var ch = *charPointer++;

                    // If it's one of the characters that need to be escaped... add a "\".
                    if (ch >= 1 && ch <= 8)
                        if (useSB)
                            sb.Append('\\');
                        else
                        {
                            *retPointer++ = '\\';

                            // Count this as a significant character.
                            significantCharacters++;
                        }

                    // Add the character.
                    if (useSB)
                        sb.Append(ch);
                    else
                        *retPointer++ = ch;

                    // Count this as another significant character.
                    if (!useSB)
                        significantCharacters++;
                }
            }

            // Now, turn the character array into a string and return it - if we need to.
            if (!useSB)
                return CharArrayToString(ret, significantCharacters);

            // If we got here, we wrote a StringBuilder, so we don't need to return anything.
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
