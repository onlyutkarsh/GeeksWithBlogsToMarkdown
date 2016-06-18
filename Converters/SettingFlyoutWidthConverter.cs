using GeeksWithBlogsToMarkdown.Converters.Base;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeeksWithBlogsToMarkdown.Converters
{
    public class SettingFlyoutWidthConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mainWindowWidth;
            if (value != null && int.TryParse(value.ToString(), out mainWindowWidth))
            {
                return mainWindowWidth * 0.7;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}