
namespace Bska.Client.UI.Converters
{
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using System;
    using System.Windows.Data;
    using System.Globalization;

    public class MovableAssetStateConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MAssetCurState state = (MAssetCurState)value;
            return state.GetDescription();
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
