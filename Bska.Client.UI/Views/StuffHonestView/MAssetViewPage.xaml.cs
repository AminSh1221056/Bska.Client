using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for MAssetViewPage.xaml
    /// </summary>
    public partial class MAssetViewPage : UserControl
    {
        public MAssetViewPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.FilterTextBox.Focus();
        }
        
        private void BuildingMAssetViewUC_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((MAssetManageViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }

        private void BuildingMAssetViewUC_StrategyTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((MAssetManageViewModel)this.DataContext).StrategySelected = buildingDesign;
        }
    }
}
