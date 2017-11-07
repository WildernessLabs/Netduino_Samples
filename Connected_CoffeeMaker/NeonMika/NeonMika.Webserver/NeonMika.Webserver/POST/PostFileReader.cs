using System;
using Microsoft.SPOT;
using System.IO;

namespace NeonMika.Webserver.POST
{
    public class PostFileReader : IDisposable
    {
        FileStream fs;

        public PostFileReader()
        {
            fs = new FileStream(Settings.POST_TEMP_PATH, FileMode.Open);
        }

        public byte[] Read(int count)
        {
            byte[] arr = new byte[count];
            fs.Read(arr, 0, count);
            return arr;
        }

        public long Length { get { return fs.Length; } }

        public void Close()
        {
            try { fs.Close(); }
            catch (Exception ex) { }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
