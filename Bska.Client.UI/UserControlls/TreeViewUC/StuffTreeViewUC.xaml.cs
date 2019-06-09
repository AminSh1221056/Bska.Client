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
    /// Interaction logic for StuffTreeViewUC.xaml
    /// </summary>
    public partial class StuffTreeViewUC : UserControl
    {
        public static readonly RoutedEvent StuffTreeViewItemSelectEvent = EventManager.RegisterRoutedEvent(
              "StuffTreeViewItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StuffTreeViewUC));

        public event RoutedEventHandler StuffTreeViewItemSelect
        {
            add { AddHandler(StuffTreeViewItemSelectEvent, value); }
            remove { RemoveHandler(StuffTreeViewItemSelectEvent, value); }
        }
        
        public StuffTreeViewUC()
        {
            InitializeComponent();
        }

        private void StuffView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StuffTreeViewItemSelectEvent, sender));
        }
    }
}
