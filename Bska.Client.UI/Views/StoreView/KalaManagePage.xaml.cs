using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StoreViewModel;
using Bska.Client.UI.ViewModels.TreeViewModels;
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
    /// Interaction logic for KalaManagePage.xaml
    /// </summary>
    public partial class KalaManagePage : Page
    {
        public KalaManagePage()
        {
            InitializeComponent();
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
        }

        private void StuffTreeViewUC_StuffTreeViewItemSelect(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as StuffTreeViewModel;
            if (stuffTree != null)
                ((KalaManageViewModel)this.DataContext).SelectedNode = stuffTree;
        }
    }
}
