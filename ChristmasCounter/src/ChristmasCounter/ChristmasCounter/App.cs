using H = Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Displays.LCD;
using System.Threading;
using Netduino.Foundation.RTCs;
using System;

namespace ChristmasCounter
{
    public class App
    {
        protected DS3231 _rtc;
        protected Lcd2004 _lcd;

        public App()
        {

            InitializePeripherals();
        }

        protected void InitializePeripherals()
        {
            _rtc = new DS3231(0x68, 100);

            _lcd = new Lcd2004
            (
                RS: N.Pins.GPIO_PIN_D8,
                E: N.Pins.GPIO_PIN_D9,
                D4: N.Pins.GPIO_PIN_D10,
                D5: N.Pins.GPIO_PIN_D11,
                D6: N.Pins.GPIO_PIN_D12,
                D7: N.Pins.GPIO_PIN_D13
            );

            H.PWM _contrast = new H.PWM(H.Cpu.PWMChannel.PWM_0, 1000, 0.6, false);
            _contrast.Start();

        }

        public void Run()
        {
            _lcd.WriteLine("Current Date:", 0);
            _lcd.WriteLine(_rtc.CurrentDateTime.Month + "/" + _rtc.CurrentDateTime.Day + "/" + "2018", 1);
            _lcd.WriteLine("Christmas Countdown:", 2);
            _lcd.WriteLine("14 Days to go!", 3);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}