using GeeksWithBlogsToMarkdown.Commands.Base;
using GeeksWithBlogsToMarkdown.ViewModels.Base;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeeksWithBlogsToMarkdown.Extensions;
using Ookii.Dialogs.Wpf;

namespace GeeksWithBlogsToMarkdown.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        public Dictionary<string, string> Errors = new Dictionary<string, string>();
        private ICommand _cancelCommand;
        private string _frontMatter;
        private string _gwbBlogUrl;
        private string _gwbPassword;
        private string _gwbUserName;
        private string _message;
        private string _outputFolder;

        private ICommand _saveCommand;

        public DelegateCommand BrowseForOutputFolderCommand { get; set; }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
            set
            {
                _cancelCommand = value;
                NotifyPropertyChanged();
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string FrontMatter
        {
            get { return _frontMatter; }
            set
            {
                _frontMatter = value;
                NotifyPropertyChanged();
            }
        }

        public string GWBBlogUrl
        {
            get { return _gwbBlogUrl; }
            set
            {
                _gwbBlogUrl = value;
                NotifyPropertyChanged();
            }
        }

        public string GWBPassword
        {
            get { return _gwbPassword; }
            set
            {
                _gwbPassword = value;
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

        public bool IsValidating { get; set; }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public string OutputFolder
        {
            get { return _outputFolder; }
            set
            {
                _outputFolder = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
            set
            {
                _saveCommand = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ShowMessageCommand { get; set; }

        public string this[string columnName]
        {
            get
            {
                if (!IsValidating) return null;

                string result = string.Empty;
                if (!IsValidating) return result;
                Errors.Remove(columnName);

                switch (columnName)
                {
                    case "GWBBlogUrl":
                        {
                            if (string.IsNullOrWhiteSpace(GWBBlogUrl))
                            {
                                result = "Please specify your GWB blog URL!";
                            }
                            break;
                        }
                    case "GWBUserName":
                        {
                            if (string.IsNullOrWhiteSpace(GWBUserName))
                            {
                                result = "Username cannot be empty!";
                            }

                            break;
                        }
                    case "OutputFolder":
                        {
                            if (string.IsNullOrWhiteSpace(OutputFolder))
                            {
                                result = "Output folder cannot be empty!";
                            }

                            break;
                        }
                }
                if (result != string.Empty) Errors.Add(columnName, result);
                return result;
            }
        }

        public SettingsViewModel()
        {
            ShowMessageCommand = new DelegateCommand(OnShowMessage);
            BrowseForOutputFolderCommand = new DelegateCommand(OnBrowseForOutputFolder);
            SaveCommand = new DelegateCommand<PasswordBox>(OnSave);
            CancelCommand = new DelegateCommand(OnCancel);

            Settings.Instance.ReadSettings();
            GWBUserName = Settings.Instance.GWBUserName;
            GWBPassword = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();
            GWBBlogUrl = Settings.Instance.GWBBlogUrl;
            OutputFolder = Settings.Instance.OutputFolder;
            FrontMatter = Settings.Instance.FrontMatter;

        }

        public bool IsValid()
        {
            IsValidating = true;
            try
            {
                NotifyPropertyChanged("GWBBlogUrl");
                NotifyPropertyChanged("GWBUserName");
                NotifyPropertyChanged("OutputFolder");
            }
            finally
            {
                IsValidating = false;
            }
            return (!Errors.Any());
        }

        private void OnBrowseForOutputFolder()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                //Show default browser dialog
            }
            else
            {
                var showDialog = dialog.ShowDialog();
                if (showDialog != null && (bool)showDialog)
                {
                    OutputFolder = dialog.SelectedPath;
                }
            }
            
        }

        private void OnCancel()
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow != null)
            {
                var settingsFlyout = metroWindow.Flyouts.Items[0] as Flyout;
                if (settingsFlyout == null)
                {
                    return;
                }
                IsValidating = false;
                NotifyPropertyChanged("GWBBlogUrl");
                NotifyPropertyChanged("GWBUserName");
                NotifyPropertyChanged("OutputFolder");
                NotifyPropertyChanged("FrontMatter");
                settingsFlyout.IsOpen = false;
            }
        }

        private void OnSave(PasswordBox passwordBox)
        {
            if (IsValid())
            {
                //Save
                Settings.Instance.GWBUserName = GWBUserName;
                Settings.Instance.GWBPassword = passwordBox.Password.ToSecureString().EncryptString();
                Settings.Instance.GWBBlogUrl = GWBBlogUrl;
                Settings.Instance.OutputFolder = OutputFolder;
                Settings.Instance.FrontMatter = FrontMatter;

                Settings.Instance.WriteOrUpdateSettings();
            }
        }

        private void OnShowMessage()
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;
            mainWindow?.ShowMessageAsync("GWB to Markdown", "Select the output folder");
        }
    }
}