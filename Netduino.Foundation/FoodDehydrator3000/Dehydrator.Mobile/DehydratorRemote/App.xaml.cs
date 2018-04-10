using Xamarin.Forms;

namespace DehydratorRemote
{
    public partial class App : Application
    {
        public static Temperature TargetTemperature { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new DehydratorRemotePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
