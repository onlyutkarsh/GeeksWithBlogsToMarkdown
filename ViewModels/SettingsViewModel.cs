using GeeksWithBlogsToMarkdown.Commands.Base;
using GeeksWithBlogsToMarkdown.Extensions;
using GeeksWithBlogsToMarkdown.ViewModels.Base;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GeeksWithBlogsToMarkdown.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IDataErrorInfo
    {
        public Dictionary<string, string> Errors = new Dictionary<string, string>();
        private ICommand _cancelCommand;
        private ICommand _editPasswordCommand;
        private string _frontMatter;
        private string _gwbBlogUrl;
        private string _gwbPassword;
        private string _gwbUserName;
        private string _message;
        private string _outputFolder;

        private ICommand _saveCommand;
        private ICommand _showPasswordCommand;

        private bool _showPasswordChecked;

        public bool ShowPasswordChecked
        {
            get { return _showPasswordChecked; }
            set
            {
                _showPasswordChecked = value;
                NotifyPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            ShowMessageCommand = new DelegateCommand(OnShowMessage);
            BrowseForOutputFolderCommand = new DelegateCommand(OnBrowseForOutputFolder);
            SaveCommand = new DelegateCommand(OnSave);
            CancelCommand = new DelegateCommand(OnCancel);
            ShowPasswordCommand = new DelegateCommand<ToggleButton>(OnShowPassword);
            EditPasswordCommand = new DelegateCommand<ToggleButton>(OnEditPassword);

            ShowPasswordChecked = false;

            Settings.Instance.ReadSettings();
            GWBUserName = Settings.Instance.GWBUserName;
            GWBBlogUrl = Settings.Instance.GWBBlogUrl;
            OutputFolder = Settings.Instance.OutputFolder;
            FrontMatter = Settings.Instance.FrontMatter;
        }

        private async void OnEditPassword(ToggleButton showPasswordButton)
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            if (metroWindow != null)
            {
                var result = await PromptForCredentials(metroWindow);
                if (result == null)
                {
                    //User pressed cancel
                    return;
                }
                //user clicked Save in prompt
                Settings.Instance.GWBUserName = result.Username;
                Settings.Instance.GWBPassword = result.Password.ToSecureString().EncryptString();

                Settings.Instance.WriteOrUpdateSettings();
                Settings.Instance.ReadSettings();
                GWBUserName = Settings.Instance.GWBUserName;

                if (showPasswordButton.IsChecked.HasValue && showPasswordButton.IsChecked.Value)
                {
                    GWBPassword = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();
                }
            }
        }

        public bool IsValid()
        {
            IsValidating = true;
            try
            {
                NotifyPropertyChanged(() => GWBBlogUrl);
                NotifyPropertyChanged(() => GWBUserName);
                NotifyPropertyChanged(() => OutputFolder);
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

                ShowPasswordChecked = false;
            }
        }

        private void OnSave()
        {
            if (IsValid())
            {
                //Save
                Settings.Instance.GWBUserName = GWBUserName;
                //No need to save the password as password will always be stored using Edit dialog
                Settings.Instance.GWBBlogUrl = GWBBlogUrl;
                Settings.Instance.OutputFolder = OutputFolder;
                Settings.Instance.FrontMatter = FrontMatter;

                Settings.Instance.WriteOrUpdateSettings();

                var metroWindow = Application.Current.MainWindow as MetroWindow;
                if (metroWindow != null)
                {
                    var settingsFlyout = metroWindow.Flyouts.Items[0] as Flyout;
                    if (settingsFlyout == null)
                    {
                        return;
                    }
                    settingsFlyout.IsOpen = false;
                }
            }
        }

        private void OnShowMessage()
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;
            mainWindow?.ShowMessageAsync("GWB to Markdown", "Select the output folder");
        }

        private void OnShowPassword(ToggleButton button)
        {
            GWBPassword = string.Empty;

            if (button.IsChecked.HasValue && button.IsChecked.Value)
            {
                var decryptedPassword = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();
                GWBPassword = decryptedPassword;
            }
        }

        private async Task<LoginDialogData> PromptForCredentials(MetroWindow metroWindow)
        {
            Settings.Instance.ReadSettings();
            var result = await metroWindow.ShowLoginAsync("Login to Geekswithblogs", "Geekswithblogs credentials",
                new LoginDialogSettings
                {
                    InitialUsername = Settings.Instance.GWBUserName,
                    InitialPassword = Settings.Instance.GWBPassword.DecryptString().ToInsecureString(),
                    ShouldHideUsername = false,
                    EnablePasswordPreview = true,
                    ColorScheme = MetroDialogColorScheme.Theme,
                    RememberCheckBoxVisibility = System.Windows.Visibility.Hidden,
                    AffirmativeButtonText = "Save"
                });
            return result;
        }

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

        public ICommand EditPasswordCommand
        {
            get { return _editPasswordCommand; }
            set
            {
                _editPasswordCommand = value;
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

        public ICommand ShowPasswordCommand
        {
            get { return _showPasswordCommand; }
            set
            {
                _showPasswordCommand = value;
                NotifyPropertyChanged();
            }
        }

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
    }
}