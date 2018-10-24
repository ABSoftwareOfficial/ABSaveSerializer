using System;
using ABSoftware.ABSave.Deserialization;

namespace ABSoftware.ABSave.Exceptions.Base
{
    [Flags]
    public enum ABSaveError
    {
        /// <summary>
        /// When there are too many constructors with a different case to the actual fields with a class that hasn't got a parameterless constructor.
        /// </summary>
        TooManyConstructorsWithDifferentCase = 1,

        /// <summary>
        /// When there are no valid constructors to create an instance of an object with.
        /// </summary>
        InvalidConstructorsForCreatingObject = 2,

        /// <summary>
        /// When the type "infer" is passed when serializing.
        /// </summary>
        InferTypeWhenSerializing = 4,

        /// <summary>
        /// When there is an invalid header when parsing.
        /// </summary>
        InvalidHeaderWhenParsing = 8,

        /// <summary>
        /// When there is an unexpected token when parsing.
        /// </summary>
        UnexpectedTokenWhenParsing = 16,

        /// <summary>
        /// When there are too many "Next Item" tokens in a single object.
        /// </summary>
        MoreNextItemTokensThanItems = 32,

        /// <summary>
        /// When there is an invalid value in ABSave - such as a number being written as "32ha".
        /// </summary>
        InvalidValueInABSaveWhenParsing = 64,

        /// <summary>
        /// When there is no name for a value in ABSave.
        /// </summary>
        MissingNameToValueWhenParsing = 128
    }
}
