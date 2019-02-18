using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for IndentWindow.xaml
    /// </summary>
    public partial class IndentWindow : Window
    {
        public IndentWindow()
        {
            InitializeComponent();
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridToolsbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;

            this.storeIndentPane.Visibility = Visibility.Collapsed;
            this.disPlaceMentIndentPane.Visibility = Visibility.Collapsed;
            this.TrenderIndentPane.Visibility = Visibility.Collapsed;
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
            var context = this.DataContext as IndentViewModel;
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

        private void radioListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lst = sender as ListBox;
            if (lst!=null)
            {
                if (lst.SelectedIndex == 0 || lst.SelectedIndex==1)
                {
                    this.storeIndentPane.Visibility = Visibility.Visible;
                    this.disPlaceMentIndentPane.Visibility = Visibility.Collapsed;
                    this.TrenderIndentPane.Visibility = Visibility.Collapsed;
                }
                else if (lst.SelectedIndex == 2)
                {
                    this.storeIndentPane.Visibility = Visibility.Collapsed;
                    this.disPlaceMentIndentPane.Visibility = Visibility.Collapsed;
                    this.TrenderIndentPane.Visibility = Visibility.Visible;
                }
                else if (lst.SelectedIndex == 3)
                {
                    this.storeIndentPane.Visibility = Visibility.Collapsed;
                    this.disPlaceMentIndentPane.Visibility = Visibility.Visible;
                    this.TrenderIndentPane.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
