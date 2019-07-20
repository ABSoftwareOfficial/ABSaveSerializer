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
using System.Collections;
using System.ComponentModel;
using System.Text;
using static ABSoftware.ABSave.ABSaveUtils;

namespace ABSoftware.ABSave.Serialization
{
    /// <summary>
    /// This class is responsible for serializing smaller units of object, but not whole objects.
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
        public static string Serialize(dynamic obj, ABSaveType type, ABSaveSettings settings, out ABSavePrimitiveType determinedType, bool useSB = false, StringBuilder sb = null, bool writeNextInstructionSymbol = true, bool dnWriteEndLevel = false)
        {
            // This will be what to return if useSB is false.
            string ret;

            // For now, make it so we output "unknown".
            determinedType = ABSavePrimitiveType.Unknown;

            // Check if the object is null... or an IntPtr, write the null symbol - otherwise, we could get a StackOverflowException.
            if (obj == null || obj is IntPtr || obj is UIntPtr)
                return ABSaveWriter.WriteNullItem(useSB, sb);

            // Remember what type the object is - as well as the TypeCode.
            Type objType = obj.GetType();
            var tCode = Type.GetTypeCode(obj.GetType());

            // If the object is a string - write it as a string.
            if (tCode == TypeCode.String)
            {
                ret = ABSaveWriter.WriteString(obj, writeNextInstructionSymbol, useSB, sb);
                determinedType = ABSavePrimitiveType.String;
            }

            // If the object is a number - write it as a number.
            else if (IsNumericType(tCode))
            {
                ret = ABSaveWriter.WriteNumerical(obj, tCode, true, useSB, sb);
                determinedType = ABSavePrimitiveType.Number;
            }

            // If the object is an array - serialize it as an array.
            else if (IsArray(objType))
            {
                ret = SerializeArray(obj, objType, type, settings, useSB, sb, dnWriteEndLevel);
                determinedType = ABSavePrimitiveType.Array;
            }

            // If the object is a dictionary - serialize it as a dictionary.
            else if (IsDictionary(objType))
            {
                ret = SerializeDictionary(obj, type, settings, useSB, sb, dnWriteEndLevel);
                determinedType = ABSavePrimitiveType.Dictionary;
            }

            // If the object is a boolean - serialize it as a boolean.
            else if (tCode == TypeCode.Boolean)
            {
                ret = SerializeBool(obj, writeNextInstructionSymbol, useSB, sb);
                determinedType = ABSavePrimitiveType.Boolean;
            }

            // If the object is a DateTime - serialize it as a DateTime.
            else if (tCode == TypeCode.DateTime)
            {
                ret = SerializeDateTime(obj, useSB, sb);
                determinedType = ABSavePrimitiveType.DateTime;
            }

            // If it's a type, just write it out using the ABSaveWriter (for some reason there is no TypeConverter built-in for a type!)
            else if (obj is Type)
            {
                ret = SerializeType(obj, useSB, sb);
                determinedType = ABSavePrimitiveType.Type;
            }

            // Otherwise, we'll attempt to find a built-in type converter (to a string)
            else
            {

                // Mark it as an object.
                determinedType = ABSavePrimitiveType.Object;

                // Attempt to get a converter for it.
                var canBeTypeConverted = false;
                var typeConv = TypeDescriptor.GetConverter(objType);

                // Check if the type converter can actually convert it to a string.
                if (typeConv.IsValid(obj))
                    if (typeConv.CanConvertTo(typeof(string)))
                        canBeTypeConverted = true;

                // If it can be type converted, convert it using that, and then write it as a string.
                if (canBeTypeConverted)
                    ret = ABSaveWriter.WriteString(typeConv.ConvertToString(obj), writeNextInstructionSymbol, useSB, sb);

                // Otherwise, if it can't be type converted... Manually convert it.
                else
                    ret = SerializeObject(obj, type, objType, settings, writeNextInstructionSymbol, useSB, sb, dnWriteEndLevel);
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
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string Serialize(dynamic obj, ABSaveType type, ABSaveSettings settings, bool useSB = false, StringBuilder sb = null, bool writeNextInstructionSymbol = true, bool dnWriteEndLevel = false)
        {
            return Serialize(obj, type, settings, out ABSavePrimitiveType _, useSB, sb, writeNextInstructionSymbol, dnWriteEndLevel);
        }

        /// <summary>
        /// Serializes a boolean.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="writeNextInstructionSymbol">Whether it will write \u0001 on the start - usually false if it serializing the first object in a class.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeBool(bool obj, bool writeNextInstructionSymbol = true, bool useSB = false, StringBuilder sb = null)
        {
            // Serialize it (use "T" or "F") and then write it as a string - it will return nothing if it needed to write to a StringBuilder.
            return ABSaveWriter.WriteString(obj ? "T" : "F", writeNextInstructionSymbol, useSB, sb);
        }

        /// <summary>
        /// Serializes an object, by using <see cref="ABSaveConvert"/>, but some extras.
        /// </summary>
        /// <param name="obj">The object to serialize manually.</param>
        /// <param name="objType">The type of the object to serialize manually.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeObject(object obj, ABSaveType type, Type objType, ABSaveSettings settings, bool writeNextInstructionSymbol = true, bool useSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            // Create a variable to store what we'll return - if we aren't using a StringBuilder.
            var ret = "";

            // First, write a "next step" symbol.
            ret += ABSaveWriter.WriteNextItem(writeNextInstructionSymbol, useSB, sb);

            // Next, serialize the type that goes before it.
            ret += SerializeTypeBeforeObject(objType, settings, useSB, sb);

            // Then, write the opening (\u0003) for the object.
            ret += ABSaveWriter.WriteObjectOpen(useSB, sb);

            // And, write the actual object, use the correct method for either string or for a StringBuilder.
            ret += ABSaveConvert.ObjectToABSave(obj, type, settings, useSB, sb);

            // Finally, write the ending for the object.
            ret += ABSaveWriter.WriteObjectClose(dnWriteEndLevel, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        /// <summary>
        /// An object (with multiple properties) to serialize manually - one that doesn't have a TypeConverter.
        /// </summary>
        /// <param name="obj">The object to serialize manually</param>
        /// <param name="objType">The type of the object.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeArray(dynamic obj, Type objType, ABSaveType type, ABSaveSettings settings, bool useSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            obj = obj as IEnumerable;

            // Create a variable to store what we'll return.
            var ret = "";

            // Keep track of whether we're on the first item or not.
            var notFirst = false;

            // Write the opening for the array.
            ret += ABSaveWriter.WriteArrayOpening(useSB, sb);

            // Keep track of what type the last property was - this allows us to decide whether to add the Next Item character to the next item.
            var lastType = ABSavePrimitiveType.Unknown;

            // If it's an array, just use a "for" loop.
            if (objType.IsArray)
                for (var i = 0; i < obj.Length; i++)
                    SerializeArrayItem(type, settings, useSB, sb, dnWriteEndLevel, ref ret, ref notFirst, ref lastType, obj[i]);

            // For anything else, foreach will work fine.
            else
                foreach (var item in obj)
                    SerializeArrayItem(type, settings, useSB, sb, dnWriteEndLevel, ref ret, ref notFirst, ref lastType, item);

            // Write the closing for the array.
            ret += ABSaveWriter.WriteObjectClose(dnWriteEndLevel, useSB, sb);

            // Now, "ret" would be empty if we were using a StringBuilder, however, if we weren't... It will have the correct string in it so return it.
            return ret;
        }

        internal static void SerializeArrayItem(ABSaveType type, ABSaveSettings settings, bool useSB, StringBuilder sb, bool dnWriteEndLevel, ref string ret, ref bool notFirst, ref ABSavePrimitiveType lastType, dynamic item)
        {
            // Serialize the item and write to either the StringBuilder or the "ret"...
            ret += Serialize(item, type, settings, out lastType, useSB, sb, RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, dnWriteEndLevel);

            // Update the "notFirst" variable.
            if (!notFirst)
                notFirst = true;
        }

        /// <summary>
        /// Serializes a dictionary.
        /// </summary>
        /// <param name="obj">The dictionary to serialize.</param>
        /// <param name="useSB">Whether this will write to a string builder (if true), or return a string (if false).</param>
        /// <param name="sb">The StringBuilder to write to - if <paramref name="useSB"/> is true.</param>
        /// <param name="dnWriteEndLevel">"Do Not Write End Level Symbol" - Marks whether to NOT write \u0005 (if true), commonly used for the last object of all.</param>
        /// <returns>If <paramref name="useSB"/> is false, this method will return the result as a string.</returns>
        public static string SerializeDictionary(dynamic obj, ABSaveType type, ABSaveSettings settings, bool useSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            // Create a variable to store what we'll return.
            var ret = "";

            // Keep track of whether we're on the first item or not.
            var notFirst = false;

            // Write the opening for the array.
            ret += ABSaveWriter.WriteDictionaryOpening(useSB, sb);

            // Now, go through each item in the dictionary.
            foreach (var element in obj)
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
                ret += Serialize(element.Value, type, settings, useSB, sb, true, dnWriteEndLevel);

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
        public static string SerializeDateTime(DateTime obj, bool useSB = false, StringBuilder sb = null)
        {
            // Write the ticks, we're using "WriteNumerical" because the ticks are a long.
            return ABSaveWriter.WriteNumerical(obj.Ticks, TypeCode.Int64, true, useSB, sb);
        }

        /// <summary>
        /// Converts a type object into ABSave text.
        /// Including: Assembly Name, Version, Culture, PublicKeyToken
        /// </summary>
        /// <param name="typ">The type to write.</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <param name="writeToCharArray">Whether we should write to a character array (which will then be returned) - it can do both a StringBuilder and character array at the same time.</param>
        /// <returns></returns>
        public static string SerializeType(Type typ, bool useSB = false, StringBuilder sb = null)
        {
            string ret = "";

            // ======
            // MAKE SURE TO VIEW THE ABSOFTWARE DOCS' SPECIFICATION FOR TYPES, IT WILL HELP MAKE THIS LOGIC MAKE SENSE.
            // ======
            // Work out all of the things we need to 
            var name = typ.Assembly.GetName();
            var publicKeyToken = name.GetPublicKeyToken();
            var version = name.Version;
            var hasPublicKeyToken = publicKeyToken != null && publicKeyToken.Length != 0;
            var hasCulture = name.CultureName != "";
            var hasVersion = !(version.Major == 1 && version.MajorRevision == 0 && version.Minor == 0 && version.MinorRevision == 0);

            // If we're writing a StringBuilder, we'll put the commas in as we go, 
            // otherwise, we'll put them all together at the end.
            // PART 1: Type Name
            ret += ABSaveWriter.WriteString(typ.FullName, false, useSB, sb);

            // PART 2: Assembly Name
            ret += ABSaveWriter.WriteString(ABSaveWriter.WriteCharacter(',', useSB, sb) + name.Name, false, useSB, sb);

            // PART 3: Version
            if (hasVersion)
                ret += SerializeVersion(version, useSB, sb);

            // Even if we don't have a version, we might still have to write a comma if there's anything after this!
            else if (hasCulture || hasPublicKeyToken)
                ret += ABSaveWriter.WriteCharacter(',', useSB, sb);

            // PART 4: Culture
            if (hasCulture)
                ret += ABSaveWriter.WriteString(ABSaveWriter.WriteCharacter(',', useSB, sb) + name.CultureName, false, useSB, sb);
            else if (hasPublicKeyToken)
                ret += ABSaveWriter.WriteCharacter(',', useSB, sb);

            // PART 5: PublicKeyToken
            if (hasPublicKeyToken)
                ret += ABSaveWriter.WriteCharacter(',', useSB, sb) + ABSaveWriter.WriteByteArray(name.GetPublicKeyToken(), useSB, sb);

            return ret;

        }

        /// <summary>
        /// Serializes a <see cref="Version"/> to either a StringBuilder or a string.
        /// </summary>
        internal static string SerializeVersion(Version ver, bool writeToSB, StringBuilder sb)
        {
            var ret = "";

            // Before we do anything, we need to determine how many decimal places there will be.
            int numberOfDecimals = WorkOutNumberOfDecimalsInVersion(ver);

            // If there were no decimals, then we don't need to write anything,
            if (numberOfDecimals == 0)
                return "";

            // Before we write each part out, we'll write a comma first.
            ret += ABSaveWriter.WriteCharacter(',', writeToSB, sb);

            // Write each part now.
            if (numberOfDecimals >= 1)
                ret += ABSaveWriter.WriteByteArray(ConvertNumberToByteArray(ver.Major, TypeCode.Int32), writeToSB, sb);
            if (numberOfDecimals >= 2)
                ret += ABSaveWriter.WriteDotAndInt32(ver.Minor, writeToSB, sb);
            // Write the minor part, with a dot before.
            if (numberOfDecimals >= 3)
                ret += ABSaveWriter.WriteDotAndInt32(ver.Build, writeToSB, sb);
            // Write the minor revision part, with a dot before.
            if (numberOfDecimals >= 4)
                ret += ABSaveWriter.WriteDotAndInt32(ver.Revision, writeToSB, sb);

            return ret;
        }


        /// <summary>
        /// Writes the type data that goes before an object or array.
        /// Which usually includes a key.
        /// </summary>
        /// <param name="typ">The type to write as a string.</param>
        /// <param name="useSB">Whether we are writing to a StringBuilder or not.</param>
        /// <param name="sb">The StringBuilder to write to - if we're writing to one at all.</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <returns>Returns the type as a string.</returns>
        public static string SerializeTypeBeforeObject(Type typ, ABSaveSettings settings, bool useSB = false, StringBuilder sb = null)
        {
            // Don't do anything if we're not meant to.
            if (!settings.WithTypes)
                return "";

            // Attempt use a remembered type (or create one) - and return with that.
            var attempt = AttemptToUseRememberedType(typ, settings, useSB, sb, out string res);

            // If that attempt was successful, then we'll use that.
            if (attempt)
                return res;

            // If we weren't able to use an identified type, then we'll just have to write the type, not bothering with any of the identified type..
            return SerializeType(typ, useSB, sb);
        }

        static bool AttemptToUseRememberedType(Type typ, ABSaveSettings settings, bool useSB, StringBuilder sb, out string identifiedType)
        {
            identifiedType = "";

            // If we can't remember types, then this won't work.
            if (!settings.RememberTypes)
                return false;

            // Check for an already identifed type.
            var key = settings.SearchForIdentifiedType(typ);

            // If we can't find one, we'll attempt to create one, and if we can't even create one, then just write it.
            if (key == null && settings.CurrentlyCanIdentifyTypes)
                return AttemptToCreateIdentifiedType(typ, settings, useSB, sb, ref identifiedType);
            else if (useSB)
                sb.Append(key);
            else
                identifiedType = new string(key);

            // Since we've got here without stopped, we were successful.
            return true;
        }

        static bool AttemptToCreateIdentifiedType(Type typ, ABSaveSettings settings, bool useSB, StringBuilder sb, ref string identifiedType)
        {
            // Create the new string.
            var str = SerializeType(typ, useSB, sb);

            // If the number "settings.HighestIdentifiedType" is too high, we can't do it anymore in the future.
            if (settings.HighestIdentifiedType + 1 == short.MaxValue)
                return settings.CurrentlyCanIdentifyTypes = false;

            // Now, add it as an item, so that this type can be identified easier in the future.
            settings.RememberedTypes.Add(new Helpers.ABSaveIdentifiedType(settings.HighestIdentifiedType++, str, typ));

            // We'll now write out the key for this type, as well as the actual type.
            if (useSB)
                sb.Append(settings.RememberedTypes[settings.HighestIdentifiedType - 1].WrittenKey);
            else
                identifiedType = str + new string(settings.RememberedTypes[settings.HighestIdentifiedType - 1].WrittenKey);
        
            return true;
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
