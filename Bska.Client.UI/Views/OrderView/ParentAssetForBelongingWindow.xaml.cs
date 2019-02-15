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

namespace Bska.Client.UI.Views.OrderView
{
    /// <summary>
    /// Interaction logic for ParentAssetForBelongingWindow.xaml
    /// </summary>
    public partial class ParentAssetForBelongingWindow : Window
    {
        public ParentAssetForBelongingWindow()
        {
            InitializeComponent();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void parentMassetForBelonging_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
