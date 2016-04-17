using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeeksWithBlogsToMarkdown.Common
{
    public sealed class AppContext
    {
        //TODO: Ensure class is sealed
        private static readonly Lazy<AppContext> _lazy = new Lazy<AppContext>(() => new AppContext());

        public static AppContext Instance
        {
            get { return _lazy.Value; }
        }

        private AppContext()
        {
        }

        public string ApplicationName => "Geekswithblogs to Markdown";
    }
}
