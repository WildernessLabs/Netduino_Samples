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
            get => _isBusy; 
            set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); }
        }

        bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set { _isEmpty = value; OnPropertyChanged(nameof(IsEmpty)); }
        }

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer;
            set { _selectedServer = value; OnPropertyChanged(nameof(SelectedServer)); }
        }

        public ObservableCollection<ServerItem> ServerList { get; set; }
        public ObservableCollection<HumidityLog> LevelList { get; set; }

        public Command GetHumidityCommand { private set; get; }
        public Command RefreshServersCommand { private set; get; }

        public MainViewModel()
        {
            plantClient = new PlantClient();

            LevelList = new ObservableCollection<HumidityLog>();
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

            LevelList.Clear();

            var humitidyLogs = await plantClient.GetHumidityAsync(SelectedServer);
            foreach(var log in humitidyLogs)
                LevelList.Insert(0, log);

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