using ABSoftware.ABSave.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Deserialization
{
    /// <summary>
    /// Handles deserializing individual objects.
    /// </summary>
    public class ABSaveDeserializer
    {
        /// <summary>
        /// Figures out what a string is and how to deserialize it. This does not deserialize objects, arrays or dictionaries, however.
        /// </summary>
        /// <param name="str">The string to deserialize.</param>
        /// <param name="objType">The type of the object to deserialize to.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="location">OPTIONAL: This is used to add a location to the error from the <paramref name="settings"/></param>
        /// <returns>The deserialized object - can be null if it needs to be manually parsed.</returns>
        public static object Deserialize(string str, Type objType, ABSaveSettings settings, int location = 0)
        {
            return Deserialize(str, objType, settings, out ABSavePrimitiveType none, out bool none2, location);
        }

        /// <summary>
        /// Figures out what a string is and how to deserialize it. This does not deserialize objects, arrays or dictionaries, however.
        /// </summary>
        /// <param name="str">The string to deserialize.</param>
        /// <param name="objType">The type of the object to deserialize to.</param>
        /// <param name="determinedType">The primitive that the object has been determined as.</param>
        /// <param name="manuallyParse">Whether a parser using this should manually parse the object or not.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="location">OPTIONAL: This is used to add a location to the error from the <paramref name="settings"/></param>
        /// <returns>The deserialized object - can be null if it needs to be manually parsed.</returns>
        public static object Deserialize(string str, Type objType, ABSaveSettings settings, out ABSavePrimitiveType determinedType, out bool manuallyParse, int location = 0)
        {
            // Set the "determinedType" to Unknown for now, and, manuallyParse to false.
            determinedType = ABSavePrimitiveType.Unknown;
            manuallyParse = false;

            // Get a type code.
            var tCode = Type.GetTypeCode(objType);

            // Now, go through what it could be and convert it.
            if (objType == typeof(string))
            {
                determinedType = ABSavePrimitiveType.String;
                return str;
            }

            // Check if it's a number.
            else if (ABSaveUtils.IsNumericType(tCode))
            {
                determinedType = ABSavePrimitiveType.Number;
                return DeserializeNumber(str, objType, settings, location);
            }

            // Check if it's an array - if so, all we can do is set the "determinedType" to "array" since this method doesn't deal with that.
            else if (ABSaveUtils.IsArray(objType))
            {
                manuallyParse = true;
                determinedType = ABSavePrimitiveType.Array;
            }

            // Check if it's a dictionary - if so, all we can do is set the "determinedType" to "dictionary" since this method doesn't deal with that.
            else if (ABSaveUtils.IsDictionary(objType))
            {
                manuallyParse = true;
                determinedType = ABSavePrimitiveType.Dictionary;
            }

            // Check if it's a boolean - if so, we can get the result out of that using "DeserializeBool"
            else if (objType == typeof(bool))
            {
                determinedType = ABSavePrimitiveType.Boolean;
                return DeserializeBool(str, settings, location);
            }

            // Check if it's a DateTime - if so, we can get the result out of that using "DeserializeDateTime"
            else if (objType == typeof(DateTime))
            {
                determinedType = ABSavePrimitiveType.DateTime;
                return DeserializeDateTime(str, settings, location);
            }

            // Check if it's a Type - if so, we can get the result using "Type.GetType()"
            else if (objType == typeof(Type))
            {
                determinedType = ABSavePrimitiveType.Type;
                return Type.GetType(str);
            }

            // If it wasn't any of the above - it's probably an object, now, we've been given a string which is actually the type name of it (unless the ABSave is incorrect).
            else
            {
                // Mark it as an object.
                determinedType = ABSavePrimitiveType.Object;

                // We're going to try and find a TypeConverter. But if we can't find one - we'll just return the correct type based on the TypeName we were given.
                TypeConverter typeConv = TypeDescriptor.GetConverter(objType);

                // Check if the type converter can actually convert it FROM a string, if it can, use it.
                if (typeConv.CanConvertFrom(typeof(string)))
                    return typeConv.ConvertFrom(str);

                // However, otherwise, since we know what the type is - we'll return that, and let the parser manually parse it.
                else
                {
                    manuallyParse = true;
                    return Type.GetType(str);
                }
            }

            // If we got here it was probably manually making the parser do the rest to return null.
            return null;
        }

        /// <summary>
        /// Deserializes a string into a number (int, long, double etc.)
        /// </summary>
        /// <param name="str">The string to deserialize.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="location">OPTIONAL: Used to put a location on errors.</param>
        /// <typeparam name="T">Used to specify what target number type you want (int, long, double etc.)</param>
        /// <returns>The deserialized value.</returns>
        public static T DeserializeNumber<T>(string str, ABSaveSettings settings, int location = 0)
        {
            return (T)DeserializeNumber(str, typeof(T), settings, location);
        }

        /// <summary>
        /// Deserializes a string into a number (int, long, double etc.)
        /// </summary>
        /// <param name="str">The string to deserialize.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="location">OPTIONAL: Used to put a location on errors.</param>
        /// <param name="targetType">Used to specify what target number type you want (int, long, double etc.)</param>
        /// <returns>The deserialized value.</returns>
        public static object DeserializeNumber(string str, Type targetType, ABSaveSettings settings, int location = 0)
        {
            // Attempt to parse it.
            var passed = decimal.TryParse(str, out decimal result);

            // If it fails to parse, the value isn't valid, so that will throw an error.
            if (!passed)
            {
                settings.ErrorHandler.InvalidValueInABSaveWhenParsing(location, "The number given: " + str + " is not valid for the type: " + targetType.Name);
                return null;
            }

            // Otherwise, go ahead and return it!
            return Convert.ChangeType(result, targetType);
        }

        /// <summary>
        /// Deserializes a string into a boolean.
        /// </summary>
        /// <param name="str">The string to deserialize.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="location">OPTIONAL: Used to put a location on errors.</param>
        /// <returns>The deserialized boolean.</returns>
        public static bool DeserializeBool(string str, ABSaveSettings settings, int location = 0)
        {
            // Get a lower case version of the string for easier comparison.
            var lower = str.ToLower();

            // If it's "F", return false.
            if (lower == "f")
                return false;

            // If it's "T", return true.
            else if (lower == "t")
                return true;

            // Otherwise, there's a problem.
            else
                settings.ErrorHandler.InvalidValueInABSaveWhenParsing(location, "The boolean given: " + str + " is not valid.");

            // If we got here, it failed and so the value doesn't matter.
            return false;
        }

        /// <summary>
        /// Deserializes a string into a DateTime - the string must be in ticks.
        /// </summary>
        /// <param name="str">The string to deserialize.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="location">OPTIONAL: Used to put a location on errors.</param>
        /// <returns>The deserialized DateTime.</returns>
        public static DateTime DeserializeDateTime(string str, ABSaveSettings settings, int location = 0)
        {
            // Just return a new DateTime, but with the number deserialized.
            return new DateTime(DeserializeNumber<long>(str, settings, location));
        }
    }
}
