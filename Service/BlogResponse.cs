using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeksWithBlogsToMarkdown.Service
{
    public class BlogResponse<T>
    {
        public T Data { get; set; }

        public Exception Exception { get; set; }
    }
}
