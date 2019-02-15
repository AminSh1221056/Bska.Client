
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public sealed class OrderManageViewModel : BaseViewModel
    {
        #region ctor

        public OrderManageViewModel(IUnityContainer container)
        {
            _container = container;
            _dialogservice = _container.Resolve<IDialogService>();
            _navigationService = _container.Resolve<INavigationService>();
            _buildingService = _container.Resolve<IBuildingService>();
            _personService = _container.Resolve<IPersonService>();
            _movableAssetService = _container.Resolve<IMovableAssetService>();
            _employeeService = _container.Resolve<IEmployeeService>();
            _orderService = _container.Resolve<IOrderService>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            _collection = new ObservableCollection<OrderDetailsModel>();
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;
            set;
        }
        
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
                if (value != null)
                {
                    SetValue(() => SelectedPerson, value);
                }
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
        
        public ObservableCollection<OrderDetailsModel> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged("Collection");
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
                    OrganizSelected = null;
                }
            }
        }

        public int EditType
        {
            get { return GetValue(() => EditType); }
            set
            {
                SetValue(() => EditType, value);
            }
        }


        public Dictionary<string, object> OrderTypes
        {
            get { return GetValue(() => OrderTypes); }
            set
            {
                SetValue(() => OrderTypes, value);
            }
        }

        public Dictionary<string, object> SelectedOrderType
        {
            get { return GetValue(() => SelectedOrderType); }
            set
            {
                SetValue(() => SelectedOrderType, value);
            }
        }
        #endregion

        #region methods

        private async void initalizObj()
        {
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                var items = _personService.Queryable().Select(p => new PersonModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    NationalId = p.NationalId,
                    PersonId = p.PersonId
                }).ToList();
                items.Insert(0, new PersonModel { PersonId = 0, FullName = "کل پرسنل" });
                Persons = items;
                SelectedPerson = Persons.First();
            }
            else
            {
                Persons = _personService.Queryable()
                    .Where(x => x.PersonId == UserLog.UniqueInstance.LogedUser.PersonId.Value).Select(x => new PersonModel
                    {
                        FullName = x.FirstName + " " + x.LastName,
                        NationalId = x.NationalId,
                        PersonId = x.PersonId
                    }).ToList();

                SelectedPerson = Persons.FirstOrDefault();
            }

            OrderTypes = new Dictionary<string, object> { { OrderType.Displacement.GetDescription(),OrderType.Displacement}, { OrderType.InternalRequest.GetDescription(), OrderType.InternalRequest }
            ,{ OrderType.InternalTransfer.GetDescription(),OrderType.InternalTransfer},{ OrderType.Procceding.GetDescription(),OrderType.Procceding},{ OrderType.Store.GetDescription(),OrderType.Store}};

            SelectedOrderType = new Dictionary<string, object> { { OrderType.Displacement.GetDescription(),OrderType.Displacement}, { OrderType.InternalRequest.GetDescription(), OrderType.InternalRequest }
            ,{ OrderType.InternalTransfer.GetDescription(),OrderType.InternalTransfer},{ OrderType.Procceding.GetDescription(),OrderType.Procceding},{ OrderType.Store.GetDescription(),OrderType.Store}};

            this.NestPropertyView = false;
            this.OrganizFiltering = false;
            this.StrategyFiltering = false;

            await this.GetParentNodeAsync();
        }

        private Task GetParentNodeAsync()
        {
            Task ts = new Task(() =>
            {
                _allOrganiz = _employeeService.GetParentNode(1).ToList();
                var orgItems = _allOrganiz.Where(x => x.ParentNode == null);
                _allStrategy = _employeeService.GetParentNode(2).ToList();
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

        private void SearchOrder()
        {
            if (EditType > 2 || EditType < 1)
            {
                _dialogservice.ShowAlert("توجه", "نوع ویرایش انتخاب نشده است");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);

            Collection = new ObservableCollection<OrderDetailsModel>(new List<OrderDetailsModel> { new OrderDetailsModel() });
            _collection.Clear();
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                _allOrder = _orderService.GetMyOrdersForManage(true, SelectedOrderType.Values.OfType<OrderType>().ToArray(), 0);
                
                if (SelectedPerson.PersonId != 0)
                {
                    _allOrder = _allOrder.Where(x => x.PersonId == SelectedPerson.PersonId);
                }
            }
            else
            {
                if ( SelectedPerson == null)
                {
                    _dialogservice.ShowInfo("توجه", "انتخاب پرسنل الزامی است");
                    return;
                }

                if (EditType == 1) _allOrder = _orderService.GetMyOrdersForManage(false, SelectedOrderType.Values.OfType<OrderType>().ToArray(), SelectedPerson.PersonId);
                else if (EditType == 2) _allOrder = _orderService.GetManagedOrdersByMy(SelectedOrderType.Values.OfType<OrderType>().ToArray(), UserLog.UniqueInstance.LogedUser.UserId);
            }
            _allOrder.ForEach(o =>
            {
                o.PersianDueDate = o.DueDate.HasValue ? o.DueDate.Value.PersianDateTime() : default(PersianDate?);
                o.PersianOrderDate = o.OrderDate.PersianDateTime();
                _collection.Add(o);
            });
            Mouse.SetCursor(Cursors.Arrow);
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

        private String GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }
        
        private void FilterByBuildingDesign()
        {
            _collection.Clear();
            if (NestPropertyView)
            {
                if (OrganizFiltering)
                {
                    if (OrganizSelected == null)
                    {
                        return;
                    }

                    _allOrder.Where(order => order.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId)
                        .ForEach(order =>
                        {
                            _collection.Add(order);
                        });
                }
                else if (StrategyFiltering)
                {
                    if (StrategySelected == null)
                    {
                        return;
                    }

                    _allOrder.Where(order => order.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId)
                       .ForEach(order =>
                       {
                           _collection.Add(order);
                       });
                }
                else
                {
                    if (OrganizSelected == null || StrategySelected == null)
                    {
                        return;
                    }
                    _allOrder.Where(order => order.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId
                            && order.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId)
                      .ForEach(order =>
                      {
                          _collection.Add(order);
                      });
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

                    _allOrder.Where(order => _allOrganizId.Contains(order.OrganizId.Value))
                      .ForEach(order =>
                      {
                          _collection.Add(order);
                      });
                }
                else if (StrategyFiltering)
                {
                    if (StrategySelected == null)
                    {
                        return;
                    }
                    _allOrder.Where(order => _allStrategyId.Contains(order.StrategyId.Value))
                    .ForEach(order =>
                    {
                        _collection.Add(order);
                    });
                }
                else
                {
                    if (OrganizSelected == null || StrategySelected == null)
                    {
                        return;
                    }
                    _allOrder.Where(order => _allStrategyId.Contains(order.StrategyId.Value) 
                    && _allOrganizId.Contains(order.OrganizId.Value))
                    .ForEach(order =>
                    {
                        _collection.Add(order);
                    });
                }
            }
        }

        private void showOrderDetails(object parameter)
        {
            var odm = parameter as OrderDetailsModel;
            if (odm != null)
            {
                Selected = odm;
                StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
                Mouse.SetCursor(Cursors.Wait);
                if (odm.OrderType == OrderType.InternalRequest)
                {
                    var viewModel = new OrderDetailsManageViewModel(_container, false);
                    var oDetails = _orderService.Query(o => o.OrderId == odm.OrderId).Include(o => o.OrderDetails).Select()
                   .SelectMany(o => o.OrderDetails).Single(od => od.OrderDetialsId == odm.OrderDetailsId);
                    viewModel.CurrentOrderDetails = oDetails;
                    _navigationService.ShowOrderDetailsWindow(viewModel);
                }
                else
                {
                    var order = _orderService.Queryable().Where(x => x.OrderId == odm.OrderId)
               .Include(x => x.MovableAssets)
               .Single();
                    var viewModel = new OrderEditViewModel(_container, order);
                    viewModel.EnableEdit = false;
                    _navigationService.ShowOrderEditWindow(viewModel);
                }
                Mouse.SetCursor(Cursors.Arrow);
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            }
        }

        private void showSuggestWindow(object parameter)
        {
            var orderModel = parameter as OrderDetailsModel;
            if (orderModel == null) return;
            this.Selected = orderModel;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut",this.Window);
            var od = _orderService.GetOrderDetails(orderModel.OrderDetailsId);
            var sod = _orderService.GetSubOrders(od.OrderDetialsId).ToList();
            var viewModel = new SubOrderDetailsViewModel();
            viewModel.CurrentOrderDetails = od;
            viewModel.SubOrders = sod;
            _navigationService.ShowSubOrderWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn",this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands
        public ICommand SearchCommand { get; private set; }
        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand IndentCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        private void initalizCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.SearchOrder(); 
                },
                (parameter) => { return true; }
                );

            OrderDetailsCommand = new MvvmCommand(
                (parameter) => { this.showOrderDetails(parameter); },
                (parameter) => { return true; }
                );

            IndentCommand = new MvvmCommand(
                (parameter) => { this.showSuggestWindow(parameter); },
                (parameter) => { return true; }
                );
        }


        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogservice;
        private readonly INavigationService _navigationService;
        private readonly IBuildingService _buildingService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private List<EmployeeDesign> _allOrganiz;
        private List<EmployeeDesign> _allStrategy;
        private IEnumerable<OrderDetailsModel> _allOrder;
        private ObservableCollection<OrderDetailsModel> _collection;
        private List<int> _allOrganizId = new List<int>();
        private List<int> _allStrategyId = new List<int>();

        #endregion
    }
}
