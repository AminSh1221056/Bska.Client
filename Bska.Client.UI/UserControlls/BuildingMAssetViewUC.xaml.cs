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
    /// Interaction logic for BuildingMAssetViewUC.xaml
    /// </summary>
    public partial class BuildingMAssetViewUC : UserControl
    {
        public BuildingMAssetViewUC()
        {
            InitializeComponent();
        }
        public static readonly RoutedEvent OrganizTreeClickEvent = EventManager.RegisterRoutedEvent(
          "OrganizTreeClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BuildingMAssetViewUC));

        public static readonly RoutedEvent StrategyTreeClickEvent = EventManager.RegisterRoutedEvent(
           "StrategyTreeClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BuildingMAssetViewUC));

        public event RoutedEventHandler OrganizTreeClick
        {
            add { AddHandler(OrganizTreeClickEvent, value); }
            remove { RemoveHandler(OrganizTreeClickEvent, value); }
        }

        public event RoutedEventHandler StrategyTreeClick
        {
            add { AddHandler(StrategyTreeClickEvent, value); }
            remove { RemoveHandler(StrategyTreeClickEvent, value); }
        }
        private void organizTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(OrganizTreeClickEvent, sender));
        }

        private void StrategyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(StrategyTreeClickEvent, sender));
        }
    }
}
