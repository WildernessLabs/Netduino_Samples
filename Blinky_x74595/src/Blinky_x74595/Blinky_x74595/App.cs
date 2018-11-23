using System.Threading;
using Netduino.Foundation.ICs.IOExpanders.x74595;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Blinky_x74595
{
    public class App
    {
        x74595 shiftRegister;

        public App()
        {
            InitializePeripherals();
        }

        protected void InitializePeripherals()
        {
            var config = new SPI.Configuration(SPI_mod: SPI_Devices.SPI1,
                                              ChipSelect_Port: Pins.GPIO_PIN_D8,
                                              ChipSelect_ActiveState: false,
                                              ChipSelect_SetupTime: 0,
                                              ChipSelect_HoldTime: 0,
                                              Clock_IdleState: true,
                                              Clock_Edge: true,
                                              Clock_RateKHz: 10);

            shiftRegister = new x74595(8, config);
        }

        public void Run()
        {
            shiftRegister.Clear(true);

            int index = 0;
            while (true)
            {
                shiftRegister[index] = true;
                Thread.Sleep(500);
                shiftRegister[index] = false;

                index = index == 8 ? 0 : index + 1;
            }
        }
    }
}