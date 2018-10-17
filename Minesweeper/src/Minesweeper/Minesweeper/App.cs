using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Netduino.Foundation.Displays;

namespace Minesweeper
{
    public class App
    {
        protected int currentColumn;

        protected SSD1306 display;
        protected GraphicsLibrary graphics;

        protected InputPort[] rowPorts = new InputPort[4];
        protected OutputPort[] columnPorts = new OutputPort[4];

        protected bool[] mines;

        public App()
        {
            mines = new bool[16];

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

        void RenderMines()
        {
            int buttonIndex = 0;

            graphics.Clear(true);
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    graphics.DrawRectangle((column * 13) + 1, (row * 17) + 1, 12, 14, true, false);

                    buttonIndex++;
                    graphics.CurrentFont = new Font8x8();
                    graphics.DrawText(108, (buttonIndex < 9)? 5 : 22, buttonIndex.ToString());

                    graphics.Show();
                    Thread.Sleep(100);
                }
            }

            Thread.Sleep(1000);
        }

        void RefreshMines()
        {
            int buttonIndex = 0;
            //graphics.Clear(true);
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    graphics.DrawRectangle((column * 13) + 1, (row * 17) + 1, 12, 14, true, mines[buttonIndex]);
                    buttonIndex++;
                }
            }

            graphics.CurrentFont = new Font8x8();
            graphics.DrawText(108, 5, "8");
            graphics.DrawText(108, 22, "16");
            graphics.Show();
            Thread.Sleep(1000);
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

                    if (currentColumn == 3)
                        currentColumn = 0;
                    else
                        currentColumn++;

                    if(currentButton != lastButton)
                    { 
                        //graphics.Clear(true);
                        //graphics.CurrentFont = new Font8x8();
                        if (currentButton != -1)
                        {
                            mines[currentButton - 1] = true;
                            //    graphics.DrawText(0, 10, "Button = " + currentButton);
                            //    graphics.Show();
                            //    Thread.Sleep(1000);
                        }
                        else
                        {
                            RefreshMines();
                        }                        
                    }

                    lastButton = currentButton;
                }
            });
            thread.Start();
        }

        public void Run()
        {
            RenderMines();
            CyclingColumnVDD();
        }
    }
}
