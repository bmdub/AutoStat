using System;

namespace BW.Diagnostics.StatCollection
{
    class DecimalByteConverter : IByteConverter<decimal>
    {
        public byte[] ToByteArray(decimal value)
        {
            var ints = decimal.GetBits(value);
            byte[] result = new byte[ints.Length * sizeof(int)];
            Buffer.BlockCopy(ints, 0, result, 0, result.Length);
            return result;
        }
    }
}
