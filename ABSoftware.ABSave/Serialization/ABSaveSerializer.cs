// ***********************************************************************
// Assembly         : ABSoftware.ABSave
// Author           : Alex
// Created          : 02-24-2018
//
// Last Modified By : Alex
// Last Modified On : 03-31-2018
// ***********************************************************************
// <copyright file="ABSaveSerializer.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ABSoftware.ABSave.ABSaveUtils;
using System.Globalization;

namespace ABSoftware.ABSave.Serialization
{
    /// <summary>
    /// Handles serialization for ABSave.
    /// </summary>
    public static class ABSaveSerializer
    {

        /// <summary>
        /// Serializes an object. The object could be anything.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="determinedType">The primitive type which has been decided for it.</param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it is serializing the first object in a class.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string Serialize(dynamic obj, ABSaveType type, out ABSavePrimitiveType determinedType, bool useSB = false, StringBuilder sb = null, bool writeNextInstructionSymbol = true, bool dnWriteEndLevel = false)
        {
            // This will be what to return if useSB is false.
            string ret = "";

            // Remember what type the object is.
            Type objType = obj.GetType();

            // For now, make it so we output "unknown".
            determinedType = ABSavePrimitiveType.Unknown;

            // Check if the object is null, if so, write the null symbol.
            if (obj == null)
                return ABSaveWriter.WriteNullItem(useSB, sb);

            // If the object is a string - write it as a string.
            if (obj is string)
            {
                ret = ABSaveWriter.WriteString(obj, writeNextInstructionSymbol, useSB, sb);
                determinedType = ABSavePrimitiveType.String;
            }

            // If the object is a number - write it as a number.
            else if (IsNumericType(objType))
            {
                ret = ABSaveWriter.WriteNumerical(obj, writeNextInstructionSymbol, useSB, sb);
                determinedType = ABSavePrimitiveType.Number;
            }

            // If the object is an array - serialize it as an array.
            else if (IsArray(objType))
            {
                ret = SerializeArray(obj, type, useSB, sb, dnWriteEndLevel);
                determinedType = ABSavePrimitiveType.Array;
            }

            // If the object is a dictionary - serialize it as a dictionary.
            else if (IsDictionary(objType))
            {
                ret = SerializeDictionary(obj, type, useSB, sb, dnWriteEndLevel);
                determinedType = ABSavePrimitiveType.Dictionary;
            }

            // If the object is a boolean - serialize it as a boolean.
            else if (obj is bool)
            {
                ret = SerializeBool(obj, writeNextInstructionSymbol, useSB, sb);
                determinedType = ABSavePrimitiveType.Boolean;
            }

            // If the object is a DateTime - serialize it as a DateTime.
            else if (obj is DateTime)
            {
                ret = SerializeDateTime(obj, writeNextInstructionSymbol, useSB, sb);
                determinedType = ABSavePrimitiveType.DateTime;
            }

            // Otherwise, we'll attempt to find a built-in type converter (to a string)
            else
            {

                // Mark it as an object.
                determinedType = ABSavePrimitiveType.Object;

                // Attempt to get a converter for it.
                bool canBeTypeConverted = false;
                TypeConverter typeConv = TypeDescriptor.GetConverter(objType);

                // Check if the type converter can actually convert it to a string.
                if (typeConv.IsValid(obj))
                    if (typeConv.CanConvertTo(typeof(string)))
                        canBeTypeConverted = true;

                // If it can be type converted, convert it using that, and then write it as a string.
                if (canBeTypeConverted)
                    ret = ABSaveWriter.WriteString(typeConv.ConvertToString(obj), writeNextInstructionSymbol, useSB, sb);

                // Otherwise, if it can't be type converted... Manually convert it.
                else
                    ret = SerializeObject(obj, type, objType, writeNextInstructionSymbol, useSB, sb, dnWriteEndLevel);
            }

            // Return the result from this.
            return ret;
        }

        /// <summary>
        /// Serializes an object. The object could be anything.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it is serializing the first object in a class.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string Serialize(dynamic obj, ABSaveType type, bool useSB = false, StringBuilder sb = null, bool writeNextInstructionSymbol = true, bool dnWriteEndLevel = false)
        {
            var outEmpty = ABSavePrimitiveType.Unknown;
            return Serialize(obj, type, out outEmpty, useSB, sb, writeNextInstructionSymbol, dnWriteEndLevel);
        }

        /// <summary>
        /// Serializes a boolean.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it serializing the first object in a class.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>    
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeBool(bool obj, bool writeNextInstructionSymbol = true, bool useSB = false, StringBuilder sb = null)
        {
            // Serialize it (use "T" or "F") and then write it as a string - it will return nothing if it needed to write to a StringBuilder.
            return ABSaveWriter.WriteString(obj ? "T" : "F", writeNextInstructionSymbol, useSB, sb);
        }

        /// <summary>
        /// An object (with multiple properties) to serialize manually - one that doesn't have a TypeConverter.
        /// </summary>
        /// <param name="obj">The object to serialize manually</param>
        /// <param name="objType">The type of the object to serialize manually</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeObject(object obj, ABSaveType type, Type objType, bool writeNextInstructionSymbol = true, bool useSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            // Create a variable to store what we'll return - if we aren't using a StringBuilder.
            var ret = "";

            // If we need to, write a "next step" symbol.
            ret += ABSaveWriter.WriteNextItem(writeNextInstructionSymbol, useSB, sb);

            // First of all, write the opening for the object.
            ret += ABSaveWriter.WriteObjectOpen(objType, useSB, sb);

            // Write the actual object, use the correct method for either string or for a StringBuilder.
            if (useSB)
                ABSaveConvert.SerializeABSaveToStringBuilder(obj, type, sb, false);
            else
                ret += ABSaveConvert.SerializeABSave(obj, type, 0, false);

            // Finally, write the ending for the object.
            ret += ABSaveWriter.WriteObjectClose(dnWriteEndLevel, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        /// <summary>
        /// An object (with multiple properties) to serialize manually - one that doesn't have a TypeConverter.
        /// </summary>
        /// <param name="obj">The object to serialize manually</param>
        /// <param name="objType">The type of the object to serialize manually</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeArray(dynamic obj, ABSaveType type, bool useSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            // Create a variable to store what we'll return.
            var ret = "";

            // Keep track of whether we're on the first item or not.
            bool notFirst = false;

            // Write the opening for the array.
            ret += ABSaveWriter.WriteArrayOpening(useSB, sb);

            // Keep track of what type the last property was - this allows us to decide whether to add the Next Item character to the next item.
            var lastType = ABSavePrimitiveType.Unknown;

            // Now, go through each item in the array.
            for (int i = 0; i < obj.Count; i++)
            {
                // Serialize the item and write to either the StringBuilder or the "ret"...
                // Now, we're not sure whether this array uses indexers or "ElementAt()"... So, we'll try an indexer, and if that doesn't work - just use "ElementAt".
                try
                { 
                    ret += Serialize(obj[i], type, out lastType, useSB, sb, RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, dnWriteEndLevel);
                } catch (Exception) {
                    ret += Serialize(obj.ElementAt(i), type, out lastType, useSB, sb, RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, dnWriteEndLevel);
                }

                // Update the "notFirst" variable.
                if (!notFirst)
                    notFirst = true;
            }

            // Write the closing for the array.
            ret += ABSaveWriter.WriteObjectClose(dnWriteEndLevel, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        /// <summary>
        /// Serializes a dictionary.
        /// </summary>
        /// <param name="obj">The dictionary to serialize.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeDictionary(dynamic obj, ABSaveType type, bool useSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            // Create a variable to store what we'll return.
            var ret = "";

            // Keep track of whether we're on the first item or not.
            bool notFirst = false;

            // Write the opening for the array.
            ret += ABSaveWriter.WriteDictionaryOpening(useSB, sb);

            // Now, go through each item in the dictionary.
            foreach (dynamic element in obj)
            {
                // If this isn't the first item, write the "next item" character.
                if (notFirst)
                    ret += ABSaveWriter.WriteNextItem(true, useSB, sb);

                // Add the key to it.
                if (useSB)
                    sb.Append(element.Key);
                else
                    ret += element.Key;

                // Serialize the item and write to either the StringBuilder or the "ret"
                ret += Serialize(element.Value, type, useSB, sb, true, dnWriteEndLevel);

                // Update the "notFirst" variable if needed.
                if (!notFirst)
                    notFirst = true;
            }

            // Write the closing for the dictionary.
            ret += ABSaveWriter.WriteObjectClose(dnWriteEndLevel, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        /// <summary>
        /// Serializes a DateTime - using the number of ticks on a DateTime.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it is serializing the first object in a class.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeDateTime(DateTime obj, bool writeNextInstructionSymbol = false,  bool useSB = false, StringBuilder sb = null)
        {
            // Write the ticks, we're using "WriteNumerical" because the ticks are a long.
            return ABSaveWriter.WriteNumerical(obj.Ticks, writeNextInstructionSymbol, useSB, sb);
        }

        // Old DateTime function
        //public static string SerializeDateTime(DateTime obj, bool UseSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        //{
        //    string ret = "";

        //    string yearAsString = obj.Year.ToString();
        //    List<byte> yearBytes = new List<byte>();

        //    for (int i = 0; i < yearAsString.Length; i += 2)
        //        if (i < yearAsString.Length - 1)
        //            yearBytes.Add(ConvertNumericalCharsToByte(yearAsString[i], yearAsString[i + 1]));
        //        else
        //            yearBytes.Add(ConvertNumericalCharsToByte(yearAsString[i], '0'));

        //    bool DoMinute = false;

        //    List<byte> finalBytes = new List<byte>();
        //    // All the bytes:
        //    // Byte 1: The month and first digit of year.
        //    // Byte 2: The hour and two bits reserved! (One bit thrown away)
        //    // Byte 3: The day and whether to store (Minute, Second, Millisecond)

        //    // ============= Unfixed (bytes could be skipped) - Depends one whether these are required =============

        //    // Byte 4: The minute/second/millisecond ending
        //    // Byte 5: The minute/second/millisecond ending
        //    // Byte 6: The minute/second/millisecond ending

        //    // Note: The millisecond ending is the last byte of the millisecond - the millisecond actually needs 10 bits, so we use the two reserved bits.
        //    // Another Note: The minute/second have two bits left over, if both the minute and second are there, this can be used to store another year digit.

        //    // Final Bytes: The rest of the year.

        //    // (byte) & 15 is used to get the last four bits of a byte! (byte) >> 4 is used to get the first four bits (the ones marked with "X"s - "XXXX0000").

        //    finalBytes.Add(Convert.ToByte(obj.Month << 4));
        //    finalBytes[0] += Convert.ToByte(yearBytes[0] >> 4);
        //    finalBytes.Add(Convert.ToByte(obj.Hour << 3));
        //    finalBytes.Add(Convert.ToByte(obj.Day << 3));

        //    if (obj.Minute > 0)
        //    {
        //        DoMinute = true;
        //        finalBytes[2] += 4; // XXXXX1XX
        //        finalBytes.Add(Convert.ToByte(obj.Minute << 2));
        //    }

        //    if (obj.Second > 0)
        //    {
        //        finalBytes[2] += 2; // XXXXXX1X
        //        finalBytes.Add(Convert.ToByte(obj.Second << 2));

        //        if (DoMinute) // If we're doing the minute AS WELL - meaning we can stick another digit of the year in!
        //        {
        //            finalBytes[3] += Convert.ToByte(yearBytes[0] & 15 >> 2);
        //            finalBytes[4] += Convert.ToByte(yearBytes[0] & 7);
        //        }
        //    }

        //    if (obj.Millisecond > 0)
        //    {
        //        finalBytes[2] += 1; // XXXXXXX1
        //        finalBytes.Add(Convert.ToByte(obj.Millisecond >> 2));
        //        finalBytes[1] += Convert.ToByte(obj.Millisecond & 7); // Put the last two bits over onto the two reserved bits
        //    }

        //    for (int i = 0; i < finalBytes.Count; i++)
        //        ret += Convert.ToString(finalBytes[i]);

        //    for (int i = (DoMinute) ? 1 : 0; i < finalBytes.Count; i++)
        //        if (i == 0)
        //            ret += Convert.ToChar((finalBytes[i] & 15));
        //        else
        //            ret += Convert.ToChar(finalBytes[i]);

        //    return ret;

        //}
    }
}
