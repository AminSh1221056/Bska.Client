
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

    public sealed class BelongingsListViewModel : BaseViewModel
    {
        #region ctor

        public BelongingsListViewModel(IUnityContainer container,UnConsumption currentAsset)
        {
            this._container = container;
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this.CurrentAsset = currentAsset;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public UnConsumption CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            set
            {
                SetValue(() => CurrentAsset, value);
            }
        }

        public List<Belonging> Belongings
        {
            get { return GetValue(() => Belongings); }
            set
            {
                SetValue(() => Belongings, value);
            }
        }

        public Belonging Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            if (CurrentAsset == null) return;
            Belongings = _movableAssetService.Queryable().OfType<UnConsumption>().Where(x => x.AssetId == CurrentAsset.AssetId)
                .SelectMany(x => x.Belongings).ToList();
        }

        private void report()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.MAssetBelongingListReport(CurrentAsset.AssetId,CurrentAsset.Name);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand ReportCommand { get; private set; }

        private void initializCommands()
        {
            ReportCommand = new MvvmCommand(
                (parameter) => { this.report(); },
                (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IMovableAssetService _movableAssetService;
        private readonly INavigationService _navigationService;

        #endregion
    }
}
