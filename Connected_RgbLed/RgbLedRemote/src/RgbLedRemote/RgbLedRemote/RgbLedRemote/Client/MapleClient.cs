using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RgbLedRemote
{
    public class MapleClient
    {
        public int ListenPort { get; set; }
        public int ListenTimeout { get; set; }

        public MapleClient(int listenPort = 17756, int listenTimeout = 5000)
        {
            ListenPort = listenPort;
            ListenTimeout = listenTimeout;
        }

        public async Task<List<ServerItem>> FindMapleServersAsync()
        {
            var hostList = new List<ServerItem>();
            var listener = new UdpClient(ListenPort);
            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListenPort);

            var timeoutTask = UdpTimeoutTask();

            try
            {
                while (timeoutTask.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for broadcast");

                    var tasks = new Task<UdpReceiveResult>[] { timeoutTask, listener.ReceiveAsync() };

                    int index = 0;

                    await Task.Run(() => index = Task.WaitAny(tasks));

                    var results = tasks[index].Result;

                    if (results.RemoteEndPoint == null)
                        break;

                    string host = Encoding.UTF8.GetString(results.Buffer, 0, results.Buffer.Length);
                    string hostIp = host.Split('=')[1];

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n", hostIp, host);

                    var serverItem = new ServerItem()
                    {
                        Name = host.Split('=')[0] + " (" + host.Split('=')[1] + ") ",
                        IpAddress = host.Split('=')[1]
                    };

                    if (!hostList.Any(server => server.IpAddress == hostIp))
                    {
                        hostList.Add(serverItem);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                listener.Dispose();
            }

            return hostList;
        }

        async Task<UdpReceiveResult> UdpTimeoutTask()
        {
            await Task.Delay(ListenTimeout);
            return new UdpReceiveResult();
        }

        protected async Task<bool> SendCommandAsync(string command, string hostAddress)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://" + hostAddress + "/"),
                Timeout = TimeSpan.FromSeconds(ListenTimeout)
            };

            try
            {
                var response = await client.PostAsync(command, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}