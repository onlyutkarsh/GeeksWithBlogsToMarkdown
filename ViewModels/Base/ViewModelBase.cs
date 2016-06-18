using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace GeeksWithBlogsToMarkdown.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var deleg = PropertyChanged;
            deleg?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            var deleg = PropertyChanged;
            deleg?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}