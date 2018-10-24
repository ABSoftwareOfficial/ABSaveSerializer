using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Deserialization
{
    public enum ABSaveTokens
    {
        Null,
        NextItem,
        StartObject,
        StartArray,
        ExitObject,
        StartDictionary
    }
}
