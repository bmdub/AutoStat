namespace BW.Diagnostics.StatCollection
{
    class ByteByteConverter : IByteConverter<byte>
    {
        public byte[] ToByteArray(byte value) => new byte[] { value };
    }
}
