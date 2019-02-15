
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.API;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Microsoft.Practices.Unity;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Data.Entity.Infrastructure;
    using System;

    public sealed class TrenderOfferRecivedRequestViewModel  :BaseViewModel
    {
        #region ctor

        public TrenderOfferRecivedRequestViewModel(IUnityContainer container,bool forMunition)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._collection = new ObservableCollection<SubOrderModel>();
            this.RecivedeIndentFilteredView = new CollectionViewSource { Source = Collection }.View;
            this._forMunition = forMunition;
            this.initializObj();
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

        public Dictionary<string,object> Suppliers
        {
            get { return GetValue(() => Suppliers); }
            set
            {
                SetValue(() => Suppliers, value);
            }
        }

        public Dictionary<string,object> SelectedSuppliers
        {
            get { return GetValue(() => SelectedSuppliers); }
            set
            {
                SetValue(() => SelectedSuppliers, value);
            }
        }

        public ICollectionView RecivedeIndentFilteredView { get; set; }

        public ObservableCollection<SubOrderModel> Collection
        {
            get { return _collection; }
        }

        public Dictionary<string, object> RecivedTypes
        {
            get { return GetValue(() => RecivedTypes); }
            set
            {
                SetValue(() => RecivedTypes, value);
            }
        }

        public Dictionary<string, object> SelectedRecivedType
        {
            get { return GetValue(() => SelectedRecivedType); }
            set
            {
                SetValue(() => SelectedRecivedType, value);
            }
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

        private void initializObj()
        {
            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.AddDays(1).PersianDateTime();
            this.Units = _unitService.Queryable().ToList();
            this.initSearchTypes();
            this.GetRecivedIndentsAsync();
        }

        private void initSearchTypes()
        {
            if (_forMunition)
            {
                RecivedTypes = new Dictionary<string, object> { { "سفارش های رسیده", "A001" }
                ,{ "سفارش تکمیل شده", "A002" }};

                SelectedRecivedType = new Dictionary<string, object> { { "سفارش های رسیده", "A001" } };

                Suppliers = new Dictionary<string, object>();
                SelectedSuppliers = new Dictionary<string, object>();

                _personService.GetUsersByPermission(PermissionsType.Supplier).ForEach(us =>
                {
                    Suppliers.Add(us.FullName, us.UserId);
                });
            }
            else
            {

                RecivedTypes = new Dictionary<string, object> { { "درخواست های رسیده", "A001" }
                ,{ "درخواست های تایید شده", "A002" },{ "درخواست های تایید نشده", "A003" }};

                SelectedRecivedType = new Dictionary<string, object> { { "درخواست های رسیده", "A001" } };
            }
        }

        private async void GetRecivedIndentsAsync()
        {
            Mouse.SetCursor(Cursors.Wait);
            _collection.Clear();
            bool isManager = false;
            string identity = "";

            HashSet<SubOrderState> reserveStates = new HashSet<SubOrderState>();
            if (_forMunition)
            {
                if (this.SelectedRecivedType.ContainsValue("A001"))
                {
                    reserveStates.Add(SubOrderState.Confirm);
                }

                if (this.SelectedRecivedType.ContainsValue("A002"))
                {
                    reserveStates.Add(SubOrderState.Deliviry);
                }
                
                await Task.Run(() =>
                {
                    _orderService.GetTrenderSubOrders(true, identity, reserveStates,
                        FromDate.ToDateTime(), ToDate.ToDateTime())
                    .ToList().ForEach(so =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _collection.Add(so);
                        });
                    });
                });
            }
            else
            {
                if (Thread.CurrentPrincipal.IsInRole("Manager"))
                {
                    isManager = true;
                }
                else
                {
                    identity = UserLog.UniqueInstance.LogedUser.UserId.ToString();
                }

                if (this.SelectedRecivedType.ContainsValue("A002"))
                {
                    reserveStates.Add(SubOrderState.Confirm);
                }

                if (this.SelectedRecivedType.ContainsValue("A003"))
                {
                    reserveStates.Add(SubOrderState.Reject);
                }

                if (this.SelectedRecivedType.ContainsValue("A001"))
                {
                    reserveStates.Add(SubOrderState.None);
                }

                await Task.Run(() =>
                {
                    _orderService.GetTrenderSubOrders(isManager, identity, reserveStates,
                        FromDate.ToDateTime(), ToDate.ToDateTime())
                    .ToList().ForEach(so =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _collection.Add(so);
                        });
                    });
                });
            }
            
            Mouse.SetCursor(Cursors.Arrow);
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

        private void showTrenderOfferWindow(object parameter)
        {
            var so = parameter as SubOrderModel;
            if (so == null) return;
            this.Selected = so;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new TrenderOfferViewModel(_container, so.SubOrderId, _forMunition);
            _navigationService.ShowTrenderOffersWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void confirmSubOrder(object parameter,SubOrderState state)
        {
            var som = parameter as SubOrderModel;
            if (som == null) return;

            if (_forMunition)
            {
                if (this.SelectedSuppliers.Count <= 0)
                {
                    _dialogService.ShowAlert("خطا", "هیچ کارپردازی انتخاب نشده است");
                    return;
                }
            }

            this.Selected = som;

            bool confrim = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confrim)
            {
                Mouse.SetCursor(Cursors.Wait);
                var order = _orderService.Query(x => x.OrderId == som.OrderId)
            .Include(o => o.OrderDetails).Select().Single();
                var orderDetails = order.OrderDetails.Single(od => od.OrderDetialsId == som.OrderDetailsId);
                var subOrders = _orderService.GetSubOrders(orderDetails.OrderDetialsId);
                var subOrder = subOrders.Single(sb => sb.SubOrderId == som.SubOrderId);

                if (_forMunition)
                {
                    this.SelectedSuppliers.ForEach(sp =>
                    {
                        int temp = Convert.ToInt32(sp.Value);

                        if (temp > 0)
                        {
                            var newTrender = new SupplierTrenderOffer
                            {
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                SupplierId=temp,
                            };
                            subOrder.SupplierTrenderOffers.Add(newTrender);
                        }
                    });
                }
                else
                {
                    subOrder.ObjectState = ObjectState.Modified;
                    subOrder.State = state;
                }

                orderDetails.SubOrders.Add(subOrder);
                order.OrderDetails.Add(orderDetails);

                _orderService.InsertOrUpdateGraph(order);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.initSearchTypes();
                    this.GetRecivedIndentsAsync();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }
        
        #endregion

            #region commands

        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand ProFormaCommand { get; private set; }
        private void initalizCommands()
        {
            OrderDetailsCommand = new MvvmCommand(
              (parameter) => { this.showOrderDetails(parameter); },
              (parameter) => { return true; }
              );

            RefreshCommand=SearchCommand = new MvvmCommand(
                 (parameter) => { this.GetRecivedIndentsAsync(); },
                 (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
                 (parameter) => { this.confirmSubOrder(parameter,SubOrderState.Confirm); },
                 (parameter) => { return true; }
                );

            RejectCommand = new MvvmCommand(
                (parameter) => { this.confirmSubOrder(parameter, SubOrderState.Reject); },
                (parameter) => { return true; }
               );

            ProFormaCommand = new MvvmCommand(
                 (parameter) => { this.showTrenderOfferWindow(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly IPersonService _personService;
        private readonly ObservableCollection<SubOrderModel> _collection;
        private readonly bool _forMunition;

        #endregion

    }
}
