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
    /// Interaction logic for StoreTreeViewUC.xaml
    /// </summary>
    public partial class StoreTreeViewUC : UserControl
    {
        public static readonly RoutedEvent StoreTreeItemSelectEvent = EventManager.RegisterRoutedEvent(
                "StoreTreeItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StuffTreeViewFilterUC));

        public event RoutedEventHandler StoreTreeItemSelect
        {
            add { AddHandler(StoreTreeItemSelectEvent, value); }
            remove { RemoveHandler(StoreTreeItemSelectEvent, value); }
        }

        public StoreTreeViewUC()
        {
            InitializeComponent();
        }

        private void StoreTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StoreTreeItemSelectEvent, e.OriginalSource));
        }
    }
}
