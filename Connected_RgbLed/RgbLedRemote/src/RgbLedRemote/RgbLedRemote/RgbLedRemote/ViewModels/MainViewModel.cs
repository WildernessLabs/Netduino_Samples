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

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged("IsLoading"); }
        }

        bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set { _isEmpty = value; OnPropertyChanged("IsEmpty"); }
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
        bool _isOn;
        public bool IsOn
        {
            get { return _isOn; }
            set { _isOn = value; OnPropertyChanged("IsOn"); }
        }

        bool _isOff;
        public bool IsOff
        {
            get { return _isOff; }
            set { _isOff = value; OnPropertyChanged("IsOff"); }
        }

        bool _isStartBlink;
        public bool IsStartBlink
        {
            get { return _isStartBlink; }
            set { _isStartBlink = value; OnPropertyChanged("IsStartBlink"); }
        }

        bool _isStartPulse;
        public bool IsStartPulse
        {
            get { return _isStartPulse; }
            set { _isStartPulse = value; OnPropertyChanged("IsStartPulse"); }
        }

        bool _isStartRunningColors;
        public bool IsStartRunningColors
        {
            get { return _isStartRunningColors; }
            set { _isStartRunningColors = value; OnPropertyChanged("IsStartRunningColors"); }
        }
        #endregion

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer; 
            set 
            { 
                _selectedServer = value;
                OnPropertyChanged("SelectedServer"); 
            }
        }

        public ObservableCollection<ServerItem> HostList { get; set; }

        public Command SendCommand { private set; get; }

        public Command ConnectCommand { private set; get; }

        public Command SwitchServersCommand { private set; get; }

        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {            
            rgbClient = new RgbLedClient();
            HostList = new ObservableCollection<ServerItem>();

            SendCommand = new Command(async (s) => await SendCommandRequest((string)s));

            ConnectCommand = new Command(async () => await SendCommandRequest("TurnOn"));

            SwitchServersCommand = new Command(async () => await GetServersAsync());

            SearchServersCommand = new Command(async () => await GetServersAsync());

            GetServersAsync();
        }

        void ResetState()
        {
            // Block the UI and Show loading spinner
            IsBusy = true;
            IsEmpty = false;
            IsLoading = true;
            ShowConfig = false;

            // All buttons inactive
            IsOn = false;
            IsOff = false;
            IsStartBlink = false;
            IsStartPulse = false;
            IsStartRunningColors = false;
        }

        async Task GetServersAsync ()
        {
            Status = "Looking for servers";
            ResetState();

            HostList.Clear();

            var servers = await rgbClient.FindMapleServers();

            foreach(var server in servers)
            {
                HostList.Add(server);
            }

            IsLoading = false;

            if(HostList.Count == 0)
            {
                Status = "No servers found...";
                IsEmpty = true;
            }
            else
            {
                SelectedServer = HostList[0];
                Status = "Select a server";
                ShowConfig = true;
            }
        }

        async Task SendCommandRequest(string command)
        {
            bool wasCommandSuccessful = false;

            Status = "Sending command...";
            ResetState();

            switch(command)
            {
                case "TurnOn":
                    wasCommandSuccessful = await rgbClient.TurnOnAsync(SelectedServer);
                    IsOn = true;
                    break;

                case "TurnOff":
                    wasCommandSuccessful = await rgbClient.TurnOffAsync(SelectedServer);
                    IsOff = true;
                    break;
                
                case "StartBlink":
                    wasCommandSuccessful = await rgbClient.BlinkAsync(SelectedServer);
                    IsStartBlink = true;
                    break;

                case "StartPulse":
                    wasCommandSuccessful = await rgbClient.PulseAsync(SelectedServer);
                    IsStartPulse = true;
                    break;

                case "StartRunningColors":
                    wasCommandSuccessful = await rgbClient.CycleColorsAsync(SelectedServer);
                    IsStartRunningColors = true;
                    break;
            }

            if (wasCommandSuccessful)
            {
                IsBusy = false;
            }
            else
            {
                await GetServersAsync();
            }
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