using System;

namespace BW.Diagnostics.StatCollection
{
    class ShortMath : IMath<short>
    {
        public short FromDouble(double value) => (short)value;
        public double ToDouble(short value) => value;
    }
}
