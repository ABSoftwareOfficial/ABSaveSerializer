using ABSoftware.ABSave.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Helpers
{
    /// <summary>
    /// Used to give a type a key identifier so that it doesn't have to be used all the time.
    /// </summary>
    public class ABSaveIdentifiedType
    {
        /// <summary>
        /// The integer identifier for this identifed type.
        /// </summary>
        public short Key;

        /// <summary>
        /// The created value for this identifed type.
        /// </summary>
        public string Value;

        /// <summary>
        /// The actual type for this identifed type.
        /// </summary>
        public Type Type;

        /// <summary>
        /// The <see cref="Key"/> but already written as a character array.
        /// </summary>
        public char[] WrittenKey;

        /// <summary>
        /// Creates a new <see cref="ABSaveIdentifiedType"/> with all the information.
        /// </summary>
        public unsafe ABSaveIdentifiedType(short key, string value, Type type)
        {
            // Set the key/value.
            Key = key;
            Value = value;

            // Set the type.
            Type = type;

            // Create the "WrittenKey".
            var bytes = ABSaveUtils.ConvertNumberToByteArray(key, TypeCode.Int16);
            WrittenKey = new char[2];

            // Add the bytes to the "WrittenKey" as characters.
            fixed (byte* fixedBytePointer = bytes)
            fixed (char* fixedCharPointer = WrittenKey)
            {
                // Create non-fixed pointers for each array.
                var charPointer = fixedCharPointer;
                var bytePointer = fixedBytePointer;

                // And, finally, copy them.
                for (int i = 0; i < bytes.Length; i++)
                    *(charPointer++) = (char)*(bytePointer++);
            }
        }
    }
}
