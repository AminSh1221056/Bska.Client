using Bska.Client.UI.ViewModels.GeneralManagerViewModels;
using Bska.Client.UI.ViewModels.OrderViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for RecivedOrderPage.xaml
    /// </summary>
    public partial class RecivedOrderPage : Page
    {
        public RecivedOrderPage()
        {
            InitializeComponent();
            this.orderToolPane.Indentbtn.Visibility = Visibility.Collapsed;
        }

        private void recivedOrderPage_Loaded(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as InternalOrderRecivedViewModel;
            if (context != null)
            {
                this.orderToolPane.personbtn.Visibility = Visibility.Collapsed;
            }
        }

        private void MultiSelectComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var ch = e.OriginalSource as CheckBox;
            if (ch != null)
            {
                var context = this.DataContext as RecivedOrderViewModel;
                if (context != null)
                {
                    context.filterOnRecivedType(ch.Content.ToString(), ch.IsChecked??false);
                }
            }
        }
    }
}
