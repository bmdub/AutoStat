using System.Numerics;

namespace BW.Diagnostics.StatCollection
{
    class BigIntegerByteConverter : IByteConverter<BigInteger>
    {
        public byte[] ToByteArray(BigInteger value) => value.ToByteArray();
    }
}
