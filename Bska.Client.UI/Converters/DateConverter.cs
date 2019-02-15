
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Windows.Data;
    using Bska.Client.Common;

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                PersianDate val = (PersianDate)value;
                return val.ToString();
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String dateString = value.ToString();
            PersianDate dateresult;
            if (PersianDate.TryParse(dateString, out dateresult))
            {
                return dateresult;
            }
            return value;
        }
    }


    public class PersianDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            DateTime date = (DateTime)value;
            return date.PersianDateTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
