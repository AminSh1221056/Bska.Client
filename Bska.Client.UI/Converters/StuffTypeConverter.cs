
namespace Bska.Client.UI.Converters
{
    using System.Windows.Data;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    
    public class StuffTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string txt = "نا مشخص";
            StuffType val = (StuffType)value;
            switch (val)
            {
                case StuffType.Belonging:
                    txt = "متعلقات";
                    break;
                case StuffType.Installable:
                    txt = "قابل نصب در بنا";
                    break;
                case StuffType.OrderConsumption:
                    txt = "در حکم مصرف";
                    break;
                case StuffType.UnConsumption:
                    txt = "غیر مصرفی";
                    break;
                case StuffType.Consumable:
                    txt = "مصرفی";
                    break;
                default:
                    txt = "نامشخص";
                    break;
            }
            return txt;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StuffToStuffTypeConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string txt = "نا مشخص";
            Stuff val = value as Stuff;
            if (val == null)
                return txt;

            switch (val.StuffType)
            {
                case StuffType.Belonging:
                    txt = "متعلقات";
                    break;
                case StuffType.Installable:
                    txt = "قابل نصب در بنا";
                    break;
                case StuffType.OrderConsumption:
                    txt = "در حکم مصرف";
                    break;
                case StuffType.UnConsumption:
                        txt = "غیر مصرفی";
                    break;
                case StuffType.Consumable:
                    txt = "مصرفی";
                    break;
                default:
                    txt = "نامشخص";
                    break;
            }
            return txt;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
