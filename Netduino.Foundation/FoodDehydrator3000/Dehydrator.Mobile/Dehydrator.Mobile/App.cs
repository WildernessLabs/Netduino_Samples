using System;
using System.Diagnostics;

namespace Dehydrator.Mobile
{
	/// <summary>
	/// Application (singleton)
	/// </summary>
	public class App
	{
		private bool _classDebug = true;

		/// <summary>
		/// Gets or sets the application instance.
		/// </summary>
		/// <value>The current instance.</value>
		public static App Current { get; set; }

		public AppConfig Config { get { return this._config; } }
		protected AppConfig _config;

		static App()
		{
			Current = new App();
		}

		protected App()
		{
			_config = new AppConfig();

			Debug.WriteLineIf(_classDebug, "App.ctor;");

		}

	}
}
