using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccountingViewModels;
using Bska.Client.UI.ViewModels.TreeViewModels;
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

namespace Bska.Client.UI.UserControlls.AccountDocView
{
    /// <summary>
    /// Interaction logic for AccountDocCodingTree.xaml
    /// </summary>
    public partial class AccountDocCodingTree : UserControl
    {
        public AccountDocCodingTree()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = sender as TreeView;
            var Design = item.SelectedItem as AccountCodingTreeViewModel;
            if (Design != null)
                ((AccountCodingDesignViewModel)this.DataContext).SelectedNode = Design;
            else
            {
                var historyDesign = item.SelectedItem as AccountCodingHistoryTreeViewModel;
                if (historyDesign != null)
                {
                    //((AccountDocumentHistoryViewModel)this.DataContext).SelectedNode = historyDesign;
                }
            }
        }
    }
}
