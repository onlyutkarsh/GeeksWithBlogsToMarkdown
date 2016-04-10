using System;
using System.Security;
using ConfOxide;

namespace GeeksWithBlogsToMarkdown
{
    sealed class Settings : SettingsBase<Settings>
    {
        public string GWBBlogUrl { get; set; }

        public string GWBUserName { get; set; }

        public string GWBPassword { get; set; }

        public string OutputPath { get; set; }

        public string FrontMatter { get; set; }

        private static readonly Lazy<Settings> lazy = new Lazy<Settings>(() => new Settings());

        public static Settings Instance { get { return lazy.Value; } }

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
