
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using Bska.Client.Repository.Model;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for OrganMovableAssetHistoryPage.xaml
    /// </summary>
    public partial class OrganMovableAssetHistoryPage : UserControl
    {
        public OrganMovableAssetHistoryPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility =Visibility.Collapsed;
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.FilterTextBox.Focus();
        }

        private void reportbtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new ViewModels.ReportViewModel(true);

            
        }
    }
}
