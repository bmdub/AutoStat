using System;

namespace BW.Diagnostics.StatCollection
{
    class ShortByteConverter : IByteConverter<short>
    {
        public byte[] ToByteArray(short value) => BitConverter.GetBytes(value);
    }
}
