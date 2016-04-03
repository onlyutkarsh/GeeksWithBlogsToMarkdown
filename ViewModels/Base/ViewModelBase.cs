using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GeeksWithBlogsToMarkdown.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var deleg = PropertyChanged;
            deleg?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
