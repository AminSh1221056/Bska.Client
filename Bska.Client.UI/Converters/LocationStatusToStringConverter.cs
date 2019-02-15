
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;

    public class LocationStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (LocationStatus)value;
            if (status == LocationStatus.Executive)
            {
                return "مال وارد سیستم شده است";
            }
            else if (status == LocationStatus.Active)
            {
                return "مال هم اکنون در این منطقه می باشد";
            }
            else if (status == LocationStatus.DeActive)
            {
                return "مال قبلا در این منطقه بوده است";
            }
            else if (status == LocationStatus.MovedRequest)
            {
                return "درخواست ارجاء برای مال صادر شده است";
            }
            else if (status == LocationStatus.StoreActive)
            {
                return "مال هم اکنون در انبار است";
            }
            else if (status == LocationStatus.StoreDeActive)
            {
                return "مال قبلا در انبار بوده است";
            }
            else if (status == LocationStatus.Trust)
            {
                return "مال به امانت داده شده است";
            }
            else if (status == LocationStatus.Accident || status==LocationStatus.AccidentDeActive)
            {
                return "برای این مال حادثه ثبت شده است";
            }
            else if (status == LocationStatus.Retiring)
            {
                return "مال به انبار اسقاط فرستاده شده است";
            }
            else if (status == LocationStatus.RetiringDeActive)
            {
                return "مال قبلا در انبار اسقاط بوده است";
            }
            else if (status == LocationStatus.Sale)
            {
                return "صورت جلسه فروش برای مال ثبت شده است";
            }
            else if (status == LocationStatus.Transfer)
            {
                return "مال انتقال داده شده است";
            }
            else if (status == LocationStatus.TransferState)
            {
                return "مال به سازمان داخلی انتقال داده شده است";
            }
            else if (status == LocationStatus.Delete)
            {
                return "مال از موجودی حذف شده است";
            }
            else if (status == LocationStatus.Send)
            {
                return "مال به خارج از اداره منتقل شده است";
            }
            else if (status == LocationStatus.TrustDeActive)
            {
                return "بازگشت از امانی";
            }
            else if (status == LocationStatus.RefundTrust)
            {
                return "استرداد امانی";
            }
            else
            {
                return "نا مشخص";
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
