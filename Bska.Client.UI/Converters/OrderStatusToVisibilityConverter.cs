
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Windows.Data;
    using Bska.Client.Common;
    using System.Windows;

    public class OrderStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderStatus status = (OrderStatus)value;
            if (status == OrderStatus.SubOrder) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
