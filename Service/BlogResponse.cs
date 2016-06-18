using System;

namespace GeeksWithBlogsToMarkdown.Service
{
    public class BlogResponse<T>
    {
        public T Data { get; set; }

        public Exception Exception { get; set; }
    }
}