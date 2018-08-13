using System;

namespace BW.Diagnostics.StatCollection
{
    class ULongMath : IMath<ulong>
    {
        public ulong FromDouble(double value) => (ulong)value;
        public double ToDouble(ulong value) => value;
    }
}
