using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sockets.Plugin;
using Xamarin.Forms;

namespace DehydratorRemote
{
    public partial class DehydratorRemotePage : ContentPage
    {
        private const int MAPLE_SERVER_BROADCAST_PORT = 17756;

        private Temperature _temperature;
        private string _hostAddress;
        private string _hostName = "dehydrator3000";

        public DehydratorRemotePage()
        {
            InitializeComponent();
            this.Title = "Dehydrator3000";
            this.MyTemp = new Temperature { DisplayName = "125°F (52°C)", Description = "Vegetables & Herbs", Degrees = 52 };
        }

		async protected override void OnAppearing()
		{
			base.OnAppearing();

            var receiver = new UdpSocketReceiver();

            receiver.MessageReceived += (sender, args) =>
            {
                var data = System.Text.Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);
                if(data.StartsWith(_hostName, System.StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(_hostAddress))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        HostAddress.Text = _hostAddress = data.Split(new char[] { '=' })[1];

                        ApiHelper helper = new ApiHelper(_hostAddress);
                        var isConnected = await helper.Connect();
                        if (isConnected)
                        {
                            PowerToggle.On = await helper.CheckStatus();
                        }
                    });
                }

                Debug.WriteLine(data);
            };

            // listen for udp traffic on listenPort
            await receiver.StartListeningAsync(MAPLE_SERVER_BROADCAST_PORT);

		}

		public Temperature MyTemp
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                this.SelectedTemperature.Text = _temperature.DisplayName;
            }
        }

		async void Handle_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new TemperatureSelectionPage());
        }

        async void Handle_Completed(object sender, System.EventArgs e)
        {
            ApiHelper helper = new ApiHelper(HostAddress.Text);
            var isConnected = await helper.Connect();
            if (isConnected)
            {
                PowerToggle.On = await helper.CheckStatus();
            }
        }

        async void Handle_OnChanged(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            ApiHelper helper = new ApiHelper(HostAddress.Text);
            var isConnected = await helper.Connect();

            if(isConnected)
            {
                if (e.Value)
                {
                    var greatSuccess = await helper.TurnOn(MyTemp.Degrees);
                    if (!greatSuccess)
                    {
                        await DisplayAlert("Error", "There was a problem turning on the dehydrator.", "OK");
                    }
                }
                else
                {
                    var greatSuccess = await helper.TurnOff();
                    if (!greatSuccess)
                    {
                        await DisplayAlert("Error", "There was a problem turning off the dehydrator.", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "There was a problem connecting to the dehydrator. Please check the Host Address.", "OK");
            }
        }
    }
}
