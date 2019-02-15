using Bska.Client.Common;
using Bska.Client.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views
{
    /// <summary>
    /// Interaction logic for ProceeedingDetailsWindow.xaml
    /// </summary>
    public partial class ProceeedingDetailsWindow : Window
    {
        public ProceeedingDetailsWindow()
        {
            InitializeComponent();
        }
        private void window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var gr = sender as Grid;
            if (gr != null)
            {
                ProceedingDetailsViewModel vm = gr.DataContext as ProceedingDetailsViewModel;
                if (vm == null) return;
                this.procAccidentList.Visibility = Visibility.Collapsed;
                this.RefundTrustList.Visibility = Visibility.Collapsed;
                this.procMainList.Visibility = Visibility.Visible;
                switch (vm.CurrentEntity.Type)
                {
                    case ProceedingsType.Fire:
                    case ProceedingsType.Flood:
                    case ProceedingsType.Earthquake:
                    case ProceedingsType.Accident:
                    case ProceedingsType.Theft:
                        this.salePane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.accidentPane.Visibility = Visibility.Visible;
                        this.transferTrustPane.Visibility = Visibility.Collapsed;
                        this.retiringAssetPane.Visibility = Visibility.Collapsed;
                        this.procAccidentList.Visibility = Visibility.Visible;
                        this.procMainList.Visibility = Visibility.Collapsed;
                        this.RefundTrustList.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.Sale:
                        this.accidentPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.salePane.Visibility = Visibility.Visible;
                        this.transferTrustPane.Visibility = Visibility.Collapsed;
                        this.retiringAssetPane.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.AssetRetiring:
                        this.accidentPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.salePane.Visibility = Visibility.Collapsed;
                        this.transferTrustPane.Visibility = Visibility.Collapsed;
                        this.retiringAssetPane.Visibility = Visibility.Visible;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.TrustTransfer:
                    case ProceedingsType.RefundTrust:
                        this.salePane.Visibility = Visibility.Collapsed;
                        this.accidentPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.transferTrustPane.Visibility = Visibility.Visible;
                        this.retiringAssetPane.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.DefinitiveTransfer:
                    case ProceedingsType.StateTransfer:
                        this.salePane.Visibility = Visibility.Collapsed;
                        this.accidentPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Visible;
                        this.transferTrustPane.Visibility = Visibility.Collapsed;
                        this.retiringAssetPane.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.Delete:
                    case ProceedingsType.SpecialLicencing:
                    case ProceedingsType.BudgetLicencing:
                        this.salePane.Visibility = Visibility.Collapsed;
                        this.accidentPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.transferTrustPane.Visibility = Visibility.Collapsed;
                        this.retiringAssetPane.Visibility = Visibility.Collapsed;
                        this.procAccidentList.Visibility = Visibility.Visible;
                        this.procMainList.Visibility = Visibility.Collapsed;
                        this.RefundTrustList.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.ReturnFromTrust:
                        this.procAccidentList.Visibility = Visibility.Collapsed;
                        this.RefundTrustList.Visibility = Visibility.Visible;
                        this.procMainList.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                    case ProceedingsType.EditRequest:
                        this.procAccidentList.Visibility = Visibility.Collapsed;
                        this.RefundTrustList.Visibility = Visibility.Collapsed;
                        this.procMainList.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Visible;
                        break;
                    default:
                        this.salePane.Visibility = Visibility.Collapsed;
                        this.accidentPane.Visibility = Visibility.Collapsed;
                        this.transferPane.Visibility = Visibility.Collapsed;
                        this.transferTrustPane.Visibility = Visibility.Collapsed;
                        this.retiringAssetPane.Visibility = Visibility.Collapsed;
                        this.permEditList.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}
