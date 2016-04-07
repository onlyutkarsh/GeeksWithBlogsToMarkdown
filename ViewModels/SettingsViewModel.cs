using System.ComponentModel;
using GeeksWithBlogsToMarkdown.Commands.Base;
using GeeksWithBlogsToMarkdown.ViewModels.Base;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace GeeksWithBlogsToMarkdown.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _gwbPassword;
        private string _gwbUserName;
        private string _message;

        public SettingsViewModel()
        {
            ShowMessageCommand = new DelegateCommand(OnShowMessage);
            BrowseForOutputFolderCommand = new DelegateCommand(OnBrowseForOutputFolder);
        }

        public DelegateCommand BrowseForOutputFolderCommand { get; set; }

        public string GWBPassword
        {
            get { return _gwbPassword; }
            set
            {
                _gwbPassword = value;
                NotifyPropertyChanged();
            }
        }

        private string _gwbBlogUrl;

        public string GWBBlogUrl
        {
            get { return _gwbBlogUrl; }
            set
            {
                _gwbBlogUrl = value;
                NotifyPropertyChanged();
            }
        }

        public string GWBUserName
        {
            get { return _gwbUserName; }
            set
            {
                _gwbUserName = value;
                NotifyPropertyChanged();
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ShowMessageCommand { get; set; }

        private void OnBrowseForOutputFolder()
        {
        }

        private void OnShowMessage()
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;
            mainWindow?.ShowMessageAsync("GWB to Markdown", "Select the output folder");
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "GWBBlogUrl" && string.IsNullOrWhiteSpace(GWBBlogUrl))
                {
                    return "Please specify your GWB blog URL!";
                }
                if (columnName == "GWBUserName" && string.IsNullOrWhiteSpace(GWBUserName))
                {
                    return "Username cannot be empty!";
                }
                if (columnName == "OutputFolder" && string.IsNullOrWhiteSpace(OutputFolder))
                {
                    return "Please set the output folder to save markdown files.";
                }
                return null;
            }
        }

        private string _outputFolder;

        public string OutputFolder
        {
            get { return _outputFolder; }
            set
            {
                _outputFolder = value;
                NotifyPropertyChanged();
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }
    }
}