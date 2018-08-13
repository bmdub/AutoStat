namespace BW.Diagnostics.StatCollection
{
    interface IMath<T>
    {
        //T Increment(T value);
        //T Decrement(T value);
        //T Add(T value1, T value2);
        //T Subtract(T value1, T value2);
        //T Multiply(T value1, T value2);
        //T Divide(T value1, T value2);
        //T Square(T value);
        //T SquareRoot(T value);

        T FromDouble(double value);
        double ToDouble(T value);
    }
}
