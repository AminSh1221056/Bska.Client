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

namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for StoreBillToDocumentWindow.xaml
    /// </summary>
    public partial class StoreBillToDocumentWindow : Window
    {
        public StoreBillToDocumentWindow()
        {
            InitializeComponent();
            this.storeToolPane.billBtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.gridsearchbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void billTodocWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
