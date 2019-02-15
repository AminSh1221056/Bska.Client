using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AssetViewModel;
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
    /// Interaction logic for CommodityUC.xaml
    /// </summary>
    public partial class CommodityUC : UserControl
    {
        public CommodityUC()
        {
            InitializeComponent();
        }

        private void btnUnit_Click(object sender, RoutedEventArgs e)
        {
            PopUpSelectUnit.IsOpen = true;
        }

        private void PopUpSelectUnit_MouseLeave(object sender, MouseEventArgs e)
        {
            PopUpSelectUnit.IsOpen = false;
        }

        private void treeUnit_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = sender as TreeView;
            var unitModel = item.SelectedItem as UnitTreeViewModel;
            if(unitModel!=null)
            ((CommodityViewModel)this.DataContext).UnitId = unitModel.UnitId;
        }
    }
}
