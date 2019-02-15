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
using System.Windows.Shapes;

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow()
        {
            InitializeComponent();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void borderProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = true;
        }

        private void PopUpSelectProp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen =false;
        }

        private void orderDetails_UnitTreeClickClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var unit = item.SelectedItem as UnitTreeViewModel;
            if (unit != null)
            {
                this.orderDetails.cmbunit.SelectedValue = unit.UnitCurrent.UnitId;
            }
        }
    }
}
