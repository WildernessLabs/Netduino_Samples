using System;
using Microsoft.SPOT;

namespace NeonMika.Util
{
    /*
     * Thanks to Chris Walker and Gilberto Garcia for this code!
     * */
    public class Stopwatch
    {
        private long m_startTicks = 0;
        private long m_stopTicks = 0;
        private bool m_isRunning = false;

        private const long m_ticksPerMillisecond = System.TimeSpan.TicksPerMillisecond;

        public static Stopwatch StartNew()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        private Stopwatch()
        {
        }

        public void Reset()
        {
            m_startTicks = 0;
            m_stopTicks = 0;
            m_isRunning = false;
        }

        public void Start()
        {
            if (m_startTicks != 0 && m_stopTicks != 0)
                m_startTicks = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - (m_stopTicks - m_startTicks); // resume existing timer
            else
                m_startTicks = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks; // start new timer
            m_isRunning = true;
        }

        public void Stop()
        {
            m_stopTicks = Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks;
            m_isRunning = false;
        }

        public long ElapsedMilliseconds
        {
            get
            {
                if (m_startTicks != 0 && m_isRunning)
                    return (Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - m_startTicks) / m_ticksPerMillisecond;
                else if (m_startTicks != 0 && !m_isRunning)
                    return (m_stopTicks - m_startTicks) / m_ticksPerMillisecond;
                else
                    throw new InvalidOperationException();
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                if (m_startTicks != 0 && m_isRunning)
                {
                    TimeSpan duration = new TimeSpan((Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - m_startTicks));
                    return duration.Seconds;
                }
                else if (m_startTicks != 0 && !m_isRunning)
                {
                    TimeSpan duration = new TimeSpan((m_stopTicks - m_startTicks));
                    return duration.Seconds;
                }
                else
                    throw new InvalidOperationException();
            }
        }

        public double ElapsedMinutes
        {
            get
            {
                if (m_startTicks != 0 && m_isRunning)
                {
                    TimeSpan duration = new TimeSpan((Microsoft.SPOT.Hardware.Utility.GetMachineTime().Ticks - m_startTicks));
                    return duration.Minutes;
                }
                else if (m_startTicks != 0 && !m_isRunning)
                {
                    TimeSpan duration = new TimeSpan((m_stopTicks - m_startTicks));
                    return duration.Minutes;
                }
                else
                    throw new InvalidOperationException();
            }
        }
    }
}