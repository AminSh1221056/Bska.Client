
namespace Bska.Client.UI.ViewModels.MunitionViewModel
{
    using API;
    using Bska.Client.Domain.Entity;
    using Common;
    using Data.Service;
    using Helper;
    using Microsoft.Practices.Unity;
    using OrderViewModel;
    using Repository.Model;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    public sealed class SupplierHistoryViewModel : BaseViewModel
    {
        #region ctor

        public SupplierHistoryViewModel(IUnityContainer container,Users currentSupplier=null)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._collection = new ObservableCollection<SubOrderModel>();
            this.RecivedeIndentFilteredView = new CollectionViewSource { Source = Collection }.View;
            this._personService = _container.Resolve<IPersonService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._orderService = _container.Resolve<IOrderService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this.initializObj(currentSupplier);
            this.initializCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;
            set;
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
                this.SearchOrder();
            }
        }

        public List<Users> SupplierList
        {
            get { return GetValue(() => SupplierList); }
            set
            {
                SetValue(() => SupplierList, value);
            }
        }

        public Users SelectedSupplier
        {
            get { return GetValue(() => SelectedSupplier); }
            set
            {
                SetValue(() => SelectedSupplier, value);
                if(value!=null)
                _collection.Clear();
            }
        }

        public Boolean AllSubOrder
        {
            get { return GetValue(() => AllSubOrder); }
            set
            {
                SetValue(() => AllSubOrder, value);
                allSubOrdersShow();
            }
        }
        
        public Boolean RbGroupView
        {
            get { return GetValue(() => RbGroupView); }
            set
            {
                SetValue(() => RbGroupView, value);
                if (!value)
                {
                    this.getOrders();
                }
                else
                {
                    this.GroupIndentItems();
                }
            }
        }
        public List<Seller> Sellers
        {
            get { return GetValue(() => Sellers); }
            set
            {
                SetValue(() => Sellers, value);
            }
        }

        public Dictionary<string, object> StuffTypes
        {
            get { return GetValue(() => StuffTypes); }
            set
            {
                SetValue(() => StuffTypes, value);
            }
        }

        public Dictionary<string, object> SelectedStuffType
        {
            get { return GetValue(() => SelectedStuffType); }
            set
            {
                SetValue(() => SelectedStuffType, value);
            }
        }
        #endregion

        #region methods

        private void initializObj(Users currentSupplier)
        {
            if (Thread.CurrentPrincipal.IsInRole("Manager")
               || Thread.CurrentPrincipal.IsInRole("MunitionLeader")
               || Thread.CurrentPrincipal.IsInRole("StuffHonest"))
            {
                SupplierList = _personService.GetUsers().Where(x => x.PermissionType == PermissionsType.Supplier).ToList();
            }
            else if(Thread.CurrentPrincipal.IsInRole("Supplier"))
            {
                SupplierList = _personService.GetUsers().Where(x => x.UserId == UserLog.UniqueInstance.LogedUser.UserId).ToList();
            }
            else if (Thread.CurrentPrincipal.IsInRole("StoreLeader"))
            {
                SupplierList = new List<Users> { currentSupplier };
            }

            if (currentSupplier==null)
            {
                SelectedSupplier = SupplierList.FirstOrDefault();
            }
            else
            {
                SelectedSupplier = SupplierList.Where(x => x.UserId == currentSupplier.UserId).SingleOrDefault() ;
            }
            Units = _unitService.Queryable().ToList();
            StuffTypes = new Dictionary<string, object> { { StuffType.Consumable.GetDescription(),StuffType.Consumable}, { StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};

            SelectedStuffType = new Dictionary<string, object> { { StuffType.Consumable.GetDescription(),StuffType.Consumable}, { StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};
            Sellers = _sellerService.Queryable().ToList();
            this.getOrders();
        }

        private void getOrders()
        {
            if (SelectedSupplier == null) return;
            int supplierId = SelectedSupplier.UserId;
            _subOrderItems = _orderService.GetSupplierIndentsForSupplier(supplierId,SupplierIndentState.Ongoing,true)
                      .Where(x => SelectedStuffType.ContainsValue(x.StuffType));

            allSubOrdersShow();
            if (RbGroupView)
            {
                this.GroupIndentItems();
            }
        }
        private void SearchOrder()
        {
            RecivedeIndentFilteredView.Filter = (obj) =>
            {
                var model = obj as SubOrderModel;
                return model.StuffName.StartsWith(SearchCriteria)
                    || model.SubOrderId.ToString() == SearchCriteria;
            };
        }
        
        private void allSubOrdersShow()
        {
            _collection.Clear();
            if (_subOrderItems.Count() <= 0)
            {
                return;
            }

            if (!AllSubOrder)
            {
                _subOrderItems.Where(x => x.SpState == SupplierIndentState.Ongoing).ForEach(x =>
                {
                    _collection.Add(x);
                });
            }
            else
            {
                _subOrderItems.ForEach(x =>
                {
                    _collection.Add(x);
                });
            }
        }

        private void GroupIndentItems()
        {
            Mouse.SetCursor(Cursors.Wait);
            var itemsGroup = (from item in _collection
                              group item by new { item.UnitId, item.StuffType, item.StuffName } into g
                              where g.Count() >= 1
                              select new SubOrderModel
                              {
                                  Num = g.Sum(x => x.Num),
                                  OrderDate = g.First().OrderDate,
                                  OrderId = g.First().OrderId,
                                  StuffName = g.Key.StuffName,
                                  UnitId = g.Key.UnitId,
                                  StuffType = g.Key.StuffType,
                                  SubOrderId = 1,
                              }).ToList();

            _collection.Clear();
            itemsGroup.ForEach(x =>
            {
                _collection.Add(x);
            });
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void report()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            string querySearch = null;
            if (!string.IsNullOrWhiteSpace(SearchCriteria))
            {
                querySearch = SearchCriteria;
            }
            int identity =SelectedSupplier.UserId;
            viewModel.SupplierIndentReport(SelectedStuffType.Select(x => ((int)x.Value)
            .ToString()).ToArray(), querySearch, RbGroupView,SelectedSupplier.FullName,identity,AllSubOrder);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showIndentDetails(IList<object> parameter)
        {
            var subOrder = parameter[0] as SubOrderModel;
            if (subOrder == null)
            {
                return;
            }
            string sb1 = "StoryboardFadeOut";
            string sb2 = "StoryboardFadeIn";
            var locWindow = this.Window;
            if (locWindow == null)
            {
                if (!(parameter[1] is Window)) return;
                locWindow = parameter[1] as Window;
                sb1 = "StoryboardHideWindow";
                sb2 = "StoryboardShowWindow";
            }
            Mouse.SetCursor(Cursors.Wait);
            this.Selected = subOrder;
            StoryboardManager.PlayStoryboard(sb1, locWindow);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            var od = _orderService.GetOrderDetails(Selected.OrderDetailsId);
            viewModel.CurrentOrderDetails = od;
            viewModel.AllOrderDetails = new List<Domain.Entity.OrderEntity.OrderDetails> { od};
            viewModel.Units = this.Units;
            _navigationService.ShowOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard(sb2,locWindow);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        private void initializCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.getOrders(); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                 (parameter) => { this.report(); },
                (parameter) => { return true; }
                );
            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showIndentDetails(parameter as IList<object>); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<SubOrderModel> _collection;
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly ISellerService _sellerService;
        private readonly INavigationService _navigationService;
        IEnumerable<SubOrderModel> _subOrderItems;

        #endregion
    }
}
