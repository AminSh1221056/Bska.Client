
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System.Windows.Data;
    using System;
    using System.Globalization;

    public class CompietionStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strState = "";
            CompietionState cState = (CompietionState)value;
            if (cState == CompietionState.Reported)
            {
                strState = "ارسال شده";
            }
            else if (cState == CompietionState.Reporting)
            {
                strState = "در حال ارسال";
            }
            else
            {
                strState = "ارسال نشده";
            }
            return strState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
