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

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for UploadArrivallPage.xaml
    /// </summary>
    public partial class UploadArrivallPage : UserControl
    {
        public UploadArrivallPage()
        {
            InitializeComponent();
            this.globalToolPane.gridToolsbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;

            this.importToolPane.excelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.FilterTextBox.Focus();
        }

        private void BuildingPersonManageUC_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((UploadAccessFileViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }

        private void BuildingPersonManageUC_StrategyTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((UploadAccessFileViewModel)this.DataContext).StrategySelected = buildingDesign;
        }
    }
}
