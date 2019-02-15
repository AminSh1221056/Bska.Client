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

namespace Bska.Client.UI.Views.AssetDetailsView
{
    /// <summary>
    /// Interaction logic for CommoditySplitWindow.xaml
    /// </summary>
    public partial class CommoditySplitWindow : Window
    {
        public CommoditySplitWindow()
        {
            InitializeComponent();
        }

        private void splitwindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
