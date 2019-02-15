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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bska.Client.UI.UserControlls
{
    /// <summary>
    /// Interaction logic for AccessExportUC.xaml
    /// </summary>
    public partial class AccessExportUC : UserControl
    {
        public AccessExportUC()
        {
            InitializeComponent();
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
        }

        private void btnContract_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ContractingStoryboard"]).Begin(this);
        }

        private void rbUnconsum_Click(object sender, RoutedEventArgs e)
        {
            if (this.leftPane.Width == 0)
            {
                ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
            }
            mAssetExportpane.Visibility = Visibility.Visible;
        }
        
        private void accessExport_Loaded(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ExpandingStoryboard"]).Begin(this);
        }

        private void rbTransmitOut_Click(object sender, RoutedEventArgs e)
        {
            if (this.leftPane.Width > 0)
            {
                ((Storyboard)this.Resources["ContractingStoryboard"]).Begin(this);
            }
            mAssetExportpane.Visibility = Visibility.Collapsed;
        }
    }
}
