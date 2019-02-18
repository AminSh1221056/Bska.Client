using Bska.Client.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bska.Client.UI.Views.StuffHonestView.ConcreteUC
{
    /// <summary>
    /// Interaction logic for AddProceddingUC.xaml
    /// </summary>
    public partial class AddProceddingUC : UserControl
    {
        public AddProceddingUC()
        {
            InitializeComponent();
            this.chIsAccidentAsset.IsChecked = false;
            this.chIsRetiringAsset.IsChecked = false;
            this.chIsBuildingAsset.IsChecked = false;
            this.chIsStoreAsset.IsChecked = false;
            this.chIsAccidentAsset.IsEnabled = false;
            this.chIsRetiringAsset.IsEnabled = false;
            this.chIsStoreAsset.IsEnabled = false;
            this.chIsBuildingAsset.IsEnabled = false;
            this.globalToolPane.editbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.reportbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.cancelbtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.deletebtn.Visibility = Visibility.Collapsed;
            this.globalToolPane.newbtn.Visibility = Visibility.Collapsed;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb.SelectedValue != null)
            {
                ProceedingsType typ = (ProceedingsType)cmb.SelectedValue;
                this.initProcType(typ);
            }
        }

        private void initProcType(ProceedingsType typ)
        {
            this.gr1.Visibility = Visibility.Visible;
            this.refundPane.Visibility = Visibility.Collapsed;
            this.procAccidentList.Visibility = Visibility.Collapsed;
            this.RefundTrustList.Visibility = Visibility.Collapsed;
            this.procMainList.Visibility = Visibility.Visible;
            this.txtOrganDesc.Text = "";
            switch (typ)
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
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.chIsStoreAsset.IsEnabled = true;
                    this.chIsBuildingAsset.IsEnabled = true;
                    this.procAccidentList.Visibility = Visibility.Visible;
                    this.procMainList.Visibility = Visibility.Collapsed;
                    this.RefundTrustList.Visibility = Visibility.Collapsed;
                    this.brOrganSelection.Visibility = Visibility.Collapsed;
                    break;
                case ProceedingsType.Sale:
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.salePane.Visibility = Visibility.Visible;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = true;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.chIsStoreAsset.IsEnabled = true;
                    this.chIsBuildingAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.brOrganSelection.Visibility = Visibility.Collapsed;
                    break;
                case ProceedingsType.AssetRetiring:
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Visible;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsEnabled = true;
                    this.chIsBuildingAsset.IsEnabled = true;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.brOrganSelection.Visibility = Visibility.Collapsed;
                    break;
                case ProceedingsType.TrustTransfer:
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Visible;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.chIsStoreAsset.IsEnabled = true;
                    this.chIsBuildingAsset.IsEnabled = true;
                    this.brOrganSelection.Visibility = Visibility.Visible;
                    this.txtOrganDesc.Text = "نام سازمان :";
                    break;
                case ProceedingsType.DefinitiveTransfer:
                case ProceedingsType.StateTransfer:
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Visible;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.chIsStoreAsset.IsEnabled = true;
                    this.chIsBuildingAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.brOrganSelection.Visibility = Visibility.Visible;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.txtOrganDesc.Text = "نام سازمان :";
                    break;
                case ProceedingsType.Delete:
                case ProceedingsType.SpecialLicencing:
                case ProceedingsType.BudgetLicencing:
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = true;
                    this.chIsRetiringAsset.IsEnabled = true;
                    this.chIsStoreAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsChecked = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.procAccidentList.Visibility = Visibility.Visible;
                    this.procMainList.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.RefundTrustList.Visibility = Visibility.Collapsed;
                    this.brOrganSelection.Visibility = Visibility.Collapsed;
                    break;
                case ProceedingsType.ReturnFromRetiring:
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = true;
                    this.chIsStoreAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsChecked = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.procAccidentList.Visibility = Visibility.Visible;
                    this.procMainList.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.RefundTrustList.Visibility = Visibility.Collapsed;
                    this.brOrganSelection.Visibility = Visibility.Collapsed;
                    break;
                case ProceedingsType.ReturnFromTrust:
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.gr1.Visibility = Visibility.Collapsed;
                    this.refundPane.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.chIsStoreAsset.IsChecked = false;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.gr1.Visibility = Visibility.Collapsed;
                    this.procAccidentList.Visibility = Visibility.Collapsed;
                    this.brOrganSelection.Visibility = Visibility.Visible;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.txtOrganDesc.Text = "نام سازمان امانت گیرنده:";
                    break;
                case ProceedingsType.RefundTrust:
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.gr1.Visibility = Visibility.Collapsed;
                    this.refundPane.Visibility = Visibility.Visible;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsEnabled = false;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.chIsStoreAsset.IsChecked = false;
                    this.procAccidentList.Visibility = Visibility.Collapsed;
                    this.RefundTrustList.Visibility = Visibility.Visible;
                    this.procMainList.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.brOrganSelection.Visibility = Visibility.Visible;
                    this.txtOrganDesc.Text = "نام سازمان امانت دهنده:";
                    break;
                case ProceedingsType.EditRequest:
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsEnabled = true;
                    this.chIsBuildingAsset.IsEnabled = true;
                    this.chIsStoreAsset.IsChecked = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.procAccidentList.Visibility = Visibility.Collapsed;
                    this.procMainList.Visibility = Visibility.Collapsed;
                    this.RefundTrustList.Visibility = Visibility.Collapsed;
                    this.brOrganSelection.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Visible;
                    break;
                default:
                    this.salePane.Visibility = Visibility.Collapsed;
                    this.accidentPane.Visibility = Visibility.Collapsed;
                    this.transferPane.Visibility = Visibility.Collapsed;
                    this.transferTrustPane.Visibility = Visibility.Collapsed;
                    this.retiringAssetPane.Visibility = Visibility.Collapsed;
                    this.PermEditProcList.Visibility = Visibility.Collapsed;
                    this.chIsAccidentAsset.IsChecked = false;
                    this.chIsRetiringAsset.IsChecked = false;
                    this.chIsBuildingAsset.IsChecked = false;
                    this.chIsStoreAsset.IsChecked = false;
                    this.chIsAccidentAsset.IsEnabled = false;
                    this.chIsRetiringAsset.IsEnabled = false;
                    this.chIsStoreAsset.IsEnabled = false;
                    this.chIsBuildingAsset.IsEnabled = false;
                    break;
            }
        }
    }
}
