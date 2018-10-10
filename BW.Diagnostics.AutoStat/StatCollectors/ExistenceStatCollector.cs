using Murmur;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    class ExistenceStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        IByteConverter<T> _byteConverter;
        const int MaxSamples = 10_000;
        List<ulong> _hashes = new List<ulong>(MaxSamples);
        byte _minRequiredZeros = 0;
        Murmur128 _murmurHash;

        public ExistenceStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            _byteConverter = ByteConverterHelpers.GetByteConverter<T>();
            _murmurHash = MurmurHash.Create128(managed: true, preference: AlgorithmPreference.X64);
            cancel = false;
        }

        public void AddValue(T value)
        {
            ulong hash = 1;
            if (value != null)
            {
                var bytes = _byteConverter.ToByteArray(value);                               
                byte[] result = _murmurHash.ComputeHash(bytes);
                hash = BitConverter.ToUInt64(result, 0);
            }

            AddHash(hash);
        }

        void AddHash(ulong hash)
        {
            // Count the trailings 0's of hash, if > n, then add hash to list.
            // If maxed, recount the 0's of the hashes at n+1, trimming the list.

            var zeros = (byte)(BitHelpers.CountTrailing0Bits64((ulong)hash));
            if (zeros >= _minRequiredZeros)
            {
                if (_hashes.Count < MaxSamples)
                {
                    _hashes.Add(hash);
                }
                else
                {
                    _minRequiredZeros++;

                    var newHashes = new List<ulong>(MaxSamples);

                    foreach (var hashRetest in _hashes)
                    {
                        var zerosRetest = (byte)(BitHelpers.CountTrailing0Bits64((ulong)hashRetest));
                        if (zerosRetest >= _minRequiredZeros) newHashes.Add(hashRetest);
                    }

                    _hashes = newHashes;

                    if (zeros >= _minRequiredZeros) _hashes.Add(hash);
                }
            }
        }

        public IEnumerable<IStat> GetStats()
        {
            yield break;
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector)
        {
            var otherHashes = (statCollector as ExistenceStatCollector<T>)._hashes;

            return new ExistenceComparedStat<T>(MemberName, _hashes, otherHashes).ToEnumerable();
        }
    }
}
