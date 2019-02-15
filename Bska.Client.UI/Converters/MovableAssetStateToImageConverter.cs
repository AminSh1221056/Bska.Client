
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using System;
    using System.Windows;
    using System.Windows.Data;
    public class MovableAssetStateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int status =(int)(MAssetCurState)value;
            FrameworkElement FrameElem = new FrameworkElement();
            if (status>=15001 && status <15005)
            {
                return FrameElem.TryFindResource("ActiveStateIcon");
            }
            else if ((status >= 15005 && status < 15011) || (status >= 15016 && status < 15019) || status==15022)
            {
                return FrameElem.TryFindResource("LicencingStateIcon");
            }
            else if ((status >= 15011 && status < 15016) || (status >= 15019 && status < 15022) || status == 15023)
            {
                return FrameElem.TryFindResource("DeletedStateIcon");
            }
            else
            {
                return FrameElem.TryFindResource("UnknownLocationIcon");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
