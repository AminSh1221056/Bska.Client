
namespace Bska.Client.UI.Converters
{
    using System;
    using Bska.Client.Common;
    using System.Windows.Data;

    public class OrderStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderStatus status = (OrderStatus)value;
            string statusStr = "";
            switch (status)
            {
                case OrderStatus.Deliviry:
                    statusStr = "تحویل";
                    break;
                case OrderStatus.ManagerConfirm:
                    statusStr = "مدیریت";
                    break;
                case OrderStatus.None:
                    statusStr = "بدون عملیات";
                    break;
                case OrderStatus.OrganizManagerConfirm:
                    statusStr = "در دست اقدام";
                    break;
                case OrderStatus.Reject:
                    statusStr = "رد درخواست";
                    break;
                case OrderStatus.StuffHonest:
                    statusStr = "امین اموال";
                    break;
                case OrderStatus.SubOrder:
                    statusStr = "سفارش";
                    break;
                default:
                    statusStr = "نامشخص";
                    break;
            }

            return statusStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
