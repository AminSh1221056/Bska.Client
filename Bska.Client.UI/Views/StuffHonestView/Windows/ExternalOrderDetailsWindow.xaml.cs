
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for ExternalOrderDetailsWindow.xaml
    /// </summary>
    public partial class ExternalOrderDetailsWindow : Window
    {
        public ExternalOrderDetailsWindow()
        {
            InitializeComponent();
        }

        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
