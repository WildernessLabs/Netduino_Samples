using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

            IsBusy = true;
            ShowConfig = true;
            Status = "Enter IP Address:";
        }

        async Task StartCommandExecute(string option)
        {
            IsOn = IsOff = IsStartBlink = IsStartPulse = IsStartRunningColors = false;

            Status = "Sending '" + option + "' Command...";
            IsBusy = true;

            bool isResponseOk = await apiHelper.SendCommand(option);
            if (isResponseOk)
            {
                IsBusy = false;

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
                Status = "Enter IP Address:";
                ShowConfig = true;
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
