using Maple;
using Microsoft.SPOT;
using Netduino.Foundation.Network;

namespace PlantHost
{
    public class App
    {
        protected MapleServer _server;

        public App()
        {
            InitializePeripherals();
            InitializeWebServer();
        }

        protected void InitializePeripherals()
        {

        }

        protected void InitializeWebServer()
        {
            var handler = new RequestHandler();

            _server = new MapleServer();
            _server.AddHandler(handler);
        }

        public void Run()
        {
            Initializer.InitializeNetwork();
            Initializer.NetworkConnected += InitializerNetworkConnected;
        }

        void InitializerNetworkConnected(object sender, EventArgs e)
        {
            Debug.Print("InitializeNetwork()");

            _server.Start("PlantHost", Initializer.CurrentNetworkInterface.IPAddress);
        }
    }
}