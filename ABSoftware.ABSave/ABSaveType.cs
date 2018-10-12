using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave
{
    /// <summary>
    /// How to serialize an ABSave document.
    /// </summary>
    public enum ABSaveType
    {
        /// <summary>
        /// This will serialize an ORDER-based ABSave document... Fine if you want to have REALLY compact ABSave and you are willing to create an ordered-map.
        /// 
        /// <para>NOTE: Sometimes we cannot ensure that reflection gives us all the fields in the EXACT order. If you want to make sure they'll DEFINITELY be in the right order - add the OrderAttribute to each field.</para>
        /// </summary>
        WithOutNames,

        /// <summary>
        /// This will use names to allow ABSave to automatically place the values with the correct properties - this may not be as compact... But when you have something like a save
        /// where the C# object is always changing, and backwards-compatibility is needed, this is useful.
        /// </summary>
        WithNames,

        /// <summary>
        /// ONLY FOR DESERIALIZATION! Figures out the type based on the header for the ABSave.
        /// </summary>
        Infer
    }
}
