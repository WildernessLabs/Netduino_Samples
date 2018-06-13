using Xamarin.Forms;

namespace RgbLedRemote
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            BindingContext = new MainViewModel();
        }
	}
}
