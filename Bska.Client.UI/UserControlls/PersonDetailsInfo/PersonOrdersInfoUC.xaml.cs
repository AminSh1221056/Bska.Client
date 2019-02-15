using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.PersonDetailsInfoViewModels;
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

namespace Bska.Client.UI.UserControlls.PersonDetailsInfo
{
    /// <summary>
    /// Interaction logic for PersonOrdersInfoUC.xaml
    /// </summary>
    public partial class PersonOrdersInfoUC : UserControl
    {
        public PersonOrdersInfoUC()
        {
            InitializeComponent();
        }

        private void BuildingMAssetViewUC1_StrategyTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((PersonOrdersInfoViewModel)this.DataContext).StrategySelected = buildingDesign;
        }

        private void BuildingMAssetViewUC1_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((PersonOrdersInfoViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }
    }
}
