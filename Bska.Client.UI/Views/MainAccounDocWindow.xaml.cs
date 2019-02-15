
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for MainAccounDocWindow.xaml
    /// </summary>
    public partial class MainAccounDocWindow : Window
    {
        public MainAccounDocWindow()
        {
            InitializeComponent();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

    }
}
