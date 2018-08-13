using System.Text;

namespace BW.Diagnostics.StatCollection
{
    class StringByteConverter : IByteConverter<string>
    {
        public byte[] ToByteArray(string value) => Encoding.UTF8.GetBytes(value);
    }
}
