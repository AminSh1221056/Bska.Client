
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for OrderHistoryWindow.xaml
    /// </summary>
    public partial class OrderHistoryWindow : Window
    {
        public OrderHistoryWindow()
        {
            InitializeComponent();
        }

        private void orderHiswindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
