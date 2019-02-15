
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for MAssetListWindow.xaml
    /// </summary>
    public partial class MAssetListWindow : Window
    {
        public MAssetListWindow()
        {
            InitializeComponent();
        }

        private void mAssetListWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
