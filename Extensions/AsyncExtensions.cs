using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeeksWithBlogsToMarkdown.Extensions
{
    public static class AsyncExtensions
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body)
        {
            return Task.WhenAll(
                from item in source
                select Task.Run(() => body(item)));
        }
    }
}