
namespace Bska.Client.UI.ViewModels.MunitionViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    public sealed class RecivetTrenderToSupplierViewModel : BaseViewModel
    {
        #region ctor

        public RecivetTrenderToSupplierViewModel(IUnityContainer container)
        {
            this._container = container;
            this._orderService = _container.Resolve<IOrderService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._collection = new ObservableCollection<SubOrderModel>();
            this.RecivedeIndentFilteredView = new CollectionViewSource { Source = Collection }.View;
            this.initalizObj();
            this.initalizCommands();
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
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }
        public ICollectionView RecivedeIndentFilteredView { get; set; }

        public ObservableCollection<SubOrderModel> Collection
        {
            get { return _collection; }
        }
        
        public PersianDate FromDate
        {
            get { return GetValue(() => FromDate); }
            set
            {
                SetValue(() => FromDate, value);
            }
        }

        public PersianDate ToDate
        {
            get { return GetValue(() => ToDate); }
            set
            {
                SetValue(() => ToDate, value);
            }
        }
        public SubOrderModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.recivedIndentFilter();
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.AddDays(1).PersianDateTime();
            this.Units = _unitService.Queryable().ToList();
            this.getRecivedTrenderOfferAsync();
        }

        private async void getRecivedTrenderOfferAsync()
        {
            _collection.Clear();
            bool isManager = false;
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                isManager = true;
            }
            await Task.Run(() =>
            {
                _orderService.GetTrenderSubOrdersForSupplier(isManager, UserLog.UniqueInstance.LogedUser.UserId,
                    FromDate.ToDateTime(), ToDate.ToDateTime()).ForEach(tr =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _collection.Add(tr);
                    });
                });
            });
        }

        private void recivedIndentFilter()
        {
            this.RecivedeIndentFilteredView.Filter = (obj) =>
            {
                var oM = obj as SubOrderModel;
                return oM.SubOrderId.ToString().Contains(SearchCriteria)
                    || oM.StuffName.StartsWith(SearchCriteria);
            };
        }

        private void showOrderDetails(object parameter)
        {
            var so = parameter as SubOrderModel;
            if (so == null) return;
            this.Selected = so;
            Mouse.SetCursor(Cursors.Wait);
            var sod = _orderService.Query(o => o.OrderId == so.OrderId).Include(o => o.OrderDetails)
                .Select().SelectMany(o => o.OrderDetails).Single(od => od.OrderDetialsId == so.OrderDetailsId);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            viewModel.Units = this.Units;
            viewModel.CurrentOrderDetails = sod;
            _navigationService.ShowOrderDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void showProFormaWindow(object parameter)
        {
            var so = parameter as SubOrderModel;
            if (so == null) return;
            this.Selected = so;
            if (so.SellerId.HasValue)
            {
                Mouse.SetCursor(Cursors.Wait);
                StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
                var trenderOffer = _orderService.GetSubOrderTrenderOffers(so.SellerId.Value);
                bool isConfirmed = _orderService.IsTrenderOffersConfirmed(so.SubOrderId);
                var viewModel = new SupplierProFormaUploadViewModel(_container, trenderOffer, isConfirmed);
                _navigationService.ShowSupplierUploadWindow(viewModel);
                Mouse.SetCursor(Cursors.Arrow);
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            }
        }
        #endregion

        #region commands

        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand ProFormaCommand { get; private set; }

        private void initalizCommands()
        {
            OrderDetailsCommand = new MvvmCommand(
              (parameter) => { this.showOrderDetails(parameter); },
              (parameter) => { return true; }
              );

           SearchCommand= RefreshCommand = SearchCommand = new MvvmCommand(
                 (parameter) => { this.getRecivedTrenderOfferAsync();
                 },
                 (parameter) => { return true; }
                );

            ProFormaCommand= new MvvmCommand(
              (parameter) => { this.showProFormaWindow(parameter); },
              (parameter) => { return true; }
              );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IOrderService _orderService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitService _unitService;
        private readonly IPersonService _personService;
        private readonly ObservableCollection<SubOrderModel> _collection;

        #endregion

    }
}
