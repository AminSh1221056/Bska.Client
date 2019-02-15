
using System.Windows.Controls;
using System.Windows;
namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for RecivedOrderStorePage.xaml
    /// </summary>
    public partial class RecivedOrderStorePage : Page
    {
        public RecivedOrderStorePage()
        {
            InitializeComponent();

            this.globalToolPane1.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolPane1.FilterTextBox.Focus();
        }

        private void borderFilterDetails_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PopUpSelectFilter.IsOpen = true;
        }

        private void pDate2_DateButtonClick(object sender, RoutedEventArgs e)
        {
            PopUpSelectFilter.IsOpen = true;
        }

        private void PopUpSelectFilter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PopUpSelectFilter.IsOpen = false;
        }
    }
}
