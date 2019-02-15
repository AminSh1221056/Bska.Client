using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StoreViewModel;
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
    /// Interaction logic for StoreDesignUserControl.xaml
    /// </summary>
    public partial class StoreDesignUserControl : UserControl
    {
        public StoreDesignUserControl()
        {
            InitializeComponent();
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = sender as TreeView;
            var storeDesign = item.SelectedItem as StoreTreeViewModel;
            if (storeDesign != null)
                ((StoreDesignViewModel)this.DataContext).SelectedNode = storeDesign;
        }
    }
}
