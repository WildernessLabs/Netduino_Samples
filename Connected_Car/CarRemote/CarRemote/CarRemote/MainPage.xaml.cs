using System;
using Xamarin.Forms;

namespace CarRemote
{
    public partial class MainPage : ContentPage
    {
        MainViewModel vm;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = vm = new MainViewModel();
        }

        private async void BtnUpPressed(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.MOVE_FORWARD);
        }

        private async void BtnUpReleased(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.STOP);
        }

        private async void BtnDownPressed(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.MOVE_BACKWARD);
        }

        private async void BtnDownReleased(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.STOP);
        }

        private async void BtnLeftPressed(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.TURN_LEFT);
        }

        private async void BtnLeftReleased(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.STOP);
        }

        private async void BtnRightPressed(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.TURN_RIGHT);
        }

        private async void BtnRightReleased(object sender, EventArgs e)
        {
            await vm.SendCommandAsync(CommandConstants.STOP);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            btnUp.Pressed += BtnUpPressed;
            btnUp.Released += BtnUpReleased;
            btnDown.Pressed += BtnDownPressed;
            btnDown.Released += BtnDownReleased;
            btnLeft.Pressed += BtnLeftPressed;
            btnLeft.Released += BtnLeftReleased;
            btnRight.Pressed += BtnRightPressed;
            btnRight.Released += BtnRightReleased;
        }

        protected override void OnDisappearing()
        {
            btnUp.Pressed -= BtnUpPressed;
            btnUp.Released -= BtnUpReleased;
            btnDown.Pressed -= BtnDownPressed;
            btnDown.Released -= BtnDownReleased;
            btnLeft.Pressed -= BtnLeftPressed;
            btnLeft.Released -= BtnLeftReleased;
            btnRight.Pressed -= BtnRightPressed;
            btnRight.Released -= BtnRightReleased;

            base.OnDisappearing();
        }
    }
}