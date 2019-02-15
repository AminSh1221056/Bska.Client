
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System.Windows.Data;

    public sealed class ProceedingTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "ناشناخته";

            ProceedingsType typ = (ProceedingsType)value;
            string str = "";
            switch (typ)
            {
                case ProceedingsType.Accident:
                    str = "حادثه-تصادف";
                    break;
                case ProceedingsType.AssetRetiring:
                    str = "اسقاط";
                    break;
                case ProceedingsType.BudgetLicencing:
                    str = "تبصرهای بودجه";
                    break;
                case ProceedingsType.DefinitiveTransfer:
                    str = "انتقال قطعی";
                    break;
                case ProceedingsType.Delete:
                    str = "حذف";
                    break;
                case ProceedingsType.Earthquake:
                    str = "حادثه-زلزله";
                    break;
                case ProceedingsType.Fire:
                    str = "حادثه-آتش سوزی";
                    break;
                case ProceedingsType.Flood:
                    str = "حادثه-سیل";
                    break;
                case ProceedingsType.ReturnFromTrust:
                    str = "بازگشت امانی";
                    break;
                case ProceedingsType.Sale:
                    str = "فروش";
                    break;
                case ProceedingsType.SpecialLicencing:
                    str = "مقررات ویژه";
                    break;
                case ProceedingsType.StateTransfer:
                    str = "انتقال درون سازمانی";
                    break;
                case ProceedingsType.Theft:
                    str = "حادثه-سرقت";
                    break;
                case ProceedingsType.TrustTransfer:
                    str = "انتقال امانی";
                    break;
                case ProceedingsType.RefundTrust:
                    str = "استرداد امانی";
                    break;
                case ProceedingsType.EditRequest:
                    str = "ویرایش مال";
                    break;
                case ProceedingsType.ReturnFromRetiring:
                    str = "بازگشت از اسقاط";
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
