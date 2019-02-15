
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System;
    using System.Windows.Data;

    public class EstateTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EstateType type = (EstateType)value;
            if (type == EstateType.Building)
            {
                return "ساختمان";
            }
            else if (type == EstateType.FarmLand)
            {
                return "مزرعه";
            }
            else if (type == EstateType.Garden)
            {
                return "باغ";
            }
            else
            {
                return "ناشناخته";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
