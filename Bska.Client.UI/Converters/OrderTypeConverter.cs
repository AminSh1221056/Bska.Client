
namespace Bska.Client.UI.Converters
{
    using System;
    using Bska.Client.Common;
    using System.Windows.Data;

    public class OrderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderType orderType = (OrderType)value;
            string ordertypeStr = "";
            switch (orderType)
            {
                case OrderType.Displacement:
                    ordertypeStr = "اعلام مازاد";
                    break;
                case OrderType.InternalRequest:
                    ordertypeStr = "داخلی";
                    break;
                case OrderType.Procceding:
                    ordertypeStr = "صورت جلسه";
                    break;
                case OrderType.Store:
                    ordertypeStr = "انبار";
                    break;
                case OrderType.InternalTransfer:
                    ordertypeStr = "انتقال داخلی";
                    break;
                default:
                    ordertypeStr = "نامشخص";
                    break;
            }

            return ordertypeStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
