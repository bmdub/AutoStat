using System;

namespace BW.Diagnostics.StatCollection
{
    class ULongByteConverter : IByteConverter<ulong>
    {
        public byte[] ToByteArray(ulong value) => BitConverter.GetBytes(value);
    }
}
