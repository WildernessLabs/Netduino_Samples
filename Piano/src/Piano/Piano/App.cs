using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using N = SecretLabs.NETMF.Hardware.Netduino;
using Netduino.Foundation.Audio;
using System.Threading;

namespace Piano
{
    public class App
    {
        InputPort[] pianoKeys = new InputPort[8];
        float[] notes = new float[] { 261.6f, 293.7f, 329.6f, 349.2f, 392.0f, 440.0f, 493.9f, 523.3f };
                                    //C4    , D4    , E4    , F4    , G4    , A4    , B4    , C5

        PiezoSpeaker[] speakers = new PiezoSpeaker[3];
        bool[] isSpeakerPlaying = new bool[3];

        public App()
        {
            InitializePeripherals();
        }

        private void InitializePeripherals()
        {
            pianoKeys[0] = new InputPort(N.Pins.GPIO_PIN_D7, true, Port.ResistorMode.PullUp);
            pianoKeys[1] = new InputPort(N.Pins.GPIO_PIN_D6, true, Port.ResistorMode.PullUp);
            pianoKeys[2] = new InputPort(N.Pins.GPIO_PIN_D5, true, Port.ResistorMode.PullUp);
            pianoKeys[3] = new InputPort(N.Pins.GPIO_PIN_D4, true, Port.ResistorMode.PullUp);
            pianoKeys[4] = new InputPort(N.Pins.GPIO_PIN_D3, true, Port.ResistorMode.PullUp);
            pianoKeys[5] = new InputPort(N.Pins.GPIO_PIN_D2, true, Port.ResistorMode.PullUp);
            pianoKeys[6] = new InputPort(N.Pins.GPIO_PIN_D1, true, Port.ResistorMode.PullUp);
            pianoKeys[7] = new InputPort(N.Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullUp);

            speakers[0] = new PiezoSpeaker(Cpu.PWMChannel.PWM_2);
            speakers[1] = new PiezoSpeaker(Cpu.PWMChannel.PWM_3);
            speakers[2] = new PiezoSpeaker(Cpu.PWMChannel.PWM_5);

            isSpeakerPlaying[0] = isSpeakerPlaying[1] = isSpeakerPlaying[2] = false;
        }

        protected void Cycle()
        {
            Thread thread = new Thread(() =>
            {
                bool[] lastState = new bool[8];
                int speakersPlaying = 0;

                while (true)
                {
                    Thread.Sleep(50);

                    bool[] currentState = new bool[8];
                    currentState[0] = pianoKeys[0].Read();
                    currentState[1] = pianoKeys[1].Read();
                    currentState[2] = pianoKeys[2].Read();
                    currentState[3] = pianoKeys[3].Read();
                    currentState[4] = pianoKeys[4].Read();
                    currentState[5] = pianoKeys[5].Read();
                    currentState[6] = pianoKeys[6].Read();
                    currentState[7] = pianoKeys[7].Read();

                    for (int i = 0; i < 8; i++)
                    {
                        if (lastState[i] != currentState[i])
                        {
                            if (!currentState[i] && speakersPlaying < 3)
                            {
                                speakers[speakersPlaying].PlayTone(notes[i]);
                                speakersPlaying++;
                            }
                            else
                            {
                                if (speakersPlaying > 0)
                                {
                                    speakers[speakersPlaying - 1].StopTone();
                                    speakersPlaying--;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 8; i++)
                        lastState[i] = currentState[i];
                }
            });
            thread.Start();
        }

        public void Run()
        {
            Debug.Print("Welcome to Piano");

            Cycle();
        }
    }
}