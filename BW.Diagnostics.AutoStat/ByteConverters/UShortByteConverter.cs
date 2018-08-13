using System;

namespace BW.Diagnostics.StatCollection
{
    class UShortByteConverter : IByteConverter<ushort>
    {
        public byte[] ToByteArray(ushort value) => BitConverter.GetBytes(value);
    }
}
