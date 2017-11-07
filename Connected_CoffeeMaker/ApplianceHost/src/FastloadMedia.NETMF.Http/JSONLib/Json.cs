using System;
using System.Reflection;

namespace FastloadMedia.NETMF.Http
{
    /// <summary>
    /// This static class contains a method to convert a value to its JSON equivalent.
    /// Programmed by Huysentruit Wouter
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Lookup table for hex values.
        /// </summary>
        private const string HEX_CHARS = "0123456789ABCDEF";
        public const string ContentType = "application/json";

        /// <summary>
        /// Converts a character to its javascript unicode equivalent.
        /// </summary>
        /// <param name="c">The character to convert.</param>
        /// <returns>The javascript unicode equivalent.</returns>
        private static string JsUnicode(char c)
        {
            string result = "\\u";
            ushort value = (ushort)c;

            for (int i = 0; i < 4; i++, value <<= 4)
                result += HEX_CHARS[value >> 12];

            return result;
        }

        /// <summary>
        /// Encodes a javascript string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        private static string JsEncode(string s)
        {
            string result = "";

            //foreach (char c in s)
            for(int i=0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < (char)127)
                {
                    switch (c)
                    {
                        case '\b': result += "\\b"; break;
                        case '\t': result += "\\t"; break;
                        case '\n': result += "\\n"; break;
                        case '\f': result += "\\f"; break;
                        case '\r': result += "\\r"; break;
                        case '"': result += "\\\""; break;
                        case '/': result += "\\/"; break;
                        case '\\': result += "\\\\"; break;
                        default:
                            if (c < ' ')
                                result += JsUnicode(c);
                            else
                                result += c;
                            break;
                    }
                }
                else
                    result += JsUnicode(c);
            }

            return result;
        }
                
        /// <summary>
        /// Convert a value to JSON.
        /// </summary>
        /// <param name="o">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, JsonObject, JsonArray, Array, Object and null.</param>
        /// <returns>The JSON object as a string or null when the value type is not supported.</returns>
        /// <remarks>For objects, only public fields are converted.</remarks>
        public static string ToJson(object o)
        {
            if (o == null)
                return "null";

            Type type = o.GetType();
            switch (type.Name)
            {
                case "Boolean":
                    return (bool)o ? "true" : "false";
                case "String":
                    return "\"" + JsEncode((string)o) + "\"";
                case "Byte":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Single":
                case "Double":
                case "Decimal":
                case "JsonObject":
                case "JsonArray":
                    return o.ToString();
            }

            if (type.IsArray)
            {
                JsonArray jsonArray = new JsonArray();
                foreach (object i in (Array)o)
                    jsonArray.Add(i);
                return jsonArray.ToString();
            }

            if (type.IsClass)
            {
                JsonObject jsonObject = new JsonObject();
                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo info in fields)
                    jsonObject.Add(info.Name, info.GetValue(o));
                return jsonObject.ToString();
            }

            return null;
        }
    }
}
