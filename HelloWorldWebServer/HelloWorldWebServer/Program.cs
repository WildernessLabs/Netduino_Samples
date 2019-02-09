using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.Threading;
using System.Net;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;

namespace HelloWorldWebServer
{
    public class Program
    {
        public static void Main()
        {
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableDhcp();
            WebServer webServer = new WebServer();
            webServer.ListenForRequest();
        }
    }
}
