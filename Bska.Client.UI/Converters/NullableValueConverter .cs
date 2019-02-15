
namespace Bska.Client.UI.Converters
{
    using Common;
    using System;
    using System.Windows.Data;
    public class NullableDecimalValueConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;
            else
                return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;

            decimal result;
            if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, out result))
            {
                return result;
            }

            return null;
        }
    }

    public class NullableIntValueConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;
            else
                return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;

            int result;
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out result))
            {
                return result;
            }

            return null;
        }
    }

    public class NullablePersianDateTimeValueConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return default(DateTime?);
            else
            {
                DateTime val = (DateTime)value;
                return val.PersianDateString();
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            
            if (!string.IsNullOrWhiteSpace(s))
            {
                PersianDate pdt;
                PersianDate.TryParse(s, out pdt);
                return pdt.ToDateTime();
            }

            return null;
        }
    }
}
