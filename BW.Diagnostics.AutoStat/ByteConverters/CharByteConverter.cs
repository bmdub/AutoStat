using System;

namespace BW.Diagnostics.StatCollection
{
    class CharByteConverter : IByteConverter<char>
    {
        public byte[] ToByteArray(char value) => BitConverter.GetBytes(value);
    }
}
