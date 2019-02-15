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

namespace Bska.Client.UI.Views.GeneralManagerView
{
    /// <summary>
    /// Interaction logic for IndentReturnRequestPage.xaml
    /// </summary>
    public partial class IndentReturnRequestPage : Page
    {
        public IndentReturnRequestPage()
        {
            InitializeComponent();
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.savebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.FilterTextBox.Focus();
        }
    }
}
