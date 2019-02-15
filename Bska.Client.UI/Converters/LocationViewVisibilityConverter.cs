
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    class LocationViewVisibilityConverter :  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = value.ToString();
            if (val == "مصرفی")
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
