using GeeksWithBlogsToMarkdown.Converters.Base;
using GeeksWithBlogsToMarkdown.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GeeksWithBlogsToMarkdown.Converters
{
    public class WatermarkConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Settings.Instance.ReadSettings();
            var password = Settings.Instance.GWBPassword.DecryptString().ToInsecureString();
            if (!string.IsNullOrWhiteSpace(password))
                return "\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF\u25CF";
            return "Password is empty.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}