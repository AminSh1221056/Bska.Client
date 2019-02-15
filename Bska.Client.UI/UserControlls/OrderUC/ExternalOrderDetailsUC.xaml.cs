using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.OrderViewModel;
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

namespace Bska.Client.UI.UserControlls.OrderUC
{
    /// <summary>
    /// Interaction logic for ExternalOrderDetailsUC.xaml
    /// </summary>
    public partial class ExternalOrderDetailsUC : UserControl
    {
        public ExternalOrderDetailsUC()
        {
            InitializeComponent();
        }

        private void btnStuffFilter_Click(object sender, RoutedEventArgs e)
        {
            this.stuffviewPopUp.PopUpSelectFilter.IsOpen = true;
        }

        private void stuffviewPopUp_StuffTreeItemSelect(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as StuffTreeViewModel;
            if (stuffTree != null)
                ((AddExternalOrderViewModel)this.DataContext).SelectedNode = stuffTree;
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
            ((AddExternalOrderViewModel)this.DataContext).UnitId = unitModel.UnitId;
        }
    }
}
