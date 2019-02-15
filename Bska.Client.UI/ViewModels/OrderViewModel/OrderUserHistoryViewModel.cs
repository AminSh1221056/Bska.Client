
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using API;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Microsoft.Practices.Unity;
    using Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    public sealed class OrderUserHistoryViewModel : BaseViewModel
    {
        #region ctor

        public OrderUserHistoryViewModel(IUnityContainer container)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public OrderDetails CurrentOrder
        {
            get { return GetValue(() => CurrentOrder); }
            set
            {
                SetValue(() => CurrentOrder, value);
            }
        }

        public List<OrderUserHistory> OrderUserHistories
        {
            get { return GetValue(() => OrderUserHistories); }
            set
            {
                SetValue(() => OrderUserHistories, value);
            }
        }

        #endregion;

        #region methods

        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.OrderUserHistoryReports(CurrentOrder.OrderDetialsId);
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

        #endregion;

        #region fields

        private readonly IUnityContainer _container;
        private readonly INavigationService _navigationService;

        #endregion;
    }
}
