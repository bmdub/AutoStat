using System;

namespace BW.Diagnostics.StatCollection
{
    class DateTimeMath : IMath<DateTime>
    {
        public DateTime FromDouble(double value) => DateTime.FromBinary(BoundTicks((long)value));
        public double ToDouble(DateTime value) => value.Ticks;

        static long BoundTicks(long dateData)
        {
            if (dateData > DateTime.MaxValue.Ticks) return DateTime.MaxValue.Ticks;
            else if (dateData < DateTime.MinValue.Ticks) return DateTime.MinValue.Ticks;
            else return dateData;
        }
    }
}
