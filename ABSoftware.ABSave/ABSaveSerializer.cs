using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static string Serialize(object obj)
        {
            string ret = "";

            if (obj != null)
            {
                if (IsArray(obj))
                    ret += SerializeArray(obj);
                else if (IsDictionary(obj))
                    ret += "\u0003" + SerializeDictionary(obj);
                else if (IsNumericType(obj))
                    ret += "\u0001" + obj.ToString();
                else if (obj is bool)
                    ret += "\u0001" + SerializeBool((bool)obj);
                else
                {
                    bool CanBeTypeConverted = false;
                    TypeConverter typeConv = null;
                    typeConv = TypeDescriptor.GetConverter(obj);

                    if (typeConv.IsValid(obj)) if (typeConv.CanConvertTo(typeof(string))) CanBeTypeConverted = true;

                    if (CanBeTypeConverted) ret += "\u0001" + typeConv.ConvertToString(obj);
                    else
                        ret += SerializeObject(obj);
                }
            }

            return ret;
        }

        public static string SerializeBool(bool obj)
        {
            if (obj)
                return "T";
            else
                return "F";
        }

        public static string SerializeObject(object obj)
        {
            string ret = "";

            ret += ABSaveWriter.WriteType(obj.GetType());
            ret += "\u0003" + ABSaveConvert.SerializeABSave(obj) + "\u0005";

            return ret;
        }

        public static string SerializeArray(dynamic obj)
        {
            string ret = "";

            //ret += ABSaveWriter.WriteType(obj.GetType().GetGenericArguments());
            ret += "\u0004";

            foreach (object element in obj)
                ret += Serialize(element);

            ret = ret.Trim('\u0002');
            ret += "\u0005";

            return ret;
        }

        public static string SerializeDictionary(dynamic obj)
        {
            string ret = "";

            ret += "\u0006";

            foreach (dynamic element in obj)
                ret += "\u0001" + element.Key.ToString() + Serialize(element.Value);

            ret = ret.Remove(ret.IndexOf('\u0001'), 1);

            ret += "\u0005";

            return ret;
        }

        public static bool IsArray(this object o)
        {
            try
            {
                Type valueType = o.GetType();
                if (valueType.IsArray)
                    return true;

                return o.GetType().GetGenericTypeDefinition() == typeof(List<>);
            }
            catch { return false; }
        }

        public static bool IsNumericType(this object o)
        {
            if (o != null)
                switch (Type.GetTypeCode(o.GetType()))
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
            else return false;
        }

        public static bool IsDictionary(this object o)
        {
            Type t = o.GetType();
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
    }
}
