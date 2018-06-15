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

        public MainViewModel()
        {
            servoClient = new ServoClient();
            HostList = new ObservableCollection<ServerItem>();

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
            //IsOn = false;
            //IsOff = false;
            //IsBlinking = false;
            //IsPulsing = false;
            //IsRunningColors = false;
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
                case "TurnOn":
                    //if (isSuccessful = await servoClient.TurnOnAsync(SelectedServer))
                        //IsOn = true;
                    break;
                case "TurnOff":
                    //if (isSuccessful = await servoClient.TurnOffAsync(SelectedServer))
                        //IsOff = true;
                    break;
                case "StartBlink":
                    //if (isSuccessful = await servoClient.BlinkAsync(SelectedServer))
                        //IsBlinking = true;
                    break;
                case "StartPulse":
                    //if (isSuccessful = await servoClient.PulseAsync(SelectedServer))
                        //IsPulsing = true;
                    break;
                case "StartRunningColors":
                    //if (isSuccessful = await servoClient.CycleColorsAsync(SelectedServer))
                        //IsRunningColors = true;
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
