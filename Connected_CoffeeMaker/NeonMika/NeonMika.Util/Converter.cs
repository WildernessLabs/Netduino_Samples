using System;
using Microsoft.SPOT;
using System.Collections;
using System.IO;

namespace NeonMika.Util
{
    public static class Converter
    {
        public static Hashtable ToHashtable(string[] lines, string seperator, int startAtLine = 0)
        {
            Hashtable toReturn = new Hashtable( );
            string[] line;
            for ( int i = startAtLine; i < lines.Length; i++ )
            {
                line = lines[i].EasySplit(seperator);
                if ( line.Length > 1 )
                    toReturn.Add(line[0], line[1]);

            }
            return toReturn;
        }       
    }
}
