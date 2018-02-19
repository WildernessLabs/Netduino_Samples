using System;
using Microsoft.SPOT;

namespace FoodDehydrator3000
{
    [Flags]
    public enum PIDActionType
    {
        Proportional = 1,
        Integral = 2,
        Derivative = 4
    }
}
