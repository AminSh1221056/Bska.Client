using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for UserConfigWindow.xaml
    /// </summary>
    public partial class UserConfigWindow : Window
    {
        public UserConfigWindow()
        {
            InitializeComponent();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
