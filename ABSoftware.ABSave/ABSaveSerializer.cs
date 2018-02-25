using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave
{
    /// <summary>
    /// Handles serialization for ABSave.
    /// </summary>
    public static class ABSaveSerializer
    {
        public static string Serialize(object obj, bool UseSB = false, StringBuilder sb = null, bool writeNextInstructionSymbol = true, bool dnWriteEndLevel = false)
        {
            string ret = "";
            Type objType = obj.GetType();

            if (obj != null)
            {
                if (obj is string || IsNumericType(objType))
                    // Strings
                    if (UseSB)
                        sb.Append((writeNextInstructionSymbol) ? "\u0001" + obj.ToString() : obj.ToString());
                    else
                        ret = (writeNextInstructionSymbol) ? "\u0001" + obj.ToString() : obj.ToString();

                else if (IsArray(objType))
                    // Arrays/Lists
                    if (UseSB)
                        SerializeArray(obj, UseSB, sb, dnWriteEndLevel);
                    else
                        ret = SerializeArray(obj, UseSB, sb, dnWriteEndLevel);

                else if (IsDictionary(objType))
                    // Dictionaries
                    if (UseSB)
                        SerializeDictionary(obj, UseSB, sb, dnWriteEndLevel);
                    else
                        ret = SerializeDictionary(obj, UseSB, sb, dnWriteEndLevel);

                else if (obj is bool)
                    // Booleans
                    if (UseSB)
                        SerializeBool((bool)obj, writeNextInstructionSymbol, UseSB, sb);
                    else
                        ret = SerializeBool((bool)obj, writeNextInstructionSymbol);

                else
                {
                    // Other - Attempt type convert otherwise manually serialize.
                    bool CanBeTypeConverted = false;
                    TypeConverter typeConv = null;
                    typeConv = TypeDescriptor.GetConverter(objType);

                    if (typeConv.IsValid(obj)) if (typeConv.CanConvertTo(typeof(string))) CanBeTypeConverted = true;

                    if (UseSB)
              
                        if (CanBeTypeConverted)
                            sb.Append("\u0001" + typeConv.ConvertToString(obj));
                        else
                            SerializeObject(obj, objType, UseSB, sb);

                    else

                        if (CanBeTypeConverted)
                            ret += "\u0001" + typeConv.ConvertToString(obj);
                        else
                            ret += SerializeObject(obj, objType);

                }
            }

            return ret;
        }

        public static string SerializeBool(bool obj, bool writeNextInstructionSymbol = true, bool UseSB = false, StringBuilder sb = null)
        {
            if (UseSB)
                if (obj)
                    sb.Append((writeNextInstructionSymbol) ? "\u0001T" : "T");
                else
                    sb.Append((writeNextInstructionSymbol) ? "\u0001F" : "F");
            else
                if (obj)
                    return (writeNextInstructionSymbol) ? "\u0001T" : "T";
                else
                    return (writeNextInstructionSymbol) ? "\u0001F" : "F";

            return "";
        }

        public static string SerializeObject(object obj, Type objType, bool UseSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            string ret = "";

            if (UseSB)
            {
                sb.Append(ABSaveWriter.WriteType(objType) + '\u0003');
                ABSaveConvert.SerializeABSaveToStringBuilder(obj, sb);
                if (!dnWriteEndLevel) sb.Append('\u0005');
            } else {
                ret += ABSaveWriter.WriteType(objType);
                ret += "\u0003" + ABSaveConvert.SerializeABSave(obj);
                if (!dnWriteEndLevel) ret += "\u0005";
            }          

            return ret;
        }

        public static string SerializeArray(dynamic obj, bool UseSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            string ret = "";
            bool notfirst = false;

            if (UseSB)
            {
                sb.Append('\u0004');
                for (int i = 0; i < obj.Count; i++)
                {
                    Serialize(obj[i], UseSB, sb, notfirst);

                    if (!notfirst)
                        notfirst = true;
                }
                if (!dnWriteEndLevel) sb.Append('\u0005');
            }
            else
            {
                ret = "\u0004";

                for (int i = 0; i < obj.Count; i++)
                {
                    ret += Serialize(obj[i], UseSB, sb, notfirst);

                    if (!notfirst)
                        notfirst = true;
                }

                if (!dnWriteEndLevel) ret += "\u0005";
            }         

            return ret;
        }

        public static string SerializeDictionary(dynamic obj, bool UseSB = false, StringBuilder sb = null, bool dnWriteEndLevel = false)
        {
            string ret = "";

            bool notfirst = false; // In case you're confused by the "notfirst" thing when it has an "!", just think that "!notfirst" means that it IS the first.
            if (UseSB)
            {
                sb.Append('\u0006');

                foreach (dynamic element in obj)
                {
                    if (notfirst)
                        sb.Append('\u0001');

                    sb.Append(element.Key);
                    Serialize(element.Value, UseSB, sb);

                    if (!notfirst)
                        notfirst = true;
                }
                if (!dnWriteEndLevel) sb.Append('\u0005');
            } else {
                ret += '\u0006';

                foreach (dynamic element in obj)
                {
                    if (notfirst)
                        ret += '\u0001';

                    ret += element.Key;
                    ret += Serialize(element.Value);

                    if (!notfirst)
                        notfirst = true;
                }

                if (!dnWriteEndLevel) ret += '\u0005';
            }        

            return ret;
        }

        public static bool IsArray(this Type t)
        {
            try
            {
                if (t.IsArray)
                    return true;

                return t.GetGenericTypeDefinition() == typeof(List<>);
            }
            catch { return false; }
        }

        public static bool IsNumericType(this Type t)
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

        public static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
    }
}
