
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Data.Entity;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Threading;
    using Domain.Concrete;

    public sealed class MainWindowViewModel : BaseViewModel
    {
        #region ctor

        public MainWindowViewModel(IUnityContainer container)
        {
            this._container = container;
            this._orderService = _container.Resolve<IOrderService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._proceedingService = _container.Resolve<IProceedingService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._personAssetCollection = new ObservableCollection<MovableAssetModel>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            Timeline = _container.Resolve<TimelineViewModel>();
            this._orderCollection = new ObservableCollection<OrderModel>();
            this.OrderFilteredView = new CollectionViewSource { Source = OrderCollection }.View;
            this._dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            OrganizationName = "سامانه بسکا";
            this.initializCommand();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;set;
        }

        public TimelineViewModel Timeline
        {
            get { return GetValue(() => Timeline); }
            set
            {
                SetValue(() => Timeline, value);
            }
        }
        public ObservableCollection<OrderModel> OrderCollection
        {
            get { return _orderCollection; }
        }
        public OrderModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }
        public ICollectionView OrderFilteredView { get; set; }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SearchOrder();
            }
        }
        public String CurrentUserDetails
        {
            get { return GetValue(() => CurrentUserDetails); }
            set
            {
                SetValue(() => CurrentUserDetails, value);
                if(!string.IsNullOrWhiteSpace(value))
                initalizObj();
            }
        }
        public ObservableCollection<MovableAssetModel> PersonAssetCollection
        {
            get { return _personAssetCollection; }
        }

        public Boolean IsOrderNotify
        {
            get { return GetValue(() => IsOrderNotify); }
            set
            {
                SetValue(() => IsOrderNotify, value);
            }
        }

        public String RecivedOrdersCount
        {
            get { return GetValue(() => RecivedOrdersCount); }
            set
            {
                SetValue(() => RecivedOrdersCount, value);
            }
        }

        public string OrganizationName
        {
            get { return GetValue(() => OrganizationName); }
            set
            {
                SetValue(() => OrganizationName, value);
            }
        }

        public Boolean IsStuffHonest
        {
            get { return GetValue(() => IsStuffHonest); }
            set
            {
                SetValue(() => IsStuffHonest, value);
            }
        }

        public Boolean IsStoreManager
        {
            get { return GetValue(() => IsStoreManager); }
            set
            {
                SetValue(() => IsStoreManager, value);
            }
        }

        public Boolean IsGeneralManager
        {
            get { return GetValue(() => IsGeneralManager); }
            set
            {
                SetValue(() => IsGeneralManager, value);
            }
        }

        public Boolean IsMunition
        {
            get { return GetValue(() => IsMunition); }
            set
            {
                SetValue(() => IsMunition, value);
            }
        }

        public Boolean IsSupplier
        {
            get { return GetValue(() => IsSupplier); }
            set
            {
                SetValue(() => IsSupplier, value);
            }
        }
        public string OrderDesc1
        {
            get { return GetValue(() => OrderDesc1); }
            set
            {
                SetValue(() => OrderDesc1, value);
            }
        }

        public string OrderDesc2
        {
            get { return GetValue(() => OrderDesc2); }
            set
            {
                SetValue(() => OrderDesc2, value);
            }
        }

        public string OrderDesc3
        {
            get { return GetValue(() => OrderDesc3); }
            set
            {
                SetValue(() => OrderDesc3, value);
            }
        }

        public string OrderDesc4
        {
            get { return GetValue(() => OrderDesc4); }
            set
            {
                SetValue(() => OrderDesc4, value);
            }
        }

        public string OrderDesc5
        {
            get { return GetValue(() => OrderDesc5); }
            set
            {
                SetValue(() => OrderDesc5, value);
            }
        }

        public string OrderDesc6
        {
            get { return GetValue(() => OrderDesc6); }
            set
            {
                SetValue(() => OrderDesc6, value);
            }
        }
        #endregion

        #region methods

        private void initalizObj()
        {
            this.initTimeLine();
            this.DispatcherStart(new TimeSpan(0, 1, 0));
        }

        private void DispatcherStart(TimeSpan interval)
        {
            _dispatcherTimer.Interval = interval;
            OrganizationName = (UserLog.UniqueInstance.LogedEmployee?.Name ?? "سامانه بسکا");
            this._dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.initTimeLine();
                ConnectionHelper.GetCurrentDateTime();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SearchOrder()
        {
            this.OrderFilteredView.Filter = ((obj) =>
            {
                var order = obj as OrderModel;
                if (order != null)
                {
                    return order.OrderId.ToString().StartsWith(SearchCriteria);
                }
                return false;
            });
        }

        private void initTimeLine()
        {
            if (!ConnectionHelper.CheckConnection())
            {
                return;
            }
            _orderCollection.Clear();
            _personAssetCollection.Clear();
            Timeline.OrderCollection.Clear();
            if (UserLog.UniqueInstance.LogedUser == null) return;
            DateTime mindate = GlobalClass._Today.AddMonths(-5);
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                var orders = _orderService.GetOrders(0, mindate);
                Task.Run(() =>
                {
                    orders.ForEach(order =>
                    {
                        if (order.Status == OrderStatus.Reject
                            || order.Status == OrderStatus.SubOrder || order.Status == OrderStatus.Deliviry)
                        {
                            order.IsEditable = true;
                        }
                        DispatchService.Invoke(() =>
                        {
                            _orderCollection.Add(order);
                            Timeline.OrderCollection.Add(order);
                        });
                    });
                });
            }
            else
            {
                Task.Run(() =>
                {
                    _orderService.GetOrders(UserLog.UniqueInstance.LogedUser.PersonId.Value, mindate).ForEach(order =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _orderCollection.Add(order);
                            Timeline.OrderCollection.Add(order);
                        });
                    });
                }).ContinueWith(t1 =>
                {
                    _movableAssetService.GetPersonAssets(UserLog.UniqueInstance.LogedUser.Person.NationalId).ForEach(mAsset =>
                    {
                        if (mAsset.MAssetType != "مصرفی")
                        {
                            DispatchService.Invoke(() =>
                            {
                                _personAssetCollection.Add(mAsset);
                            });
                        }
                    });
                }).ContinueWith(t2 =>
                {
                    bool isNotify =APPSettings.Default.OrderNotify ;
                    if (isNotify)
                    {
                        int globalCounter = 0;
                        int ordercount = 0;
                        int counter1 = 0;
                        int counter2 = 0;
                        int counter3 = 0;
                        int counter4 = 0;
                        int counter5 = 0;
                        ordercount = _orderService.GetRecivedOrdersCount(UserLog.UniqueInstance.LogedUser.UserId, OrderDetailsState.OrganizManagerConfirm, OrderDetailsState.ManagerConfirm);
                        if (Thread.CurrentPrincipal.IsInRole("StuffHonest"))
                        {
                            counter1 = _orderService.GetRecivedOrdersForStuffHonest(UserLog.UniqueInstance.LogedUser.UserId);
                            counter2 = _proceedingService.GetRecivedProceedingsCount(ProceedingState.Confirmed);
                            IsStuffHonest = true;
                        }
                        else if (Thread.CurrentPrincipal.IsInRole("GeneralManager"))
                        {
                            counter1 = _orderService.GetStoreOrdersForManager();
                            counter2 = _proceedingService.GetRecivedProceedingsCount(ProceedingState.ManagerConfirming);
                            counter3 = _storeBillService.CountRecivedEditsByState(GlobalRequestStatus.Pending);
                            counter4 = _employeeService.CountReturnRequestByStatus(GlobalRequestStatus.Pending);
                            counter5 =_storeBillService.CountAssetReserveHistories(MAssetReserveStatus.UnReservedRequested);
                            IsGeneralManager = true;
                        }
                        else if (Thread.CurrentPrincipal.IsInRole("StoreLeader"))
                        {
                            var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                           .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                            var storeId = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId))
                            .ToList().Select(x => x.StoreId.ToString()).ToArray();
                            counter1 = _orderService.GetRecivedStoreIndentsCount(storeId);
                            this.IsStoreManager = true;
                        }
                        else if (Thread.CurrentPrincipal.IsInRole("MunitionLeader"))
                        {
                            counter1 = _orderService.GetRecivedMunituinIndentsCount("Buy");
                            IsMunition = true;
                        }
                        else if (Thread.CurrentPrincipal.IsInRole("Supplier"))
                        {
                            string identity = "Supplier-" + UserLog.UniqueInstance.LogedUser.UserId;
                            counter1 = _orderService.GetRecivedMunituinIndentsCount(identity);
                            IsSupplier = true;
                        }

                        globalCounter = ordercount + counter1 + counter2 + counter3 + counter4 + counter5;

                        DispatchService.Invoke(() =>
                        {
                            OrderDesc1 = ordercount.ToString();
                            OrderDesc2 = counter1.ToString();
                            OrderDesc3 = counter2.ToString();
                            OrderDesc4 = counter3.ToString();
                            OrderDesc5 = counter4.ToString();
                            OrderDesc6 = counter5.ToString();
                            RecivedOrdersCount = globalCounter.ToString();
                        });
                    }
                });
            }
        }

        private void showOrdertrack(object parameter)
        {
            if (!ConnectionHelper.CheckConnection())
            {
                return;
            }
            
            var orderModel = parameter as OrderModel;
            if (orderModel == null) return;
            Mouse.SetCursor(Cursors.Wait);
            var order = _orderService.Queryable().Where(x => x.OrderId == orderModel.OrderId).Include(x => x.OrderDetails)
                .Include(x=>x.MovableAssets).AsNoTracking()
                .Single();
            Selected = orderModel;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderTrackViewModel(_container);
            viewModel.CurrentOrder = order;
            Window window = _navigationService.ShowOrderTrackWindow(viewModel);
            if (window.DialogResult == true)
            {
                _orderCollection.Remove(orderModel);
                Timeline.OrderCollection.Remove(orderModel);
            }
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showOrderEdit(object parameter)
        {
            if (!ConnectionHelper.CheckConnection())
            {
                return;
            }
            
            var orderModel = parameter as OrderModel;
            if (orderModel == null) return;
            Mouse.SetCursor(Cursors.Wait);
            var order = _orderService.Queryable().Where(x => x.OrderId == orderModel.OrderId).Include(x => x.OrderDetails)
                .Include(x => x.MovableAssets).AsNoTracking()
                .Single();
            Selected = orderModel;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderEditViewModel(_container,order);
            _navigationService.ShowOrderEditWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showSuggestWindow(object parameter)
        {
            if (!ConnectionHelper.CheckConnection())
            {
                return;
            }
            
            var orderModel = parameter as OrderModel;
            if (orderModel == null) return;
            Mouse.SetCursor(Cursors.Wait);
            var order = _orderService.Queryable().Where(x => x.OrderId == orderModel.OrderId).Include(x => x.OrderDetails).AsNoTracking()
                .Single();
            Selected = orderModel;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut",this.Window);
            var viewModel = new SuggestViewModel(_container);
            viewModel.CurrentOrder = order;
            _navigationService.ShowSuggestWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void ShowMAssetDetails(object parameter)
        {
            MovableAssetModel item = parameter as MovableAssetModel;
            if (item == null) return;
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new MovableAssetDetailsViewModel(_container,item.AssetId);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut",this.Window);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private Task<List<MovableAsset>> getAssetAsync()
        {
            var ts = new Task<List<MovableAsset>>(() =>
            {
                var mAssetList = new List<MovableAsset>();
                _personAssetCollection.ForEach(ma =>
                {
                    var asset = _movableAssetService.Find(ma.AssetId);
                    mAssetList.Add(asset);
                });
                return mAssetList;
            });
            ts.Start();
            return ts;
        }

        private void openHelpDoc()
        {
            //var viewModel = new HelpViewModel(HelpPageingAddress.Default.HelpFileName, HelpPageingAddress.Default.mainWinHelpPage);
            //_navigationService.ShowHelpWindow(viewModel);
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showMainWindowHelp();
            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand TrackCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand IndentCommand { get; private set; }
        public ICommand MAssetDetailsCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initializCommand()
        {
            TrackCommand = new MvvmCommand(
                (parameter) => { this.showOrdertrack(parameter); },
                (parameter) => { return true; }
                );

            RefreshCommand = new MvvmCommand(
               (parameter) => 
               {
                   this.IsOrderNotify = true;
                   this.initTimeLine();
               },
               (parameter) => { return true; }
               );

            EditCommand = new MvvmCommand(
               (parameter) => { this.showOrderEdit(parameter); },
               (parameter) => { return true; }
               );

            IndentCommand = new MvvmCommand(
                (parameter) => { this.showSuggestWindow(parameter); },
                (parameter) => { return true; }
                );

            MAssetDetailsCommand = new MvvmCommand(
               (parameter) => { this.ShowMAssetDetails(parameter); },
               (parameter) => { return true; });

            DoubleClickListViewItemCommand = new MvvmCommand(
              (parameter) => { this.showOrdertrack(parameter); },
              (parameter) => { return true; }
              );

            HelpCommand = new MvvmCommand(
                (parameter) => { this.openHelpDoc(); },
                (parameter) => { return true; }
               );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly ObservableCollection<OrderModel> _orderCollection;
        DispatcherTimer _dispatcherTimer;
        private readonly IOrderService _orderService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IEmployeeService _employeeService;
        private readonly IProceedingService _proceedingService;
        private readonly IPersonService _personService;
        private readonly IStoreBillService _storeBillService;
        private readonly IStoreService _storeService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<MovableAssetModel> _personAssetCollection;

        #endregion

    }
}
