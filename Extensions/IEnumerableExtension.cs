using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
            return items;
        }
    }
}
