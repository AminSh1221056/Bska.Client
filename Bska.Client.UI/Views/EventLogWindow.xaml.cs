
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for EventLoqWindow.xaml
    /// </summary>
    public partial class EventLogWindow : Window
    {
        public EventLogWindow()
        {
            InitializeComponent();
        }

        private void eventLogWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
