using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void BtnUpPressed(object sender, EventArgs e)
        {
            vm.IsButtonUpPressed = true;
        }

        private void BtnUpReleased(object sender, EventArgs e)
        {
            vm.IsButtonUpPressed = false;
        }

        private void BtnDownPressed(object sender, EventArgs e)
        {
            vm.IsButtonDownPressed = true;
        }

        private void BtnDownReleased(object sender, EventArgs e)
        {
            vm.IsButtonDownPressed = false;
        }

        private void BtnLeftPressed(object sender, EventArgs e)
        {
            vm.IsButtonLeftPressed = true;
        }

        private void BtnLeftReleased(object sender, EventArgs e)
        {
            vm.IsButtonLeftPressed = false;
        }

        private void BtnRightPressed(object sender, EventArgs e)
        {
            vm.IsButtonRightPressed = true;
        }

        private void BtnRightReleased(object sender, EventArgs e)
        {
            vm.IsButtonRightPressed = false;
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