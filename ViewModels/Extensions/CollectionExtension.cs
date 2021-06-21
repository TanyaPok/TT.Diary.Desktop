using System;
using System.Collections.Generic;
using System.Linq;
using TT.Diary.Desktop.ViewModels.Common;
using TT.Diary.Desktop.ViewModels.Lists;

namespace TT.Diary.Desktop.ViewModels.Extensions
{
    public static class CollectionExtension
    {
        public static IEnumerable<T> GetFlatSequence<T>(this IEnumerable<T> items,
            Func<T, IEnumerable<T>> childSelector)
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

        /// <summary>
        /// Reloading collection and calling subscribers - changing collection items
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void ReUpload<T>(this MtObservableCollection<T> collection, IEnumerable<T> data)
            where T : AbstractListItem
        {
            foreach (var item in collection.ToArray())
            {
                collection.Remove(item);
            }

            foreach (var item in data)
            {
                collection.Add(item);
            }
        }
    }
}