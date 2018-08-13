using System;

namespace BW.Diagnostics.StatCollection
{
    class BoolByteConverter : IByteConverter<bool>
    {
        public byte[] ToByteArray(bool value) => BitConverter.GetBytes(value);
    }
}
