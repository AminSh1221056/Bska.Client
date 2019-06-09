using Bska.Client.Common;
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
    /// Interaction logic for StoreReciptUC.xaml
    /// </summary>
    public partial class StoreReciptUC : UserControl
    {
        public static readonly RoutedEvent CmbSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "CmbSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StoreReciptUC));

        // Raise an event when the filter is reset
        public event RoutedEventHandler CmbSelectionChanged
        {
            add { AddHandler(CmbSelectionChangedEvent, value); }
            remove { RemoveHandler(CmbSelectionChangedEvent, value); }
        }

        public StoreReciptUC()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CmbSelectionChangedEvent,sender));
        }
        
    }
}
