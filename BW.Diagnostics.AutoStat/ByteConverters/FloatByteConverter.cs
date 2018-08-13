using System;

namespace BW.Diagnostics.StatCollection
{
    class FloatByteConverter : IByteConverter<float>
    {
        public byte[] ToByteArray(float value) => BitConverter.GetBytes(value);
    }
}
