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

namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for StoreOrderPage.xaml
    /// </summary>
    public partial class StoreOrderPage : Page
    {
        public StoreOrderPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.internalOrderPane.buildingPersonManage.Visibility = Visibility.Collapsed;
            this.internalOrderPane.storeManage.Visibility = Visibility.Visible;
        }
    }
}
