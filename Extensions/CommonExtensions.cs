using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace GeeksWithBlogsToMarkdown.Extensions
{
    public static class CommonExtensions
    {
        public static void AddToUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            var col = new ObservableCollection<T>();
            foreach (var cur in enumerable)
            {
                col.Add(cur);
            }
            return col;
        }
    }
}