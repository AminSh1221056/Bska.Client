using Bska.Client.Domain.Entity;
using Bska.Client.UI.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Bska.Client.UI.Converters
{
    public class StuffTreeIconConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tree = value as StuffTreeViewModel;
            var str = tree.StuffCurrent as Stuff;
            FrameworkElement FrameElem = new FrameworkElement();
            return FrameElem.TryFindResource("StuffIcon");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
