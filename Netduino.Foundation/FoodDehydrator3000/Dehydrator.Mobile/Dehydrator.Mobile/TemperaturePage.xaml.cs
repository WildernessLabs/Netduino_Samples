using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Dehydrator.Mobile
{
	public partial class TemperaturePage : ContentPage
	{



		public TemperaturePage()
		{
			InitializeComponent();

			TempList.ItemsSource = App.Current.Config.AvailableTemps;

		}
	}
}
