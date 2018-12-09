using Microsoft.SPOT;

namespace CarHost
{
    public class CarEventArgs : EventArgs
    {
        public int Speed { get; set; }

        public CarEventArgs(int speed)
        {
            Speed = speed;
        }
    }
}