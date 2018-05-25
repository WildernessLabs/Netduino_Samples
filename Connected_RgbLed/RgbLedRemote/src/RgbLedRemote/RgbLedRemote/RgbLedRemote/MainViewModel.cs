using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RgbLedRemote
{
    public class MainViewModel
    {
        ApiHelper apiHelper;

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        bool showConfig;
        public bool ShowConfig
        {
            get { return showConfig; }
            set { showConfig = value; OnPropertyChanged("ShowConfig"); }
        }

        bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set { isOn = value; OnPropertyChanged("IsOn"); }
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

        string status;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }

        public string HostAddress
        {
            get { return App.HostAddress; }
            set { App.HostAddress = value; OnPropertyChanged("HostAddress"); }
        }

        public Command StartCommand { private set; get; }

        public Command ConnectCommand { private set; get; }

        public MainViewModel()
        {
            IsOn = IsStartBlink = IsStartPulse = IsStartRunningColors = false;
            apiHelper = new ApiHelper();

            StartCommand = new Command((s) =>
            {
                StartCommandExecute((String)s);
            });

            ConnectCommand = new Command(async () =>
            {
                if (!string.IsNullOrEmpty(HostAddress))
                    await ConnectAsync();
            });

            IsBusy = true;
            ShowConfig = true;
            Status = "Enter IP Address:";
        }

        async Task ConnectAsync()
        {
            Status = "Connecting...";
            ShowConfig = false;

            App.IsConnected = await apiHelper.Connect();
            if (App.IsConnected)
            {
                IsBusy = false;
                IsOn = true;
            }
            else
            {
                Status = "Enter IP Address:";
                ShowConfig = true;
            }
        }

        async void StartCommandExecute(string option)
        {
            IsOn = IsStartBlink = IsStartPulse = IsStartRunningColors = false;

            bool isResponseOk = await apiHelper.SendCommand(option);

            if (isResponseOk)
            {
                switch (option)
                {
                    case "on":
                        IsOn = true;
                        break;

                    case "blink":
                        IsStartBlink = true;
                        break;

                    case "pulse":
                        IsStartPulse = true;
                        break;

                    case "runningcolors":
                        IsStartRunningColors = true;
                        break;
                }
            }
        }

        async Task SendRequest(string option)
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            finally
            {
                IsBusy = false;
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
}
