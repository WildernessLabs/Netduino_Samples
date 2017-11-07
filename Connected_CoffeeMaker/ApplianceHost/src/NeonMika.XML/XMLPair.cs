using System;
using System.Collections;

namespace NeonMika.XML
{
    public class XMLPair : XMLObject
    {
        public XMLPair(string Key, object Value)
        {
            this.Key = Key;
            this.Value = Value;
            this.Attributes = new Hashtable();
        }

        public XMLPair(string Key)
        {
            this.Key = Key;
            this.Value = null;
            this.Attributes = new Hashtable();
        }

        public override string ToString()
        {
            string attributes = "";
            foreach ( object o in Attributes.Keys )
                attributes += " " + o.ToString( ) + "=\"" + Attributes[o] + "\"";

            string s = "<" + Key + attributes + ">";
            s += Value.ToString( );
            s += "</" + Key + ">";
            return s;
        }

        #region XMLObject Members

        public string Key
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public Hashtable Attributes
        {
            get;
            set;
        }

        #endregion
    }
}
