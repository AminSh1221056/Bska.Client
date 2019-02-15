
namespace Bska.Client.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            int index = 0;
            if (value is ListViewItem)
            {
                var item = (ListViewItem)value;
                var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            }
            else if(value is ListBoxItem)
            {
                var item = (ListBoxItem)value;
                var listbox = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
                index = listbox.ItemContainerGenerator.IndexFromContainer(item) + 1;
            }
            else if(value is DataGridRow)
            {
                var item = (DataGridRow)value;
                index = item.GetIndex() + 1;
            }
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
