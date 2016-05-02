using BlogML.Xml;
using GeeksWithBlogsToMarkdown.Commands.Base;
using GeeksWithBlogsToMarkdown.Common;
using GeeksWithBlogsToMarkdown.Extensions;
using GeeksWithBlogsToMarkdown.Service;
using GeeksWithBlogsToMarkdown.TokenReplacement;
using GeeksWithBlogsToMarkdown.ViewModels.Base;
using Html2Markdown;
using HtmlAgilityPack;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeeksWithBlogsToMarkdown.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<BlogMLPost> _blogPosts;
        private string _blogTitle;
        private string _blogUrl;
        private IDialogCoordinator _dialogCoordinator;
        private ICommand _getPostsCommand;
        private string _htmlMarkup;
        private string _markdown;
        private ICommand _saveAllPostsCommand;
        private ICommand _savePostCommand;
        private DelegateCommand<object> _selectionChangedCommand;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            ShowSettingsCommand = new DelegateCommand<MetroWindow>(ShowSettings, (window) => true);
            GetPostsCommand = new DelegateCommand<MetroWindow>(OnGetPosts, (window) => true);
            SelectionChangedCommand = new DelegateCommand<object>(OnSelectionChanged);
            SaveAllPostsCommand = new DelegateCommand(OnSaveAllPosts, ()=> BlogPosts != null && BlogPosts.Count > 0);
            SavePostCommand = new DelegateCommand(OnSavePost, () => SelectedPost != null);

            Settings.Instance.ReadSettings();
        }

        public string GetSuggestedFilenameFromTitle(BlogMLPost post)
        {
            var postTitle = post.Title;
            if (string.IsNullOrEmpty(postTitle)) return string.Empty;
            if (string.IsNullOrEmpty(postTitle)) return string.Empty;
            var filename = post.DateCreated.ToString("yyyy-MM-dd-") + postTitle.ToSlug(true);
            return filename;
        }

        private string AddFrontmatter(BlogMLPost post, string markdown)
        {
            var frontMatter = Settings.Instance.FrontMatter;

            if (string.IsNullOrWhiteSpace(frontMatter))
            {
                return markdown;
            }
            var fr = new FastReplacer("<$", "$>");
            var title = post.Title.ToHtmlDecodedString().Replace("\"", "");
            fr.Append(frontMatter);
            fr.Replace("<$GWB_TITLE$>", $"\"{title}\"");
            var description = post.HasExcerpt
                ? post.Excerpt.Text.ToHtmlDecodedString()
                : title;
            fr.Replace("<$GWB_DESCRIPTION$>", $"\"{description}\"");
            fr.Replace("<$GWB_AUTHOR$>", "Utkarsh Shigihalli");

            var categories = post.Categories.Cast<BlogMLCategoryReference>().Select(x => $"\"{x.Ref}\"");

            fr.Replace("<$GWB_CATEGORIES$>", $"{string.Join($"{Environment.NewLine}- ", categories)}");
            fr.Replace("<$GWB_DATE$>", post.DateCreated.ToString("yyyy-MM-dd HH:mm:ss"));
            return $"{fr}{Environment.NewLine}{markdown}";
        }

        private async void OnGetPosts(MetroWindow metroWindow)
        {
            LoginDialogData result = null;
            bool canConnect = true;
            var userName = string.Empty;
            var password = string.Empty;
            //check if username and password already available

            if (string.IsNullOrWhiteSpace(Settings.Instance.GWBUserName))
            {
                //prompt is username is empty
                result = await PromptForCredentials(metroWindow);
                canConnect = result != null;
            }

            if (result == null && !canConnect)
            {
                //User pressed cancel
                return;
            }

            if (result == null)
            {
                //if everything okay in settings, continue
                userName = Settings.Instance.GWBUserName;
                password = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();
            }
            else
            {
                userName = result.Username;
                password = result.Password;
                //user clicked connect in prompt
                if (result.ShouldRemember)
                {
                    Settings.Instance.GWBUserName = userName;
                    Settings.Instance.GWBPassword = password.ToSecureString().EncryptString();
                    Settings.Instance.WriteOrUpdateSettings();
                }
            }
            ProgressDialogController progressController = await _dialogCoordinator.ShowProgressAsync(this, AppContext.Instance.ApplicationName, "Getting blog posts...");
            progressController.SetIndeterminate();
            //Connect to GWB
            var response = await GWBService.Instance.GetAllBlogPostsAsync(progressController);

            await progressController.CloseAsync();

            if (response.Exception != null)
            {
                await _dialogCoordinator.ShowMessageAsync(this, AppContext.Instance.ApplicationName, response.Exception.Message);
            }
            else
            {
                var blog = response.Data;
                BlogTitle = blog.Title.ToUpper();
                BlogUrl = blog.RootUrl;
                BlogPosts = blog.Posts.ToObservableCollection();
            }
        }

        private async void OnSaveAllPosts()
        {
            var posts = BlogPosts;
            ProgressDialogController progressController = await _dialogCoordinator.ShowProgressAsync(this, AppContext.Instance.ApplicationName, "Generating markdown...");
            progressController.SetIndeterminate();
          
            await posts.ForEachAsync(async post =>
            {
                await SavePost(post, false, progressController);
            });

            await progressController.CloseAsync();
        }

        private async void OnSavePost()
        {
            ProgressDialogController progressController = await _dialogCoordinator.ShowProgressAsync(this, AppContext.Instance.ApplicationName, "Generating markdown...");
            progressController.SetIndeterminate();
            progressController.SetMessage($"Generating...{SelectedPost.Title.ToHtmlDecodedString()}");
            await SavePost(SelectedPost, true, progressController);
            await progressController.CloseAsync();
        }

        private void OnSelectionChanged(object selectedItems)
        {
            System.Collections.IList items = (System.Collections.IList)selectedItems;

            if (items.Count <= 0)
                return;

            var post = items[0] as BlogMLPost;

            SelectedPost = post;

            if (post != null)
            {
                HtmlMarkup = post.Content.Text;
                var converter = new Converter();
                var markdown = converter.Convert(post.Content.Text);
                Markdown = AddFrontmatter(post, markdown);
            }
        }

        private async Task<LoginDialogData> PromptForCredentials(MetroWindow metroWindow)
        {
            var result = await metroWindow.ShowLoginAsync("Login to Geekswithblogs", "Enter your Geekswithblogs credentials",
                new LoginDialogSettings
                {
                    ShouldHideUsername = false,
                    EnablePasswordPreview = true,
                    ColorScheme = MetroDialogColorScheme.Theme,
                    RememberCheckBoxVisibility = System.Windows.Visibility.Visible,
                    AffirmativeButtonText = "Connect"
                });
            return result;
        }

        private async Task SavePost(BlogMLPost blogPost, bool promptBeforFileOverwrite, ProgressDialogController progressController)
        {
            var message = $"Generating...{blogPost.Title}";
            progressController.SetMessage(message);
            await Task.Delay(800);

            var htmlMarkup = blogPost.Content.Text;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlMarkup);

            var imageTags = doc.DocumentNode.SelectNodes("//img/@src");

            if (imageTags != null)
            {
                if (!Directory.Exists(Settings.Instance.ImagesFolder))
                {
                    Directory.CreateDirectory(Settings.Instance.ImagesFolder);
                }
                //if images in post

                var attributesCollectionOfImage = imageTags.Select(i => i.Attributes);

                var attributeCollection = attributesCollectionOfImage as IList<HtmlAttributeCollection> ?? attributesCollectionOfImage.ToList();
                var totalImages = attributeCollection.Select(x => x.Where(a => a.Name == "src")).Count();

                int imageCount = 0;

                foreach (var attribute in attributeCollection.Select(x => x.Where(a => a.Name == "src")))
                {
                    foreach (var url in attribute)
                    {
                        imageCount++;

                        progressController.SetMessage($"{message}\nDownloading images in post...{imageCount} of {totalImages}");
                        var client = new WebClient();

                        var imageUri = new Uri(url.Value);
                        var twoWordsFromPost = blogPost.Title.ToLower().Split(' ').Take(4);
                        var imageName = $"{blogPost.DateCreated.ToString("yyyy_MM_dd")}_{string.Join("_", twoWordsFromPost)}_Image{imageCount}{Path.GetExtension(imageUri.AbsoluteUri)}";
                        var imageSavePath = Path.Combine(Settings.Instance.ImagesFolder, imageName);

                        await client.DownloadFileTaskAsync(imageUri, imageSavePath);

                        url.Value = $"{Settings.Instance.CustomImagesFolder}/{imageName}";
                    }
                }
            }
            var converter = new Converter();
            var markdown = converter.Convert(doc.DocumentNode.InnerHtml);
            markdown = AddFrontmatter(blogPost, markdown);
            var suggestedFileName = GetSuggestedFilenameFromTitle(blogPost);

            var markdownFileName = $"{suggestedFileName}.md";

            var markdownPath = Path.Combine(Settings.Instance.OutputFolder, markdownFileName);
            if (promptBeforFileOverwrite && File.Exists(markdownPath))
            {
                MessageDialogResult messageDialogResult = await _dialogCoordinator.ShowMessageAsync(this, AppContext.Instance.ApplicationName, "File already exists. Do you want to overwrite?",
                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Yes",
                        NegativeButtonText = "No"
                    });
                if (messageDialogResult == MessageDialogResult.Affirmative)
                {
                    File.WriteAllText(markdownPath, markdown);
                }
                else
                {
                    var dialog = new SaveFileDialog();
                    dialog.Filter = "*.md (Markdown files) | *.md";
                    dialog.InitialDirectory = Settings.Instance.OutputFolder;
                    dialog.FileName = markdownFileName;
                    dialog.ShowDialog();
                }
            }
            else
            {
                File.WriteAllText(markdownPath, markdown);
            }
        }

        private void ShowSettings(MetroWindow metroWindow)
        {
            var settingsFlyout = metroWindow.Flyouts.Items[0] as Flyout;
            if (settingsFlyout == null)
            {
                return;
            }
            settingsFlyout.DataContext = new SettingsViewModel();
            settingsFlyout.IsOpen = true;
        }

        public ObservableCollection<BlogMLPost> BlogPosts
        {
            get { return _blogPosts; }
            set
            {
                _blogPosts = value;
                NotifyPropertyChanged();
            }
        }

        public string BlogTitle
        {
            get { return _blogTitle; }
            set
            {
                _blogTitle = value;
                NotifyPropertyChanged();
            }
        }

        public string BlogUrl
        {
            get { return _blogUrl; }
            set
            {
                _blogUrl = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand GetPostsCommand
        {
            get { return _getPostsCommand; }
            set
            {
                _getPostsCommand = value;
                NotifyPropertyChanged();
            }
        }

        public string HtmlMarkup
        {
            get { return _htmlMarkup; }
            set
            {
                _htmlMarkup = value;
                NotifyPropertyChanged();
            }
        }

        public string Markdown
        {
            get { return _markdown; }
            set
            {
                _markdown = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SaveAllPostsCommand
        {
            get { return _saveAllPostsCommand; }
            set
            {
                _saveAllPostsCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SavePostCommand
        {
            get { return _savePostCommand; }
            set
            {
                _savePostCommand = value;
                NotifyPropertyChanged();
            }
        }

        public BlogMLPost SelectedPost { get; set; }

        public DelegateCommand<object> SelectionChangedCommand
        {
            get { return _selectionChangedCommand; }
            set
            {
                _selectionChangedCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ShowSettingsCommand { get; set; }
    }
}