using System;

namespace BW.Diagnostics.StatCollection
{
    class TimeSpanMath : IMath<TimeSpan>
    {
        public TimeSpan FromDouble(double value) => TimeSpan.FromTicks(BoundTicks((long)value));
        public double ToDouble(TimeSpan value) => value.Ticks;

        static long BoundTicks(long dateData)
        {
            if (dateData > TimeSpan.MaxValue.Ticks) return TimeSpan.MaxValue.Ticks;
            else if (dateData < TimeSpan.MinValue.Ticks) return TimeSpan.MinValue.Ticks;
            else return dateData;
        }
    }
}
