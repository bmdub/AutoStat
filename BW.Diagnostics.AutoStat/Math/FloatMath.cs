using System;

namespace BW.Diagnostics.StatCollection
{
    class FloatMath : IMath<float>
    {
        public float FromDouble(double value) => (float)value;
        public double ToDouble(float value) => value;
    }
}
