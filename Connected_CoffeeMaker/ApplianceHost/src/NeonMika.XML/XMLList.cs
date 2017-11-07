using System;
using Microsoft.SPOT;
using System.Collections;

namespace NeonMika.XML
{
    public class XMLList : ArrayList, XMLObject
    {
        public XMLList(string key)
        {
            Key = key;
            Attributes = new Hashtable( );
        }

        public override string ToString( )
        {
            string attributes = "";
            foreach ( object o in Attributes.Keys )
                attributes += " " + o.ToString( ) + "=\"" + Attributes[o] + "\"";

            string s = "<"+Key+attributes+">";

            for ( int i = 0; i < Count; i++ )
                s += this[i].ToString( );

            return s + "</" + Key + ">";
        }

        #region XMLObject Members

        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Don't use Value, work with the list itself!
        /// </summary>
        public object Value
        {
            get { return null; }
            set { }
        }

        public Hashtable Attributes
        {
            get;
            set;
        }

        #endregion
    }
}
