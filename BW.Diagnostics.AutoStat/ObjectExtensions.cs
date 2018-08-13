using System.Collections.Generic;

namespace BW.Diagnostics.StatCollection
{
    static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static string ToStringOrNull(this object obj)
        {
            return obj == null ? null : obj.ToString();
        }
    }
}
