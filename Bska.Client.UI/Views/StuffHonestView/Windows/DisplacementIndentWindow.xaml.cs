
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for DisplacementIndentWindow.xaml
    /// </summary>
    public partial class DisplacementIndentWindow : Window
    {
        public DisplacementIndentWindow()
        {
            InitializeComponent();
        }

        private void indentWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
