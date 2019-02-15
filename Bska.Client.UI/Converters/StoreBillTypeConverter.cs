
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Windows.Data;
    using Bska.Client.Common;
    public class StoreBillTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StateOwnership ownerShip = (StateOwnership)value;
            if (ownerShip == StateOwnership.Donation)
            {
                return "اهدائی";
            }
            else if (ownerShip == StateOwnership.Purchase)
            {
                return "خریداری";
            }
            else if (ownerShip == StateOwnership.Trust)
            {
                return "امانی";
            }
            else if (ownerShip == StateOwnership.Owned)
            {
                return "تملیکی";
            }
            else if (ownerShip == StateOwnership.GovCompanyRecived)
            {
                return "انتقالی";
            }
            else return "ناشناخته";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
