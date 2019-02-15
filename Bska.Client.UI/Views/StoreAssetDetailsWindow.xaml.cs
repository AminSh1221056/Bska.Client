using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for StoreAssetDetailsWindow.xaml
    /// </summary>
    public partial class StoreAssetDetailsWindow : Window
    {
        public StoreAssetDetailsWindow()
        {
            InitializeComponent();
        }

        private void storeAssetDetailswindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void borderFilterDetails_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }

        private void pDate1_DateButtonClick(object sender, RoutedEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }

        private void pDate2_DateButtonClick(object sender, RoutedEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = true;
        }

        private void PopUpSelectFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectFilter.IsOpen = false;
        }
    }
}
