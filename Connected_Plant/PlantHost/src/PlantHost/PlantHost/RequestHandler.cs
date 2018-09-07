﻿using System.Collections;
using System.Threading;
using Maple;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;
using SLH = SecretLabs.NETMF.Hardware;
using System;

namespace PlantHost
{
    public class RequestHandler : RequestHandlerBase
    {
        protected float humidity;
        //public event EventHandler GetPlantHumidity = delegate { };

        public void getPlantHumidity()
        {
            var a0 = new SLH.AnalogInput(Pins.GPIO_PIN_A0);
            var d7 = new Microsoft.SPOT.Hardware.OutputPort(Pins.GPIO_PIN_D7, false);

            int sample = 0;
            for (int i = 0; i < 3; i++)
            {
                d7.Write(true);
                Thread.Sleep(5);
                sample += a0.Read();
                d7.Write(false);

                Thread.Sleep(500);
            }

            humidity = 100 - Map((sample / 3), 250, 1023, 0, 100);
            Debug.Print("humidity =" + humidity);
            StatusResponse();
        }

        float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            return (((toHigh - toLow) * (value - fromLow)) / (fromHigh - fromLow)) - toLow;
        }

        protected void StatusResponse()
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = 200;
            Hashtable result = new Hashtable
            {
                { "humidity", humidity > 100? 100 : (int)humidity },
            };
            Send(result);
        }
    }
}