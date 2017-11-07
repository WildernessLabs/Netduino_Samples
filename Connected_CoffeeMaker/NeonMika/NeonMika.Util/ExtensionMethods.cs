using System;
using Microsoft.SPOT;

namespace NeonMika.Util
{
    public static class ExtensionMethods
    {
        public static string[] EasySplit(this string s, string seperator)
        {
            int pos = s.IndexOf(seperator);
            if (pos != -1)
                return new string[] { s.Substring(0, pos).Trim(new char[] { ' ', '\n', '\r' }), s.Substring(pos + seperator.Length, s.Length - pos - seperator.Length).Trim(new char[] { ' ', '\n', '\r' }) };
            else
                return new string[] { s.Trim(new char[] { ' ', '\n', '\r' }) };
        }

        public static bool StartsWith(this string s, string start)
        {
            for (int i = 0; i < start.Length; i++)
                if (s[i] != start[i])
                    return false;

            return true;
        }

        public static string Replace(this string s, char replaceThis, char replaceWith)
        {
            string temp = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == replaceThis)
                    temp += replaceWith;
                else
                    temp += s[i];
            }
            return temp;
        }
    }
}
