using CarRemote.Client;
using Maple;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        CarClient carClient;

        bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
        }

        bool isButtonUpPressed;
        public bool IsButtonUpPressed
        {
            get => isButtonUpPressed;
            set { isButtonUpPressed = value; OnPropertyChanged(nameof(IsButtonUpPressed)); }
        }

        bool isButtonDownPressed;
        public bool IsButtonDownPressed
        {
            get => isButtonDownPressed;
            set { isButtonDownPressed = value; OnPropertyChanged(nameof(IsButtonDownPressed)); }
        }

        bool isButtonLeftPressed;
        public bool IsButtonLeftPressed
        {
            get => isButtonLeftPressed;
            set { isButtonLeftPressed = value; OnPropertyChanged(nameof(IsButtonLeftPressed)); }
        }

        bool isButtonRightPressed;
        public bool IsButtonRightPressed
        {
            get => isButtonRightPressed;
            set { isButtonRightPressed = value; OnPropertyChanged(nameof(IsButtonRightPressed)); }
        }

        bool _isServerListEmpty;
        public bool IsServerListEmpty
        {
            get => _isServerListEmpty;
            set { _isServerListEmpty = value; OnPropertyChanged(nameof(IsServerListEmpty)); }
        }

        string status;
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer;
            set { _selectedServer = value; OnPropertyChanged(nameof(SelectedServer)); }
        }

        public ObservableCollection<ServerItem> ServerList { get; set; }

        public Command RefreshServersCommand { private set; get; }

        public MainViewModel()
        {
            ServerList = new ObservableCollection<ServerItem>();

            RefreshServersCommand = new Command(async ()=> { await GetServersAsync(); });

            carClient = new CarClient();

            GetServersAsync();
        }

        async Task GetServersAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            Status = "Searching...";
            ServerList.Clear();

            var servers = await carClient.FindMapleServersAsync();
            foreach (var server in servers)
                ServerList.Add(server);

            if (servers.Count > 0)
            {
                SelectedServer = ServerList[0];
                Status = "Connected";
                IsConnected = true;
            }
            else
            {
                Status = "No cars found";
                IsConnected = false;
            }

            IsServerListEmpty = servers.Count == 0;
            

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
