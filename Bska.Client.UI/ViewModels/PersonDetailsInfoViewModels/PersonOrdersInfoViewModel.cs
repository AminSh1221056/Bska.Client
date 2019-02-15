
namespace Bska.Client.UI.ViewModels.PersonDetailsInfoViewModels
{
    using Bska.Client.Data.Service;
    using Bska.Client.UI.Services;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using Helper;
    using System.Threading.Tasks;

    public sealed class PersonOrdersInfoViewModel : BaseViewModel
    {
        #region ctor

        public PersonOrdersInfoViewModel(IUnityContainer container, int personId)
        {
            this._container = container;
            this._orderService = _container.Resolve<IOrderService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._currentPersonId = personId;
            this._collection = new ObservableCollection<OrderDetailsModel>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this.OrderFilteredView = new CollectionViewSource { Source = PersonOrders }.View;
            this._orderTypes = new HashSet<OrderType>();
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties

        public ObservableCollection<OrderDetailsModel> PersonOrders
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

        public Dictionary<string, OrderStatus> OrderStatusItems
        {
            get { return GetValue(() => OrderStatusItems); }
            set
            {
                SetValue(() => OrderStatusItems, value);
            }
        }

        public OrderStatus OrderStatus
        {
            get { return GetValue(() => OrderStatus); }
            set
            {
                SetValue(() => OrderStatus, value);
            }
        }

        public Boolean ChDisplacement
        {
            get { return GetValue(() => ChDisplacement); }
            set
            {
                SetValue(() => ChDisplacement, value);
                if (value)
                {
                    _orderTypes.Add(OrderType.Displacement);
                }
                else
                {
                    _orderTypes.Remove(OrderType.Displacement);
                }
            }
        }

        public Boolean ChInternalOrder
        {
            get { return GetValue(() => ChInternalOrder); }
            set
            {
                SetValue(() => ChInternalOrder, value);
                if (value)
                {
                    _orderTypes.Add(OrderType.InternalRequest);
                }
                else
                {
                    _orderTypes.Remove(OrderType.InternalRequest);
                }
            }
        }

        public Boolean ChProcceding
        {
            get { return GetValue(() => ChProcceding); }
            set
            {
                SetValue(() => ChProcceding, value);
                if (value)
                {
                    _orderTypes.Add(OrderType.Procceding);
                }
                else
                {
                    _orderTypes.Remove(OrderType.Procceding);
                }
            }
        }

        public Boolean ChInternalTransfer
        {
            get { return GetValue(() => ChInternalTransfer); }
            set
            {
                SetValue(() => ChInternalTransfer, value);
                if (value)
                {
                    _orderTypes.Add(OrderType.InternalTransfer);
                }
                else
                {
                    _orderTypes.Remove(OrderType.InternalTransfer);
                }
            }
        }
        public OrderDetailsModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }
        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> StrategyCollection
        {
            get { return _strategyCollection; }
        }
        public EmployeeDesignTreeViewModel OrganizSelected
        {
            get { return GetValue(() => OrganizSelected); }
            set
            {
                SetValue(() => OrganizSelected, value);
                if (value != null)
                {
                    _allOrganizId.Clear();
                    this.GetAllOrganizId(value.BuildingDesignCurrent);
                    this.FilterByBuildingDesign();
                }
            }
        }

        public EmployeeDesignTreeViewModel StrategySelected
        {
            get { return GetValue(() => StrategySelected); }
            set
            {
                SetValue(() => StrategySelected, value);
                if (value != null)
                {
                    _allStrategyId.Clear();
                    this.GetAllStrategyId(value.BuildingDesignCurrent);
                    this.FilterByBuildingDesign();
                }
            }
        }

        public Boolean NestPropertyView
        {
            get { return GetValue(() => NestPropertyView); }
            set
            {
                SetValue(() => NestPropertyView, value);
            }
        }

        public Boolean OrganizFiltering
        {
            get { return GetValue(() => OrganizFiltering); }
            set
            {
                SetValue(() => OrganizFiltering, value);
                if (value)
                {
                    _filteNo = 2;
                    StrategySelected = null;
                }
            }
        }

        public Boolean StrategyFiltering
        {
            get { return GetValue(() => StrategyFiltering); }
            set
            {
                SetValue(() => StrategyFiltering, value);
                if (value)
                {
                    _filteNo = 3;
                    OrganizSelected = null;
                }
            }
        }

        public ICollectionView OrderFilteredView { get; set; }

        public List<PersonModel> Persons
        {
            get { return GetValue(() => Persons); }
            set
            {
                SetValue(() => Persons, value);
            }
        }

        public PersonModel SelectedPerson
        {
            get { return GetValue(() => SelectedPerson); }
            set
            {
                SetValue(() => SelectedPerson, value);
            }
        }

        public Person CurrentPerson
        {
            get { return GetValue(() => CurrentPerson); }
            set
            {
                SetValue(() => CurrentPerson, value);
            }
        }

        public bool ChGroupView
        {
            get { return GetValue(() => ChGroupView); }
            set
            {
                SetValue(() => ChGroupView, value);
                FilterByStuffTyp();
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
        #endregion

        #region methods

        private async void initalizObj()
        {
            FromDate = GlobalClass._Today.AddMonths(-6).PersianDateTime();
            ToDate = GlobalClass._Today.PersianDateTime().AddDays(1);
            OrderStatusItems = new Dictionary<string, OrderStatus> { { "کل وضعیت ها", OrderStatus.None }, { "در دست اقدام", OrderStatus.OrganizManagerConfirm }, { "مدیریت ساختمان", OrderStatus.ManagerConfirm }, { "امین اموال", OrderStatus.StuffHonest },
            { "سفارش", OrderStatus.SubOrder }, { "تحویل", OrderStatus.Deliviry }, { "رد درخواست", OrderStatus.Reject } };
            OrderStatus = OrderStatus.None;
            ChInternalOrder=ChInternalTransfer=ChProcceding=ChDisplacement = true;

            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager)
            {
                Persons = _personService.Queryable().Select(p => new PersonModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    NationalId = p.NationalId,
                    PersonId = p.PersonId
                }).ToList();
            }
            else
            {
                Persons = _personService.Queryable()
                   .Where(x => x.PersonId == _currentPersonId).Select(x => new PersonModel
                   {
                       FullName = x.FirstName + " " + x.LastName,
                       NationalId = x.NationalId,
                       PersonId = x.PersonId
                   }).ToList();
            }

            if (Persons.Count <= 0) return;

            SelectedPerson = Persons.SingleOrDefault(p => p.PersonId == _currentPersonId);
            this.Units = _unitService.Queryable().ToList();
            await this.GetParentNodeAsync();
            ChGroupView = false;
        }

        private Task GetParentNodeAsync()
        {
            Task ts = new Task(() =>
            {
               var _allOrganiz = _employeeService.GetParentNode(1).ToList();
               var orgItems = _allOrganiz.Where(x => x.ParentNode == null);
               var _allStrategy = _employeeService.GetParentNode(2).ToList();
               var stgyItems = _allStrategy.Where(x => x.ParentNode == null);

                DispatchService.Invoke(() =>
                {
                    StrategySelected = null;
                    OrganizSelected = null;
                    _organizCollection.Clear();
                    _strategyCollection.Clear();

                    foreach (var org in orgItems)
                    {
                        _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null));
                    }

                    foreach (var stgy in stgyItems)
                    {
                        _strategyCollection.Add(new EmployeeDesignTreeViewModel(stgy, null, null));
                    }
                });
            });
            ts.Start();
            return ts;
        }

        private void GetAllOrganizId(EmployeeDesign selectedItem)
        {
            if (selectedItem.ChildNode.Count > 0)
            {
                foreach (var k in selectedItem.ChildNode)
                {
                    this.GetAllOrganizId(k);
                }
            }

            _allOrganizId.Add(selectedItem.BuidldingDesignId);
        }

        private void GetAllStrategyId(EmployeeDesign selectedItem)
        {
            if (selectedItem.ChildNode.Count > 0)
            {
                foreach (var k in selectedItem.ChildNode)
                {
                    this.GetAllStrategyId(k);
                }
            }

            _allStrategyId.Add(selectedItem.BuidldingDesignId);
        }

        private void FilterByBuildingDesign()
        {
            if (NestPropertyView)
            {
                if (OrganizFiltering)
                {
                    if (OrganizSelected == null)
                    {
                        return;
                    }

                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = (OrderDetailsModel)obj;
                        _filteNo = 5;
                        return order.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId;
                    };
                }
                else if (StrategyFiltering)
                {
                    if (StrategySelected == null)
                    {
                        return;
                    }

                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = (OrderDetailsModel)obj;
                        _filteNo = 6;
                        int stg = order.StrategyId ?? -1;
                        return  stg== StrategySelected.BuildingDesignCurrent.BuidldingDesignId;
                    };
                }
                else
                {
                    if (OrganizSelected == null || StrategySelected == null)
                    {
                        return;
                    }

                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = (OrderDetailsModel)obj;
                        _filteNo = 4;
                        int stg = order.StrategyId ?? -1;
                        return (order.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId
                            && stg == StrategySelected.BuildingDesignCurrent.BuidldingDesignId);
                    };
                }
            }
            else
            {
                if (OrganizFiltering)
                {
                    if (OrganizSelected == null)
                    {
                        return;
                    }

                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = (OrderDetailsModel)obj;
                        _filteNo = 2;
                        return _allOrganizId.Contains(order.OrganizId.Value);
                    };
                }
                else if (StrategyFiltering)
                {
                    if (StrategySelected == null)
                    {
                        return;
                    }

                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = (OrderDetailsModel)obj;
                        _filteNo = 3;
                        int stg = order.StrategyId ?? -1;
                        return _allStrategyId.Contains(stg);
                    };
                }
                else
                {
                    if (OrganizSelected == null || StrategySelected == null)
                    {
                        return;
                    }

                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = (OrderDetailsModel)obj;
                        _filteNo = 1;
                        int stg = order.StrategyId ?? -1;
                        return (_allStrategyId.Contains(stg) && _allOrganizId.Contains(order.OrganizId.Value));
                    };
                }
            }
        }
        
        private void FilterByStuffTyp()
        {
            if (SelectedPerson == null) return;
            _collection.Clear();

            Mouse.SetCursor(Cursors.Wait);
            CurrentPerson = _personService.Find(SelectedPerson.PersonId);
            DateTime date1 = FromDate.ToDateTime();
            DateTime dt = ToDate.ToDateTime();
            DateTime date2 = new DateTime(dt.Year, dt.Month, dt.Day, GlobalClass._Today.Hour, GlobalClass._Today.Minute, GlobalClass._Today.Second);
           _allOrder= _orderService.GetMyOrdersForManage(false, _orderTypes.ToArray(), SelectedPerson.PersonId);

            if (ChGroupView)
            {
                _allOrder = (from c in _allOrder
                             group c by new { c.UnitId, c.kalaUid } into g
                             where g.Count() >= 1
                             select new OrderDetailsModel
                             {
                                 kalaUid = g.Key.kalaUid,
                                 Num = g.Sum(x => x.Num),
                                 UnitId = g.Key.UnitId,
                                 PersonId = SelectedPerson.PersonId,
                                 OrganizId = g.First().OrganizId,
                                 StrategyId = g.First().StrategyId,
                                 DueDate=g.First().DueDate,
                                 OrderDate=g.First().OrderDate,
                                 StuffName=g.First().StuffName,
                                 StuffType=g.First().StuffType,
                                 OrderStatus=OrderStatus.None,
                                 PersonName=SelectedPerson.FullName,
                                 OrderType=g.First().OrderType,
                                 NationalId=SelectedPerson.NationalId,
                                 OrderDetailsId=g.First().OrderDetailsId,
                                 OrderId=g.First().OrderId
                             }).AsEnumerable();

            }

            if (OrderStatus != 0)
            {
                _allOrder = _allOrder.Where(x =>x.OrderStatus == OrderStatus);
            }

            _allOrder.ToList().ForEach(o =>
            {
                _collection.Add(o);
            });
            this.FilterByBuildingDesign();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showOrderEdit(IList<object> parameter)
        {
            if (!(parameter[1] is Window)) return;
            var orderModel = parameter[0] as OrderDetailsModel;
            if (orderModel == null) return;
            Mouse.SetCursor(Cursors.Wait);
            Selected = orderModel;
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", parameter[1] as Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            var od = _orderService.GetOrderDetails(orderModel.OrderDetailsId);
            viewModel.Units = _unitService.Queryable().Where(u=>u.UnitId==orderModel.UnitId).ToList();
            viewModel.CurrentOrderDetails = od;
            viewModel.AllOrderDetails = _orderService.Queryable().Where(x => x.OrderId == orderModel.OrderId)
                .SelectMany(o => o.OrderDetails).ToList();
            _navigationService.ShowOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", parameter[1] as Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        private void initalizCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.FilterByStuffTyp(); },
                (parameter) => { return true; }
                );

            EditCommand = new MvvmCommand(
            (parameter) => { this.showOrderEdit(parameter as IList<object>); },
            (parameter) => { return true; }
            );
        }

        #endregion

        #region fields

        private readonly int _currentPersonId;
        private readonly IUnityContainer _container;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private HashSet<StuffType> _mAssetTypes = new HashSet<StuffType>();
        private List<int> _allOrganizId = new List<int>();
        private List<int> _allStrategyId = new List<int>();
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private readonly ObservableCollection<OrderDetailsModel> _collection;
        private readonly HashSet<OrderType> _orderTypes;
        private IEnumerable<OrderDetailsModel> _allOrder;
        private int _filteNo = 7;

        #endregion
    }
}
