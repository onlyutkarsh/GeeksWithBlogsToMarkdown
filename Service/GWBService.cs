using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using BlogML;
using BlogML.Xml;
using CookComputing.XmlRpc;
using GeeksWithBlogsToMarkdown.Extensions;
using MahApps.Metro.Controls.Dialogs;

namespace GeeksWithBlogsToMarkdown.Service
{
    public sealed class GWBService
    {
        //TODO: Ensure class is sealed
        private static readonly Lazy<GWBService> _lazy = new Lazy<GWBService>(() => new GWBService());
        private readonly ICSMetaWeblog _proxy;
        private string _userName;
        private string _password;
        private string _blogId;

        public static GWBService Instance
        {
            get { return _lazy.Value; }
        }

        private GWBService()
        {
            _proxy = XmlRpcProxyGen.Create<ICSMetaWeblog>();
            Settings.Instance.ReadSettings();

            _userName = Settings.Instance.GWBUserName;
            _password = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();

            var url = new Uri(Settings.Instance.GWBBlogUrl);
            _blogId = url.Segments[1].Replace("/", "");

            _proxy.Url = $"http://www.geekswithblogs.net/{_blogId}/services/metablogapi.aspx";
        }

        public async Task<BlogResponse<BlogMLBlog>> GetAllBlogPostsAsync(ProgressDialogController progressController)
        {
            var response = new BlogResponse<BlogMLBlog>();
            try
            {
                _password = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();
                var blogs = _proxy.getUsersBlogs(_blogId, _userName, _password);

                await Task.Run(() =>
                {

                    foreach (BlogInfo blog in blogs)
                    {
                        BlogMLBlog xblog = new BlogMLBlog
                        {
                            Title = blog.blogName,
                            RootUrl = blog.url
                        };
                        progressController.SetMessage("Getting categories");
                        var categories = GetCategories(blog);
                        var blogMlCategories = categories as IList<BlogMLCategory> ?? categories.ToList();
                        xblog.Categories.AddRange(blogMlCategories);
                        progressController.SetMessage("Getting posts");
                        var posts = GetPosts(blog, blogMlCategories);
                        xblog.Posts.AddRange(posts);
                        response.Data = xblog;
                    }

                });
            }
            catch (Exception exception)
            {
                response.Exception = exception;
            }
            return response;
        }

        private IEnumerable<BlogMLPost> GetPosts(BlogInfo blog, IEnumerable<BlogMLCategory> categories)
        {
            var posts = from p in _proxy.getRecentPosts(blog.blogid, _userName, _password, 1000)
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
                PostUrl = p.permalink //$"http://www.geekswithblogs.net{p.link}",
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

            var categories = from c in _proxy.getCategories(blog.blogid, _userName, _password)
                             select new BlogMLCategory() { ID = c.categoryid, Title = c.title, Description = c.description, Approved = true };
            return categories;

        }

    }
}
