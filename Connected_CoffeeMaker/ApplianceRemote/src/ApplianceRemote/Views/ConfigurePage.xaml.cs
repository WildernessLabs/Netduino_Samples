using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ApplianceRemote
{
    public partial class ConfigurePage : ContentPage
    {
        public ConfigurePage()
        {
            InitializeComponent();
            IPAddressEntry.Text = App.HostAddress;
        }

        void IPAddressEntry_TextChanged(object sender, EventArgs e)
        {
            App.HostAddress = ((TextChangedEventArgs)e).NewTextValue;
        }
    }
}
