using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RgbLedRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        RgbLedClient rgbClient;

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged("IsLoading"); }
        }

        bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; OnPropertyChanged("IsEmpty"); }
        }


        public string Status
        {
            get =>_status; 
            set { _status = value; OnPropertyChanged("Status"); }
        }
        string _status;

        bool _showConfig;
        public bool ShowConfig
        {
            get =>_showConfig; 
            set { _showConfig = value; OnPropertyChanged("ShowConfig"); }
        }

        #region Toggle Buttons Flags
        bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set { isOn = value; OnPropertyChanged("IsOn"); }
        }

        bool isOff;
        public bool IsOff
        {
            get { return isOff; }
            set { isOff = value; OnPropertyChanged("IsOff"); }
        }

        bool isStartBlink;
        public bool IsStartBlink
        {
            get { return isStartBlink; }
            set { isStartBlink = value; OnPropertyChanged("IsStartBlink"); }
        }

        bool isStartPulse;
        public bool IsStartPulse
        {
            get { return isStartPulse; }
            set { isStartPulse = value; OnPropertyChanged("IsStartPulse"); }
        }

        bool isStartRunningColors;
        public bool IsStartRunningColors
        {
            get { return isStartRunningColors; }
            set { isStartRunningColors = value; OnPropertyChanged("IsStartRunningColors"); }
        }
        #endregion

        public string HostAddress
        {
            get => App.HostAddress; 
            set { App.HostAddress = value; OnPropertyChanged("HostAddress"); }
        }

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer; 
            set 
            { 
                _selectedServer = value;
                App.HostAddress = _selectedServer.IpAddress;
                OnPropertyChanged("SelectedServer"); 
            }
        }

        public ObservableCollection<ServerItem> HostList { get; set; }

        public Command BlinkCommand { private set; get; }
        public Command PulseCommand { private set; get; }
        public Command CycleColorsCommand { private set; get; }
        public Command OffCommand { private set; get; }
        public Command OnCommand { private set; get; }


        public Command ConnectCommand { private set; get; }

        public Command SwitchServersCommand { private set; get; }

        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {            
            rgbClient = new RgbLedClient();
            HostList = new ObservableCollection<ServerItem>();

            BlinkCommand = new Command(async () => await rgbClient.BlinkAsync(SelectedServer));
            PulseCommand = new Command(async () => await rgbClient.PulseAsync(SelectedServer));
            CycleColorsCommand = new Command(async () => await rgbClient.CycleColorsAsync(SelectedServer));
            OnCommand = new Command(async () => await rgbClient.TurnOnAsync(SelectedServer));
            OffCommand = new Command(async () => await rgbClient.TurnOffAsync(SelectedServer));

            ConnectCommand = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(HostAddress))
                {
                    Status = "Connecting...";
                    ShowConfig = false;

                    await rgbClient.TurnOnAsync(SelectedServer);
                }
            });

            SwitchServersCommand = new Command(() =>
            {
                IsBusy = true;
                Status = "Select a server:";
                ShowConfig = true;
            });

            SearchServersCommand = new Command(async () =>
            {
                await GetServersAsync();
            });

            GetServersAsync();
        }

        async Task GetServersAsync ()
        {
            IsBusy = true;
            var servers = await rgbClient.FindMapleServers();

            foreach(var server in servers)
            {
                HostList.Add(server);
            }
            IsBusy = false;
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}