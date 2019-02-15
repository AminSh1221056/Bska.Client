using Bska.Client.UI.ViewModels.TreeViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for StuffInformationPage.xaml
    /// </summary>
    public partial class StuffInformationPage : UserControl
    {
        public StuffInformationPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
        }
        
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            switch (cmb.SelectedIndex)
            {
                case 0:
                    this.realStufflst.Visibility = Visibility.Visible;
                    this.massetlst.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    this.realStufflst.Visibility = Visibility.Collapsed;
                    this.massetlst.Visibility = Visibility.Visible;
                    break;
                default:
                    this.realStufflst.Visibility = Visibility.Collapsed;
                    this.massetlst.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void StuffView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as KalaManageTreeViewModel;
            if (stuffTree != null)
                ((StuffInformationViewModel)this.DataContext).SelectedNode = stuffTree;
        }
    }
}
