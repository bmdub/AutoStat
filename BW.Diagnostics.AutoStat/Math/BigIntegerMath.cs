using System;
using System.Numerics;

namespace BW.Diagnostics.StatCollection
{
    class BigIntegerMath : IMath<BigInteger>
    {
        public BigInteger FromDouble(double value) => (BigInteger)value;
        public double ToDouble(BigInteger value) => (double)value;
    }
}
