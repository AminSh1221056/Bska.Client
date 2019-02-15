
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System.Windows.Data;
    public class SubOrderStateConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (SubOrderState)value;
            string str = "نامشخص";
            switch (val)
            {
                case SubOrderState.Confirm:
                    str = "تایید شده";
                    break;
                case SubOrderState.Reject:
                    str = "رد شده";
                    break;
                case SubOrderState.Deliviry:
                    str = "تحویل";
                    break;
                case SubOrderState.None:
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
