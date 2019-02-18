
namespace Bska.Client.UI.Services
{
    using System.Windows;
    public interface INavigationService
    {
        bool ConfirmClose();
        Window ShowStartWindow();
        Window ShowMainWindow(object viewModel);
        Window ShowConfigWindow();
        Window ShowAddInfoWindow(object viewModel);
        Window ShowSplitWindow(object viewModel);
        Window ShowMAssetDetailsWindow(object viewModel);
        Window ShowOrderDetailsWindow(object viewModel);
        Window ShowOrderTrackWindow(object viewModel);
        Window ShowOrderEditWindow(object viewModel);
        Window ShowIndentWindow(object viewModel);
        Window ShowSuggestWindow(object viewModel);
        Window ShowDisplacementIndentWindow(object viewModel);
        Window ShowStoreIndentConfirmWindow(object viewModel);
        Window ShowPersonDetailsWindow(object viewModel);
        Window ShowDocumentShowWindow(object viewModel);
        Window ShowOrderHistoryWindow(object viewModel);
        Window ShowStoreAssetDetailsWindow(object viewModel);
        Window ShowProceedingDetailsWindow(object viewModel);
        Window ShowProceedingHistoryWindow(object viewModel);
        Window ShowDocumentHistoryWindow(object viewModel);
        Window ShowStoreBillDetailsWindow(object viewModel);
        Window ShowMeterBillWindow(object viewModel);
        Window ShowParentAssetForBelongingWindow(object viewModel);
        Window ShowBelongingsWindow(object viewModel);
        Window ShowSubOrderWindow(object viewModel);
        Window ShowMAssetListWindow(object viewModel);
        Window ShowReportViewWindow(object viewModel);
        Window ShowOrderUserHistoryWindow(object viewModel);
        Window ShowOldLabelEditWindow(object viewModel);
        Window ShowMAssetCostWindow(object viewModel);
        Window ShowAccessFileWindow(object viewModel);
        Window ShowPermEditWindow(object viewModel);
        Window ShowExternalOrderDetailsWindow(object viewModel);
        Window ShowAddExternalWindow(object viewModel);
        Window ShowSupplierWindow(object viewModel);
        Window ShowSupplierIndentWindow(object viewModel);
        Window ShowAccountDocMainWindow(object viewModel);
        Window ShowInsuranceManageWindow(object viewModel);
        Window ShowCommodityDetailsWindow(object viewModel);
        Window ShowCommoditySplitWindow(object viewModel);
        Window ShowStoreBillToDocumentWindow(object viewModel);
        Window ShowAssetBuyWindow(object viewModel);
        Window ShowHelpWindow(object viewModel);
        Window ShowStoreBillMAssetEditWindow(object viewModel);
        Window ShowAddIndentReturnRequestWindow(object viewModel);
        Window ShowSupplierUploadWindow(object viewModel);
        Window ShowTrenderOffersWindow(object viewModel);
    }
}
