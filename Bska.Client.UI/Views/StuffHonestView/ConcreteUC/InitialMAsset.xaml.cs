using Bska.Client.Common;
using Bska.Client.Domain.Entity;
using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccessoriesViewModels;
using Bska.Client.UI.ViewModels.StuffHonestViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StuffHonestView
{
    /// <summary>
    /// Interaction logic for InitialMAsset.xaml
    /// </summary>
    public partial class InitialMAsset : UserControl
    {
        bool _isOldsystem = false;
        bool _isFlorOld = false;
        bool _isCommodity = false;
        public InitialMAsset()
        {
            InitializeComponent();
            this.stuffviewPopUp.PopUpSelectFilter.PlacementTarget = this.btnStuffFilter;
            this.mainBorder.Visibility = Visibility.Collapsed;
            this.globalToolbar.gridMainBtn.Visibility = Visibility.Collapsed;
            this.globalToolbar.gridToolsbtn.Visibility = Visibility.Collapsed;
            this.importToolbar.excelbtn.Visibility = Visibility.Collapsed;
            this.grBelonging.Visibility = Visibility.Collapsed;
            this.KeyDown += new KeyEventHandler(initPage_KeyDown);
        }

        void initPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S)
            {
                this.globalToolbar.FilterTextBox.Focus();
            }
        }

        private void bookTypeDropDown_FilterButtonChanged(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            if (btn != null)
            {
                string insertType = btn.Content.ToString();
                bookTypeDropDown.popupToggle.Content = insertType;
                bookTypeDropDown.Popup.IsOpen = false;
                this.billPane.cmbBillType.IsEnabled = true;
                var viewModel = this.DataContext as InitialMAssetViewModel;
                this.grChToLabel.Visibility = Visibility.Visible;
                this.grOldSysDesc.Visibility = Visibility.Visible;
                switch (insertType)
                {
                    case "نظام نوین":
                        _isCommodity = false;
                        this.grOldSysDesc.Visibility = Visibility.Collapsed;
                        viewModel._isOldSystem = false;
                        _isOldsystem = false;
                        this.gridStoreDetails.IsEnabled = true;
                        this.unConsuptionPane.grOldSystem.Visibility = Visibility.Collapsed;
                        this.unConsuptionPane.grOrganLabel.Visibility = Visibility.Visible;
                        this.InCommodityUCPane.grOldLabel.Visibility = Visibility.Collapsed;
                        this.InCommodityUCPane.grOrganLabel.Visibility = Visibility.Visible;
                        break;
                    case "فهرست موجودی اولیه":
                        _isCommodity = false;
                        this.unConsuptionPane.grOldSystem.Visibility = Visibility.Visible;
                        this.unConsuptionPane.grOrganLabel.Visibility = Visibility.Collapsed;
                        this.InCommodityUCPane.grOldLabel.Visibility = Visibility.Visible;
                        this.InCommodityUCPane.grOrganLabel.Visibility = Visibility.Collapsed;
                        this.grOldSysDesc.Visibility = Visibility.Visible;
                        viewModel._isOldSystem = true;
                        if (APPSettings.Default.OldSystemFloorType == 701 ||
                            APPSettings.Default.OldSystemFloorType == 704)
                        {
                            if (APPSettings.Default.OldSystemFloorType == 701)
                            {
                                _isFlorOld = false;
                                this.txtOldFloor.Text = "707---قدیم قدیم";
                            }
                            else
                            {
                                _isFlorOld = true;
                                this.txtOldFloor.Text = "قدیم قدیم";
                            }
                        }
                        else
                        {
                            _isFlorOld = false;
                            this.txtOldFloor.Text = APPSettings.Default.OldSystemFloorType.ToString();
                        }

                        this.gridStoreDetails.IsEnabled =false;
                        this.billPane.cmbBillType.SelectedIndex = 0;
                        _isOldsystem = true;
                        break;
                    case "موجودی اولیه اموال مصرفی":
                        _isCommodity = true;
                        _isOldsystem = false;
                        this.gridStoreDetails.IsEnabled = false;
                        this.txtOldFloor.Text = "";
                        this.billPane.cmbBillType.SelectedIndex = 0;
                        this.billPane.cmbBillType.IsEnabled = false;
                        this.grChToLabel.Visibility = Visibility.Collapsed;
                        this.grOldSysDesc.Visibility = Visibility.Collapsed;
                        break;
                }

                if (viewModel.IsInStore)
                {
                    this.draftPane.Visibility = Visibility.Collapsed;
                }

                this.mainBorder.Visibility = Visibility.Visible;
                if (!_isCommodity)
                {
                     viewModel.stuffGenerationAsync(StuffType.None);
                }
                else
                {
                    viewModel.stuffGenerationAsync(StuffType.Consumable);
                }
            }
        }

        private void cmbStuffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var selected = cmb.SelectedItem as Stuff;
            if (selected != null)
            {
                this.grBelonging.Visibility = Visibility.Collapsed;
                var viewModel = this.DataContext as InitialMAssetViewModel;
                
                viewModel.initViewModels(selected.StuffType);
                if (selected.StuffType == StuffType.UnConsumption)
                {
                    if (selected.Parent != null)
                    {
                        this.ShowRelatedUnConsumDetails(selected.Parent.StuffId);
                        if (_isOldsystem)
                        {
                            if (_isFlorOld)
                                viewModel.UnConsumptionViewModel.Floor = selected.FloorOld != 0 ? selected.FloorOld.ToString() : null;
                            else viewModel.UnConsumptionViewModel.Floor = selected.FloorNew != 0 ? selected.FloorNew.ToString() : null;
                        }
                    }
                    this.unConsuptionPane.DataContext = viewModel.UnConsumptionViewModel;
                    this.unConsuptionPane.Visibility = Visibility.Visible;
                    this.BelongingUserControlPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUCPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.InstallableUCPane.Visibility = Visibility.Collapsed;
                }
                else if (selected.StuffType == StuffType.OrderConsumption)
                {
                    this.InCommodityUCPane.DataContext = viewModel.InCommodityViewModel;
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.BelongingUserControlPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUCPane.Visibility = Visibility.Visible;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.InstallableUCPane.Visibility = Visibility.Collapsed;
                    this.grChToLabel.Visibility = Visibility.Collapsed;
                }
                else if (selected.StuffType == StuffType.Installable)
                {
                    this.InstallableUCPane.DataContext = viewModel.InstallableViewModel;
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.BelongingUserControlPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUCPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.InstallableUCPane.Visibility = Visibility.Visible;
                    this.grChToLabel.Visibility = Visibility.Collapsed;
                }
                else if (selected.StuffType == StuffType.Belonging)
                {
                    this.BelongingUserControlPane.DataContext = viewModel.BelongingViewModel;
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.BelongingUserControlPane.Visibility = Visibility.Visible;
                    this.InCommodityUCPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.InstallableUCPane.Visibility = Visibility.Collapsed;
                    this.grChToLabel.Visibility = Visibility.Collapsed;
                    this.grBelonging.Visibility = Visibility.Visible;
                }
                else if (selected.StuffType == StuffType.Consumable)
                {
                    this.commodityPane.DataContext = viewModel.CommodityViewModel;
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.BelongingUserControlPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUCPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Visible;
                    this.InstallableUCPane.Visibility = Visibility.Collapsed;
                    this.grChToLabel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.BelongingUserControlPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUCPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.InstallableUCPane.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ShowRelatedUnConsumDetails(int stuffId)
        {
            var context = this.DataContext as InitialMAssetViewModel;
            var unViewModel = context.UnConsumptionViewModel;
            var sbViewModel = context.StoreBillViewModel;
            int childNo = 1;
            BaseDetailsViewModel<MovableAsset> viewModel = null;
            if (sbViewModel.AcqTyp == StateOwnership.Purchase)
            {
                this.unConsuptionPane.txtNum.IsEnabled = true;
            }

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
                string vin = "0";
                if (!_isOldsystem)
                {
                    vin = "";
                }

                unViewModel.AutomotiveModel = new AutomotiveViewModel(unViewModel.CurrentEntity)
                {
                    MotorNo = "",
                    ChassisNo = "",
                    CommissionNo = "",
                    Plaque = "",
                    VIN = vin,
                    CountryItem = null,
                    SelectedCompany = null,
                    CarDetails = null
                };

                viewModel = unViewModel.AutomotiveModel;
                unViewModel._checkErrorNum = 3;
                this.unConsuptionPane.txtNum.IsEnabled = false;
                this.unConsuptionPane.txtNum.Text = "1";
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

            for (int i = 0; i < this.unConsuptionPane.grAccessories.Children.Count; i++)
            {
                this.unConsuptionPane.grAccessories.Children[i].Visibility = Visibility.Collapsed;
            }

            var childView = this.unConsuptionPane.grAccessories.Children[childNo] as UserControl;
            if (childView != null)
            {
                childView.Visibility = Visibility.Visible;
                childView.DataContext = viewModel;
            }
        }

        private void BuildingPersonManageUC_OrganizTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((InitialMAssetViewModel)this.DataContext).OrganizSelected = buildingDesign;
        }

        private void BuildingPersonManageUC_StrategyTreeClick(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var buildingDesign = item.SelectedItem as EmployeeDesignTreeViewModel;
            if (buildingDesign != null)
                ((InitialMAssetViewModel)this.DataContext).StrategySelected = buildingDesign;
        }

        private void billPane_CmbSelectionChanged(object sender, RoutedEventArgs e)
        {
            var cmb = e.OriginalSource as ComboBox;
            if (cmb.SelectedValue == null) return;
            var item = (StateOwnership)cmb.SelectedValue;

            this.unConsuptionPane.grLabel.Visibility = Visibility.Visible;
            this.unConsuptionPane.grOrganLabel.Visibility = Visibility.Collapsed;
            this.InCommodityUCPane.grOrganLabel.Visibility = Visibility.Collapsed;

            this.unConsuptionPane.txtNum.IsEnabled = false;
            if (item == StateOwnership.Donation)
            {
                this.donationPane.Visibility = Visibility.Visible;
                this.purchasePane.Visibility = Visibility.Collapsed;
                this.transferPane.Visibility = Visibility.Collapsed;
                this.trustPane.Visibility = Visibility.Collapsed;
                this.ownedPane.Visibility = Visibility.Collapsed;
            }
            else if (item == StateOwnership.GovCompanyRecived)
            {
                this.donationPane.Visibility = Visibility.Collapsed;
                this.purchasePane.Visibility = Visibility.Collapsed;
                this.transferPane.Visibility = Visibility.Visible;
                this.trustPane.Visibility = Visibility.Collapsed;
                this.ownedPane.Visibility = Visibility.Collapsed;
                this.unConsuptionPane.grOrganLabel.Visibility = Visibility.Visible;
                this.InCommodityUCPane.grOrganLabel.Visibility = Visibility.Visible;
            }
            else if (item == StateOwnership.Owned)
            {
                this.donationPane.Visibility = Visibility.Collapsed;
                this.purchasePane.Visibility = Visibility.Collapsed;
                this.transferPane.Visibility = Visibility.Collapsed;
                this.trustPane.Visibility = Visibility.Collapsed;
                this.ownedPane.Visibility = Visibility.Visible;
            }
            else if (item == StateOwnership.Purchase)
            {
                this.donationPane.Visibility = Visibility.Collapsed;
                this.purchasePane.Visibility = Visibility.Visible;
                this.transferPane.Visibility = Visibility.Collapsed;
                this.trustPane.Visibility = Visibility.Collapsed;
                this.ownedPane.Visibility = Visibility.Collapsed;
                this.unConsuptionPane.txtNum.IsEnabled = true;
            }
            else if (item == StateOwnership.Trust)
            {
                this.donationPane.Visibility = Visibility.Collapsed;
                this.purchasePane.Visibility = Visibility.Collapsed;
                this.transferPane.Visibility = Visibility.Collapsed;
                this.trustPane.Visibility = Visibility.Visible;
                this.ownedPane.Visibility = Visibility.Collapsed;
                this.unConsuptionPane.grLabel.Visibility = Visibility.Collapsed;
                this.unConsuptionPane.grOrganLabel.Visibility = Visibility.Visible;
                this.InCommodityUCPane.grOrganLabel.Visibility = Visibility.Visible;
            }
        }

        private void page_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = this.DataContext as InitialMAssetViewModel;
            var dics = context.getAvailableBookTypes();
            this.bookTypeDropDown.SourceContoller.ItemsSource = dics;
        }

        private void stuffviewPopUp_StuffTreeItemSelect(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeView;
            var stuffTree = item.SelectedItem as StuffTreeViewModel;
            if (stuffTree != null)
                ((InitialMAssetViewModel)this.DataContext).SelectedNode = stuffTree;
        }

        private void btnStuffFilter_Click(object sender, RoutedEventArgs e)
        {
            this.stuffviewPopUp.PopUpSelectFilter.IsOpen = true;
        }
    }
}
