using System;

namespace BW.Diagnostics.StatCollection
{
    class UIntByteConverter : IByteConverter<uint>
    {
        public byte[] ToByteArray(uint value) => BitConverter.GetBytes(value);
    }
}
