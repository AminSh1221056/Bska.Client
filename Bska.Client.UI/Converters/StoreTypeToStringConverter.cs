
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;

    public class StoreTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StoreType val = (StoreType)value;
            if (val == StoreType.Retiring) return "اسقاط";
            else if (val == StoreType.Mixed) return "اصلی";
            else return "نا معتبر";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
