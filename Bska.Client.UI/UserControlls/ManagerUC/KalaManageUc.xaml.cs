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

namespace Bska.Client.UI.UserControlls.ManagerUC
{
    /// <summary>
    /// Interaction logic for KalaManageUc.xaml
    /// </summary>
    public partial class KalaManageUc : UserControl
    {
        public KalaManageUc()
        {
            InitializeComponent();
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
