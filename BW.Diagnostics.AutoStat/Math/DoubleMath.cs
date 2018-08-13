using System;

namespace BW.Diagnostics.StatCollection
{
    class DoubleMath : IMath<double>
    {
        public double FromDouble(double value) => value;
        public double ToDouble(double value) => value;
    }
}
