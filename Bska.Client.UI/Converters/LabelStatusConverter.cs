
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Windows.Data;

    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (Boolean)value;
            if (!val) return "/Images/FreeLabel.ico";
            else return "/Images/BusyLabel.ico";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
