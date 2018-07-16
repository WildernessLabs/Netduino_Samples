using Microsoft.SPOT;

namespace ServoHost
{
    public class ServoEventArgs : EventArgs
    {
        public int Angle { get; set; }

        public ServoEventArgs(int angle)
        {
            Angle = angle;
        }
    }
}
