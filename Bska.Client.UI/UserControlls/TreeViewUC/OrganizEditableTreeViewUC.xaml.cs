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

namespace Bska.Client.UI.UserControlls.TreeViewUC
{
    /// <summary>
    /// Interaction logic for OrganizEditableTreeViewUC.xaml
    /// </summary>
    public partial class OrganizEditableTreeViewUC : UserControl
    {
        public static readonly RoutedEvent OrganizTreeEditableItemSelectEvent = EventManager.RegisterRoutedEvent(
                   "OrganizEditableItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OrganizEditableTreeViewUC));

        public event RoutedEventHandler OrganizTreeEditableItemSelect
        {
            add { AddHandler(OrganizTreeEditableItemSelectEvent, value); }
            remove { RemoveHandler(OrganizTreeEditableItemSelectEvent, value); }
        }

        public OrganizEditableTreeViewUC()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(OrganizTreeEditableItemSelectEvent, e.OriginalSource));
        }
    }
}
