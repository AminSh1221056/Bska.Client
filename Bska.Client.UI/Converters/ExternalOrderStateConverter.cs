
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bska.Client.UI.Converters
{
    public class ExternalOrderStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (val == 1)
            {
                return "تایید مدیریت داخلی";
            }
            else if (val == 2)
            {
                return "تایید اداره اموال";
            }
            else if (val == 3)
            {
                return "در دست اقدام";
            }
            else if (val == 4)
            {
                return "تحویل";
            }
            else if (val == 5)
            {
                return "رد شده";
            }
            else
            {
                return "ناشناخته";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
