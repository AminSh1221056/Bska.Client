
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for AddExternalOrderWindow.xaml
    /// </summary>
    public partial class AddExternalOrderWindow : Window
    {
        public AddExternalOrderWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
