using Bska.Client.Common;
using Bska.Client.Data.Service;
using Bska.Client.Domain.Entity.OrderEntity;
using Bska.Client.UI.API;
using Bska.Client.UI.Helper;
using Bska.Client.UI.Services;
using Bska.Client.UI.ViewModels.OrderViewModel;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    public sealed class ProcedingOrderViewModel : BaseViewModel
    {
        #region ctor

        public ProcedingOrderViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._orderService = _container.Resolve<IOrderService>();
            this._navigationService = _container.Resolve<INavigationService>();

            this._ordrs = new ObservableCollection<Order>();
            this.OrderFilteredView = new CollectionViewSource { Source = Orders }.View;

            this.initializObj();
            this.initalizCommand();
        }

        #endregion

        #region properties
        public Window Window
        {
            get { return GetValue(() => Window); }
            set
            {
                SetValue(() => Window, value);
            }
        }

        public ICollectionView OrderFilteredView { get; set; }

        public string OrderTextSearch
        {
            get { return GetValue(() => OrderTextSearch); }
            set
            {
                SetValue(() => OrderTextSearch, value);
                this.searchOrders();
            }
        }

        public ObservableCollection<Order> Orders
        {
            get { return _ordrs; }
        }

        public Order OMSelected
        {
            get { return GetValue(() => OMSelected); }
            set
            {
                SetValue(() => OMSelected, value);
                this.getOrderHistory();
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
        #endregion

        #region methods

        private async void initializObj()
        {
            _ordrs.Clear();
            await this.getOrdersAsync();
        }

        private Task getOrdersAsync()
        {
            Task ts = new Task(() =>
            {
                var orders = _orderService.Query(x => x.Status == OrderStatus.StuffHonest && x.OrderType == OrderType.Procceding)
                .Include(o => o.Person).Include(o => o.OrderDetails).Include(o => o.MovableAssets)
              .OrderBy(o => o.OrderByDescending(od => od.OrderDate)).Select().ToList();
                orders.ForEach(o =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _ordrs.Add(o);
                    });
                });
            });
            ts.Start();
            return ts;
        }

        private void searchOrders()
        {
            OrderFilteredView.Filter = (obj) =>
            {
                var od = obj as Order;
                return od.OrderId.ToString().StartsWith(OrderTextSearch);
            };
        }

        private void getOrderHistory()
        {
            if (OMSelected != null)
            {
                OrderUserHistories = _orderService.GetUserHistories(OMSelected.OrderDetails.First().OrderDetialsId)
                     .Where(ou => ou.UserDecision).ToList();
            }
        }

        private void showOrderEdit(object parameter)
        {
            var od = parameter as Order;
            if (od == null) return;
            Mouse.SetCursor(Cursors.Wait);
            OMSelected = od;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderEditViewModel(_container, od) { EnableEdit = false };
            _navigationService.ShowOrderEditWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand RecivedOrderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        private void initalizCommand()
        {
            RecivedOrderCommand = new MvvmCommand(
               (parameter) => { this.showOrderEdit(parameter); },
               (parameter) => { return true; }
               );

            RefreshCommand = new MvvmCommand(
            async (parameter) =>
            {
                await this.getOrdersAsync();
            },
            (parameter) => { return true; }
            );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IOrderService _orderService;

        private readonly ObservableCollection<Order> _ordrs;
        #endregion
    }
}
