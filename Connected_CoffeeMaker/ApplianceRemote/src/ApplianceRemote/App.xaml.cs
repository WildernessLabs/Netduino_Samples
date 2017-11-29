using System;

using Xamarin.Forms;

namespace ApplianceRemote
{
    public partial class App : Application
    {
        public static string HostAddress = string.Empty;
        public static bool IsConnected = false;
        public static ApplianceStatus ApplianceStatus = ApplianceStatus.Off;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StatusPage());

            //if (Device.RuntimePlatform == Device.iOS)
            //    MainPage = new MainPage();
            //else
                //MainPage = new NavigationPage(new StatusPage());
        }
    }
}
