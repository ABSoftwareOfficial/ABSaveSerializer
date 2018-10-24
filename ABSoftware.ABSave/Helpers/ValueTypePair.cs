using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Helpers
{
    /// <summary>
    /// Represents a value and type - used in indexers to pull out both the value/type based on a name.
    /// </summary>
    /// <typeparam name="TValue">The value</typeparam>
    /// <typeparam name="TType">The type</typeparam>
    public struct ValueTypePair<TValue, TType>
    {
        /// <summary>
        /// The name.
        /// </summary>
        public TValue Value;

        /// <summary>
        /// The value.
        /// </summary>
        public TType Type;

        /// <summary>
        /// Creates a new ValueTypePair, with both.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public ValueTypePair(TValue value, TType type)
        {
            Value = value;
            Type = type;
        }
    }
}
