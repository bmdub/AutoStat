using System;

namespace BW.Diagnostics.StatCollection
{
    class DateTimeOffsetMath : IMath<DateTimeOffset>
    {
        public DateTimeOffset FromDouble(double value) => new DateTimeOffset(DateTime.FromBinary(BoundTicks((long)value)));
        public double ToDouble(DateTimeOffset value) => value.Ticks;

        static long BoundTicks(long dateData)
        {
            if (dateData > DateTime.MaxValue.Ticks) return DateTime.MaxValue.Ticks;
            else if (dateData < DateTime.MinValue.Ticks) return DateTime.MinValue.Ticks;
            else return dateData;
        }
    }
}
