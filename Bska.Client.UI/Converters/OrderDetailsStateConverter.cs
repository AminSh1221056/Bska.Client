
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System.Windows.Data;
    public class OrderDetailsStateConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OrderDetailsState state = (OrderDetailsState)value;
            string str = "نامشخص";
            if (state == OrderDetailsState.ManagerConfirm)
            {
                str = "مدیریت";
            }
            else if (state == OrderDetailsState.OrganizManagerConfirm)
            {
                str = "اداری";
            }
            else if (state == OrderDetailsState.StuffHonest)
            {
                str = "امین اموال";
            }
            else if (state == OrderDetailsState.SubOrder)
            {
                str = "سفارش";
            }
            else if (state == OrderDetailsState.Deliviry)
            {
                str = "تحویل";
            }
            else if (state == OrderDetailsState.ToOther)
            {
                str = "انتقال به دیگری";
            }
            else
            {
                str = "نامشخص";
            }
            return str;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
