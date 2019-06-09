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
    /// Interaction logic for StrategyEditableTreeViewUC.xaml
    /// </summary>
    public partial class StrategyEditableTreeViewUC : UserControl
    {
        public static readonly RoutedEvent StrategyTreeEditableItemSelectEvent = EventManager.RegisterRoutedEvent(
                 "StrategyEditableItemSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StrategyEditableTreeViewUC));

        public event RoutedEventHandler StrategyTreeEditableItemSelect
        {
            add { AddHandler(StrategyTreeEditableItemSelectEvent, value); }
            remove { RemoveHandler(StrategyTreeEditableItemSelectEvent, value); }
        }

        public StrategyEditableTreeViewUC()
        {
            InitializeComponent();
        }

        private void TreeStrategy_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StrategyTreeEditableItemSelectEvent, e.OriginalSource));
        }
    }
}
