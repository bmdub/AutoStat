using System;

namespace BW.Diagnostics.StatCollection
{
    class UShortMath : IMath<ushort>
    {
        public ushort FromDouble(double value) => (ushort)value;
        public double ToDouble(ushort value) => value;
    }
}
