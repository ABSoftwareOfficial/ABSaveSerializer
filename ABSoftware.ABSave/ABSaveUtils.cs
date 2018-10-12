// ***********************************************************************
// Assembly         : ABSoftware.ABSave
// Author           : Alex
// Created          : 02-25-2018
//
// Last Modified By : Alex
// Last Modified On : 02-28-2018
// ***********************************************************************
// <copyright file="ABSaveUtils.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave
{
    public static class ABSaveUtils
    {
        public static bool IsArray(Type t)
        {
            if (t.IsArray)
                return true;

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>));
        }

        public static bool IsNumericType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Whether the type is a dictionary.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>Whether it's a dictionary.</returns>
        public static bool IsDictionary(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        /// <summary>
        /// Whether the specified primitive type requires a lower innerLevel symbol at the end.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Whether it requires a lower innerLevel symbol at the end.</returns>
        public static bool RequiresLowerInnerLevelSymbol(ABSavePrimitiveType type)
        {
            return type == ABSavePrimitiveType.Object || type == ABSavePrimitiveType.Array || type == ABSavePrimitiveType.Dictionary;
        }

        /// <summary>
        /// Turns two characters like "3" and "7" into a byte - like "0011|0111"
        /// </summary>
        /// <param name="first">The first character to transform - at the start of the byte.</param>
        /// <param name="second">The second character to transform - at the end of the byte.</param>
        /// <returns>Returns the transformed byte.</returns>
        public static byte ConvertNumericalCharsToByte(char first, char second)
        {
            byte ret;
            switch (first)
            {
                // NOTE: EVERYTHING IS SHIFTED OVER ONE TO ALLOW THE CHARACTER \u0001 TO END THE DATETIME
                case '1':
                    ret = 32; // 00100000
                    break;
                case '2':
                    ret = 48; // 00110000
                    break;
                case '3':
                    ret = 64; // 01000000
                    break;
                case '4':
                    ret = 80; // 01010000
                    break;
                case '5':
                    ret = 96; // 01100000
                    break;
                case '6':
                    ret = 112; // 01110000
                    break;
                case '7':
                    ret = 128; // 10000000
                    break;
                case '8':
                    ret = 144; // 10010000
                    break;
                case '9':
                    ret = 160; // 10100000
                    break;
                default:
                    ret = 0;
                    break;
            }

            switch (second)
            {
                // (Remember that they are all shifted over to the left by 1)
                case '1':
                    ret += 2; // XXXX0010
                    break;
                case '2':
                    ret += 3; // XXXX0011
                    break;
                case '3':
                    ret += 4; // XXXX0100
                    break;
                case '4':
                    ret += 5; // XXXX0101
                    break;
                case '5':
                    ret += 6; // XXXX0110
                    break;
                case '6':
                    ret += 7; // XXXX0111
                    break;
                case '7':
                    ret += 8; // XXXX1000
                    break;
                case '8':
                    ret += 9; // XXXX1001
                    break;
                case '9':
                    ret += 10; // XXXX1010
                    break;
                default:
                    // do the default action
                    break;
            }
            return ret;
        }
    }
}
