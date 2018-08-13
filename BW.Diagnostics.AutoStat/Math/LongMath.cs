using System;

namespace BW.Diagnostics.StatCollection
{
    class LongMath : IMath<long>
    {
        public long FromDouble(double value) => (long)value;
        public double ToDouble(long value) => value;
    }
}
