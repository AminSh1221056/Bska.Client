using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.StoreViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for StoreIndentConfirmWindow.xaml
    /// </summary>
    public partial class StoreIndentConfirmWindow : Window
    {
        public StoreIndentConfirmWindow()
        {
            InitializeComponent();
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridToolsbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.stroDocToolPane.billBtn.Visibility = Visibility.Collapsed;
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void storeTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = sender as TreeView;
            var storeDesign = item.SelectedItem as StoreTreeViewModel;
            if (storeDesign != null)
                ((StoreIndentViewModel)this.DataContext).StoreDesignSelected = storeDesign;
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
