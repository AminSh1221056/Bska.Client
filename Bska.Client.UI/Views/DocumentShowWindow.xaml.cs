
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for DocumentShowWindow.xaml
    /// </summary>
    public partial class DocumentShowWindow : Window
    {
        public DocumentShowWindow()
        {
            InitializeComponent();
            this.storeBillPane.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.storeDraftPane.globalToolPane.gridMainBtn.Visibility = Visibility.Collapsed;
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
