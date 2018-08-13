using System;

namespace BW.Diagnostics.StatCollection
{
    class TimeSpanByteConverter : IByteConverter<TimeSpan>
    {
        public byte[] ToByteArray(TimeSpan value) => BitConverter.GetBytes(value.Ticks);
    }
}
