
namespace Bska.Client.UI.Converters
{
    using Domain.Entity;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using ViewModels;

    public class StrategyTreeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tree = value as EmployeeDesignTreeViewModel;
            var str = tree.BuildingDesignCurrent as StrategyDesign;
            FrameworkElement FrameElem = new FrameworkElement();
            if (str != null)
            {
                if(str.Building!=null) return FrameElem.TryFindResource("UserInfo");
                else return FrameElem.TryFindResource("TreeviewIcon");
            }
            return FrameElem.TryFindResource("TreeviewIcon");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
