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

        string _status;
        public string Status
        {
            get =>_status; 
            set { _status = value; OnPropertyChanged("Status"); }
        }

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
        public bool IsBlinking
        {
            get { return _isStartBlink; }
            set { _isStartBlink = value; OnPropertyChanged("IsBlinking"); }
        }

        bool _isStartPulse;
        public bool IsPulsing
        {
            get { return _isStartPulse; }
            set { _isStartPulse = value; OnPropertyChanged("IsPulsing"); }
        }

        bool _isStartRunningColors;
        public bool IsRunningColors
        {
            get { return _isStartRunningColors; }
            set { _isStartRunningColors = value; OnPropertyChanged("IsRunningColors"); }
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

            SendCommand = new Command(async (s) => await SendRgbCommand((string)s));

            ConnectCommand = new Command(async () => await SendRgbCommand("TurnOn"));

            SwitchServersCommand = new Command(async () => await GetServersAsync());

            SearchServersCommand = new Command(async () => await GetServersAsync());

            GetServersAsync();
        }

        void ResetState()
        {
            // Cover the UI and show loading spinner
            IsBusy = true;
            IsEmpty = false;
            IsLoading = true;
            ShowConfig = false;

            // All buttons inactive
            IsOn = false;
            IsOff = false;
            IsBlinking = false;
            IsPulsing = false;
            IsRunningColors = false;
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

        async Task SendRgbCommand(string command)
        {
            bool isSuccessful = false;

            Status = "Sending command...";
            ResetState();

            switch(command)
            {
                case "TurnOn":
                    if(isSuccessful = await rgbClient.TurnOnAsync(SelectedServer))
                        IsOn = true;
                    break;
                case "TurnOff":
                    if(isSuccessful = await rgbClient.TurnOffAsync(SelectedServer))
                        IsOff = true;
                    break;
                case "StartBlink":
                    if(isSuccessful = await rgbClient.BlinkAsync(SelectedServer))
                        IsBlinking = true;
                    break;
                case "StartPulse":
                    if(isSuccessful = await rgbClient.PulseAsync(SelectedServer))
                        IsPulsing = true;
                    break;
                case "StartRunningColors":
                    if(isSuccessful = await rgbClient.CycleColorsAsync(SelectedServer))
                        IsRunningColors = true;
                    break;
            }

            if (isSuccessful)
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