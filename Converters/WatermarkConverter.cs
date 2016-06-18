using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GeeksWithBlogsToMarkdown.Converters.Base;
using GeeksWithBlogsToMarkdown.Extensions;

namespace GeeksWithBlogsToMarkdown.Converters
{
    public class WatermarkConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Settings.Instance.ReadSettings();
            var password = Settings.Instance.GWBPassword;//.DecryptString().ToInsecureString();
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
