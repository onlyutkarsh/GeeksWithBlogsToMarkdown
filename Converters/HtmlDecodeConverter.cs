using GeeksWithBlogsToMarkdown.Converters.Base;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeeksWithBlogsToMarkdown.Converters
{
    public class HtmlDecodeConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value as string))
            {
                return System.Net.WebUtility.HtmlDecode(value.ToString());
            };
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}