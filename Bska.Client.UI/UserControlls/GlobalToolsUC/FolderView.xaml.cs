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
    /// Interaction logic for FolderView.xaml
    /// </summary>
    public partial class FolderView : UserControl
    {
        public static readonly RoutedEvent ComboColorSelectionEventChanged = EventManager.RegisterRoutedEvent("ComboColorOrganizChanged", RoutingStrategy.Bubble,
             typeof(RoutedEventHandler), typeof(FolderView));

        public event RoutedEventHandler ComboColorOrganizChanged
        {
            add { AddHandler(ComboColorSelectionEventChanged, value); }
            remove { RemoveHandler(ComboColorSelectionEventChanged, value); }
        }
        public FolderView()
        {
            InitializeComponent();
        }

        private void m_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
                RaiseEvent(new RoutedEventArgs(ComboColorSelectionEventChanged, sender));
        }
    }
}
