
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using System.Windows.Data;
    using System;
    public class ProceedingExchangeStaterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int32 val = (int)value;
            if (val == 2)
            {
                return "تایید شده";
            }
            else if (val ==3)
            {
                return "رد شده";
            }
            else if (val == 4)
            {
                return "تکمیل - تایید شده";
            }
            else if (val == 5)
            {
                return "تکمیل - رد شده";
            }
            else
            {
                return "در درست اقدام";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProceedingExchangeTypeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = (int)value;
            return ((ProceedingsType)val).GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
