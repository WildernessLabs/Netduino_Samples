using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ServoRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        ServoClient servoClient;

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
            get => _status;
            set { _status = value; OnPropertyChanged("Status"); }
        }

        bool _showConfig;
        public bool ShowConfig
        {
            get => _showConfig;
            set { _showConfig = value; OnPropertyChanged("ShowConfig"); }
        }

        #region Toggle Buttons Flags
        bool _startCycling;
        public bool StartCycling
        {
            get { return _startCycling; }
            set { _startCycling = value; OnPropertyChanged("StartCycling"); }
        }

        bool _stopCycling;
        public bool StopCycling
        {
            get { return _stopCycling; }
            set { _stopCycling = value; OnPropertyChanged("StopCycling"); }
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
            servoClient = new ServoClient();
            HostList = new ObservableCollection<ServerItem>();

            SendCommand = new Command(async (s) => await SendServoCommand((string)s));

            ConnectCommand = new Command(async () => await SendServoCommand("StartCycling"));

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
            StartCycling = false;
            StopCycling = false;
        }

        async Task GetServersAsync()
        {
            Status = "Looking for servers";
            ResetState();

            HostList.Clear();

            var servers = await servoClient.FindMapleServers();

            foreach (var server in servers)
            {
                HostList.Add(server);
            }

            IsLoading = false;

            if (HostList.Count == 0)
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

        async Task SendServoCommand(string command)
        {
            bool isSuccessful = false;

            Status = "Sending command...";
            ResetState();

            switch (command)
            {
                case "RotateTo":
                    //if (isSuccessful = await servoClient.TurnOnAsync(SelectedServer))
                        //IsOn = true;
                    break;
                case "StartCycling":
                    if (isSuccessful = await servoClient.StartCyclingAsync(SelectedServer))
                        StartCycling = true;
                    break;
                case "StopCycling":
                    if (isSuccessful = await servoClient.StopCyclingAsync(SelectedServer))
                        StopCycling = true;
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
