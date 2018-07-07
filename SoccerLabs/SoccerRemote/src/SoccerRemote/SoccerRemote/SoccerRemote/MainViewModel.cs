using System;
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

        bool _areButtonsBlocked;
        public bool AreButtonsBlocked
        {
            get => _areButtonsBlocked;
            set { _areButtonsBlocked = value; OnPropertyChanged("AreButtonsBlocked"); }
        }

        bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged("IsLoading"); }
        }

        bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty; 
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
            get => _teamA;
            set { _teamA = value; OnPropertyChanged("TeamA"); }
        }

        bool _teamB;
        public bool TeamB
        {
            get => _teamB;
            set { _teamB = value; OnPropertyChanged("TeamB"); }
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
            ServerList = new ObservableCollection<ServerItem>();

            TeamACommand = new Command(async () => 
            {
                TeamA = true;
                if (await _servoClient.ThrowKickAAsync(SelectedServer))
                    TeamA = false;
                else
                    await GetServersAsync();
            });

            TeamBCommand = new Command(async () => 
            {
                TeamB = true;
                if (await _servoClient.ThrowKickBAsync(SelectedServer))
                    TeamB = false;
                else
                    await GetServersAsync();
            });

            ConnectCommand = new Command(async () => 
            {
                if (await _servoClient.ConnectAsync(SelectedServer))
                    AreButtonsBlocked = false;
                else
                    await GetServersAsync();
            });

            SearchServersCommand = new Command(async () => await GetServersAsync());

            GetServersAsync();
        }

        void ResetState()
        {
            // Cover the UI and show loading spinner
            AreButtonsBlocked = true;
            IsEmpty = false;
            IsLoading = true;
            ShowConfig = false;

            // All buttons inactive
            TeamA = false;
            TeamB = false;
        }

        void ShowCommandSent(string message)
        {
            Status = message;
            ResetState();
        }

        async Task GetServersAsync()
        {
            ShowCommandSent("Looking for servers");

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

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
