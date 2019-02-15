
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bska.Client.UI.Converters
{
    class ExternalOrderDetailsStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (val == 1)
            {
                return "تحویل";
            }
            else if (val ==2)
            {
                return "رد شده";
            }
            else if (val == 3)
            {
                return "در دست اقدام";
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
