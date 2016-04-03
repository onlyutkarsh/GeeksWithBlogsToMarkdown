using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GeeksWithBlogsToMarkdown.Commands.Base;
using GeeksWithBlogsToMarkdown.ViewModels.Base;
using MahApps.Metro.Controls;

namespace GeeksWithBlogsToMarkdown.ViewModels
{
    public class MainVewModel : ViewModelBase
    {
        public ICommand ShowSettingsCommand { get; set; }
        public MainVewModel()
        {
            ShowSettingsCommand = new DelegateCommand<MetroWindow>(ShowSettings, (window) => true);
        }

        private void ShowSettings(MetroWindow metroWindow)
        {
            var settingsFlyout = metroWindow.Flyouts.Items[0] as Flyout;
            if (settingsFlyout == null)
            {
                return;
            }
            settingsFlyout.IsOpen = true;
        }
    }
}
