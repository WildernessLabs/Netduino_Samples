using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Maple;
using Xamarin.Forms;

namespace PlantRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        PlantClient plantClient;

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); }
        }

        bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set { _isEmpty = value; OnPropertyChanged(nameof(IsEmpty)); }
        }

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer;
            set { _selectedServer = value; OnPropertyChanged(nameof(SelectedServer)); }
        }

        public ObservableCollection<ServerItem> ServerList { get; set; }
        public ObservableCollection<HumidityLevel> LevelList { get; set; }

        public Command GetHumidityCommand { private set; get; }
        public Command RefreshServersCommand { private set; get; }

        public MainViewModel()
        {
            plantClient = new PlantClient();

            LevelList = new ObservableCollection<HumidityLevel>();
            ServerList = new ObservableCollection<ServerItem>();

            GetHumidityCommand = new Command(async (s) => await GetHumidityCommandExecute());
            RefreshServersCommand = new Command(async () => await GetServersAsync());

            GetServersAsync();
        }

        async Task GetServersAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            ServerList.Clear();

            var servers = await plantClient.FindMapleServersAsync();
            foreach (var server in servers)
            {
                ServerList.Add(server);
            }

            if (servers.Count > 0)
            {
                SelectedServer = ServerList[0];
                await GetHumidityCommandExecute();
            }
            else
            {
                IsEmpty = true;
            }

            IsBusy = false;
        }

        async Task GetHumidityCommandExecute()
        {
            if (SelectedServer == null)
                return;

            int humitidy = -1;
            while(humitidy == -1)
            {
                humitidy = await plantClient.GetHumidityAsync(SelectedServer);
            }

            LevelList.Insert(0, new HumidityLevel()
            {
                Date = DateTime.Now.ToString("hh:mm tt dd'/'MMM'/'yyyy"),
                Level = humitidy
            });

            IsRefreshing = false;
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