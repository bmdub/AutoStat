using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    internal class BitHelpers
    {
        static readonly int[] MultiplyDeBruijnBitPosition64 =
        {
            0, 1, 17, 2, 18, 50, 3, 57,
            47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6,
            61, 44, 12, 35, 60, 11, 10, 9,
        };

        public static byte CountTrailing0Bits64(ulong b)
        {
            return (byte)MultiplyDeBruijnBitPosition64[((ulong)((long)b & -(long)b) * 0x37E84A99DAE458F) >> 58];
        }
    }
}
