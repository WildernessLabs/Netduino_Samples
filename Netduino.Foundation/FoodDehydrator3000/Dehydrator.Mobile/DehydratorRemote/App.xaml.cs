using System.Diagnostics;
using Sockets.Plugin;
using Xamarin.Forms;

namespace DehydratorRemote
{
    public partial class App : Application
    {
        private const int MAPLE_SERVER_BROADCAST_PORT = 17756;
        private const string MAPLE_HOSTNAME = "dehydrator3000";

        public static string DehydratorHostAddress { get; set; }
        public static Temperature TargetTemperature { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new DehydratorRemotePage());
        }

        async protected override void OnStart()
        {
            var receiver = new UdpSocketReceiver();
            receiver.MessageReceived += (sender, args) =>
            {
                var data = System.Text.Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);
                if (data.StartsWith(MAPLE_HOSTNAME, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    var address = data.Split(new char[] { '=' })[1];
                    if(string.Compare(address, DehydratorHostAddress)!=0)
                    {
                        DehydratorHostAddress = address;
                        Debug.WriteLine("HostAddress updated: " + DehydratorHostAddress);
                    }
                }
            };
            await receiver.StartListeningAsync(MAPLE_SERVER_BROADCAST_PORT);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
