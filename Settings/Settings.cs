using ConfOxide;
using System;

namespace GeeksWithBlogsToMarkdown
{
    public sealed class Settings : SettingsBase<Settings>
    {
        private static readonly Lazy<Settings> lazy = new Lazy<Settings>(() => new Settings());
        public static Settings Instance { get { return lazy.Value; } }
        public string CustomImagesFolder { get; set; }
        public string FrontMatter { get; set; }
        public string GWBBlogUrl { get; set; }

        public string GWBPassword { get; set; }
        public string GWBUserName { get; set; }
        public string ImagesFolder { get; set; }
        public string OutputFolder { get; set; }

        private Settings()
        {
        }

        public Settings ReadSettings(string fileName = "Settings.json")
        {
            return this.ReadJsonFile(fileName);
        }

        public Settings WriteOrUpdateSettings(string fileName = "Settings.json")
        {
            return this.WriteJsonFile(fileName);
        }
    }
}