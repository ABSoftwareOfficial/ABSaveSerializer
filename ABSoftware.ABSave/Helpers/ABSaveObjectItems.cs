using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Helpers
{
    /// <summary>
    /// A helper dictionary with a name, value, type AND "FieldInfo" (used for fields/properties when parsing)
    /// </summary>
    /// <typeparam name="TName"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TType"></typeparam>
    public class ABSaveObjectItemsDictionary
    {
        /// <summary>
        /// The actual items inside.
        /// </summary>
        public List<ABSaveObjectItem> Items = new List<ABSaveObjectItem>();

        /// <summary>
        /// Finds the index of an item based on a name.
        /// </summary>
        /// <param name="name"></param>
        public int FindIndex(string name)
        {
            // Go through each item, and find its index.
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Info.Name == name)
                    return i;

            // If it's didn't find one, just return -1.
            return -1;
        }

        ///// <summary>
        ///// Gets the type at a certain index.
        ///// </summary>
        ///// <param name="index">The index to get the type at.</param>
        ///// <returns></returns>
        //public Type GetTypeAtIndex(int index)
        //{
        //    return Items[index].Type;
        //}

        /// <summary>
        /// The amount of items in this dictionary.
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Adds a item to this NameValueTypeDictionary with a certain name, not caring about anything else.
        /// </summary>
        /// <param name="info">Information about the item.</param>
        public void Add(FieldInfo info)
        {
            Items.Add(new ABSaveObjectItem(info));
        }

        /// <summary>
        /// Adds a item to this NameValueTypeDictionary with a certain name and value AND type.
        /// </summary>
        /// <param name="value">The actual value for this item.</param>
        /// <param name="info">The type for this item.</param>
        public void Add(object value, FieldInfo info)
        {
            Items.Add(new ABSaveObjectItem(value, info));
        }
    }
}
