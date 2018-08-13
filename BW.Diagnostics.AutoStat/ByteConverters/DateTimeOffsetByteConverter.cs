using System;

namespace BW.Diagnostics.StatCollection
{
    class DateTimeOffsetByteConverter : IByteConverter<DateTimeOffset>
    {
        public byte[] ToByteArray(DateTimeOffset value) => BitConverter.GetBytes(value.Ticks);
    }
}
