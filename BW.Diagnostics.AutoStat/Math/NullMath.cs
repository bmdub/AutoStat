namespace BW.Diagnostics.StatCollection
{
    class NullMath<T> : IMath<T>
    {
        public T FromDouble(double value) => default;
        public double ToDouble(T value) => 0.0;
    }
}
