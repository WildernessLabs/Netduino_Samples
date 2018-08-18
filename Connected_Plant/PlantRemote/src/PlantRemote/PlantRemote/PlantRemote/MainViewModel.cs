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

        bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set { _isEmpty = value; OnPropertyChanged(nameof(IsEmpty)); }
        }

        string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        bool _showConfig;
        public bool ShowConfig
        {
            get => _showConfig;
            set { _showConfig = value; OnPropertyChanged(nameof(ShowConfig)); }
        }

        bool _isOn;
        public bool IsOn
        {
            get { return _isOn; }
            set { _isOn = value; OnPropertyChanged(nameof(IsOn)); }
        }

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer;
            set { _selectedServer = value; OnPropertyChanged(nameof(SelectedServer)); }
        }

        public ObservableCollection<ServerItem> HostList { get; set; }

        public ObservableCollection<HumidityItem> LevelList { get; set; }

        public Command SendCommand { private set; get; }

        public Command ConnectCommand { private set; get; }

        public Command SwitchServersCommand { private set; get; }

        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {
            plantClient = new PlantClient();
            HostList = new ObservableCollection<ServerItem>();

            LevelList = new ObservableCollection<HumidityItem>();
            LevelList.Add(new HumidityItem() { Date = "17/Aug/2018", Level = 100 });
            LevelList.Add(new HumidityItem() { Date = "19/Aug/2018", Level = 70 });
            LevelList.Add(new HumidityItem() { Date = "20/Aug/2018", Level = 95 });
            LevelList.Add(new HumidityItem() { Date = "24/Aug/2018", Level = 80 });
            LevelList.Add(new HumidityItem() { Date = "25/Aug/2018", Level = 30 });
            LevelList.Add(new HumidityItem() { Date = "27/Aug/2018", Level = 50 });
            LevelList.Add(new HumidityItem() { Date = "29/Aug/2018", Level = 98 });
            LevelList.Add(new HumidityItem() { Date = "30/Aug/2018", Level = 87 });
            LevelList.Add(new HumidityItem() { Date = "1/Sep/2018",  Level = 66 });
            LevelList.Add(new HumidityItem() { Date = "2/Sep/2018",  Level = 44 });

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
        }

        async Task GetServersAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            Status = "Looking for servers";
            ResetState();

            HostList.Clear();

            var servers = await plantClient.FindMapleServersAsync();

            foreach (var server in servers)
            {
                HostList.Add(server);
            }

            if (HostList.Count == 0)
            {
                IsEmpty = true;
            }
            else
            {
                SelectedServer = HostList[0];
            }

            IsBusy = false;
        }

        async Task SendRgbCommand(string command)
        {
            bool isSuccessful = false;

            Status = "Sending command...";
            ResetState();

            switch (command)
            {
                case "TurnOn":
                    if (isSuccessful = await plantClient.TurnOnAsync(SelectedServer))
                        IsOn = true;
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

    public class HumidityItem
    {
        public float Level { get; set; }
        public string Date { get; set; }
    }
}
