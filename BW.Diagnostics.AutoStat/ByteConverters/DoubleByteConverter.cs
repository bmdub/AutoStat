using System;

namespace BW.Diagnostics.StatCollection
{
    class DoubleByteConverter : IByteConverter<double>
    {
        public byte[] ToByteArray(double value) => BitConverter.GetBytes(value);
    }
}
