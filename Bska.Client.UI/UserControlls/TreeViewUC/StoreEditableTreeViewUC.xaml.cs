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
    /// Interaction logic for StoreEditableTreeViewUC.xaml
    /// </summary>
    public partial class StoreEditableTreeViewUC : UserControl
    {
        public static readonly RoutedEvent StoreTreeEditableItemSelectEvent = EventManager.RegisterRoutedEvent(
                "StoreTreeEditableItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StuffTreeViewFilterUC));

        public event RoutedEventHandler StoreTreeEditableItemSelect
        {
            add { AddHandler(StoreTreeEditableItemSelectEvent, value); }
            remove { RemoveHandler(StoreTreeEditableItemSelectEvent, value); }
        }

        public StoreEditableTreeViewUC()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StoreTreeEditableItemSelectEvent, e.OriginalSource));
        }
    }
}
