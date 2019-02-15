
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for DocumentHistoryWindow.xaml
    /// </summary>
    public partial class DocumentHistoryWindow : Window
    {
        public DocumentHistoryWindow()
        {
            InitializeComponent();
        }

        private void docHiswindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb == null) return;
            var type = cmb.SelectedValue as string;
            if (type == "B00B")
            {
                this.AccountDocumentGridView.Visibility = Visibility.Visible;
                this.DocumentGridView.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.AccountDocumentGridView.Visibility = Visibility.Collapsed;
                this.DocumentGridView.Visibility = Visibility.Visible;
            }
        }
    }
}
