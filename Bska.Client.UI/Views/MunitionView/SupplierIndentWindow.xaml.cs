using Bska.Client.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for SupplierIndentWindow.xaml
    /// </summary>
    public partial class SupplierIndentWindow : Window
    {
        public SupplierIndentWindow()
        {
            InitializeComponent();
            this.orderToolPane.personbtn.Visibility = Visibility.Collapsed;
            this.orderToolPane.analizbtn.Visibility = Visibility.Collapsed;
            this.orderToolPane.rejectbtn.Visibility = Visibility.Collapsed;
            this.orderToolPane.Indentbtn.Visibility = Visibility.Collapsed;
            this.txtNum.Focus();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUnit_Click(object sender, RoutedEventArgs e)
        {
            this.PopUpSelectUnit.IsOpen = true;
        }

        private void treeUnit_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.OriginalSource as TreeView;
            var context = this.DataContext as SupplierIndentViewModel;
            var unit = item.SelectedItem as UnitTreeViewModel;
            if (unit != null)
            {
                context.SelectedUnit = unit.UnitCurrent;
            }
        }

        private void PopUpSelectUnit_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectUnit.IsOpen = false;
        }

        private void borderProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = true;
        }

        private void PopUpSelectProp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = false;
        }
    }
}
