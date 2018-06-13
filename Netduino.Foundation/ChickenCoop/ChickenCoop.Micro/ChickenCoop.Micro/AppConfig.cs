using System;
using Microsoft.SPOT;

namespace ChickenCoop.Micro
{
    public class AppConfig
    {
        /// <summary>
        /// The minimum temp to keep the coop at when the door is open.
        /// </summary>
        public float DaytimeTemp { get; set; } = 50.0f;
        /// <summary>
        /// The minimum temp to keep the coop at when the door is closed
        /// </summary>
        public float NightimeTemp { get; set; } = 60.0f;
        /// <summary>
        /// Whether or not to automatically open the door at
        /// sunrise and close at sunset.
        /// </summary>
        public bool AutoDoorIsOn { get; set; } = true;

        public float UtcOffset { get; set; } = -7;

        public AppConfig()
        {
            
        }
    }
}
