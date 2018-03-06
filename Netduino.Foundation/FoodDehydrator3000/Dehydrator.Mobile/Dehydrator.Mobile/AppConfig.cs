using System;
using System.Collections.Generic;
using Dehydrator.Mobile.BL;

namespace Dehydrator.Mobile
{
	public class AppConfig
	{
		public List<Temperature> AvailableTemps { get; set; } = new List<Temperature>();

		public AppConfig()
		{
			this.AvailableTemps.Add(new Temperature { Name="155º F (68º C)", Description="Jerky", Value=155 });
			this.AvailableTemps.Add(new Temperature { Name = "145º F (63º C)", Description = "Meat & Fish", Value = 145 });
			this.AvailableTemps.Add(new Temperature { Name = "135º F (57º C)", Description = "Fruits", Value = 135 });
			this.AvailableTemps.Add(new Temperature { Name = "125º F (52º C)", Description = "Vegetables & Herbs", Value = 125 });
			this.AvailableTemps.Add(new Temperature { Name = "115º F (46º C)", Description = "Herbs", Value = 115 });
			this.AvailableTemps.Add(new Temperature { Name = "105º F (41º C)", Description = "Herbs", Value = 105 });
			this.AvailableTemps.Add(new Temperature { Name = "95º F (35º C)", Description = "Herbs", Value = 95 });
		}
	}
}
