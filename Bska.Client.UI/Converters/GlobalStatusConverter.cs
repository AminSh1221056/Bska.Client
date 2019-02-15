
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    class GlobalStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GlobalRequestStatus val = (GlobalRequestStatus)value;
            string str = "";
            switch (val)
            {
                case GlobalRequestStatus.Confirmed:
                    str = "تایید شده";
                    break;
                case GlobalRequestStatus.Rejected:
                    str = "رد شده";
                    break;
                case GlobalRequestStatus.Pending:
                    str = "در دست اقدام";
                    break;
                case GlobalRequestStatus.Completed:
                    str = "تکمیل شده";
                    break;
                default:
                    break;
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
