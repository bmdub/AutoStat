using System;

namespace BW.Diagnostics.StatCollection
{
    class DateTimeByteConverter : IByteConverter<DateTime>
    {
        public byte[] ToByteArray(DateTime value) => BitConverter.GetBytes(value.Ticks);
    }
}
