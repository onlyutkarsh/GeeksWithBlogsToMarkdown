using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GeeksWithBlogsToMarkdown.Converters.Base;

namespace GeeksWithBlogsToMarkdown.Converters
{
    class ShowPasswordConverter : BaseConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var pwd = values[0];
            var showPasswordChecked = values[1] as bool? ?? false;

            if (!showPasswordChecked)
                return "********";

            return pwd;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
