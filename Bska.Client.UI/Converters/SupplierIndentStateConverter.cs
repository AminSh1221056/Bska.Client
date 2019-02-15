
namespace Bska.Client.UI.Converters
{
    using Common;
    using System.Windows.Data;

    public class SupplierIndentStateConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (SupplierIndentState)value;
            string str = "نامشخص";
            switch (val)
            {
                case SupplierIndentState.Delivery:
                    str = "تحویل";
                    break;
                case SupplierIndentState.Ongoing:
                    str = "در دست اقدام";
                    break;
            }
            return str;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
