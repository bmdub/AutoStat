using System;

namespace BW.Diagnostics.StatCollection
{
    class SByteMath : IMath<sbyte>
    {
        public sbyte FromDouble(double value) => (sbyte)value;
        public double ToDouble(sbyte value) => value;
    }
}
