using ABSoftware.ABSave.Deserialization;
using ABSoftware.ABSave.Serialization;
using System.Text;

namespace ABSoftware.ABSave
{
    // Table of characters to use:
    // \u0000 - Multi-Byte Numerical
    // \u0001 - Next Item
    // \u0002 - The NULL character
    // \u0003 - Represents "starting an object" - it is ended with a "lower innerLevel" sign
    // \u0004 - Represents "starting an array" - it is ended with a "lower innerLevel" sign
    // \u0005 - The "lower innerLevel sign"
    // \u0006 - The "DICTIONARY" starting marker
    // \u0007 - 

    /// <summary>
    /// This class is in charge of converting whole objects to and from ABSave.
    /// </summary>
    public static class ABSaveConvert
    {
        #region Serialization
        /// <summary>
        /// The main parser which is used over and over again to parse strings.
        /// </summary>
        //internal static ABSaveParser<T> MainParser;

        /// <summary>
        /// Turns a whole object into a string with an ABSave document.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="type">The way of serializing this ABSave (unnamed/named).</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="writeHeader">Whether we should write the header in or not.</param>
        /// <returns>The ABSave string.</returns>
        public static string ObjectToABSaveDocument(object obj, ABSaveType type, ABSaveSettings settings, bool writeHeader = true)
        {
            var ret = new StringBuilder();

            // NOTE: (tl;dr)
            // Why do we use a StringBuilder, and not just a string?
            // Well, there's one word to explain that:
            // Performance.
            // The ONLY reason we are using a StringBuilder instead of a string is because it's much faster.
            // HOWEVER, we don't want to just support StringBuilder - we want to allow users to use strings as well.
            // And THAT'S why we have a string mode (just set UseSB to false on most methods) and a StringBuilder mode!
            // StringBuilder is faster, but less flexible - I mean, you can do basically anything with a string, because it's a standard type.

            ObjectToABSaveDocument(obj, type, ret, settings, writeHeader);
            return ret.ToString();
        }

        /// <summary>
        /// Turns a whole object into an ABSave document, and places the result in a StringBuilder.
        /// </summary>        
        /// <param name="obj">The object to convert.</param>
        /// <param name="type">The way of serializing this ABSave (unnamed/named).</param>
        /// <param name="sb">The StringBuilder to write to</param>
        /// <param name="settings">The settings for how to handle certain parts.</param>
        /// <param name="writeHeader">Whether we should write the header in or not.</param>
        public static void ObjectToABSaveDocument(object obj, ABSaveType type, StringBuilder sb, ABSaveSettings settings, bool writeHeader = true)
        {
            // If the type is invalid, throw an exception.
            if (type == ABSaveType.Infer)
            {
                settings.ErrorHandler.InferTypeWhenSerializing();
                return;
            }

            // Write the header.
            ABSaveWriter.WriteHeader(type, sb, settings, writeHeader);

            // Write the actual object.
            ObjectToABSave(obj, type, sb, settings);
        }

        // An internal helper that automatically decides which version of "ObjectToABSave" to use.
        internal static string ObjectToABSave(object obj, ABSaveType type, ABSaveSettings settings, bool writeToSB = false, StringBuilder sb = null)
        {
            if (writeToSB)
                ObjectToABSave(obj, type, sb, settings);
            else
                return ObjectToABSave(obj, type, settings);

            return "";

        }

        /// <summary>
        /// Turns a whole object into ABSave, excluding everything extra, and places the result in a StringBuilder.
        /// </summary>
        public static string ObjectToABSave(object obj, ABSaveType type, ABSaveSettings settings)
        {
            var sb = new StringBuilder();
            ObjectToABSave(obj, type, sb, settings);
            return sb.ToString();
        }

        /// <summary>
        /// Turns a whole object into ABSave, and places the result in a StringBuilder.
        /// (Does not include anything extra, such as "\u0003" or the type that goes before - for that, use <see cref="ABSaveSerializer.SerializeObject(object, ABSaveType, System.Type, ABSaveSettings, bool, bool, StringBuilder, bool)"/>)
        /// </summary>
        public static void ObjectToABSave(object obj, ABSaveType type, StringBuilder sb, ABSaveSettings settings)
        {
            // If the object is null, don't bother with doing anything.
            if (obj == null)
                return;

            // Get all of the variables inside this object.
            var members = ABSaveUtils.GetFieldsAndPropertiesWithValues(obj);

            // This is a variable used across the whole process to decide whether this is the first one or not (to write the "Next Item" character or not).
            var notFirst = false;

            // Keep track of what type the last property was - this allows us to decide whether to add the Next Item character to the next item.
            var lastType = ABSavePrimitiveType.Unknown;

            // Go through each variable, and process it.
            for (var i = 0; i < members.Count; i++)
                ConvertVariableToABSave(type, sb, settings, members, ref notFirst, ref lastType, i);
        }

        static void ConvertVariableToABSave(ABSaveType type, StringBuilder sb, ABSaveSettings settings, Helpers.ABSaveObjectItems members, ref bool notFirst, ref ABSavePrimitiveType lastType, int i)
        {
            // If we're doing it named - write the name.
            if (type == ABSaveType.WithNames)
            {
                // Write the name out, don't write the Next Instruction character if it's the first item or the last item had a "lowerInnerlevel" sign after it.
                ABSaveWriter.WriteString(members.Items[i].Info.Name, ABSaveUtils.RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, true, sb);

                // Since we've written the name out... And the "notFirst" variable is used to determine whether to write the next instruction symbol or not... Set "notFirst" to true since it will HAVE to have the next instruction symbol now.
                notFirst = true;
            }

            // Serialize each variable, to the StringBuilder. If the last member was an array or object, then instead of getting it to write the
            // "next instruction" character, we need to get it to write the "lower" symbol instead.
            ABSaveSerializer.Serialize(members.Items[i].Value, type, settings, out lastType, true, sb, ABSaveUtils.RequiresLowerInnerLevelSymbol(lastType) ? false : notFirst, i == members.Count - 1);

            // Update the "notFirst" variable if it's false and we've gone through one item.
            if (!notFirst)
                notFirst = true;
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// Deserializes an ABSave document into a whole object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="str">The string to deserialize from.</param>
        /// <param name="type">The type of the ABSave Document.</param>
        /// <param name="errorHandler">The way of handling errors through the process.</param>
        /// <returns></returns>
        public static T ABSaveToObject<T>(string str, ABSaveType type, ABSaveSettings errorHandler = null)
        {
            // Create a new parser.
            var parser = new ABSaveParser<T>(type, errorHandler);

            // Start the new parser.
            parser.Start(str);

            // Return the result.
            return parser.Result;
        }

        #endregion
    }
}
