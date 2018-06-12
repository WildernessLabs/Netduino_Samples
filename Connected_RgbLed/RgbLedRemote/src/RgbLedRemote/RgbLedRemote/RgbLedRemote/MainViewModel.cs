using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RgbLedRemote
{
    public class MainViewModel : INotifyPropertyChanged
    {
        ApiHelper apiHelper;

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged("IsLoading"); }
        }

        bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; OnPropertyChanged("IsEmpty"); }
        }

        string status;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }

        bool showConfig;
        public bool ShowConfig
        {
            get { return showConfig; }
            set { showConfig = value; OnPropertyChanged("ShowConfig"); }
        }

        #region Toggle Buttons Flags
        bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set { isOn = value; OnPropertyChanged("IsOn"); }
        }

        bool isOff;
        public bool IsOff
        {
            get { return isOff; }
            set { isOff = value; OnPropertyChanged("IsOff"); }
        }

        bool isStartBlink;
        public bool IsStartBlink
        {
            get { return isStartBlink; }
            set { isStartBlink = value; OnPropertyChanged("IsStartBlink"); }
        }

        bool isStartPulse;
        public bool IsStartPulse
        {
            get { return isStartPulse; }
            set { isStartPulse = value; OnPropertyChanged("IsStartPulse"); }
        }

        bool isStartRunningColors;
        public bool IsStartRunningColors
        {
            get { return isStartRunningColors; }
            set { isStartRunningColors = value; OnPropertyChanged("IsStartRunningColors"); }
        }
        #endregion

        public string HostAddress
        {
            get { return App.HostAddress; }
            set { App.HostAddress = value; OnPropertyChanged("HostAddress"); }
        }

        MapleServerItem selectedServer;
        public MapleServerItem SelectedServer
        {
            get { return selectedServer; }
            set 
            { 
                selectedServer = value;
                App.HostAddress = selectedServer.IpAddress;
                OnPropertyChanged("SelectedServer"); 
            }
        }

        public ObservableCollection<MapleServerItem> HostList { get; set; }

        public Command StartCommand { private set; get; }

        public Command ConnectCommand { private set; get; }

        public Command SwitchServersCommand { private set; get; }

        public Command SearchServersCommand { private set; get; }

        public MainViewModel()
        {            
            apiHelper = new ApiHelper();
            HostList = new ObservableCollection<MapleServerItem>();

            StartCommand = new Command(async (s) =>
            {
                await StartCommandExecute((String)s);
            });

            ConnectCommand = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(HostAddress))
                {
                    Status = "Connecting...";
                    ShowConfig = false;
                    
                    await StartCommandExecute("TurnOn");
                }
            });

            SwitchServersCommand = new Command(() =>
            {
                IsBusy = true;
                Status = "Select a server:";
                ShowConfig = true;
            });

            SearchServersCommand = new Command(async () =>
            {
                await SearchMapleServers();
            });

            SearchMapleServers();
        }

        async Task SearchMapleServers()
        {
            IsBusy = true;
            IsEmpty = false;
            IsLoading = true;
            ShowConfig = false;
            Status = "Looking for servers...";

            int listenPort = 17756;
            bool isSearching = true;

            UdpClient listener = new UdpClient(listenPort);  
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);  

            try   
            {
                HostList.Clear();
                while (isSearching)
                {
                    Console.WriteLine("Waiting for broadcast");

                    var bytes = await Task.Run(() =>
                    {
                        var task = listener.ReceiveAsync();
                        task.Wait(5000);
                        if (task.IsCompleted)
                        { return task.Result; }
                        throw new TimeoutException();
                    });

                    string Host = Encoding.UTF8.GetString(bytes.Buffer, 0, bytes.Buffer.Length);
                    string HostIp = Host.Split('=')[1];

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n", HostIp, Host);

                    var serverItem = new MapleServerItem()
                    {
                        Name = Host.Split('=')[0] + " (" + Host.Split('=')[1] + ") ",
                        IpAddress = Host.Split('=')[1]
                    };

                    if (!HostList.Any(server => server.IpAddress == HostIp))
                    {
                        HostList.Add(serverItem);
                    }

                    if(HostList.Count > 0)
                    {
                        SelectedServer = HostList[0];
                        Status = "Select a server:";
                        IsLoading = false;
                        ShowConfig = true;
                    }
                }
            }   
            catch (Exception e)   
            {  
                if(HostList.Count==0)
                {
                    IsLoading = false;
                    Status = "Servers not found";
                    IsEmpty = true;
                }

                Console.WriteLine(e.Message);  
            }  
            finally  
            {
                isSearching = false;
                listener.Close();  
            }  
        }

        async Task StartCommandExecute(string option)
        {
            IsOn = IsOff = IsStartBlink = IsStartPulse = IsStartRunningColors = false;

            Status = "Sending '" + option + "' Command...";
            IsBusy = true;
            IsLoading = true;

            bool isResponseOk = await apiHelper.SendCommand(option);
            if (isResponseOk)
            {
                IsBusy = false;
                IsLoading = false;

                switch (option)
                {
                    case "TurnOn":
                        IsOn = true;
                        break;

                    case "TurnOff":
                        IsOff = true;
                        break;

                    case "StartBlink":
                        IsStartBlink = true;
                        break;

                    case "StartPulse":
                        IsStartPulse = true;
                        break;

                    case "StartRunningColors":
                        IsStartRunningColors = true;
                        break;
                }
            }
            else
            {
                await SearchMapleServers();
            }
        }

        #region INotifyPropertyChanges Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class MapleServerItem
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
    }
}
