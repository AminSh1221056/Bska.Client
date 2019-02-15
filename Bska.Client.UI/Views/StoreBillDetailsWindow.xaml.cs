using Bska.Client.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for StoreBillDetailsWindow.xaml
    /// </summary>
    public partial class StoreBillDetailsWindow : Window
    {
        public StoreBillDetailsWindow()
        {
            InitializeComponent();
            this.sbEditUc.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.sbEditUc.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.sbEditUc.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
            this.sbEditUc.globalToolPane.filterbtn.Visibility = Visibility.Collapsed;
            this.sbEditUc.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
        }

        private void StoreBillDetailsWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void billDetailsWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = this.DataContext as StoreBillDetailsViewModel;
            if (context != null)
            {
                switch (context.CurrentBill.AcqType)
                {
                    case Common.StateOwnership.Purchase:
                        this.donationPane.Visibility = Visibility.Collapsed;
                        this.ownedPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.trustPane.Visibility = Visibility.Collapsed;
                        this.purchasePane.Visibility = Visibility.Visible;
                        break;
                    case Common.StateOwnership.Donation:
                        this.donationPane.Visibility = Visibility.Visible;
                        this.ownedPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.trustPane.Visibility = Visibility.Collapsed;
                        this.purchasePane.Visibility = Visibility.Collapsed;
                        break;
                    case Common.StateOwnership.GovCompanyRecived:
                        this.donationPane.Visibility = Visibility.Collapsed;
                        this.ownedPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Visible;
                        this.trustPane.Visibility = Visibility.Collapsed;
                        this.purchasePane.Visibility = Visibility.Collapsed;
                        break;
                    case Common.StateOwnership.Owned:
                        this.donationPane.Visibility = Visibility.Collapsed;
                        this.ownedPane.Visibility = Visibility.Visible;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.trustPane.Visibility = Visibility.Collapsed;
                        this.purchasePane.Visibility = Visibility.Collapsed;
                        break;
                    case Common.StateOwnership.Trust:
                        this.donationPane.Visibility = Visibility.Collapsed;
                        this.ownedPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.purchasePane.Visibility = Visibility.Collapsed;
                        this.trustPane.Visibility = Visibility.Visible;
                        break;
                    default:
                        this.donationPane.Visibility = Visibility.Collapsed;
                        this.ownedPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.trustPane.Visibility = Visibility.Collapsed;
                        this.purchasePane.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}
