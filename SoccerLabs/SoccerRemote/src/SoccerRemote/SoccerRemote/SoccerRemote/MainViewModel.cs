using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Maple;
using Xamarin.Forms;

namespace SoccerRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        ServoClient _servoClient;

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

        bool _teamA;
        public bool TeamA
        {
            get { return _teamA; }
            set { _teamA = value; OnPropertyChanged("TeamCommandA"); }
        }

        bool _teamB;
        public bool TeamB
        {
            get { return _teamB; }
            set { _teamB = value; OnPropertyChanged("TeamCommandB"); }
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

        public ObservableCollection<ServerItem> ServerList { get; set; }

        public Command TeamACommand { private set; get; }

        public Command TeamBCommand { private set; get; }

        public Command ConnectCommand { private set; get; }

        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {
            _servoClient = new ServoClient();
            _servoClient.ListenTimeout = 5000;
            ServerList = new ObservableCollection<ServerItem>();

            TeamACommand = new Command(async () => await SendServoCommand("ThrowKickA"));

            TeamBCommand = new Command(async () => await SendServoCommand("ThrowKickB"));

            ConnectCommand = new Command(async () => await SendServoCommand("ThrowKickA"));

            SearchServersCommand = new Command(async () => await GetServersAsync());

            GetServersAsync();
        }

        async Task GetServersAsync()
        {
            Status = "Looking for servers";
            ResetState();

            ServerList.Clear();

            var servers = await _servoClient.FindMapleServersAsync();

            foreach (var server in servers)
            {
                ServerList.Add(server);
            }

            IsLoading = false;

            if (ServerList.Count == 0)
            {
                Status = "No servers found...";
                IsEmpty = true;
            }
            else
            {
                SelectedServer = ServerList[0];
                Status = "Select a server";
                ShowConfig = true;
            }
        }

        void ResetState()
        {
            // Cover the UI and show loading spinner
            IsBusy = true;
            IsEmpty = false;
            IsLoading = true;
            ShowConfig = false;

            // All buttons inactive
            TeamA = false;
            TeamB = false;
        }

        async Task SendServoCommand(string command)
        {
            bool isSuccessful = false;

            Status = "Sending command...";
            ResetState();

            switch (command)
            {
                case "ThrowKickA":
                    if (isSuccessful = await _servoClient.TurnOnAsync(SelectedServer))
                        TeamA = true;
                    break;
                case "ThrowKickB":
                    if (isSuccessful = await _servoClient.TurnOffAsync(SelectedServer))
                        TeamB = true;
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
