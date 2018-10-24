using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Helpers
{
    /// <summary>
    /// This represents a single item used in the ABSaveParser...
    /// </summary>
    /// <typeparam name="TType">The type this item represents (for example: List or Dictionary or string[])</typeparam>
    /// <typeparam name="TInnerType">ONLY USED ON ARRAYS! When this is an array, the types that each item has.</typeparam>
    /// <typeparam name="TKey">ONLY USED ON DICTIONARIES! The type for the key of this dictionary.</typeparam>
    /// <typeparam name="TValue">ONLY USED ON DICTIONARIES! The type for the value of this dictionary.</typeparam>
    public struct ABSaveParserItem<TType, TInnerType, TKey, TValue>
    {
        /// <summary>
        /// The type for this item (object, array, dictionary).
        /// </summary>
        public ABSaveParserItemType ItemType;

        /// <summary>
        /// A list representing the inner items in the array.
        /// </summary>
        public List<TInnerType> InnerItems;

        /// <summary>
        /// A dictionary representing the inner items in an object.
        /// </summary>
        public ABSaveObjectItemsDictionary ObjectItems;

        /// <summary>
        /// An actual representation of the dictionary in question.
        /// </summary>
        public Dictionary<TKey, TValue> ActualDictionary;

        /// <summary>
        /// Adds an item to the array.
        /// </summary>
        public void AddItemWhenArray(TInnerType item)
        {
            
        }

        /// <summary>
        /// Gets the type for the object that this item refers to.
        /// </summary>
        /// <returns>The type for the object.</returns>
        public Type GetObjectType()
        {
            return typeof(TType);
        }

        /// <summary>
        /// Sets this item as an array, with a certain item type.
        /// </summary>
        /// <param name="items">Items to add at the beginning.</param>
        public ABSaveParserItem(List<TInnerType> items)
        {
            // This constructor is for an array, so make it an array.
            ItemType = ABSaveParserItemType.Array;

            // Now, create a new instance of the InnerItems list.
            InnerItems = new List<TInnerType>(items);

            // Since this is an array, the rest isn't needed.
            ObjectItems = null;
            ActualDictionary = null;
        }

        /// <summary>
        /// Adds an item for an object - with a dictionary of KeyValue pairs.
        /// </summary>
        /// <param name="name">The name of this item.</param>
        /// <param name="value">The value in this item.</param>
        public ABSaveParserItem(ABSaveObjectItemsDictionary items)
        {
            // This constructor is for an object, so make it an object.
            ItemType = ABSaveParserItemType.Object;

            // Make the ObjectItems a new dictionary with all the items in it.
            ObjectItems = items;

            // Since this is an object, the rest isn't needed.
            InnerItems = null;
            ActualDictionary = null;
        }
    }
}
