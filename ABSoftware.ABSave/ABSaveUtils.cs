// ***********************************************************************
// Assembly         : ABSoftware.ABSave
// Author           : Alex
// Created          : 02-25-2018
//
// Last Modified By : Alex
// Last Modified On : 02-28-2018
// ***********************************************************************
// <copyright file="ABSaveUtils.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABSoftware.ABSave.Exceptions;
using ABSoftware.ABSave.Exceptions.Base;
using ABSoftware.ABSave.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave
{
    internal enum ABSaveConstructorInferer
    {
        /// <summary>
        /// If there was no valid match
        /// </summary>
        Failed,

        /// <summary>
        /// If it had the same name, but at a different case (like lower case), remember that.
        /// </summary>
        NoCaseMatch,

        /// <summary>
        /// If it was a perfect match.
        /// </summary>
        Perfect
    }

    /// <summary>
    /// Various tools used across ABSave.
    /// </summary>
    public static class ABSaveUtils
    {
        #region Types
        public static bool IsArray(Type t)
        {
            if (t.IsArray)
                return true;

            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsNumericType(TypeCode tCode)
        {
            switch (tCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Whether the type is a dictionary.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>Whether it's a dictionary.</returns>
        public static bool IsDictionary(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>));
            
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Whether the specified primitive type requires a lower innerLevel symbol at the end.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Whether it requires a lower innerLevel symbol at the end.</returns>
        public static bool RequiresLowerInnerLevelSymbol(ABSavePrimitiveType type)
        {
            return type == ABSavePrimitiveType.Object || type == ABSavePrimitiveType.Array || type == ABSavePrimitiveType.Dictionary;
        }

        /// <summary>
        /// Works out the number of decimal places that need to be written in a version.
        /// </summary>
        public static int WorkOutNumberOfDecimalsInVersion(Version ver)
        {
            if (ver.MinorRevision != 0)
                return 4;
            else if (ver.Minor != 0)
                return 3;
            else if (ver.MajorRevision != 0)
                return 2;
            else if (ver.Major != 0)
                return 1;
            return 0;
        }

        #endregion

        #region Type Creation

        /// <summary>
        /// Attempts to create an instance of an object, based on a NameValueTypeDictionary - if the object's constructors have parameters, we'll attempt to figure out how to use them.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="settings">The way of handling any errors.</param>
        /// <param name="location">OPTIONAL: The location this happened in an ABSave string - used in any errors.</param>
        public static object CreateInstance(Type type, ABSaveSettings settings, ABSaveObjectItems values, int location = 0)
        {
            // =======================
            // Variables
            // =======================

            // First of all, get all the constructor out of the type.
            var constructors = type.GetConstructors();

            // Remember all the values if there was a constructor with "No Case Match".
            object[] noCaseMatchValues = new object[0];

            // Next, remember all the REMAINING values if there was a constructor with "No Case Match"
            ABSaveObjectItems noCaseMatchRemaining = null;

            // Also, if we don't find a perfect constructor and there's too many "No Case Match" constructor, it will cause an error, so remember if there are more than one "No Case Match" constructors.
            var multipleNoCaseMatchConstructors = false;

            // =======================
            // Each Constructor
            // =======================

            for (int i = 0; i < constructors.Length; i++)
            {
                // Keep track of whether all the parameters in this constructor are valid and how close they are to the real thing.
                var passed = ABSaveConstructorInferer.Failed;

                // Get all the parameters for this constructor.
                var parameters = constructors[i].GetParameters();
                var parametersCount = parameters.Length;

                // If there aren't any parameters, this is the one we preferably want.
                if (parametersCount == 0)
                    return InsertValuesIntoObject(Activator.CreateInstance(type), values);

                // Keep track of the parameters' values in order.
                var parametersArguments = new object[parametersCount];

                // Also, keep track of any objects we were unable to pass into this constructor.
                var remainingValues = new ABSaveObjectItems();

                // If there are too many parameters, this constructor won't work.
                if (parametersCount > values.Count)
                    continue;

                // =======================
                // Each Item - To Find A Matching Parameter
                // =======================

                for (int j = 0; j < values.Count; j++)
                {
                    // Whether a parameter was found or not.
                    var parameterFound = ABSaveConstructorInferer.Failed;

                    // The index of the best parameter for this value.
                    var bestParam = -1;

                    // =======================
                    // Each Parameter
                    // =======================

                    for (int k = 0; k < parametersCount; k++)
                    {
                        // If the types don't match, this parameter failed, so try a different one.
                        if (!(values.Items[j].Info.FieldType == parameters[k].ParameterType))
                            continue;

                        // Check if the names perfectly match now.
                        if (values.Items[k].Info.Name == parameters[j].Name)
                        {
                            // This parameter is perfect.
                            parameterFound = ABSaveConstructorInferer.Perfect;
                            bestParam = k;

                            // We want to break out of the "parameter" loop since this is a perfect match and it can't be anything else.
                            break;
                        }

                        // Otherwise, try and ignore the case and see if that helps.
                        else if (values.Items[k].Info.Name.ToLower() == parameters[j].Name.ToLower())
                        {
                            // Set the bestParam so that we can remember which parameter this was.
                            bestParam = k;

                            // Now, make sure we set this as a POSSIBLE value for this parameter and see if any other fields would work better.
                            parameterFound = ABSaveConstructorInferer.NoCaseMatch;
                            continue;
                        }
                    }

                    // =======================
                    // Check Value Results
                    // ======================= 

                    // If it failed, add this as failed value since it obviously can't work, and try some others!
                    if (bestParam == -1)
                    {
                        remainingValues.Add(values.Items[j]);
                        continue;
                    }

                    // Now, set the main "passed" variable for this constructor to how well this value did - but, if the constructor is on "No Case Match", leave it, since we know that this parameter didn't fail.
                    // Essentially, when the constructor is on "NoCaseMatch" because one of the parameters doesn't match case, it doesn't matter what the others are, they can't fix that one parameter.
                    if (passed != ABSaveConstructorInferer.NoCaseMatch)
                        passed = parameterFound;

                    // Now that we've decided the best object for a certain parameter, add that as the value at the correct index.
                    parametersArguments[bestParam] = values.Items[j].Value;
                }

                // =======================
                // Check Constructor Results
                // =======================

                // If this was a perfect constructor, return this one - with all the remaining items getting added in!
                if (passed == ABSaveConstructorInferer.Perfect)
                    return InsertValuesIntoObject(Activator.CreateInstance(type, parametersArguments), remainingValues);

                // If this constructor overall got a "No Case Match", attempt to place the values into the "noCaseMatchValues".
                if (passed == ABSaveConstructorInferer.NoCaseMatch)
                {
                    // Determine how many values we were actually able to fill in a parameter as well as how many the LAST "No Case Match" constructor did.
                    var filledParameters = values.Count - remainingValues.Count;
                    var lastFilledParameters = (noCaseMatchRemaining == null) ? 0 : values.Count - noCaseMatchRemaining.Count;

                    // If there was already a "No Case Match"... and we CAN'T override the last one, make sure we remember there's too many.
                    if (noCaseMatchValues.Count() > 0 && ((lastFilledParameters == values.Count) || (lastFilledParameters > filledParameters)))
                        multipleNoCaseMatchConstructors = true;

                    // If we can override the last one, though, mark it as "false" since this is the parameter we need.
                    else
                        multipleNoCaseMatchConstructors = false;

                    // Now, place the current values/remaining values into the main arrays.
                    noCaseMatchValues = parametersArguments;
                    noCaseMatchRemaining = remainingValues;
                }
            }

            // =======================
            // Check Final Results
            // =======================

            // If we made it here, it means there weren't any PERFECT ones, so, before we attempt anything, we'll want to throw an error if we have come across more than one "No Case Match" constructor (in which case we wouldn't know what to use).
            if (multipleNoCaseMatchConstructors)
                settings.ErrorHandler.TooManyConstructorsWithDifferentCase(type, location);

            // And, now see if there was a "No Case Match" one, if so, that's the constructor we need!
            if (noCaseMatchValues.Count() > 0)
                return InsertValuesIntoObject(Activator.CreateInstance(type, noCaseMatchValues), noCaseMatchRemaining);

            // And, if we got all the way here - it failed completely.
            settings.ErrorHandler.InvalidConstructorsForCreatingObject(type, location);
            return null;
        }

        /// <summary>
        /// Inserts values into an object.
        /// </summary>
        /// <param name="obj">The object to insert values into.</param>
        /// <param name="values">The </param>
        /// <returns>The object with the values inserted.</returns>
        public static object InsertValuesIntoObject(object obj, ABSaveObjectItems values)
        {
            // Go through each value and add it.
            for (int i = 0; i < values.Count; i++)
                values.Items[i].Info.SetValue(obj, values.Items[i].Value);

            // Now return the modified object.
            return obj;
        }

        /// <summary>
        /// Gets all the fields AND properties with their correct names.
        /// </summary>
        /// <returns>The fields and properties.</returns>
        public static ABSaveObjectItems GetFieldsAndPropertiesWithTypes(Type objType)
        {
            // Make a name/value/type dictionary - this is what we'll return.
            var ret = new ABSaveObjectItems();

            // Get all the fields.
            var fieldNames = objType.GetFields();

            // Add all the fields to the dictionary.
            for (int i = 0; i < fieldNames.Count(); i++)
                ret.Add(fieldNames[i]);

            // Return the final result.
            return ret;
        }

        /// <summary>
        /// Gets all the fields AND properties with their correct names and values based on an object.
        /// </summary>
        /// <typeparam name="T">The type of the object we're getting values for.</typeparam>
        /// <returns>The fields/properties and values for the object.</returns>
        public static ABSaveObjectItems GetFieldsAndPropertiesWithValues(object obj)
        {
            // Get the type of the object to use in the rest of the method.
            var objType = obj.GetType();

            // Make a dictionary - this is what we'll return.
            var ret = new ABSaveObjectItems();

            // First of all, get all of the fields.
            var fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            // Add all of the fields to the ObjectItems dictionary.
            for (int i = 0; i < fields.Count(); i++)
                ret.Add(fields[i].GetValue(obj), fields[i]);

            // Return the final result.
            return ret;
        }

        #endregion

        #region Numerical
        /// <summary>
        /// Converts a number to a byte array.
        /// </summary>
        /// <param name="num">The number to convert.</param>
        /// <returns></returns>
        public static byte[] ConvertNumberToByteArray(dynamic num, TypeCode tCode)
        {
            switch (tCode)
            {
                case TypeCode.Byte:

                    // Just return the byte.
                    return new byte[] { num };

                case TypeCode.SByte:

                    // Just return the sbyte - we can cast it to a byte without losing info.
                    return new byte[] { (byte)num };

                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Double:
                case TypeCode.Single:

                    // Convert it to a byte array.
                    return NumberToLittleEndianBytes(num);

                case TypeCode.Decimal:

                    return ConvertDecimalToByteArray(num);

                default:

                    return null;
            }
        }

        private static byte[] ConvertDecimalToByteArray(dynamic num)
        {
            // Converting a decimal to a byte array is hard, first of all, it takes up 16 bytes.
            var arr = new byte[16];

            // And, it's made up of four smaller 32-bit integers.
            var bits = decimal.GetBits(num);

            // We'll do the first two - the "low" and "mid", adding them to the array.
            NumberToLittleEndianBytes(bits[0]).CopyTo(arr, 0);
            NumberToLittleEndianBytes(bits[1]).CopyTo(arr, 4);

            // We'll also do the "high" and "flags" parts.
            NumberToLittleEndianBytes(bits[2]).CopyTo(arr, 8);
            NumberToLittleEndianBytes(bits[3]).CopyTo(arr, 12);

            // Now, we have our byte array in "arr", and we can simply return it.
            return arr;
        }

        /// <summary>
        /// Convert a byte array to a number.
        /// </summary>
        /// <param name="arr">The byte array.</param>
        /// <returns>The final object</returns>
        public static object ConvertByteArrayToNumber(byte[] arr, TypeCode tCode, Type actualType)
        {
            // If there's nothing in the array, just return nothing.
            if (arr.Length == 0)
                return Convert.ChangeType(0, actualType);

            // If we're working on big-endian, we need to reverse it to become little endian.
            if (BitConverter.IsLittleEndian)
                arr = arr.Reverse().ToArray();

            switch (tCode)
            {
                case TypeCode.Byte:

                    // Just return the byte.
                    return arr[0];

                case TypeCode.SByte:

                    // Just return the sbyte - we can cast it from a byte without losing info.
                    return (sbyte)arr[0];

                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Double:
                case TypeCode.Single:

                    // Convert it from a byte array.
                    return Convert.ChangeType(BitConverter.ToDouble(arr, 0), actualType);

                case TypeCode.Decimal:

                    return ConvertByteArrayToDecimal(arr);

                default:
                    return null;
            }
        }

        private static object ConvertByteArrayToDecimal(byte[] arr)
        {
            // Converting a byte array to a decimal is also really hard, we'll start with storing all the bits.
            var bits = new int[4];

            // Now, we'll convert the "low" and "mid" first - setting an offset.
            bits[0] = BitConverter.ToInt32(arr, 0);
            bits[1] = BitConverter.ToInt32(arr, 4);

            // We'll also convert the "high" and "flags".
            bits[2] = BitConverter.ToInt32(arr, 8);
            bits[3] = BitConverter.ToInt32(arr, 12);

            // Now, we can return the decimal.
            return new decimal(bits);
        }

        /// <summary>
        /// Gets the number of bytes required to store a number.
        /// </summary>
        /// <param name="tCode">The type for the number.</param>
        /// <returns>The length</returns>
        public static byte GetBytesForNumber(TypeCode tCode)
        {
            switch (tCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:

                    // This is one byte long, obviously.
                    return 1;

                case TypeCode.UInt16:
                case TypeCode.Int16:

                    // This is 2 bytes long.
                    return 2;

                case TypeCode.UInt32:
                case TypeCode.Int32:
                case TypeCode.Single:

                    // This is 4 bytes long
                    return 4;

                case TypeCode.UInt64:
                case TypeCode.Int64:
                case TypeCode.Double:

                    // This is 8 bytes long.
                    return 8;

                case TypeCode.Decimal:

                    // This is 16 bytes long.
                    return 16;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts a number to a byte array in the form of a little endian (we use little endian since on most machines it will save performance by not having to constantly reverse it)
        /// </summary>
        /// <param name="num">The number to convert.</param>
        /// <returns></returns>
        public static byte[] NumberToLittleEndianBytes(dynamic num)
        {
            // If we're working on big-endian, we need to reverse it to become little endian.
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(num);
            else
                return BitConverter.GetBytes(num).Reverse().ToArray();
        }

        /// <summary>
        /// Converts a number to a byte array in the form of a little endian (we use little endian since on most machines it will save performance by not having to constantly reverse it)
        /// </summary>
        /// <param name="num">The number to convert.</param>
        /// <returns></returns>
        public static object LittleEndianBytesToNumber(byte[] bytes, bool asDouble, int startIndex = 0)
        {
            // If we're working on big-endian, we need to reverse it to become little endian.
            if (BitConverter.IsLittleEndian)
                return asDouble ? BitConverter.ToDouble(bytes, startIndex) : BitConverter.ToInt32(bytes, startIndex);
            else
            {
                // Reverse the bytes and then return use that.
                var reversed = bytes.Reverse() as byte[];
                return asDouble ? BitConverter.ToDouble(reversed, startIndex) : BitConverter.ToInt32(reversed, startIndex);
            }
        }
        #endregion
    }
}
