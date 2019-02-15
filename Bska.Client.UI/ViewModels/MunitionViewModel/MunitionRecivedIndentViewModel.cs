
namespace Bska.Client.UI.ViewModels.MunitionViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.UI.API;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using System.Windows.Threading;

    public sealed class MunitionRecivedIndentViewModel : BaseViewModel
    {
        #region ctor

        public MunitionRecivedIndentViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._collection = new ObservableCollection<SubOrderModel>();
            this._supplierSelected = new Dictionary<SubOrderModel, Users>();
            this.RecivedeIndentFilteredView = new CollectionViewSource { Source = Collection }.View;
            this._dispatcherTimer = new DispatcherTimer();
            this.initalizObj();
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
        
        public Boolean RbGroupView
        {
            get { return GetValue(() => RbGroupView); }
            set
            {
                SetValue(() => RbGroupView, value);
                if (!value)
                {
                    this.FilterByStuffType();
                }
                else
                {
                    this.GroupIndentItems();
                }
            }
        }

        public bool IsAllIndent
        {
            get { return GetValue(() => IsAllIndent); }
            set
            {
                SetValue(() => IsAllIndent, value);
                allSubOrdersShow();
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

        private void initalizObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            Units = _unitService.Queryable().ToList();
            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.AddDays(1).PersianDateTime();
             StuffTypes = new Dictionary<string, object> { { StuffType.Consumable.GetDescription(),StuffType.Consumable}, { StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};

            SelectedStuffType = new Dictionary<string, object> { { StuffType.Consumable.GetDescription(),StuffType.Consumable}, { StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};

            this.FilterByStuffType();
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            this._dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            this._dispatcherTimer.Start();
            Mouse.SetCursor(Cursors.Arrow);
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.FilterByStuffType();
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void FilterByStuffType()
        {
            Mouse.SetCursor(Cursors.Wait);
            _supplierSelected.Clear();
            _subOrderItems = _orderService.GetBuySubOrders()
                      .OrderByDescending(x => x.OrderDate);
            this.allSubOrdersShow();
            if (RbGroupView)
            {
                this.GroupIndentItems();
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void allSubOrdersShow()
        {
            if (_subOrderItems.Count() <= 0)
            {
                return;
            }

            _collection.Clear();
            if (!IsAllIndent)
            {
                _subOrderItems.Where(x => x.State == SubOrderState.None).ForEach(x =>
                {
                    _collection.Add(x);
                });
            }
            else
            {
                DateTime date1 = FromDate.ToDateTime();
                DateTime date2 = ToDate.ToDateTime();
                _subOrderItems.Where(x => SelectedStuffType.ContainsValue(x.StuffType)
                && x.OrderDate > date1 && x.OrderDate <= date2).ForEach(x =>
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
            viewModel.MunitionIndentReport(SelectedStuffType.Select(x=>((int)x.Value)
            .ToString()).ToArray(),querySearch,RbGroupView);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showSupplierIndent(object parameter)
        {
            var som = parameter as SubOrderModel;
            if (som == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            this.Selected = som;
            var viewModel = new SupplierIndentViewModel(_container) { SubOrders=_collection.ToList(),CurrentSubOrder=som};
            var window = _navigationService.ShowSupplierIndentWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showIndentDetails(object parameter)
        {
            var subOrder = parameter as SubOrderModel;
            if (subOrder == null)
            {
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            this.Selected = subOrder;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            var od = _orderService.GetOrderDetails(Selected.OrderDetailsId);
            viewModel.CurrentOrderDetails = od;
            viewModel.AllOrderDetails = new List<OrderDetails> { od };
            viewModel.Units = this.Units;
            _navigationService.ShowOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand SupplierCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand IndentDetailsCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        private void initalizCommand()
        {
            SearchCommand = new MvvmCommand(
              (parameter) => { this.FilterByStuffType(); },
              (parameter) => { return true; }
              );

            SupplierCommand = new MvvmCommand(
                (parameter) => { this.showSupplierIndent(parameter); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.report(); },
                (parameter) => { return true; }
                );

            DoubleClickListViewItemCommand = new MvvmCommand(
           (parameter) => { this.showSupplierIndent(parameter); },
           (parameter) => { return true; }
           );

            IndentDetailsCommand = new MvvmCommand(
          (parameter) => { this.showIndentDetails(parameter); },
          (parameter) => { return true; }
          );

            RefreshCommand = new MvvmCommand(
            (parameter) =>
            {
                this.FilterByStuffType();
            },
            (parameter) => { return true; }
            );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPersonService _personService;
        private readonly IStoreService _storeService;
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<SubOrderModel> _collection;
        private readonly Dictionary<SubOrderModel,Users> _supplierSelected;
        IEnumerable<SubOrderModel> _subOrderItems;
        private readonly  DispatcherTimer _dispatcherTimer;

        #endregion
    }
}
