using System;

namespace BW.Diagnostics.StatCollection
{
    class DefaultByteConverter<T> : IByteConverter<T>
    {
        public byte[] ToByteArray(T value) => BitConverter.GetBytes(value.GetHashCode());
    }
}
