using Bska.Client.UI.ViewModels;
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
    /// Interaction logic for BuildingDesignUserControl.xaml
    /// </summary>
    public partial class BuildingDesignUserControl : UserControl
    {
        public BuildingDesignUserControl()
        {
            InitializeComponent();
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = sender as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
              ((OrganizationDesignViewModel)this.DataContext).SelectedNode = buildingDesign;
        }
    }
}
