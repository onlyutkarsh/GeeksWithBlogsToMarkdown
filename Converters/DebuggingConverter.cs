using GeeksWithBlogsToMarkdown.Converters.Base;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeeksWithBlogsToMarkdown.Converters
{
    public class DebuggingConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}