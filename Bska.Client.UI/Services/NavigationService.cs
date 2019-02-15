
namespace Bska.Client.UI.Services
{
    using Views;
    using Views.MunitionView;
    using Views.OrderView;
    using Views.StoreView;
    using Views.StuffHonestView;
    using Microsoft.Practices.Unity;
    using System.Windows;
    using System;
    using Views.AssetDetailsView;

    public class NavigationService : INavigationService
    {
        [Dependency]
        public IDialogService DialogService { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public bool ConfirmClose()
        {
            throw new System.NotImplementedException();
        }

        public Window ShowStartWindow()
        {
            var window = Container.Resolve<StartWindow>();
            window.ShowDialog();
            return window;
        }

        public Window ShowMainWindow(object viewModel)
        {
            var window = Container.Resolve<MainWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowConfigWindow()
        {
            var window = Container.Resolve<ConfigWindow>();
            window.ShowDialog();
            return window;
        }

        public Window ShowAddInfoWindow(object viewModel)
        {
            var window = Container.Resolve<AddInfoWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowSplitWindow(object viewModel)
        {
            var window = Container.Resolve<MovableAssetSplitWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowMAssetDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<MovableAssetDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowOrderDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<OrderDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowOrderTrackWindow(object viewModel)
        {
            var window = Container.Resolve<OrderTrackWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowOrderEditWindow(object viewModel)
        {
            var window = Container.Resolve<OrderEditWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowIndentWindow(object viewModel)
        {
            var window = Container.Resolve<IndentWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowSuggestWindow(object viewModel)
        {
            var window = Container.Resolve<SuggestViewWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowDisplacementIndentWindow(object viewModel)
        {
            var window = Container.Resolve<DisplacementIndentWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowStoreIndentConfirmWindow(object viewModel)
        {
            var window = Container.Resolve<StoreIndentConfirmWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowAddProceedingWindow(object viewModel)
        {
            var window = Container.Resolve<AddProceddingWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowPersonDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<PersonDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowDocumentShowWindow(object viewModel)
        {
            var window = Container.Resolve<DocumentShowWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowOrderHistoryWindow(object viewModel)
        {
            var window = Container.Resolve<OrderHistoryWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowStoreAssetDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<StoreAssetDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowProceedingDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<ProceeedingDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowProceedingHistoryWindow(object viewModel)
        {
            var window = Container.Resolve<ProceedingHistoryWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowDocumentHistoryWindow(object viewModel)
        {
            var window = Container.Resolve<DocumentHistoryWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowStoreBillDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<StoreBillDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowMeterBillWindow(object viewModel)
        {
            var window = Container.Resolve<MeterBillWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowParentAssetForBelongingWindow(object viewModel)
        {
            var window = Container.Resolve<ParentAssetForBelongingWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowBelongingsWindow(object viewModel)
        {
            var window = Container.Resolve<BelongingsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowSubOrderWindow(object viewModel)
        {
            var window = Container.Resolve<SubOrderDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowMAssetListWindow(object viewModel)
        {
            var window = Container.Resolve<MAssetListWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowReportViewWindow(object viewModel)
        {
            var window = Container.Resolve<ReportViewWindow>();
            window.DataContext = viewModel;
            window.Show();
            return window;
        }

        public Window ShowOrderUserHistoryWindow(object viewModel)
        {
            var window = Container.Resolve<OrderUserHistoryWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowOldLabelEditWindow(object viewModel)
        {
            var window = Container.Resolve<OldLabelEditWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowMAssetCostWindow(object viewModel)
        {
            var window = Container.Resolve<MAssetCostWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowAccessFileWindow(object viewModel)
        {
            var window = Container.Resolve<ShowAccessFileWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowPermEditWindow(object viewModel)
        {
            var window = Container.Resolve<PermEditDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }
        
        public Window ShowExternalOrderDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<ExternalOrderDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowAddExternalWindow(object viewModel)
        {
            var window = Container.Resolve<AddExternalOrderWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowSupplierWindow(object viewModel)
        {
            var window = Container.Resolve<SupplierDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowSupplierIndentWindow(object viewModel)
        {
            var window = Container.Resolve<SupplierIndentWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowAccountDocMainWindow(object viewModel)
        {
            var window = Container.Resolve<MainAccounDocWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowInsuranceManageWindow(object viewModel)
        {
            var window = Container.Resolve<InsuranceManageWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowCommodityDetailsWindow(object viewModel)
        {
            var window = Container.Resolve<CommodityDetailsWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowCommoditySplitWindow(object viewModel)
        {
            var window = Container.Resolve<CommoditySplitWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }
        public Window ShowStoreBillToDocumentWindow(object viewModel)
        {
            var window = Container.Resolve<StoreBillToDocumentWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowAssetBuyWindow(object viewModel)
        {
            var window = Container.Resolve<AddBuyAssetWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowHelpWindow(object viewModel)
        {
            var window = Container.Resolve<HelpWindow>();
            window.DataContext = viewModel;
            window.Show();
            return window;
        }

        public Window ShowStoreBillMAssetEditWindow(object viewModel)
        {
            var window = Container.Resolve<StoreBillMAssetEdtiWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowAddIndentReturnRequestWindow(object viewModel)
        {
            var window = Container.Resolve<AddReturnIndentRequestWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowSupplierUploadWindow(object viewModel)
        {
            var window = Container.Resolve<SupplierProFormaUploadWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }

        public Window ShowTrenderOffersWindow(object viewModel)
        {
            var window = Container.Resolve<TrenderOffersWindow>();
            window.DataContext = viewModel;
            window.ShowDialog();
            return window;
        }
    }
}
