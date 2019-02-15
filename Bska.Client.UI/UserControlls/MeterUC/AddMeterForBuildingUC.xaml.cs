using Bska.Client.Domain.Entity.AssetEntity.Meters;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.MetersViewModels;
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

namespace Bska.Client.UI.UserControlls.MetersUC
{
    /// <summary>
    /// Interaction logic for AddMeterForBuildingUC.xaml
    /// </summary>
    public partial class AddMeterForBuildingUC : UserControl
    {
        public AddMeterForBuildingUC()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = sender as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((AddMeterForBuildingViewModel)this.DataContext).SelectedNode = buildingDesign;
        }
    }
}
