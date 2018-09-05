using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Audio;

namespace Piano
{
    public class App
    {
         Led[] leds = new Led[4];
         float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f };

         InterruptPort[] buttons = new InterruptPort[4];
         PiezoSpeaker speaker;

        public void Run ()
        {
            Debug.Print("Welcome to Piano");

            InitializePeripherals();
        }

        private void InitializePeripherals()
        {
            leds[0] = new Led(N.Pins.GPIO_PIN_D0);
            leds[1] = new Led(N.Pins.GPIO_PIN_D1);
            leds[2] = new Led(N.Pins.GPIO_PIN_D2);
            leds[3] = new Led(N.Pins.GPIO_PIN_D3);

            buttons[0] = new InterruptPort(N.Pins.GPIO_PIN_D10, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[1] = new InterruptPort(N.Pins.GPIO_PIN_D11, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[2] = new InterruptPort(N.Pins.GPIO_PIN_D12, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);
            buttons[3] = new InterruptPort(N.Pins.GPIO_PIN_D13, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeBoth);

            buttons[0].OnInterrupt += OnButton0;
            buttons[1].OnInterrupt += OnButton1;
            buttons[2].OnInterrupt += OnButton2;
            buttons[3].OnInterrupt += OnButton3;

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D5);

            SetAllLEDs(false);
        }

        private void SetAllLEDs(bool isOn)
        {
            leds[0].IsOn = isOn;
            leds[1].IsOn = isOn;
            leds[2].IsOn = isOn;
            leds[3].IsOn = isOn;
        }

        private void ToggleNote(int index, bool play)
        {
            if (play)
            {
                speaker.PlayTone(notes[index]);
                leds[index].IsOn = true;
            }
            else
            {
                speaker.StopTone();
                leds[index].IsOn = false;
            }
        }

        private void OnButton0(uint data1, uint data2, DateTime time)
        {
            ToggleNote(0, data2 == 1);
        }

        private void OnButton1(uint data1, uint data2, DateTime time)
        {
            ToggleNote(1, data2 == 1);
        }

        private void OnButton2(uint data1, uint data2, DateTime time)
        {
            ToggleNote(2, data2 == 1);
        }
        private void OnButton3(uint data1, uint data2, DateTime time)
        {
            ToggleNote(3, data2 == 1);
        }
    }
}
