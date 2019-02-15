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

namespace Bska.Client.UI.Views.AccountingView
{
    /// <summary>
    /// Interaction logic for AccountDocumentHistoryPage.xaml
    /// </summary>
    public partial class AccountDocumentHistoryPage : Page
    {
        public AccountDocumentHistoryPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
        }

        private void borderFilterDetails_MouseEnter(object sender, MouseEventArgs e)
        {
            PopUpSelectFilter.IsOpen = true;
        }

        private void PopUpSelectFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            PopUpSelectFilter.IsOpen = false;
        }
    }
}
