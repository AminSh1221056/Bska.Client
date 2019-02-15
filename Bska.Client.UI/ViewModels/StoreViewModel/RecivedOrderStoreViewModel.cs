
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Domain.Entity.AssetEntity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;

    public sealed class RecivedOrderStoreViewModel : BaseViewModel
    {
        #region ctor

        public RecivedOrderStoreViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._orderService = _container.Resolve<IOrderService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._collection = new ObservableCollection<SubOrderModel>();
            this._storeBillCollection = new ObservableCollection<StoreBillModel>();
            this.RecivedeIndentFilteredView = new CollectionViewSource { Source = Collection }.View;
            this.RecivedeStoreBillFilteredView = new CollectionViewSource { Source = StoreBillCollection }.View;
            this._dispatcherTimer = new DispatcherTimer();
            this.initalizObj();
            this.initializCommand();
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
        public List<Store> Stores
        {
            get { return GetValue(() => Stores); }
            set
            {
                SetValue(() => Stores, value);
            }
        }

        public Store SelectedStore
        {
            get { return GetValue(() => SelectedStore); }
            set
            {
                SetValue(() => SelectedStore, value);
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
                this.recivedIndentFilter();
            }
        }

        public Boolean IsBillIndent
        {
            get { return GetValue(() => IsBillIndent); }
            set
            {
                SetValue(() => IsBillIndent, value);
                this.initOnBillIndent();
            }
        }

        public ObservableCollection<StoreBillModel> StoreBillCollection
        {
            get { return _storeBillCollection; }
        }

        public StoreBillModel SelectedBill
        {
            get { return GetValue(() => SelectedBill); }
            set
            {
                SetValue(() => SelectedBill, value);
            }
        }
        public ICollectionView RecivedeStoreBillFilteredView { get; set; }

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

        #endregion

        #region methods

        private void initalizObj()
        {
            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager ||
                UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest)
            {
                Stores = _storeService.Queryable().ToList();
            }
            else if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
            {
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                  .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                Stores = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId)).ToList();
            }

            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.AddDays(1).PersianDateTime();

            SelectedStore = Stores.FirstOrDefault();
            this.Units = _unitService.Queryable().ToList();
            IsBillIndent = false;
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            this._dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            this._dispatcherTimer.Start();
        }

        private void initOnBillIndent()
        {
            if (IsBillIndent)
            {
                RecivedTypes = new Dictionary<string, object> { { "قبض انبار های آماده برای حواله انبار", "A001" }
                ,{ "قبض انبار با اموال دارای درخواست آزاد کردن", "A002" },{ "قبض انبار با درخواست آزاد کردن تایید شده", "A003" }};

                SelectedRecivedType = new Dictionary<string, object> { { "قبض انبار های آماده برای حواله انبار", "A001" } };
            }
            else
            {
                RecivedTypes = new Dictionary<string, object> { { "سفارش های رسیده", "A001" },
                    { "سفارش های تکمیل شده", "A002"}};
                SelectedRecivedType = new Dictionary<string, object> { { "سفارش های رسیده", "A001" } };
            }
            this.GetRecivedIndentsAsync();
        }

        private async void GetRecivedIndentsAsync()
        {
            if (SelectedStore == null)
            {
                _dialogService.ShowAlert("توجه", "انبار انتخاب نشده است");
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            if (IsBillIndent)
            {
                _storeBillCollection.Clear();
                HashSet<MAssetReserveStatus> reserveStates = new HashSet<MAssetReserveStatus>();
                if (this.SelectedRecivedType.ContainsValue("A002"))
                {
                    reserveStates.Add(MAssetReserveStatus.UnReservedRequested);
                }
                if (this.SelectedRecivedType.ContainsValue("A003"))
                {
                    reserveStates.Add(MAssetReserveStatus.UnReservedToStore);
                }
                if (this.SelectedRecivedType.ContainsValue("A001"))
                {
                    reserveStates.Add(MAssetReserveStatus.Reserved);
                }

                await Task.Run(() =>
                {
                    _storeBillService.GetStoreBillsForInternalDraft(SelectedStore.StoreId, reserveStates).ToList()
                    .ForEach(sb =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _storeBillCollection.Add(sb);
                        });
                    });
                });
            }
            else
            {
                _collection.Clear();
                SubOrderState subState = SubOrderState.None;
                bool allIndent = false;
                if (SelectedRecivedType.Count == RecivedTypes.Count)
                {
                    allIndent = true;
                }
                if (!allIndent)
                {
                    if (this.SelectedRecivedType.ContainsValue("A002"))
                    {
                        subState = SubOrderState.Deliviry;
                    }
                }

                string identity = SelectedStore.StoreId.ToString();

                await Task.Run(() =>
                {
                    _orderService.GetSubOrdersByIdentity(identity, subState, SubOrderType.Store,
                        allIndent, FromDate.ToDateTime(),ToDate.ToDateTime())
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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.GetRecivedIndentsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            viewModel.CurrentOrderDetails =sod;
            _navigationService.ShowOrderDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void showStoreIndent(object parameter)
        {
            var so = parameter as SubOrderModel;
            if (so == null) return;
            this.Selected = so;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new StoreIndentViewModel(_container, so, SelectedStore.StoreId);
            var window = _navigationService.ShowStoreIndentConfirmWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            this.GetRecivedIndentsAsync();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showStoreBillDetails(IList<object> parameters)
        {
            var storeBill = parameters[0] as StoreBillModel;
            if (storeBill == null) return;
            SelectedBill = storeBill;
            Mouse.SetCursor(Cursors.Wait);
            var bill = _storeBillService.Find(storeBill.StoreBillId);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new StoreBillDetailsViewModel(_container, bill, false);
            _navigationService.ShowStoreBillDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);

        }
       
        private void showStoreBillConfirmWindow(IList<object> parameters)
        {
            var storeBill = parameters[0] as StoreBillModel;
            if (storeBill == null) return;
            SelectedBill = storeBill;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new StoreBillToDocumentViewModel(_container, storeBill.StoreBillId, SelectedStore);
            _navigationService.ShowStoreBillToDocumentWindow(viewModel);
            this.GetRecivedIndentsAsync();
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        #endregion

        #region commands

        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand IndentCompleteCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand BillDetailsCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        private void initializCommand()
        {
            OrderDetailsCommand = new MvvmCommand(
                (parameter) => { this.showOrderDetails(parameter); },
                (parameter) => { return true; }
                );

            IndentCompleteCommand = new MvvmCommand(
                (parameter) => { this.showStoreIndent(parameter); },
                (parameter) => { return true; }
                );

            SearchCommand = new MvvmCommand(
                (parameter) => { this.GetRecivedIndentsAsync(); },
                (parameter) => { return true; }
                );

            BillDetailsCommand = new MvvmCommand(
              (parameter) => { this.showStoreBillDetails(parameter as IList<object>); },
              (parameter) => { return true; }
              );

            DetailsCommand = new MvvmCommand(
              (parameter) => { this.showStoreBillConfirmWindow(parameter as IList<object>); },
              (parameter) => { return true; }
              );

            DoubleClickListViewItemCommand = new MvvmCommand(
            (parameter) => {
            if (IsBillIndent)
            {
                    IList<object> ls = new List<object> { { parameter } };
                    this.showStoreBillDetails(ls);
                }
                else
                {
                    this.showOrderDetails(parameter);
                };
            },

            (parameter) => { return true; });

            RefreshCommand = new MvvmCommand(
              (parameter) =>
              {
                  this.GetRecivedIndentsAsync();
              },
              (parameter) => { return true; }
              );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IStoreBillService _storeBillService;
        private readonly IPersonService _personService;
        private readonly IUnitService _unitService;
        private readonly ObservableCollection<SubOrderModel> _collection;
        private readonly ObservableCollection<StoreBillModel> _storeBillCollection;
        private readonly DispatcherTimer _dispatcherTimer;

        #endregion
    }
}
