using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.OrderViewModel;
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

namespace Bska.Client.UI.UserControlls.OrderUC
{
    /// <summary>
    /// Interaction logic for InternalOrderUC.xaml
    /// </summary>
    public partial class InternalOrderUC : UserControl
    {
        public InternalOrderUC()
        {
            InitializeComponent();
            this.stuffviewPopUp.PopUpSelectFilter.PlacementTarget = this.btnStuffFilter;
        }

        private void buildingPersonManage_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((InternalOrderViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }

        private void buildingPersonManage_StrategyTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((InternalOrderViewModel)this.DataContext).StrategySelected = buildingDesign;
        }
        
        private void InternalOrderDetails_UnitTreeClickClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var unit = item.SelectedItem as UnitTreeViewModel;
            if (unit != null)
            {
                this.InternalOrderDetails.cmbunit.SelectedValue = unit.UnitCurrent.UnitId;
            }
        }

        private void storeManage_StoreTreeViewClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var storeTree = item.SelectedItem as StoreTreeViewModel;
            if (storeTree != null)
                ((InternalOrderViewModel)this.DataContext).StoreDesignSelected = storeTree;
        }
        
        private void btnStuffFilter_Click(object sender, RoutedEventArgs e)
        {
            this.stuffviewPopUp.PopUpSelectFilter.IsOpen = true;
        }

        private void StuffTreeViewFilterUC_StuffTreeItemSelect(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as StuffTreeViewModel;
            if (stuffTree != null)
                ((InternalOrderViewModel)this.DataContext).SelectedNode = stuffTree;
        }
    }
}
