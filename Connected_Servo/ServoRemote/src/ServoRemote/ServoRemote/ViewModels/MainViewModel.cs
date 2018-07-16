using Maple;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ServoRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        ServoControllerClient servoControllerClient;

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; 
            set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
        }

        bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading; 
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty; 
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

        int angleDegrees;
        public int AngleDegrees
        {
            get => angleDegrees;
            set { angleDegrees = value; OnPropertyChanged(nameof(AngleDegrees)); }
        }

        #region Toggle Buttons Flags
        bool _isRotating;
        public bool IsRotating
        {
            get => _isRotating;
            set { _isRotating = value; OnPropertyChanged(nameof(IsRotating)); }
        }

        bool _startSweep;
        public bool StartSweep
        {
            get => _startSweep;
            set { _startSweep = value; OnPropertyChanged(nameof(StartSweep)); }
        }

        bool _stopSweep;
        public bool StopSweep
        {
            get => _stopSweep;
            set { _stopSweep = value; OnPropertyChanged(nameof(StopSweep)); }
        }
        #endregion

        ServerItem _selectedServer;
        public ServerItem SelectedServer
        {
            get => _selectedServer;
            set { _selectedServer = value; OnPropertyChanged(nameof(SelectedServer)); }
        }

        public ObservableCollection<ServerItem> HostList { get; set; }

        public Command SendCommand { private set; get; }

        public Command ConnectCommand { private set; get; }

        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {
            servoControllerClient = new ServoControllerClient();
            HostList = new ObservableCollection<ServerItem>();

            SendCommand = new Command(async (s) => await SendServoCommandAsync((string)s));

            ConnectCommand = new Command(async () => await SendServoCommandAsync("StartSweep"));

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
            StartSweep = false;
            StopSweep = false;
        }

        async Task GetServersAsync()
        {
            Status = "Looking for servers";
            ResetState();

            HostList.Clear();

            var servers = await servoControllerClient.FindMapleServersAsync();

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

        async Task SendServoCommandAsync(string command)
        {
            bool isSuccessful = false;

            Status = "Sending command...";
            ResetState();

            switch (command)
            {
                case "RotateTo":
                    if (isSuccessful = await servoControllerClient.RotateToAsync(SelectedServer, AngleDegrees))
                        IsRotating = true;
                        Device.StartTimer(TimeSpan.FromMilliseconds(500), () => 
                        {
                            IsRotating = false;
                            return false;
                        });
                    break;
                case "StartSweep":
                    if (isSuccessful = await servoControllerClient.StartSweepAsync(SelectedServer))
                        StartSweep = true;
                    break;
                case "StopSweep":
                    if (isSuccessful = await servoControllerClient.StopSweepAsync(SelectedServer))
                        StopSweep = true;
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
