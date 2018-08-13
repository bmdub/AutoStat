using System;

namespace BW.Diagnostics.StatCollection
{
    class IntByteConverter : IByteConverter<int>
    {
        public byte[] ToByteArray(int value) => BitConverter.GetBytes(value);
    }
}
