using System;
using Microsoft.SPOT;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using NeonMika.Util;
using NeonMika.Webserver.Responses;

namespace NeonMika.Webserver.POST
{
    /// <summary>
    /// Saves a POST-request at Setting.POST_TEMP_PATH
    /// Sends back "OK" on success
    /// </summary>
    class PostToSdWriter : IDisposable
    {
        private byte[] _buffer;
        private int _startAt;
        private Request _e;

        public PostToSdWriter(Request e)
        {
            _e = e;
        }

        /// <summary>
        /// Saves content to Setting.POST_TEMP_PATH
        /// </summary>
        /// <param name="e">The request which should be handeld</param>
        /// <returns>True if 200_OK was sent, otherwise false</returns>
        public bool ReceiveAndSaveData()
        {
            Debug.Print(Debug.GC(true).ToString());
            Debug.Print(Debug.GC(true).ToString());

            int availableBytes = Convert.ToInt32(_e.Headers["Content-Length"].ToString().TrimEnd('\r'));

            try
            {
                Debug.Print("Open temporary post file\n");
                FileStream fs = new FileStream(Settings.POST_TEMP_PATH, FileMode.Create, FileAccess.Write);
                Debug.GC(true).ToString();
                Debug.GC(true).ToString();

                _buffer = new byte[availableBytes > Settings.MAX_REQUESTSIZE ? Settings.MAX_REQUESTSIZE : availableBytes];

                while (availableBytes > 0)
                {
                    Debug.Print("Bystes left to read: " + availableBytes);

                    if(availableBytes < Settings.MAX_REQUESTSIZE)
                        _buffer = new byte[availableBytes];

                    while (_e.Client.Available < _buffer.Length)
                        Thread.Sleep(1);

                    _e.Client.Receive(_buffer, _buffer.Length, SocketFlags.None);
                    fs.Write(_buffer, 0, _buffer.Length);
                    availableBytes -= Settings.MAX_REQUESTSIZE;
                }

                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.Print("Error writing POST-data");
                return false;
            }

            return true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _buffer = new byte[0];            
        }

        #endregion
    }
}
