
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity.MeterBills;
    using Bska.Client.UI.API;
    using Bska.Client.UI.ViewModels.MunitionViewModel;
    using System.Windows.Data;
    using System.Windows.Media;
    public class MeterBillTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MeterBillDetailsViewModel val = value as MeterBillDetailsViewModel;
            if (val == null) return "#FFFE7C22";
            MeterBillTypeColor color = MeterBillTypeColor.FFFE7C22;
            if (val.CurrentEntity is PowerBill)
            {
                color = MeterBillTypeColor.FFFE7C22;
            }
            else if (val.CurrentEntity is GasBill)
            {
                color = MeterBillTypeColor.FFE1B700;
            }
            else if (val.CurrentEntity is TellBill)
            {
                color = MeterBillTypeColor.FFDE4AAD;
            }
            else if (val.CurrentEntity is WaterBill)
            {
                color = MeterBillTypeColor.FF4294DE;
            }
            else if (val.CurrentEntity is MobileBill)
            {
                color = MeterBillTypeColor.FF439D9A;
            }

            return BOT.ParseHexColor("#" + (color).ToString());
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
