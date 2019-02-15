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

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for OrderManagePage.xaml
    /// </summary>
    public partial class OrderManagePage : Page
    {
        public OrderManagePage()
        {
            InitializeComponent();
            Dictionary<int, string> EditTypes = new Dictionary<int, string> { { 1, "درخواست های داده شده" }, { 2, "درخواست های مدیریت شده" }, { 3, "درخواست های در راه" } };
            this.EditTypeDropDown.SourceContoller.ItemsSource = EditTypes;
            this.globalTollPane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.gr1.Visibility = Visibility.Collapsed;
            this.globalTollPane.gridsearchbtn.Visibility = Visibility.Collapsed;
        }

        private void EditTypeDropDown_FilterButtonChanged(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            string editType = btn.Content.ToString();
            EditTypeDropDown.popupToggle.Content = editType;
            EditTypeDropDown.Popup.IsOpen = false;
            this.gr1.Visibility = Visibility.Visible;
            var context = this.DataContext as OrderManageViewModel;
            if (string.Equals(editType, "درخواست های داده شده", StringComparison.InvariantCulture))
            {
                context.EditType = 1;
            }
            else if (string.Equals(editType, "درخواست های مدیریت شده", StringComparison.InvariantCulture))
            {
                context.EditType = 2;
            }
            else
            {
                context.EditType = 3;
            }
        }

        private void BuildingMAssetViewUC_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((OrderManageViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }

        private void BuildingMAssetViewUC_StrategyTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((OrderManageViewModel)this.DataContext).StrategySelected = buildingDesign;
        }
    }
}
