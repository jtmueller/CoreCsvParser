using System;
using System.Collections.Generic;

namespace CoreCsvParser.Extensions
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, ReadOnlySpan<T> span)
        {
            foreach (var item in span)
            {
                list.Add(item);
            }
        }
    }
}
