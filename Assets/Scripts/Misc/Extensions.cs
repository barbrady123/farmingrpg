using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items) =>  items.Select((item, index) => (item, index));
}
