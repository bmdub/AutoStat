using System;

namespace BW.Diagnostics.StatCollection
{
    class IntMath : IMath<int>
    {
        public int FromDouble(double value) => (int)value;
        public double ToDouble(int value) => value;
    }
}
