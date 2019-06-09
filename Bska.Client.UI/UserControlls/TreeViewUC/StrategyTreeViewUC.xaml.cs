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
    /// Interaction logic for StrategyTreeViewUC.xaml
    /// </summary>
    public partial class StrategyTreeViewUC : UserControl
    {
        public static readonly RoutedEvent StrategyTreeItemSelectEvent = EventManager.RegisterRoutedEvent(
                    "StrategyItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StrategyTreeViewUC));

        public event RoutedEventHandler StrategyTreeItemSelect
        {
            add { AddHandler(StrategyTreeItemSelectEvent, value); }
            remove { RemoveHandler(StrategyTreeItemSelectEvent, value); }
        }

        public StrategyTreeViewUC()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StrategyTreeItemSelectEvent, e.OriginalSource));
        }
    }
}
