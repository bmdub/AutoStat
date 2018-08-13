using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    interface IByteConverter<T>
    {
        byte[] ToByteArray(T value);
    }
}
