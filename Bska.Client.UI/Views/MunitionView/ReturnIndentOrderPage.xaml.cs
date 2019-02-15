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
    /// Interaction logic for ReturnIndentOrderPage.xaml
    /// </summary>
    public partial class ReturnIndentOrderPage : Page
    {
        public ReturnIndentOrderPage()
        {
            InitializeComponent();
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.reportbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.FilterTextBox.Focus();
        }
    }
}
