
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System.Windows.Data;
    public class ProceedingStateConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ProceedingState proTyp = (ProceedingState)value;
            string val = "ناشناخته";
            switch (proTyp)
            {
                case ProceedingState.None:
                    val = "در دست اقدام";
                    break;
                case ProceedingState.ManagerConfirming:
                    val = "تایید مدیریت";
                    break;
                case ProceedingState.Confirmed:
                    val = "تایید شده";
                    break;
                case ProceedingState.CompletedConfirm:
                    val = "تکمیل - تایید شده";
                    break;
                case ProceedingState.CompletedReject:
                    val = "تکمیل - رد شده";
                    break;
                case ProceedingState.Rejected:
                    val = "رد شده";
                    break;
                case ProceedingState.StoreConfirm:
                    val = "تایید شده - انبار";
                    break;
            }
            return val;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
