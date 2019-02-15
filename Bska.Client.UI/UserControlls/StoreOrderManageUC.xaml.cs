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

namespace Bska.Client.UI.UserControlls
{
    /// <summary>
    /// Interaction logic for StoreOrderManageUC.xaml
    /// </summary>
    public partial class StoreOrderManageUC : UserControl
    {
        public static readonly RoutedEvent StoreTreeViewClickEvent = EventManager.RegisterRoutedEvent(
               "StoreTreeViewClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StoreOrderManageUC));

        public event RoutedEventHandler StoreTreeViewClick
        {
            add { AddHandler(StoreTreeViewClickEvent, value); }
            remove { RemoveHandler(StoreTreeViewClickEvent, value); }
        }

        public StoreOrderManageUC()
        {
            InitializeComponent();
        }

        private void storeTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StoreTreeViewClickEvent, sender));
        }
    }
}
