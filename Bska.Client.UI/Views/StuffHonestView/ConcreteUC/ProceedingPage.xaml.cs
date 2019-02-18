
using System.Windows.Controls;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for ProceedingPage.xaml
    /// </summary>
    public partial class ProceedingPage : UserControl
    {
        public ProceedingPage()
        {
            InitializeComponent();
            this.globalToolPane.gridMainBtn.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void MultiSelectComboBox_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
