using Xamarin.Forms;

namespace Dehydrator.Mobile
{
	public partial class FormsApp : Application
	{
		public FormsApp()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new HomePage());
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
