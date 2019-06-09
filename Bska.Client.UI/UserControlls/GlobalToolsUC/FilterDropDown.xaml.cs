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
    /// Interaction logic for FilterDropDown.xaml
    /// </summary>
    public partial class FilterDropDown : UserControl
    {
        public static readonly RoutedEvent FilterButtonSelectionEventChanged = EventManager.RegisterRoutedEvent("FilterButtonSelectionEventChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(FilterDropDown));

        public event RoutedEventHandler FilterButtonChanged
        {
            add { AddHandler(FilterButtonSelectionEventChanged, value); }
            remove { RemoveHandler(FilterButtonSelectionEventChanged, value); }
        }

        public FilterDropDown()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button)
            RaiseEvent(new RoutedEventArgs(FilterButtonSelectionEventChanged, sender));
        }
    }
}
