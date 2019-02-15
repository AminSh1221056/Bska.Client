
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;
    using System.Windows;

    public class LocationStautsConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (LocationStatus)value;
            FrameworkElement FrameElem = new FrameworkElement();
            if (status == LocationStatus.Executive)
            {
                return FrameElem.TryFindResource("ExecutiveIcon");
            }
            else if (status == LocationStatus.Active)
            {
                return FrameElem.TryFindResource("ActiveIcon");
            }
            else if (status == LocationStatus.DeActive)
            {
                return FrameElem.TryFindResource("DeactiveIcon");
            }
            else if (status == LocationStatus.MovedRequest)
            {
                return FrameElem.TryFindResource("MovedRequestIcon");
            }
            else if (status == LocationStatus.StoreActive)
            {
                return FrameElem.TryFindResource("StoreActiveIcon");
            }
            else if (status == LocationStatus.StoreDeActive)
            {
                return FrameElem.TryFindResource("StoreDeactiveIcon");
            }
            else if (status == LocationStatus.Trust || status==LocationStatus.RefundTrust)
            {
                return FrameElem.TryFindResource("TrustLocation");
            }
            else if (status == LocationStatus.TrustDeActive)
            {
                return FrameElem.TryFindResource("TrustDeActiveLocation");
            }
            else if (status == LocationStatus.Accident)
            {
                return FrameElem.TryFindResource("AccidentLocation");
            }
            else if (status == LocationStatus.AccidentDeActive)
            {
                return FrameElem.TryFindResource("AccidentDeActiveLocation");
            }
            else if (status == LocationStatus.Retiring)
            {
                return FrameElem.TryFindResource("RetiringLocation");
            }
            else if (status == LocationStatus.RetiringDeActive)
            {
                return FrameElem.TryFindResource("RetiringDeActiveLocation");
            }
            else if (status == LocationStatus.Sale)
            {
                return FrameElem.TryFindResource("SaleLocation");
            }
            else if (status == LocationStatus.Transfer)
            {
                return FrameElem.TryFindResource("TransferLocation");
            }
            else if (status == LocationStatus.TransferState)
            {
                return FrameElem.TryFindResource("StateTransferLocation");
            }
            else if (status == LocationStatus.Delete)
            {
                return FrameElem.TryFindResource("DeleteLocation");
            }
            else if (status == LocationStatus.Send)
            {
                return FrameElem.TryFindResource("sendLocationIcon");
            }
            else
            {
                return FrameElem.TryFindResource("UnknownLocationIcon");
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
