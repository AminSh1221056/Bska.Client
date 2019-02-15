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
    /// Interaction logic for DisplacementOrderUC.xaml
    /// </summary>
    public partial class DisplacementOrderUC : UserControl
    {
        public DisplacementOrderUC()
        {
            InitializeComponent();
        }

        private void buildingPersonManage_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((DisplacementOrderViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }
    }
}
