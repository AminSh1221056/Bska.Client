
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using System.Windows.Media;

    public class StyffTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StuffType val = (StuffType)value;
            int sTypeNo = System.Convert.ToInt32(val);
            return BOT.ParseHexColor("#" + ((StuffTypeColor)sTypeNo).ToString());
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
    public class OrderStatusToColorConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderStatus val = (OrderStatus)value;
            int statusNo = System.Convert.ToInt32(val);
            return BOT.ParseHexColor("#" + ((OrderStatusColor)statusNo).ToString());
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SubOrderStatusToColorConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SubOrderState val = (SubOrderState)value;
            int statusNo = System.Convert.ToInt32(val);
            return BOT.ParseHexColor("#" + ((OrderStatusColor)statusNo).ToString());
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
