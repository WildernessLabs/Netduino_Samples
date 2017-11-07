using System.Collections;

namespace FastloadMedia.NETMF.Http
{
    /// <summary>
    /// A Json Object.
    /// Programmed by Huysentruit Wouter
    /// 
    /// Keys must be strings!
    /// Supported value types are those supported by the Json.ToJson method.
    /// See the Json.ToJson method for more information.
    /// </summary>
    public class JsonObject : Hashtable
    {
        /// <summary>
        /// Convert the object to its JSON representation.
        /// </summary>
        /// <returns>A string containing the JSON representation of the object.</returns>
        public override string ToString()
        {
            string result = "";

            string[] keys = new string[Count];
            object[] values = new object[Count];

            Keys.CopyTo(keys, 0);
            Values.CopyTo(values, 0);
            
            for (int i = 0; i < Count; i++)
            {
                if (result.Length > 0)
                    result += ", ";

                string value = Json.ToJson(values[i]);
                if (value == null)
                    continue;
                
                result += "\"" + keys[i] + "\"";
                result += ": ";
                result += value;
            }

            return "{" + result + "}";
        }
    }
}
