
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;

    public class SubOrderTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SubOrderType val = (SubOrderType)value;
            string str = "";
            switch (val)
            {
                case SubOrderType.Displacement:
                    str = "اعلام مازاد";
                    break;
                case SubOrderType.Store:
                    str = "انبار";
                    break;
                case SubOrderType.Supplier:
                    str = "تدارکات";
                    break;
                case SubOrderType.StoreBillDirect:
                    str = "قبض انبار مستقیم";
                    break;
                case SubOrderType.TenderOffer:
                    str = "مناقصه";
                    break;
                default:
                    str = "ناشناخته";
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
