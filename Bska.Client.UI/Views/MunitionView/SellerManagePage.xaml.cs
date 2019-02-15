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

namespace Bska.Client.UI.Views.MunitionView
{
    /// <summary>
    /// Interaction logic for SellerManagePage.xaml
    /// </summary>
    public partial class SellerManagePage : Page
    {
        public SellerManagePage()
        {
            InitializeComponent();
            this.sellerUc.globalToolPnae.gridToolsbtn.Visibility = Visibility.Collapsed;
            this.sellerUc.globalToolPnae.cancelbtn.Visibility = Visibility.Collapsed;
            this.sellerUc.globalToolPnae.deletebtn.Visibility = Visibility.Collapsed;
            this.sellerUc.globalToolPnae.editbtn.Visibility = Visibility.Collapsed;
            this.sellerUc.globalToolPnae.FilterTextBox.Focus();
        }
    }
}
