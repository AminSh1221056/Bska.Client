using Bska.Client.Common;
using Bska.Client.Domain.Entity;
using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccessoriesViewModels;
using Bska.Client.UI.ViewModels.AssetViewModel;
using Bska.Client.UI.ViewModels.StoreViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for StoreBillIssuancePage.xaml
    /// </summary>
    public partial class StoreBillIssuancePage : Page
    {
        public StoreBillIssuancePage()
        {
            InitializeComponent();
            this.uncounsuptionPane.txtNum.IsEnabled = false;
            this.belongingPane.txtNum.IsEnabled = false;
            this.installablePane.txtNum.IsEnabled = false;
            this.incommodityPane.txtNum.IsEnabled = false;
            this.storeRecipetPane.txtRecipetDate.IsEnabled = false;
            this.storeRecipetPane.txtRecipetNo.IsEnabled = false;
            this.uncounsuptionPane.grOldSystem.Visibility = Visibility.Collapsed;
            this.grBelonging.Visibility = Visibility.Collapsed;
            this.stuffviewPopUp.PopUpSelectFilter.PlacementTarget = this.btnStuffFilter;
            this.storeManage.grStoreSelect.Visibility = Visibility.Collapsed;
            this.globalToolpane.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolpane.gridToolsbtn.Visibility = Visibility.Collapsed;
            this.globalToolpane.FilterTextBox.Focus();
        }
        
        private void cmbStuffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var selected = cmb.SelectedItem as Stuff;
            if (selected == null) return;
            var context = this.DataContext as StoreBillIssuanceViewModel;
            if (selected.StuffType==StuffType.Belonging)
            {
                this.belongingPane.Visibility = Visibility.Visible;
                this.uncounsuptionPane.Visibility = Visibility.Collapsed;
                this.incommodityPane.Visibility = Visibility.Collapsed;
                this.installablePane.Visibility = Visibility.Collapsed;
                this.grBelonging.Visibility = Visibility.Visible;
                this.commodityPane.Visibility = Visibility.Collapsed;
            }
            else if (selected.StuffType == StuffType.Installable)
            {
                this.belongingPane.Visibility = Visibility.Collapsed;
                this.uncounsuptionPane.Visibility = Visibility.Collapsed;
                this.incommodityPane.Visibility = Visibility.Collapsed;
                this.installablePane.Visibility = Visibility.Visible;
                this.grBelonging.Visibility = Visibility.Collapsed;
                this.commodityPane.Visibility = Visibility.Collapsed;
            }
            else if (selected.StuffType == StuffType.OrderConsumption)
            {
                this.belongingPane.Visibility = Visibility.Collapsed;
                this.uncounsuptionPane.Visibility = Visibility.Collapsed;
                this.incommodityPane.Visibility = Visibility.Visible;
                this.installablePane.Visibility = Visibility.Collapsed;
                this.grBelonging.Visibility = Visibility.Collapsed;
                this.incommodityPane.grOldLabel.Visibility = Visibility.Collapsed;
                this.commodityPane.Visibility = Visibility.Collapsed;
            }
            else if (selected.StuffType == StuffType.Consumable)
            {
                this.belongingPane.Visibility = Visibility.Collapsed;
                this.uncounsuptionPane.Visibility = Visibility.Collapsed;
                this.incommodityPane.Visibility = Visibility.Collapsed;
                this.installablePane.Visibility = Visibility.Collapsed;
                this.grBelonging.Visibility = Visibility.Collapsed;
                this.incommodityPane.grOldLabel.Visibility = Visibility.Collapsed;
                this.commodityPane.Visibility = Visibility.Visible;
            }
            else
            {
                this.belongingPane.Visibility = Visibility.Collapsed;
                this.uncounsuptionPane.Visibility = Visibility.Visible;
                this.incommodityPane.Visibility = Visibility.Collapsed;
                this.installablePane.Visibility = Visibility.Collapsed;
                this.grBelonging.Visibility = Visibility.Collapsed;

                this.commodityPane.Visibility = Visibility.Collapsed;
                context.UnConsumptionVM.CurrentEntity.Name = selected.Name;
                this.ShowRelatedUnConsumDetails(selected.Parent.StuffId, context.UnConsumptionVM);
            }
        }

        private void ShowRelatedUnConsumDetails(int stuffId, UnConsumptionViewModel unViewModel)
        {
            int childNo = 1;
            BaseDetailsViewModel<MovableAsset> viewModel = null;
            if (stuffId == 23102)
            {
                childNo = 0;
                unViewModel.ElectricModel = new ElectricViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.ElectricModel;
                unViewModel.CreateListener(unViewModel.ElectricModel);
                unViewModel._checkErrorNum = 1;
            }
            else if (stuffId == 26101 || stuffId == 262)
            {
                childNo = 8;
                unViewModel.PrintedBooksModel = new PrintedBooksViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.PrintedBooksModel;
                unViewModel._checkErrorNum = 2;
            }
            else if (stuffId >= 25101 && stuffId <= 25110)
            {
                childNo = 2;
                unViewModel.AutomotiveModel = new AutomotiveViewModel(unViewModel.CurrentEntity)
                {
                    MotorNo = null,
                    ChassisNo = null,
                    CommissionNo = null,
                    Plaque = null,
                    VIN = null,
                    CountryItem = null,
                    SelectedCompany = null,
                    CarDetails = null
                };
                viewModel = unViewModel.AutomotiveModel;
                unViewModel._checkErrorNum = 3;
                var unit = unViewModel.Units.FirstOrDefault(x => x.Name == "دستگاه");
                if (unit != null)
                {
                    unViewModel.UnitId = unit.UnitId;
                }
            }
            else if (stuffId == 23101)
            {
                childNo = 11;
                unViewModel.VideoAudioModel = new VideoAudioViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.VideoAudioModel;
                unViewModel.CreateListener(unViewModel.VideoAudioModel);
                unViewModel._checkErrorNum = 4;
            }
            else if (stuffId == 23104)
            {
                childNo = 6;
                unViewModel.ComputerModel = new ComputerViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.ComputerModel;
                unViewModel.CreateListener(unViewModel.ComputerModel);
                unViewModel._checkErrorNum = 5;
            }
            else if (stuffId == 23105)
            {
                childNo = 7;
                unViewModel.HandmadeCarpetModel = new HandmadeCarpetViewModel(unViewModel.CurrentEntity) { Area = "", RowCount = "", Diagram = "", Color = "" };
                viewModel = unViewModel.HandmadeCarpetModel;
                unViewModel._checkErrorNum = 6;
            }
            else if (stuffId == 23108)
            {
                childNo = 5;
                unViewModel.CDModel = new CDViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.CDModel;
                unViewModel._checkErrorNum = 7;
            }
            else if (stuffId == 23201)
            {
                childNo = 9;
                unViewModel.SportModel = new SportViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.SportModel;
                unViewModel.CreateListener(unViewModel.SportModel);
                unViewModel._checkErrorNum = 8;
            }
            else if (stuffId == 23202)
            {
                childNo = 3;
                unViewModel.AutomativeSportsModel = new AutomativeSportsViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.AutomativeSportsModel;
                unViewModel.CreateListener(unViewModel.AutomativeSportsModel);
                unViewModel._checkErrorNum = 9;
            }
            else if (stuffId >= 23501 && stuffId <= 23505)
            {
                childNo = 4;
                unViewModel.CameraModel = new CameraViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.CameraModel;
                unViewModel.CreateListener(unViewModel.CameraModel);
                unViewModel._checkErrorNum = 10;
            }
            else if ((stuffId >= 24101 && stuffId <= 24135) || stuffId == 24201)
            {
                childNo = 10;
                unViewModel.ToolModel = new ToolViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.ToolModel;
                unViewModel.CreateListener(unViewModel.ToolModel);
                unViewModel._checkErrorNum = 11;
            }
            else
            {
                childNo = 1;
                unViewModel.NonElectricModel = new NonElectricViewModel(unViewModel.CurrentEntity);
                viewModel = unViewModel.NonElectricModel;
                unViewModel._checkErrorNum = 12;
            }

            for (int i = 0; i < this.uncounsuptionPane.grAccessories.Children.Count; i++)
            {
                this.uncounsuptionPane.grAccessories.Children[i].Visibility = Visibility.Collapsed;
            }

            var childView = this.uncounsuptionPane.grAccessories.Children[childNo] as UserControl;
            if (childView != null)
            {
                childView.Visibility = Visibility.Visible;
                childView.DataContext = viewModel;
            }
        }

        private void storeRecipetPane_CmbSelectionChanged(object sender, RoutedEventArgs e)
        {
            var cmb = e.OriginalSource as ComboBox;
            if (cmb.SelectedValue == null) return;
            var item = (StateOwnership)cmb.SelectedValue;
            this.uncounsuptionPane.grLabel.Visibility = Visibility.Visible;
            if (item == StateOwnership.Donation)
            {
                this.donationDraftPane.Visibility = Visibility.Visible;
                this.transferDraftPane.Visibility = Visibility.Collapsed;
                this.trustDraftPane.Visibility = Visibility.Collapsed;
                this.ownedDraftPane.Visibility = Visibility.Collapsed;
            }
            else if (item == StateOwnership.GovCompanyRecived)
            {
                this.donationDraftPane.Visibility = Visibility.Collapsed;
                this.transferDraftPane.Visibility = Visibility.Visible;
                this.trustDraftPane.Visibility = Visibility.Collapsed;
                this.ownedDraftPane.Visibility = Visibility.Collapsed;
            }
            else if (item == StateOwnership.Owned)
            {
                this.donationDraftPane.Visibility = Visibility.Collapsed;
                this.transferDraftPane.Visibility = Visibility.Collapsed;
                this.trustDraftPane.Visibility = Visibility.Collapsed;
                this.ownedDraftPane.Visibility = Visibility.Visible;
            }
            else if (item == StateOwnership.Trust)
            {
                this.donationDraftPane.Visibility = Visibility.Collapsed;
                this.transferDraftPane.Visibility = Visibility.Collapsed;
                this.trustDraftPane.Visibility = Visibility.Visible;
                this.ownedDraftPane.Visibility = Visibility.Collapsed;
                this.uncounsuptionPane.grLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void btnStuffFilter_Click(object sender, RoutedEventArgs e)
        {
            this.stuffviewPopUp.PopUpSelectFilter.IsOpen = true;
        }

        private void stuffviewPopUp_StuffTreeItemSelect(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as StuffTreeViewModel;
            if (stuffTree != null)
                ((StoreBillIssuanceViewModel)this.DataContext).SelectedNode = stuffTree;
        }

        private void storeManage_StoreTreeViewClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var storeTree = item.SelectedItem as StoreTreeViewModel;
            if (storeTree != null)
                ((StoreBillIssuanceViewModel)this.DataContext).StoreDesignSelected = storeTree;
        }
    }
}
