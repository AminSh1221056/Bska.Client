using Bska.Client.Common;
using Bska.Client.Domain.Entity;
using Bska.Client.Domain.Entity.AssetEntity;
using Bska.Client.UI.ViewModels;
using Bska.Client.UI.ViewModels.AccessoriesViewModels;
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

namespace Bska.Client.UI.Views.AssetDetailsView
{
    /// <summary>
    /// Interaction logic for AssetMainDetailsUC.xaml
    /// </summary>
    public partial class AssetMainDetailsUC : UserControl
    {
        public AssetMainDetailsUC()
        {
            InitializeComponent();
        }

        private void cmbStuffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var stuff = cmb.SelectedItem as Stuff;
            if (stuff == null) return;
            var context = this.DataContext as MovableAssetDetailsViewModel;
            if (context != null)
            {
                if (context.CurrentAsset is Belonging)
                {
                    this.unConsumptionPane.Visibility = Visibility.Collapsed;
                    this.inCommodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Visible;
                    this.installableUC.Visibility = Visibility.Collapsed;
                    this.belongingPane.DataContext = context.BelongingViewModel;
                    context.BelongingViewModel.Units = context.Units;
                    context.BelongingViewModel.UnitId = context.CurrentAsset.UnitId;
                    context.BelongingViewModel.Labels = new List<int> { context.CurrentAsset.Label ?? 0 };
                }
                if (context.CurrentAsset is Installable)
                {
                    this.unConsumptionPane.Visibility = Visibility.Collapsed;
                    this.inCommodityPane.Visibility = Visibility.Collapsed;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    this.installableUC.Visibility = Visibility.Visible;
                    this.installableUC.DataContext = context.InstallableViewModel;
                    context.InstallableViewModel.Units = context.Units;
                    context.InstallableViewModel.UnitId = context.CurrentAsset.UnitId;
                    context.InstallableViewModel.Labels = new List<int> { context.CurrentAsset.Label ?? 0 };
                }
                else if (context.CurrentAsset is UnConsumption)
                {
                    if (context.CurrentAsset.Documetns.Any(x => x.DocumentType == DocumentType.InitialBalance))
                    {
                        this.unConsumptionPane.grOldSystem.Visibility = Visibility.Visible;
                        this.unConsumptionPane.grOrganLabel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.unConsumptionPane.grOldSystem.Visibility = Visibility.Collapsed;
                        this.unConsumptionPane.grOrganLabel.Visibility = Visibility.Visible;
                    }

                    int childNo = 1;
                    BaseDetailsViewModel<MovableAsset> viewModel = null;
                    var unViewModel = context.UnConsumptionViewModel;
                    int stuffId = stuff.Parent.StuffId;

                    if (stuffId == 23102)
                    {
                        childNo = 0;
                        unViewModel.ElectricModel = new ElectricViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.ElectricModel;
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
                        unViewModel.AutomotiveModel = new AutomotiveViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.AutomotiveModel;
                        unViewModel._checkErrorNum = 3;
                    }
                    else if (stuffId == 23101)
                    {
                        childNo = 11;
                        unViewModel.VideoAudioModel = new VideoAudioViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.VideoAudioModel;
                        unViewModel._checkErrorNum = 4;
                    }
                    else if (stuffId == 23104)
                    {
                        childNo = 6;
                        unViewModel.ComputerModel = new ComputerViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.ComputerModel;
                        unViewModel._checkErrorNum = 5;
                    }
                    else if (stuffId == 23105)
                    {
                        childNo = 7;
                        unViewModel.HandmadeCarpetModel = new HandmadeCarpetViewModel(unViewModel.CurrentEntity);
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
                        unViewModel._checkErrorNum = 8;
                    }
                    else if (stuffId == 23202)
                    {
                        childNo = 3;
                        unViewModel.AutomativeSportsModel = new AutomativeSportsViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.AutomativeSportsModel;
                        unViewModel._checkErrorNum = 9;
                    }
                    else if (stuffId >= 23501 && stuffId <= 23505)
                    {
                        childNo = 4;
                        unViewModel.CameraModel = new CameraViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.CameraModel;
                        unViewModel._checkErrorNum = 10;
                    }
                    else if ((stuffId >= 24101 && stuffId <= 24135) || stuffId == 24201)
                    {
                        childNo = 10;
                        unViewModel.ToolModel = new ToolViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.ToolModel;
                        unViewModel._checkErrorNum = 11;
                    }
                    else
                    {
                        childNo = 1;
                        unViewModel.NonElectricModel = new NonElectricViewModel(unViewModel.CurrentEntity);
                        viewModel = unViewModel.NonElectricModel;
                        unViewModel._checkErrorNum = 12;
                    }

                    for (int i = 0; i < this.unConsumptionPane.grAccessories.Children.Count; i++)
                    {
                        this.unConsumptionPane.grAccessories.Children[i].Visibility = Visibility.Collapsed;
                    }

                    var childView = this.unConsumptionPane.grAccessories.Children[childNo] as UserControl;
                    if (childView != null)
                    {
                        this.unConsumptionPane.Visibility = Visibility.Visible;
                        this.inCommodityPane.Visibility = Visibility.Collapsed;
                        this.belongingPane.Visibility = Visibility.Collapsed;
                        this.installableUC.Visibility = Visibility.Collapsed;
                        this.unConsumptionPane.DataContext = unViewModel;
                        childView.Visibility = Visibility.Visible;
                        childView.DataContext = viewModel;

                        if (context.CurrentAsset.StoreBill != null)
                        {
                            if (context.CurrentAsset.StoreBill.AcqType == Common.StateOwnership.Trust)
                            {
                                this.unConsumptionPane.grLabel.Visibility = Visibility.Collapsed;
                            }
                        }

                        this.unConsumptionPane.txtNum.IsEnabled = false;
                        unViewModel.Labels = new List<int> { context.CurrentAsset.Label ?? 0 };
                        unViewModel.Units = context.Units;
                        unViewModel.UnitId = context.CurrentAsset.UnitId;
                    }
                }
                else if (context.CurrentAsset is InCommidity)
                {
                    if (context.CurrentAsset.Documetns.Any(x => x.DocumentType == DocumentType.InitialBalance))
                    {
                        this.inCommodityPane.grOldLabel.Visibility = Visibility.Visible;
                        this.inCommodityPane.grOrganLabel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.inCommodityPane.grOldLabel.Visibility = Visibility.Collapsed;
                        this.inCommodityPane.grOrganLabel.Visibility = Visibility.Visible;
                    }
                    this.installableUC.Visibility = Visibility.Collapsed;
                    this.unConsumptionPane.Visibility = Visibility.Collapsed;
                    this.inCommodityPane.Visibility = Visibility.Visible;
                    this.belongingPane.Visibility = Visibility.Collapsed;
                    context.InCommodityViewModel.Units = context.Units;
                    context.InCommodityViewModel.UnitId = context.CurrentAsset.UnitId;
                    this.inCommodityPane.DataContext = context.InCommodityViewModel;
                }
            }
        }
    }
}
