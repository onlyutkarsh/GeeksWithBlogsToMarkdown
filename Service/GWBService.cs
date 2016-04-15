using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using BlogML;
using BlogML.Xml;
using CookComputing.XmlRpc;

namespace GeeksWithBlogsToMarkdown.Service
{
    public sealed class GWBService
    {
        //TODO: Ensure class is sealed
        private static readonly Lazy<GWBService> _lazy = new Lazy<GWBService>(() => new GWBService());
        private readonly ICSMetaWeblog proxy;
        private string _userName;
        private string _password;

        public static GWBService Instance
        {
            get { return _lazy.Value; }
        }

        private GWBService()
        {
            proxy = XmlRpcProxyGen.Create<ICSMetaWeblog>();
            Settings.Instance.ReadSettings();

            _userName = Settings.Instance.GWBUserName;
            _password = Settings.Instance.GWBPassword;

            var url = new Uri(Settings.Instance.GWBBlogUrl);

            proxy.Url = Settings.Instance.GWBBlogUrl;
        }
        private IEnumerable<BlogMLPost> GetPosts(BlogInfo blog, IEnumerable<BlogMLCategory> categories)
        {
            var posts = from p in proxy.getRecentPosts(blog.blogid, _userName, _password, 1000)
                        select BuildPost(p, categories);
            return posts;

        }

        private BlogMLPost BuildPost(Post p, IEnumerable<BlogMLCategory> categories)
        {
            BlogMLPost post = new BlogMLPost()
            {
                PostType = BlogPostTypes.Normal,
                Title = p.title,
                Content = new BlogMLContent()
                {
                    Text = p.description,
                },
                DateCreated = p.dateCreated,
                DateModified = DateTime.Today,
                ID = p.postid.ToString(),

            };
            if (p.categories != null && p.categories.Any())
            {
                var categoryReference = p.categories.Select(x => new BlogMLCategoryReference
                {
                    Ref = x
                }).ToList();

                post.Categories.AddRange(categoryReference);
            }
            return post;
        }

        private IEnumerable<BlogMLCategory> GetCategories(BlogInfo blog)
        {

            var categories = from c in proxy.getCategories(blog.blogid, _userName, _password)
                             select new BlogMLCategory() { ID = c.categoryid, Title = c.title, Description = c.description, Approved = true };
            return categories;

        }

    }
}
