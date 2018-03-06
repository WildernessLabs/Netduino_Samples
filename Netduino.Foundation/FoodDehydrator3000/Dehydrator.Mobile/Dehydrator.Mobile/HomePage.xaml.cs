using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Dehydrator.Mobile
{
	public partial class HomePage : ContentPage
	{

		public HomePage()
		{
			InitializeComponent();

			for (int i = 0; i <= 24; i++) { this.RunTimeHoursPicker.Items.Add(i.ToString()); }
			for (int i = 0; i <= 60; i++) { this.RunTimeMinutesPicker.Items.Add(i.ToString()); }


		}

		void Temp_Tapped(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new TemperaturePage(), true);
		}

		void Runtime_Tapped(object sender, System.EventArgs e)
		{
			
		}
	}
}
