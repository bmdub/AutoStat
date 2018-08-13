using System;

namespace BW.Diagnostics.StatCollection
{
    class LongByteConverter : IByteConverter<long>
    {
        public byte[] ToByteArray(long value) => BitConverter.GetBytes(value);
    }
}
