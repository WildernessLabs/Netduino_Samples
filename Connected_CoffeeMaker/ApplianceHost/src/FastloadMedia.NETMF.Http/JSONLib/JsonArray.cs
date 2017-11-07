using System.Collections;

namespace FastloadMedia.NETMF.Http
{
    /// <summary>
    /// A Json Array.
    /// Programmed by Huysentruit Wouter
    /// 
    /// Supported value types are those supported by the Json.ToJson method.
    /// See the Json.ToJson method for more information.
    /// </summary>
    public class JsonArray : ArrayList
    {
        /// <summary>
        /// Convert the array to its JSON representation.
        /// </summary>
        /// <returns>A string containing the JSON representation of the array.</returns>
        public override string ToString()
        {
            string[] parts = new string[Count];

            for (int i = 0; i < Count; i++)
                parts[i] = Json.ToJson(this[i]);

            string result = "";

            foreach (string part in parts)
            {
                if (result.Length > 0)
                    result += ", ";

                result += part;
            }

            return "[" + result + "]";
        }
    }
}
