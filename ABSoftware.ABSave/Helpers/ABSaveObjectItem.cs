using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Helpers
{
    /// <summary>
    /// An item used in <see cref="ABSaveObjectItems" /> for storing a value as well as info (such as name/type)
    /// </summary>
    public class ABSaveObjectItem
    {
        /// <summary>
        /// The value.
        /// </summary>
        public object Value;

        /// <summary>
        /// The actual FieldInfo for this item - where the .
        /// </summary>
        public FieldInfo Info;

        /// <summary>
        /// Creates a new NameValuePair, with just a name and no type or value. This is for a field.
        /// </summary>
        /// <param name="info">Information about the field this points to.</param>
        public ABSaveObjectItem(FieldInfo info)
        {
            // Set the info now.
            Info = info;

            // Set the value to null since we haven't specified it yet.
            Value = null;
        }

        /// <summary>
        /// Creates a new NameValuePair, with just a name and no type or value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="info">Information about the field this points to.</param>
        public ABSaveObjectItem(object value, FieldInfo info)
        {
            // Set the general info.
            Info = info;

            // Also, set the value.
            Value = value;
        }
    }
}
