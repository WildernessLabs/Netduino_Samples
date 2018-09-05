using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.LEDs;
using Netduino.Foundation.Audio;

namespace Metronome
{
    public class App
    {
         int ANIMATION_DELAY = 200;

         Led[] leds = new Led[4];
         float[] notes = new float[] { 261.63f, 329.63f, 392, 523.25f };

         InterruptPort[] buttons = new InterruptPort[4];
         PiezoSpeaker speaker;

         float MAX_BPM = 240;
         float MIN_BPM = 60;
         float BPM = 140;
         float interval;
         int step = 0;
         DateTime lastTick = DateTime.Now;
         bool isPlaying = false;
         bool isThreadActive = false;

         bool isAnimating = false;

        public void Run()
        {
            Debug.Print("Welcome to Metronome");

            InitializePeripherals();

            ShowStartAnimation();
        }

        private void InitializePeripherals()
        {
            leds[0] = new Led(N.Pins.GPIO_PIN_D0);
            leds[1] = new Led(N.Pins.GPIO_PIN_D1);
            leds[2] = new Led(N.Pins.GPIO_PIN_D2);
            leds[3] = new Led(N.Pins.GPIO_PIN_D3);

            buttons[0] = new InterruptPort(N.Pins.GPIO_PIN_D10, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[1] = new InterruptPort(N.Pins.GPIO_PIN_D11, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[2] = new InterruptPort(N.Pins.GPIO_PIN_D12, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
            buttons[3] = new InterruptPort(N.Pins.GPIO_PIN_D13, true, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);

            buttons[0].OnInterrupt += OnButton0;
            buttons[1].OnInterrupt += OnButton1;
            buttons[2].OnInterrupt += OnButton2;
            buttons[3].OnInterrupt += OnButton3;

            speaker = new PiezoSpeaker(N.PWMChannels.PWM_PIN_D5);

            SetAllLEDs(false);
        }

        private void StartMetronome()
        {
            if (isThreadActive)
                return;

            isPlaying = true;

            interval = 60000f / BPM; //ms
            lastTick = DateTime.Now;

            var thread = new Thread(() =>
            {
                isThreadActive = true;
                while (isPlaying)
                {
                    lastTick = DateTime.Now;

                    leds[step].IsOn = true;
                    if (step == 3)
                        speaker.PlayTone(440, 50);
                    else
                        speaker.PlayTone(220, 50);
                    Thread.Sleep((int)interval);

                    if (step == 3)
                        SetAllLEDs(false);

                    step = (step + 1) % 4;
                }
                isThreadActive = false;
            });
            thread.Start();
        }

        private void StopMetronome()
        {
            isPlaying = false;
            Thread.Sleep(100);
            SetAllLEDs(false);
        }

        private void SetAllLEDs(bool isOn)
        {
            leds[0].IsOn = isOn;
            leds[1].IsOn = isOn;
            leds[2].IsOn = isOn;
            leds[3].IsOn = isOn;
        }

        private void ShowStartAnimation()
        {
            if (isAnimating)
                return;

            isAnimating = true;

            SetAllLEDs(false);

            for (int i = 0; i < 4; i++)
            {
                leds[i].IsOn = true;
                Thread.Sleep(ANIMATION_DELAY);
            }

            for (int i = 0; i < 4; i++)
            {
                leds[3 - i].IsOn = false;
                Thread.Sleep(ANIMATION_DELAY);
            }

            isAnimating = false;
        }

        private void OnButton0(uint data1, uint data2, DateTime time)
        {
            StartMetronome();
        }

        private void OnButton1(uint data1, uint data2, DateTime time)
        {
            StopMetronome();
        }

        private void OnButton2(uint data1, uint data2, DateTime time)
        {
            if (BPM == MAX_BPM)
                return;

            BPM++;
            interval = 60000f / BPM;

        }
        private void OnButton3(uint data1, uint data2, DateTime time)
        {
            if (BPM == MIN_BPM)
                return;

            BPM--;
            interval = 60000f / BPM;
        }
    }
}