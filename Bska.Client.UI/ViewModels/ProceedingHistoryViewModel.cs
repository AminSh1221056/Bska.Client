
namespace Bska.Client.UI.ViewModels
{
    using API;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    public sealed class ProceedingHistoryViewModel : BaseViewModel
    {
        #region ctor

        public ProceedingHistoryViewModel(IUnityContainer container,MovableAsset currentAsset)
        {
            this._container = container;
            this._proceedingService = _container.Resolve<IProceedingService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this.CurrentAsset = currentAsset;
            this.initializObj(); 
            this.initalizCommand();
        }

        #endregion

        #region properties

        public MovableAsset CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            set
            {
                SetValue(() => CurrentAsset, value);
            }
        }

        public List<AssetProceeding> Proceedings
        {
            get {return GetValue(() => Proceedings); }
            set
            {
                SetValue(() => Proceedings, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            Proceedings = _proceedingService.getAssetProceedingsByAssetId(CurrentAsset.AssetId).ToList();
        }
        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.mAssetItemsReports(CurrentAsset.Name, CurrentAsset.GetType().Name, CurrentAsset.AssetId, CurrentAsset.Label,3);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand ReportCommand { get; private set; }

        private void initalizCommand()
        {
            ReportCommand = new MvvmCommand(
                (parameter) => { this.reports(); },
                (parameter) =>
                {
                    return true;
                }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IProceedingService _proceedingService;
        private readonly INavigationService _navigationService;

        #endregion
    }
}
