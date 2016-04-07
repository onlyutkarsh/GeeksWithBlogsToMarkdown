using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GeeksWithBlogsToMarkdown.Commands.Base;
using GeeksWithBlogsToMarkdown.ViewModels.Base;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GeeksWithBlogsToMarkdown.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _message;
        public ICommand ShowMessageCommand { get; set; }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            ShowMessageCommand = new DelegateCommand(OnShowMessage);
            BrowseForOutputFolderCommand = new DelegateCommand(OnBrowseForOutputFolder);
        }

        public DelegateCommand BrowseForOutputFolderCommand { get; set; }

        private void OnBrowseForOutputFolder()
        {
            
        }

        private void OnShowMessage()
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;
            mainWindow?.ShowMessageAsync("GWB to Markdown", "Select the output folder");
        }
    }
}
