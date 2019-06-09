using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
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

namespace Bska.Client.UI.UserControlls
{
    /// <summary>
    /// Interaction logic for StoreInsertUC.xaml
    /// </summary>
    public partial class StoreInsertUC : UserControl
    {
        public StoreInsertUC()
        {
            InitializeComponent();
        }


        private void StoreTreeViewUC_StoreTreeItemSelect(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var storeDesign = item.SelectedItem as StoreTreeViewModel;
            if (storeDesign != null)
                ((InitialMAssetViewModel)this.DataContext).StoreDesignSelected = storeDesign;
        }
    }
}
