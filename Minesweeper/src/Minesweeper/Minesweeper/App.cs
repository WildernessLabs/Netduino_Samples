using Microsoft.SPOT.Hardware;
using System.Threading;
using Netduino.Foundation.Displays;
using System;
using Microsoft.SPOT;

namespace Minesweeper
{
    public class App
    {
        protected SSD1306 display;
        protected GraphicsLibrary graphics;

        protected int currentColumn;
        protected InputPort[] rowPorts = new InputPort[4];
        protected OutputPort[] columnPorts = new OutputPort[4];

        protected char[] options;
        protected bool[] optionsSolved;
        protected char[] optionsPossible;
        protected int option1, option2;

        public App()
        {
            options = new char[16];
            optionsSolved = new bool[16];
            optionsPossible = new char[8] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            option1 = option2 = -1;
            InitializePeripherals();
        }

        protected void InitializePeripherals()
        {
            display = new SSD1306(0x3c, 400, SSD1306.DisplayType.OLED128x32);
            graphics = new GraphicsLibrary(display);

            rowPorts[0] = new InputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullDown);
            rowPorts[1] = new InputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D1, true, Port.ResistorMode.PullDown);
            rowPorts[2] = new InputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D2, true, Port.ResistorMode.PullDown);
            rowPorts[3] = new InputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D3, true, Port.ResistorMode.PullDown);

            columnPorts[0] = new OutputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D4, false);
            columnPorts[1] = new OutputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D5, false);
            columnPorts[2] = new OutputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D6, false);
            columnPorts[3] = new OutputPort(SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D7, false);

            currentColumn = 0;
        }

        void LoadMemoryBoard()
        {
            for (int i = 0; i < 16; i++)
                options[i] = ' ';

            var r = new Random();
            for (int i = 0; i < 8; i++)
            {
                bool isPlaced = false;
                while(!isPlaced)
                {
                    int index = r.Next(16);
                    if(options[index] == ' ')
                    {
                        options[index] = optionsPossible[i];
                        isPlaced = true;
                    }
                }

                isPlaced = false;
                while (!isPlaced)
                {
                    int index = r.Next(16);
                    if (options[index] == ' ')
                    {
                        options[index] = optionsPossible[i];
                        isPlaced = true;
                    }
                }
            }

            for (int i = 0; i < 16; i++)
                Debug.Print((i+1).ToString() + " " + options[i].ToString() + " ");
        }

        void StartGameAnimation()
        {
            DisplayText("MEMORY GAME", 20, 12);
            Thread.Sleep(1000);
            DisplayText("Ready?", 40, 12);
            Thread.Sleep(1000);
            DisplayText("Start!", 40, 12);
            Thread.Sleep(1000);
            DisplayText("Select Button");
        }

        void CyclingColumnVDD()
        {
            Thread thread = new Thread(()=> 
            {
                int lastButton = -1;

                while (true)
                {
                    Thread.Sleep(50);

                    int currentButton = -1;
                    switch (currentColumn)
                    {
                        case 0:
                            columnPorts[0].Write(true);
                            columnPorts[1].Write(false);
                            columnPorts[2].Write(false);
                            columnPorts[3].Write(false);

                            if (rowPorts[0].Read()) currentButton = 13;
                            if (rowPorts[1].Read()) currentButton = 9;
                            if (rowPorts[2].Read()) currentButton = 5;
                            if (rowPorts[3].Read()) currentButton = 1;
                            break;

                        case 1:
                            columnPorts[0].Write(false);
                            columnPorts[1].Write(true);
                            columnPorts[2].Write(false);
                            columnPorts[3].Write(false);

                            if (rowPorts[0].Read()) currentButton = 14;
                            if (rowPorts[1].Read()) currentButton = 10;
                            if (rowPorts[2].Read()) currentButton = 6;
                            if (rowPorts[3].Read()) currentButton = 2;
                            break;

                        case 2:
                            columnPorts[0].Write(false);
                            columnPorts[1].Write(false);
                            columnPorts[2].Write(true);
                            columnPorts[3].Write(false);

                            if (rowPorts[0].Read()) currentButton = 15;
                            if (rowPorts[1].Read()) currentButton = 11;
                            if (rowPorts[2].Read()) currentButton = 7;
                            if (rowPorts[3].Read()) currentButton = 3;
                            break;

                        case 3:
                            columnPorts[0].Write(false);
                            columnPorts[1].Write(false);
                            columnPorts[2].Write(false);
                            columnPorts[3].Write(true);

                            if (rowPorts[0].Read()) currentButton = 16;
                            if (rowPorts[1].Read()) currentButton = 12;
                            if (rowPorts[2].Read()) currentButton = 8;
                            if (rowPorts[3].Read()) currentButton = 4;
                            break;
                    }

                    currentColumn = (currentColumn == 3)? 0 : currentColumn + 1;

                    if(currentButton != lastButton)
                    {
                        if (currentButton != -1)
                        {
                            if (optionsSolved[currentButton - 1])
                            {
                                DisplayText("OPTION SOLVED", 12, 12);
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                if (option1 == -1)
                                    option1 = currentButton - 1;
                                else
                                    option2 = currentButton - 1;

                                DisplayText("Button = " + options[currentButton - 1], 24, 12);
                                Thread.Sleep(1000);

                                if (option2 != -1 && option1 != option2)
                                {
                                    if (options[option1] == options[option2])
                                    {
                                        DisplayText(options[option1] + " == " + options[option2], 40, 12);
                                        optionsSolved[option1] = optionsSolved[option2] = true;
                                    }
                                    else
                                        DisplayText(options[option1] + " != " + options[option2], 40, 12);

                                    Thread.Sleep(1000);
                                    option1 = option2 = -1;
                                }
                            }
                        }
                        else
                        {
                            DisplayText("Select Button");
                        }
                    }

                    lastButton = currentButton;
                }
            });
            thread.Start();
        }

        protected void DisplayText(string text, int x = 12, int y = 12)
        {
            graphics.Clear(true);
            graphics.CurrentFont = new Font8x12();
            graphics.DrawRectangle(0, 0, 128, 32);
            graphics.DrawText(x, y, text);
            graphics.Show();
        }

        public void Run()
        {
            LoadMemoryBoard();
            StartGameAnimation();
            CyclingColumnVDD();
        }
    }
}