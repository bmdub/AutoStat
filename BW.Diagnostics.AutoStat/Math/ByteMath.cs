using System;

namespace BW.Diagnostics.StatCollection
{
    class ByteMath : IMath<byte>
    {
        public byte FromDouble(double value) => (byte)value;
        public double ToDouble(byte value) => value;
    }
}
