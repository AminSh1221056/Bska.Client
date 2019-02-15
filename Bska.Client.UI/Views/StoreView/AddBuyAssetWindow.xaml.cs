using Bska.Client.Common;
using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.Repository.Model;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccessoriesViewModels;
using Bska.Client.UI.ViewModels.StoreViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bska.Client.UI.Views.StoreView
{
    /// <summary>
    /// Interaction logic for AddBuyAssetWindow.xaml
    /// </summary>
    public partial class AddBuyAssetWindow : Window
    {
        public AddBuyAssetWindow()
        {
            InitializeComponent();
            this.initializobj();
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void exofstoreWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void borderProperty_MouseEnter(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = true;
        }

        private void PopUpSelectProp_MouseLeave(object sender, MouseEventArgs e)
        {
            this.PopUpSelectProp.IsOpen = false;
        }

        private void initializobj()
        {
            this.unConsuptionPane.grOldSystem.Visibility = Visibility.Collapsed;
            this.unConsuptionPane.grOrganLabel.Visibility = Visibility.Collapsed;
            this.InCommodityUC.grOrganLabel.Visibility = Visibility.Collapsed;
            this.InCommodityUC.grOldLabel.Visibility = Visibility.Collapsed;
            this.billPane.txtRecipetNo.IsEnabled = false;
            this.billPane.txtRecipetDate.IsEnabled = false;
            this.billPane.cmbBillType.IsEnabled = false;
        }

        private void cmbAllProperty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var selected = cmb.SelectedItem as SupplierIndentModel;
            var context = this.DataContext as AddBuyAssetViewModel;
            int pId = context.initOnIndent();
            if (selected != null)
            {
                if (selected.StuffType == StuffType.Belonging)
                {
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Visible;
                    this.InCommodityUC.Visibility = Visibility.Collapsed;
                    this.installablePane.Visibility = Visibility.Collapsed;
                    this.belongingPane.DataContext = context.BelongingViewModel;
                }
                else if (selected.StuffType == StuffType.UnConsumption)
                {
                    this.unConsuptionPane.Visibility = Visibility.Visible;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUC.Visibility = Visibility.Collapsed;
                    this.installablePane.Visibility = Visibility.Collapsed;
                    this.unConsuptionPane.DataContext = context.UnConsumptionViewModel;
                    this.ShowRelatedUnConsumDetails(pId, context);
                }
                else if (selected.StuffType == StuffType.Consumable)
                {
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Visible;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUC.Visibility = Visibility.Collapsed;
                    this.installablePane.Visibility = Visibility.Collapsed;
                    this.commodityPane.DataContext = context.CommodityViewModel;
                }
                else if (selected.StuffType == StuffType.OrderConsumption)
                {
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUC.Visibility = Visibility.Visible;
                    this.installablePane.Visibility = Visibility.Collapsed;
                    this.InCommodityUC.DataContext = context.InCommodityViewModel;
                }
                else if (selected.StuffType == StuffType.Installable)
                {
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUC.Visibility = Visibility.Collapsed;
                    this.installablePane.Visibility = Visibility.Visible;
                    this.installablePane.DataContext = context.InstallableViewModel;
                }
                else
                {
                    this.unConsuptionPane.Visibility = Visibility.Collapsed;
                    this.commodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    this.InCommodityUC.Visibility = Visibility.Collapsed;
                    this.installablePane.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ShowRelatedUnConsumDetails(int stuffId,AddBuyAssetViewModel context)
        {
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
                unViewModel.AutomotiveModel = new AutomotiveViewModel(unViewModel.CurrentEntity)
                {
                    MotorNo = "",
                    ChassisNo = "",
                    CommissionNo = "",
                    Plaque = "",
                    VIN = "",
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
    }
}
