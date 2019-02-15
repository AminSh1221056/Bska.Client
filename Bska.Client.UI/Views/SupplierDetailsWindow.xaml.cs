using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for SupplierDetailsWindow.xaml
    /// </summary>
    public partial class SupplierDetailsWindow : Window
    {
        public SupplierDetailsWindow()
        {
            InitializeComponent();
        }

        private void supplierWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
