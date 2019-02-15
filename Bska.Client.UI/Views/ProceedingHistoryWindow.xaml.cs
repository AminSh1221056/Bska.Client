
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for ProceedingHistoryWindow.xaml
    /// </summary>
    public partial class ProceedingHistoryWindow : Window
    {
        public ProceedingHistoryWindow()
        {
            InitializeComponent();
        }

        private void proceedingHistoryWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
