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
    /// Interaction logic for StuffTreeViewFilterUC.xaml
    /// </summary>
    public partial class StuffTreeViewFilterUC : UserControl
    {
        public static readonly RoutedEvent StuffTreeItemSelectEvent = EventManager.RegisterRoutedEvent(
              "StuffTreeItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StuffTreeViewFilterUC));

        public event RoutedEventHandler StuffTreeItemSelect
        {
            add { AddHandler(StuffTreeItemSelectEvent, value); }
            remove { RemoveHandler(StuffTreeItemSelectEvent, value); }
        }

        public StuffTreeViewFilterUC()
        {
            InitializeComponent();
        }

        private void PopUpSelectFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = false;
        }

        private void StuffTreeViewUC_StuffTreeItemSelect(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(StuffTreeItemSelectEvent, e.OriginalSource));
        }
    }
}
