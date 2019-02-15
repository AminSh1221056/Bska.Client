
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for MAssetCostWindow.xaml
    /// </summary>
    public partial class MAssetCostWindow : Window
    {
        public MAssetCostWindow()
        {
            InitializeComponent();
        }

        private void mAssetCosttWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
