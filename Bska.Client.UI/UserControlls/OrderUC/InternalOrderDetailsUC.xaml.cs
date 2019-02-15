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

namespace Bska.Client.UI.UserControlls.OrderUC
{
    /// <summary>
    /// Interaction logic for InternalOrderDetailsUC.xaml
    /// </summary>
    public partial class InternalOrderDetailsUC : UserControl
    {
        public static readonly RoutedEvent UnitTreeClickEvent = EventManager.RegisterRoutedEvent(
       "UnitTreeClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InternalOrderDetailsUC));

        public event RoutedEventHandler UnitTreeClickClick
        {
            add { AddHandler(UnitTreeClickEvent, value); }
            remove { RemoveHandler(UnitTreeClickEvent, value); }
        }
        public InternalOrderDetailsUC()
        {
            InitializeComponent();
        }

        private void PopUpSelectUnit_MouseLeave(object sender, MouseEventArgs e)
        {
            PopUpSelectUnit.IsOpen = false;
        }

        private void btnUnit_Click(object sender, RoutedEventArgs e)
        {
            PopUpSelectUnit.IsOpen = true;
        }
        private void treeUnit_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedEventArgs(UnitTreeClickEvent, sender));
        }
    }
}
