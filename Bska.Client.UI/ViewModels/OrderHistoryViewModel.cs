
namespace Bska.Client.UI.ViewModels
{
    using API;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    public sealed class OrderHistoryViewModel : BaseViewModel
    {
        #region ctor

        public OrderHistoryViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public MovableAssetModel CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            set
            {
                SetValue(() => CurrentAsset, value);
            }
        }

        public List<Order> Orders
        {
            get { return GetValue(() => Orders); }
            set
            {
                SetValue(() => Orders, value);
            }
        }

        #endregion

        #region methods
        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.mAssetItemsReports(CurrentAsset.Name, CurrentAsset.GetType().Name, CurrentAsset.AssetId, CurrentAsset.Label,2);
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
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        #endregion
    }
}
