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
        static readonly int LISTEN_PORT = 17756;
        static readonly int LISTEN_TIMEOUT = 10000; //ms

        public Task<UdpReceiveResult> UdpTimeoutTask()
        {
            Task.Delay(LISTEN_TIMEOUT);
            return null;
        }

        public Task<List<ServerItem>> FindMapleServers()
        {
            var hostList = new List<ServerItem>();
            var listener = new UdpClient(LISTEN_PORT);
            var ipEndPoint = new IPEndPoint(IPAddress.Any, LISTEN_PORT);

            var timeoutTask = UdpTimeoutTask();

            try
            {
                while (timeoutTask.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for broadcast");

                    var tasks = new Task<UdpReceiveResult>[] { timeoutTask, listener.ReceiveAsync() };
                    int index = Task.WaitAny(tasks);
                    var results = tasks[index].Result;

                    if (results == null)
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
                listener.Close();
            }

            return Task.FromResult(hostList);
        }

        protected async Task<bool> SendCommandAsync(string command, string hostAddress)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + hostAddress + "/");
            client.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                var response = await client.PostAsync(command, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
        }
    }
}