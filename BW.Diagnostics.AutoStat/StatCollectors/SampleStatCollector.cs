using Murmur;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    class SampleStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        const int MaxSamples = 10_000;
        List<(ulong hash, T value)> _keyHashes = new List<(ulong hash, T value)>(MaxSamples);
        byte _minRequiredZeros = 0;

        public SampleStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            cancel = false;
        }

        public void AddValue(ulong keyHash, T value)
        {
            // Count the trailings 0's of hash, if > n, then add hash to list.
            // If maxed, recount the 0's of the hashes at n+1, trimming the list.

            var zeros = (byte)(BitHelpers.CountTrailing0Bits64((ulong)keyHash));
            if (zeros >= _minRequiredZeros)
            {
                if (_keyHashes.Count < MaxSamples)
                {
                    _keyHashes.Add((keyHash, value));
                }
                else
                {
                    _minRequiredZeros++;

                    var newHashes = new List<(ulong hash, T value)>(MaxSamples);

                    foreach (var hashRetest in _keyHashes)
                    {
                        var zerosRetest = (byte)(BitHelpers.CountTrailing0Bits64((ulong)hashRetest.hash));
                        if (zerosRetest >= _minRequiredZeros) newHashes.Add(hashRetest);
                    }

                    _keyHashes = newHashes;

                    if (zeros >= _minRequiredZeros) _keyHashes.Add((keyHash, value));
                }
            }
        }

        public IEnumerable<IStat> GetStats()
        {
            yield break;
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector)
        {
            var otherHashes = (statCollector as SampleStatCollector<T>)._keyHashes;

            if (_keyHashes.Count == 0 && otherHashes.Count == 0) return Enumerable.Empty<IComparedStat>();

            return new SampleComparedStat<T>(MemberName, _keyHashes, otherHashes).ToEnumerable();
        }
    }
}
