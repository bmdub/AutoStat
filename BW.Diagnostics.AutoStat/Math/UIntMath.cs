using System;

namespace BW.Diagnostics.StatCollection
{
    class UIntMath : IMath<uint>
    {
        public uint FromDouble(double value) => (uint)value;
        public double ToDouble(uint value) => value;
    }
}
