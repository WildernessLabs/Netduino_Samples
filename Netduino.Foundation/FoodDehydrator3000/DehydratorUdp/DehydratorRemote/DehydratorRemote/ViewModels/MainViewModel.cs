using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DehydratorRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        DehydratorClient dehydratorClient;

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged("IsLoading"); }
        }

        bool _showRetryButton;
        public bool ShowRetryButton
        {
            get => _showRetryButton;
            set { _showRetryButton = value; OnPropertyChanged("ShowRetryButton"); }
        }

        bool _isOn;
        public bool IsOn
        {
            get => _isOn;
            set { _isOn = value; OnPropertyChanged("IsOn"); }
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

        Temperature _selectedTemperature;
        public Temperature SelectedTemperature
        {
            get => _selectedTemperature;
            set
            {
                _selectedTemperature = value;
                OnPropertyChanged("SelectedTemperature");
            }
        }

        public ObservableCollection<Temperature> TemperatureList { get; set; }

        public Command ConnectCommand { private set; get; }
        public Command TogglePowerCommand { private set; get; }
        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {
            dehydratorClient = new DehydratorClient();
            TemperatureList = new ObservableCollection<Temperature>();
            HostList = new ObservableCollection<ServerItem>();

            ConnectCommand = new Command(async () => await dehydratorClient.StatusAsync(SelectedServer));
            TogglePowerCommand = new Command(async (s) => await TogglePowerAsync());
            SearchServersCommand = new Command(async () => await GetServersAsync());

            LoadTemperatureList();
            GetServersAsync();
        }

        void LoadTemperatureList()
        {
            TemperatureList.Add(new Temperature() { DisplayName = "95°F/35°C (Herbs)", Degrees = 35 });
            TemperatureList.Add(new Temperature() { DisplayName = "105°F/41°C (Herbs)", Degrees = 41 });
            TemperatureList.Add(new Temperature() { DisplayName = "115°F/46°C (Herbs)", Degrees = 46 });
            TemperatureList.Add(new Temperature() { DisplayName = "125°F/52°C (Vegetables & Herbs)", Degrees = 52 });
            TemperatureList.Add(new Temperature() { DisplayName = "135°F/57°C (Fruits)", Degrees = 57 });
            TemperatureList.Add(new Temperature() { DisplayName = "145°F/63°C (Meat & Fish)", Degrees = 63 });
            TemperatureList.Add(new Temperature() { DisplayName = "155°F/68°C (Jerky)", Degrees = 68 });
        }

        void ResetState()
        {
            // Cover the UI and show loading spinner
            IsBusy = true;
            IsLoading = true;
            ShowConfig = false;
            ShowRetryButton = false;
        }

        async Task GetServersAsync()
        {
            Status = "Looking for servers";
            ResetState();

            HostList.Clear();

            var servers = await dehydratorClient.FindMapleServers();

            foreach (var server in servers)
            {
                HostList.Add(server);
            }

            IsLoading = false;

            if (HostList.Count == 0)
            {
                Status = "No servers found...";
                ShowRetryButton = true;
            }
            else
            {
                SelectedServer = HostList[0];
                Status = "Select a server";
                ShowConfig = true;
            }
        }

        async Task TogglePowerAsync()
        {
            bool isSuccessful = false;
            Status = (IsOn)? "Turning off..." : "Turning on...";
            ResetState();

            if (IsOn)
            {
                isSuccessful = await dehydratorClient.TurnOffAsync(SelectedServer);
            }
            else
            {
                isSuccessful = await dehydratorClient.TurnOnAsync(SelectedServer, SelectedTemperature.Degrees);
            }

            if (isSuccessful)
            {
                IsOn = !IsOn;
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
