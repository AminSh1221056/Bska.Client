
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Common;
    using Domain.Entity;
    using Microsoft.Practices.Unity;
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using Data.Service;
    using System.Windows.Input;
    using System.Collections.ObjectModel;
    using Repository.Model;
    using System.ComponentModel;
    using System.Windows.Data;
    using API;
    using System.Windows.Controls;
    using Services;
    using OrderViewModel;
    using System;
    using System.Threading.Tasks;
    using Helper;
    using System.Windows.Threading;

    public sealed class StoreBuyIndentViewModel : BaseViewModel
    {
        #region ctor

        public StoreBuyIndentViewModel(IUnityContainer container)
        {
            this._container = container;
            this._personService = _container.Resolve<IPersonService>();
            this._collection = new ObservableCollection<SupplierIndentModel>();
            this._orderService = _container.Resolve<IOrderService>();
            this._unitService = _container.Resolve<IUnitService>();
            this.IndentFilteredView = new CollectionViewSource { Source = Collection }.View;
            this._selectedItems = new HashSet<SupplierIndentModel>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dispatcherTimer = new DispatcherTimer();
            this.initializObj();
            this.initializCommand();
        }

        #endregion

        #region properties
        
        public Window Window
        {
            get;
            set;
        }

        public bool ToDocumentIssue
        {
            get { return GetValue(() => ToDocumentIssue); }
            set
            {
                SetValue(() => ToDocumentIssue, value);
                this.initOnToDocumentIssue();
            }
        }

        public int CounterSelect
        {
            get { return GetValue(() => CounterSelect); }
            set
            {
                SetValue(() => CounterSelect, value);
            }
        }
        public List<Users> Suppliers
        {
            get { return GetValue(() => Suppliers); }
            set
            {
                SetValue(() => Suppliers, value);
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

        public Users CurrentSupplier
        {
            get { return GetValue(() => CurrentSupplier); }
            set
            {
                SetValue(() => CurrentSupplier, value);
                this.getSupplierIndentsAsync();
            }
        }
        public ObservableCollection<SupplierIndentModel> Collection
        {
            get { return _collection; }
        }

        public ICollectionView IndentFilteredView { get; set; }

        public SupplierIndentModel SelectedIndent
        {
            get { return GetValue(() => SelectedIndent); }
            set
            {
                SetValue(() => SelectedIndent, value);
            }
        }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchOnItems();
            }
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
        #endregion

        #region methods

        private void initializObj()
        {
            RecivedTypes = new Dictionary<string, object> { { "سفارش های در دست اقدام", "A001" }
                ,{ "سفارش ها تکمیل شده", "A002" }};

            SelectedRecivedType = new Dictionary<string, object> { { "سفارش های در دست اقدام", "A001" } };

            Suppliers = _personService.GetUsers().Where(x => x.PermissionType == PermissionsType.Supplier).ToList();
            CurrentSupplier = Suppliers.FirstOrDefault();
            Units = _unitService.Queryable().ToList();
            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.AddDays(1).PersianDateTime();
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            this._dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            this._dispatcherTimer.Start();
        }

        private void initOnToDocumentIssue()
        {
            _selectedItems.Clear();
            getSupplierIndentsAsync();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.getSupplierIndentsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void getSupplierIndentsAsync()
        {
            if (CurrentSupplier == null) return;
            Mouse.SetCursor(Cursors.Wait);
            _collection.Clear();

            HashSet<SupplierIndentState> states = new HashSet<SupplierIndentState>();
            if (this.SelectedRecivedType.ContainsValue("A001"))
            {
                states.Add(SupplierIndentState.Ongoing);
            }
            if (this.SelectedRecivedType.ContainsValue("A002"))
            {
                states.Add(SupplierIndentState.Delivery);
            }

            await Task.Run(() =>
            {
                if (ToDocumentIssue)
                {
                    _orderService.GetSupplierIndentModel(CurrentSupplier.UserId, states)
                    .Where(x =>x.StoreId==null && (x.OrderDate>FromDate.ToDateTime() && x.OrderDate<=ToDate.ToDateTime()))
                    .ToList().ForEach(spi =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _collection.Add(spi);
                        });
                    });
                }
                else
                {
                    _orderService.GetSupplierIndentModel(CurrentSupplier.UserId, states)
                    .Where(x=>(x.OrderDate > FromDate.ToDateTime() && x.OrderDate <= ToDate.ToDateTime())).ToList().ForEach(spi =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _collection.Add(spi);
                        });
                    });
                }
            });
            CounterSelect = 0;
            _selectedItems.Clear();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void selectingItem(object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);
            var checkbox = parameter as CheckBox;
            if (checkbox == null) return;
            var selectedIndent = checkbox.Tag as SupplierIndentModel;
            if (selectedIndent == null) return;
            if (checkbox.IsChecked == true)
            {
                if (!_selectedItems.Contains(selectedIndent))
                {
                    if (_selectedItems.Count > 0 && ToDocumentIssue)
                    {
                        var fItem = _selectedItems.FirstOrDefault();
                        if (selectedIndent.NationalId != fItem.NationalId || selectedIndent.OrganizId!=fItem.OrganizId || selectedIndent.StrategyId!=fItem.StrategyId )
                        {
                            _dialogService.ShowAlert("توجه", "درحالت صدور قبض انبار و حواله انبار مستقیم شما تنها می توانید درخواست های مربوط به یک پرسنل  از یک منطقه سازمانی و استراتژیکی را انتخاب کنید");
                            checkbox.IsChecked = false;
                            return;
                        }
                    }
                    _selectedItems.Add(selectedIndent);
                }
            }
            else
            {
                if (_selectedItems.Contains(selectedIndent))
                {
                    _selectedItems.Remove(selectedIndent);
                }
            }
            CounterSelect = _selectedItems.Count;
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showOrderDetails(object parameter)
        {
            var od = parameter as SupplierIndentModel;
            if (od == null) return;
            this.SelectedIndent = od;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            var items = _orderService.GetRelatedOrderDetailsByIndent(od.IndentId);
            viewModel.Units = this.Units;
            viewModel.CurrentOrderDetails = items.Item2;
            viewModel.AllOrderDetails = items.Item1.ToList();
            _navigationService.ShowOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void searchOnItems()
        {
            this.IndentFilteredView.Filter = (obj =>
            {
                var indentModel = obj as SupplierIndentModel;
                return indentModel.StuffName.Contains(SearchCriteria)
                || indentModel.PersonName.Contains(SearchCriteria);
            });
        }

        private void showAddAssetWindow()
        {
            if (_selectedItems.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new AddBuyAssetViewModel(_container, _selectedItems, ToDocumentIssue, CurrentSupplier);
            var window= _navigationService.ShowAssetBuyWindow(viewModel);
            this.getSupplierIndentsAsync();
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands
        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand  NewCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        private void initializCommand()
        {
            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectingItem(parameter); },
                (parameter) => { return true; }
                );

            OrderDetailsCommand = new MvvmCommand(
                (parameter) => { this.showOrderDetails(parameter); },
                (parameter) => { return true; }
                );

            NewCommand = new MvvmCommand(
                (parameter) => { this.showAddAssetWindow(); },
                (parameter) => { return true; }
                );

            DoubleClickListViewItemCommand = new MvvmCommand(
               (parameter) => { this.showOrderDetails(parameter); },
               (parameter) => { return true; }
               );

            SearchCommand = new MvvmCommand(
                  (parameter) => { this.getSupplierIndentsAsync(); },
               (parameter) => { return true; }
                );

            RefreshCommand = new MvvmCommand(
           (parameter) =>
           {
               this.getSupplierIndentsAsync();
           },
           (parameter) => { return true; }
           );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IPersonService _personService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<SupplierIndentModel> _collection;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly HashSet<SupplierIndentModel> _selectedItems;
        private readonly DispatcherTimer _dispatcherTimer;

        #endregion
    }
}
