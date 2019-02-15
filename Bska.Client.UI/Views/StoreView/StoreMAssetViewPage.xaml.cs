using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StoreViewModel;
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
    /// Interaction logic for StoreMAssetViewPage.xaml
    /// </summary>
    public partial class StoreMAssetViewPage : Page
    {
        public StoreMAssetViewPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.FilterTextBox.Focus();
        }
        private void borderFilterDetails_MouseEnter(object sender, MouseEventArgs e)
        {
            PopUpSelectFilter.IsOpen = true;
        }

        private void PopUpSelectFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            PopUpSelectFilter.IsOpen = false;
        }

        private void storeManageUc_StoreTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var storeDesign = item.SelectedItem as StoreTreeViewModel;
            if (storeDesign != null)
                ((StoreMAssetManageViewModel)this.DataContext).StoreDesignSelected = storeDesign;
        }

        private void pDate1_DateButtonClick(object sender, RoutedEventArgs e)
        {
            PopUpSelectFilter.IsOpen = true;
        }
    }
}
