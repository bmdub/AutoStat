using Murmur;
using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    class Hasher<T>
    {
        IByteConverter<T> _byteConverter;
        Murmur128 _murmurHash;

        public Hasher()
        {
            _byteConverter = ByteConverterHelpers.GetByteConverter<T>();
            _murmurHash = MurmurHash.Create128(managed: true, preference: AlgorithmPreference.X64);
        }

        public ulong GetHash(T value)
        {
            ulong hash = 1;
            if (value != null)
            {
                var bytes = _byteConverter.ToByteArray(value);
                byte[] result = _murmurHash.ComputeHash(bytes);
                hash = BitConverter.ToUInt64(result, 0);
            }

            return hash;
        }
    }
}
