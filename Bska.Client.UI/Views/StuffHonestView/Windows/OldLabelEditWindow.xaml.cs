
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for OldLabelEditWindow.xaml
    /// </summary>
    public partial class OldLabelEditWindow : Window
    {
        public OldLabelEditWindow()
        {
            InitializeComponent();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void oldEditWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
