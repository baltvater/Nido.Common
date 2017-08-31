using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ForEach<T>(this IQueryable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
            return items;
        }
    }
}
