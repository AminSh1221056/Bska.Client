
using System.Windows.Controls;
using System.Windows;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for OrderManagePage.xaml
    /// </summary>
    public partial class OrderManagePage : Page
    {
        public OrderManagePage()
        {
            InitializeComponent();
            this.orderToolPane.confirmbtn.Visibility =Visibility.Collapsed;
            this.orderToolPane.rejectbtn.Visibility =Visibility.Collapsed;
        }

        private void MultiSelectComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var ch = e.OriginalSource as CheckBox;
            if (ch != null)
            {
                var context = this.DataContext as OrderManageViewModel;
                if (context != null)
                {
                    context.filterOnRecivedType(ch.Content.ToString(), ch.IsChecked ?? false);
                }
            }
        }
    }
}
