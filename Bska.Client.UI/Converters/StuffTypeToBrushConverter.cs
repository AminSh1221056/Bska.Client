
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using System.Windows.Media;

    public class StuffTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StuffType val = (StuffType)value;
            int sTypeNo = System.Convert.ToInt32(val);
            SolidColorBrush alphaBrush = new SolidColorBrush(BOT.ParseHexColor("#" + ((StuffTypeColor)sTypeNo).ToString()));
            return alphaBrush;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class OrderStatusToBrushConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderStatus val = (OrderStatus)value;
            int orderNo = System.Convert.ToInt32(val);
            SolidColorBrush alphaBrush = new SolidColorBrush(BOT.ParseHexColor("#" + ((OrderStatusColor)orderNo).ToString()));
            return alphaBrush;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SubOrderStatusToBrushConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SubOrderState val = (SubOrderState)value;
            int orderNo = System.Convert.ToInt32(val);
            SolidColorBrush alphaBrush = new SolidColorBrush(BOT.ParseHexColor("#" + ((OrderStatusColor)orderNo).ToString()));
            return alphaBrush;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
