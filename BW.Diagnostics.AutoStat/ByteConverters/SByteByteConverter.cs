namespace BW.Diagnostics.StatCollection
{
    class SByteByteConverter : IByteConverter<sbyte>
    {
        public byte[] ToByteArray(sbyte value) => new byte[] { (byte)value };
    }
}
