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
        public static bool IsArray(Type t)
        {
            if (t.IsArray)
                return true;

            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsNumericType(Type t)
        {
            switch (Type.GetTypeCode(t))
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
        /// Attempts to create an instance of an object, based on a NameValueTypeDictionary - if the object's constructors have parameters, we'll attempt to figure out how to use them.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="errorHandler">The way of handling any errors.</param>
        /// <param name="location">OPTIONAL: The location this happened in an ABSave string - used in any errors.</param>
        public static object CreateInstance(Type type, ABSaveErrorHandler errorHandler, ABSaveObjectItemsDictionary values, int location = 0)
        {
            // Make sure that the error handler isn't null.
            errorHandler = ABSaveErrorHandler.EnsureNotNull(errorHandler);

            // First of all, get all the constructor out of the type.
            var constructors = type.GetConstructors();

            // Remember all the values if there was one with "No Case Match".
            var noCaseMatchValues = new List<object>();

            // Also, if we don't find a perfect constructor and there's too many "No Case Match" constructor, it will cause an error, so remember if there are more than one "No Case Match" constructors.
            var multipleNoCaseMatchConstructors = false;

            // Then, go through each one and see which one is appropriate.
            for (int i = 0; i < constructors.Length; i++)
            {
                // Keep track of whether all the parameters in this constructor are valid and how close they are to the real thing.
                var passed = ABSaveConstructorInferer.Failed;

                // Get all the parameters for this constructor.
                var parameters = constructors[i].GetParameters();
                var parametersCount = parameters.Length;

                // If there aren't any parameters, this is the one we preferably want.
                if (parametersCount == 0)
                    return InsertValueIntoObject(Activator.CreateInstance(type), values);

                // Keep track of the parameters' values in order.
                var parametersArguments = new List<object>();

                // If there are too few parameters, this constructor won't work either, since data will get lost.
                if (parametersCount < values.Count)
                    continue;

                // If there are too many parameters, this constructor won't work.
                if (parametersCount > values.Count)
                    continue;

                // Otherwise, check them and see how well the parameters match their names and types.
                for (int j = 0; j < parametersCount; j++)
                {
                    // Keep track of what the value for this parameters could be.
                    object value = null;

                    // Whether this parameter has passed or not.
                    var parameterPassed = ABSaveConstructorInferer.Failed;

                    // Now, figure out which item in the object this parameter could possibly point to.
                    for (int k = 0; k < values.Count; k++)
                    {
                        // If the types don't match, this failed, so break out of it.
                        if (!(values.Items[k].Info.FieldType == parameters[j].ParameterType))
                            break;
                        
                        // Check if the names perfectly match now.
                        if (values.Items[k].Info.Name == parameters[j].Name)
                        {
                            // This parameter is perfect.
                            parameterPassed = ABSaveConstructorInferer.Perfect;

                            // Now, make sure we set this as the field for this parameter.
                            value = values.Items[k].Value;

                            // We want to break out of the "value" loop since this is a perfect match and it can't be anything else.
                            break;
                        }

                        // Otherwise, try and ignore the case and see if that helps.
                        else if (values.Items[k].Info.Name.ToLower() == parameters[j].Name.ToLower())
                        {
                            parameterPassed = ABSaveConstructorInferer.NoCaseMatch;

                            // Now, make sure we set this as a POSSIBLE value for this parameter and see if any other fields would work better.
                            value = values.Items[k].Value;
                            continue;
                        }
                    }

                    // If it failed, break out of here since this parameter obviously can't work, and if one parameter doesn't work, the whole constructor won't either!
                    if (parameterPassed == ABSaveConstructorInferer.Failed)
                    {
                        passed = ABSaveConstructorInferer.Failed;
                        break;
                    }

                    // Now, set the main "passed" variable for this constructor to how well this parameter did - but, if the constructor is on "No Case Match", leave it, since we know that this parameter didn't fail.
                    // Essentially, when the constructor is on "NoCaseMatch" because one the parameters doesn't match case, it doesn't matter what the others are, they can't fix that one parameter.
                    if (passed != ABSaveConstructorInferer.NoCaseMatch)
                        passed = parameterPassed;

                    // Now that we've decided the best object, add that as the value.
                    parametersArguments.Add(value);
                }

                // If this was a perfect constructor, return this one!
                if (passed == ABSaveConstructorInferer.Perfect)
                    return Activator.CreateInstance(type, parametersArguments.ToArray());

                // If it this constructor overall got a "No Case Match", attempt to place the values into the "noCaseMatchValues".
                if (passed == ABSaveConstructorInferer.NoCaseMatch)
                {
                    // However, if there was already a "No Case Match", make sure we remember there's too many.
                    if (noCaseMatchValues.Count > 0)
                        multipleNoCaseMatchConstructors = true;

                    // Now, place the current values into the main array.
                    noCaseMatchValues = parametersArguments;
                }
            }

            // If we made it here, it means there weren't any PERFECT ones, so, before we attempt anything, we'll want to throw an error if we have come across more than one "No Case Match" constructor (in which case we wouldn't know what to use).
            if (multipleNoCaseMatchConstructors)
                errorHandler.TooManyConstructorsWithDifferentCase(type, location);

            // And, now see if there was a "No Case Match" one, if so, that's the constructor we need!
            if (noCaseMatchValues.Count > 0)
                return Activator.CreateInstance(type, noCaseMatchValues.ToArray());

            // And, if we got all the way here - it failed completely.
            errorHandler.InvalidConstructorsForCreatingObject(type, location);
            return null;
        }

        /// <summary>
        /// Inserts values into an object.
        /// </summary>
        /// <param name="obj">The object to insert values into.</param>
        /// <param name="values">The </param>
        /// <returns>The object with the values inserted.</returns>
        public static object InsertValueIntoObject(object obj, ABSaveObjectItemsDictionary values)
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
        public static ABSaveObjectItemsDictionary GetFieldsAndPropertiesWithTypes(Type objType)
        {
            // Make a name/value/type dictionary - this is what we'll return.
            var ret = new ABSaveObjectItemsDictionary();

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
        public static ABSaveObjectItemsDictionary GetFieldsAndPropertiesWithValues(object obj)
        {
            // Get the type of the object to use in the rest of the method.
            var objType = obj.GetType();

            // Make a dictionary - this is what we'll return.
            var ret = new ABSaveObjectItemsDictionary();

            // First of all, get all of the fields.
            var fields = objType.GetFields();

            // Add all of the fields to the ObjectItems dictionary.
            for (int i = 0; i < fields.Count(); i++)
                ret.Add(fields[i].GetValue(obj), fields[i]);

            // Return the final result.
            return ret;
        }

        /// <summary>
        /// Turns two characters like "3" and "7" into a byte - like "0011|0111"
        /// </summary>
        /// <param name="first">The first character to transform - at the start of the byte.</param>
        /// <param name="second">The second character to transform - at the end of the byte.</param>
        /// <returns>Returns the transformed byte.</returns>
        public static byte ConvertNumericalCharsToByte(char first, char second)
        {
            byte ret;
            switch (first)
            {
                // NOTE: EVERYTHING IS SHIFTED OVER ONE TO ALLOW THE CHARACTER \u0001 TO END THE DATETIME
                case '1':
                    ret = 32; // 00100000
                    break;
                case '2':
                    ret = 48; // 00110000
                    break;
                case '3':
                    ret = 64; // 01000000
                    break;
                case '4':
                    ret = 80; // 01010000
                    break;
                case '5':
                    ret = 96; // 01100000
                    break;
                case '6':
                    ret = 112; // 01110000
                    break;
                case '7':
                    ret = 128; // 10000000
                    break;
                case '8':
                    ret = 144; // 10010000
                    break;
                case '9':
                    ret = 160; // 10100000
                    break;
                default:
                    ret = 0;
                    break;
            }

            switch (second)
            {
                // (Remember that they are all shifted over to the left by 1)
                case '1':
                    ret += 2; // XXXX0010
                    break;
                case '2':
                    ret += 3; // XXXX0011
                    break;
                case '3':
                    ret += 4; // XXXX0100
                    break;
                case '4':
                    ret += 5; // XXXX0101
                    break;
                case '5':
                    ret += 6; // XXXX0110
                    break;
                case '6':
                    ret += 7; // XXXX0111
                    break;
                case '7':
                    ret += 8; // XXXX1000
                    break;
                case '8':
                    ret += 9; // XXXX1001
                    break;
                case '9':
                    ret += 10; // XXXX1010
                    break;
                default:
                    // do the default action
                    break;
            }
            return ret;
        }
    }
}
