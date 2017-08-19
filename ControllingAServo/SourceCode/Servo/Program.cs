using System.Threading;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using ArduinoLib;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;

namespace ServoTest
{
	public class Program
	{
		public static void Main()
		{
            Servo servo = new Servo(PWMChannels.PWM_PIN_D9, 500, 2400);
            while (true)
            {
                try
                {
                    for (int angle = 0; angle <= 180; angle++)
                    {
                        servo.Angle = angle;
                        Thread.Sleep(40);
                    }
                    for (int angle = 179; angle > 0; angle--)
                    {
                        servo.Angle = angle;
                        Thread.Sleep(40);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
 		}
	}
}
