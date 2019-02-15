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
    /// Interaction logic for MunitionRecivedMonagheseIndent.xaml
    /// </summary>
    public partial class RecivedTrenderOfferPage : Page
    {
        public RecivedTrenderOfferPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
        }

        private void borderFilterDetails_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }

        private void PopUpSelectFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = false;
        }

        private void pDate2_DateButtonClick(object sender, RoutedEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }
    }
}
