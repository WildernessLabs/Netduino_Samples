using System;
using System.Text;
using System.Collections;

namespace NeonMika.XML
{
    interface XMLObject
    {
        string Key { get; set; }
        object Value { get; set; }
        Hashtable Attributes { get; set; }
    }
}
