
using System.Windows.Controls;

namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for StoreBuyIndentPage.xaml
    /// </summary>
    public partial class StoreBuyIndentPage : Page
    {
        public StoreBuyIndentPage()
        {
            InitializeComponent();
            this.globalToolPane.deletebtn.Visibility = System.Windows.Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = System.Windows.Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = System.Windows.Visibility.Collapsed;
            this.globalToolPane.savebtn.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void borderFilterDetails_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }

        private void pDate1_DateButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }

        private void PopUpSelectFilter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = false;
        }
    }
}
