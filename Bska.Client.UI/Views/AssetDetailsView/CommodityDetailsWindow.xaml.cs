using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for CommodityDetailsWindow.xaml
    /// </summary>
    public partial class CommodityDetailsWindow : Window
    {
        public CommodityDetailsWindow()
        {
            InitializeComponent();
        }

        private void Cowindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void borderProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = true;
        }

        private void PopUpSelectProp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = false;
        }

        private void cmbStuffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
