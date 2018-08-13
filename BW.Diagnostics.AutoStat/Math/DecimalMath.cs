using System;

namespace BW.Diagnostics.StatCollection
{
    class DecimalMath : IMath<Decimal>
    {
        public Decimal FromDouble(double value) => (Decimal)value;
        public double ToDouble(Decimal value) => (double)value;
    }
}
