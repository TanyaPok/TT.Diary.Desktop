using System;
using System.Collections.Generic;
using System.Linq;

namespace TT.Diary.Desktop.ViewModels.Common.Extensions
{
    public static class CollectionExtension
    {
        public static IEnumerable<T> GetFlatSequence<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push(child);
            }
        }
    }
}
