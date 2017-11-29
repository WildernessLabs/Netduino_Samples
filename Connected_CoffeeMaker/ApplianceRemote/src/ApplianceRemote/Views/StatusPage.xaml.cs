using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace ApplianceRemote
{
    public partial class StatusPage : ContentPage
    {
        ApiHelper apiHelper;

        public StatusPage()
        {
            InitializeComponent();
            btnAction.CommandParameter = Commands.Configure;
            this.Title = "Connected Appliance";
        }

        async protected override void OnAppearing()
        {
            if (string.IsNullOrEmpty(App.HostAddress))
            {
                lblStatus.Text = "Not Connected";
            }
            else
            {
                apiHelper = new ApiHelper(App.HostAddress);

                lblStatus.Text = "Connecting to " + App.HostAddress;

                App.IsConnected = await apiHelper.Connect();
                if(App.IsConnected)
                {
                    if (await apiHelper.CheckStatus())
                    {
                        App.ApplianceStatus = ApplianceStatus.On;
                    }
                    else
                    {
                        App.ApplianceStatus = ApplianceStatus.Off;
                    }    
                }
                UpdateStatus();
            }
        }

        async void Configure_Clicked(object sender, EventArgs e)
        {
            if(sender is Button)
            {
                var s = (Button)sender;
                if (s.CommandParameter.ToString() == Commands.Configure.ToString())
                {
                    await Navigation.PushAsync(new ConfigurePage());
                }
                else if (s.CommandParameter.ToString() == Commands.TurnOn.ToString())
                {
                    if (await apiHelper.TurnOn())
                    {
                        UpdateStatus();
                    }
                }
                else if (s.CommandParameter.ToString()== Commands.TurnOff.ToString())
                {
                    if (await apiHelper.TurnOff())
                    {
                        UpdateStatus();
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                await Navigation.PushAsync(new ConfigurePage());
            }

        }

        void UpdateStatus()
        {
            if (!App.IsConnected)
            {
                lblStatus.Text = "Not Connected";
                btnAction.CommandParameter = Commands.Configure;
                btnAction.Image = "ConfigureAppliance.png";
            }
            else if (App.ApplianceStatus == ApplianceStatus.On)
            {
                lblStatus.Text = "On";
                btnAction.CommandParameter = Commands.TurnOff;
                btnAction.Image = "Stop.png";
            }
            else
            {
                lblStatus.Text = "Off";
                btnAction.CommandParameter = Commands.TurnOn;
                btnAction.Image = "Start.png";
            }
        }
    }

    public enum Commands{
        Configure,
        TurnOn,
        TurnOff
    }
}
