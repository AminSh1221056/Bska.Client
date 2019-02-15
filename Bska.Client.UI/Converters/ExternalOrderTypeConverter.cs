
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bska.Client.UI.Converters
{
    public class ExternalOrderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            string str = "";
            switch (val)
            {
                case 2001:
                    str = "خارجی";
                    break;
                case 2002:
                    str = "انتقال";
                    break;
                default:
                    str = "ناشناخته";
                    break;
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
