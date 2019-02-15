
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class SellerTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SellerType val = (SellerType)value;
            return val.GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
