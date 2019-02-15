
namespace Bska.Client.UI.ViewModels.MunitionViewModel
{
    using Microsoft.Practices.Unity;
    using System.Linq;
    using System.Collections.Generic;
    using Bska.Client.UI.Services;
    using System.Windows;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Data.Service;
    using Bska.Client.Common;
    using System.Collections.ObjectModel;
    using Bska.Client.Repository.Model;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows.Input;
    using Bska.Client.UI.API;
    using System;
    using System.Threading.Tasks;
    using Bska.Client.UI.Helper;
    using System.Windows.Controls;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.API.UnitOfWork;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.API.Infrastructure;
    public sealed class AddReturnIndentRequestViewModel :BaseViewModel
    {
        #region ctor

        public AddReturnIndentRequestViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._supplierIndents = new ObservableCollection<SubOrderModel>();
            this.SupplierIndentFilteredView = new CollectionViewSource { Source = SupplierIndents }.View;
            this._unitHelper = new UnitHelper();
            this.initializObj();
            this.initializCommands();
        }
        #endregion

        #region properties

        public Window Window
        {
            get; set;
        }

        public List<Users> Suppliers
        {
            get { return GetValue(() => Suppliers); }
            set
            {
                SetValue(() => Suppliers, value);
            }
        }

        public Users CurrentSupplier
        {
            get { return GetValue(() => CurrentSupplier); }
            set
            {
                SetValue(() => CurrentSupplier, value);
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
        
        public SubOrderModel SelectedIndent
        {
            get { return GetValue(() => SelectedIndent); }
            set
            {
                SetValue(() => SelectedIndent, value);
            }
        }

        public ICollectionView SupplierIndentFilteredView { get; set; }

        public ObservableCollection<SubOrderModel> SupplierIndents
        {
            get { return _supplierIndents; }
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


        public List<AnalizModel> Analizes
        {
            get { return GetValue(() => Analizes); }
            set
            {
                SetValue(() => Analizes, value);
            }
        }
        public string SelectedIndentDescription
        {
            get { return GetValue(() => SelectedIndentDescription); }
            set
            {
                SetValue(() => SelectedIndentDescription, value);
            }
        }
        
        #endregion

        #region methods

        private void SearchOrder()
        {
            SupplierIndentFilteredView.Filter = (obj) =>
            {
                var model = obj as SubOrderModel;
                return model.StuffName.StartsWith(SearchCriteria)
                    || model.SubOrderId.ToString() == SearchCriteria;
            };
        }

        private void initializObj()
        {
            var suppliers = _personService.GetUsersByPermission(PermissionsType.Supplier).ToList();
            suppliers.Insert(0, new Users { FullName = "کل سفارش های باقیمانده", UserId = 0 });
            CurrentSupplier = suppliers.First();
            this.Suppliers = suppliers;
            this.Units = _unitService.Queryable().ToList();
            this.getSupplierIndents();

        }

        private void getSupplierIndents()
        {
            if (CurrentSupplier == null)
            {
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            SelectedIndentDescription = $"تعداد سفارش انتخاب شده: {0}";
            _supplierIndents.Clear();
            _orderService.GetSupplierIndentWithouReturnRequest(CurrentSupplier.UserId)
               .ForEach(sbm =>
               {
                   sbm.IsSelected = false;
                   _supplierIndents.Add(sbm);
               });
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void initOnSelecting(object parameter)
        {
            var ch = parameter as CheckBox;
            var sbm = ch.Tag as SubOrderModel;
            if (sbm != null)
            {
                if (ch.IsChecked == true)
                    sbm.IsSelected = true;
                else sbm.IsSelected = false;
                SelectedIndentDescription = $"تعداد سفارش انتخاب شده: {_supplierIndents.Count(sb => sb.IsSelected)}";
            }
        }

        private void saveReturnRequest()
        {
            if (!_supplierIndents.Any(sbm => sbm.IsSelected))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }

            var employee = _employeeService.Query().Include(e => e.ReturnIndentRequests).Select().FirstOrDefault();
            if (employee == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ سازمانی یافت نشد");
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                var newReturnRequest = new ReturnIndentRequest
                {
                    Status = GlobalRequestStatus.Pending,
                    ObjectState = ObjectState.Added,
                    Description = "ثبت درخواست بازگشت سفارش های خرید در تاریخ " + GlobalClass._Today.PersianDateString(),
                    InsertDate = GlobalClass._Today
                };

                _supplierIndents.Where(sp => sp.IsSelected).ForEach(sp =>
                {
                    var spO = _orderService.GetSupplierIndent(sp.SupplierIndentId);
                    newReturnRequest.SupplierIndents.Add(spO);
                });

                employee.ReturnIndentRequests.Add(newReturnRequest);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _employeeService.InsertOrUpdateGraph(employee);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.getSupplierIndents();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void showIndentDetails(object parameter)
        {
            var subOrder = parameter as SubOrderModel;
            if (subOrder == null)
            {
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            this.SelectedIndent = subOrder;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            var od = _orderService.GetOrderDetails(SelectedIndent.OrderDetailsId);
            viewModel.CurrentOrderDetails = od;
            viewModel.AllOrderDetails = new List<Domain.Entity.OrderEntity.OrderDetails> { od };
            viewModel.Units = this.Units;
            _navigationService.ShowOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void confirmRRequest(object parameter)
        {
            var rRequest = parameter as ReturnIndentRequest;
            if (rRequest != null)
            {
                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {

                }
            }
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ConfirmRequest { get; private set; }
        public ICommand ReturnRequestDetailsCommand { get; private set; }
        public ICommand IndentDetailsCommand { get; private set; }
        private void initializCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.getSupplierIndents(); },
                (parameter) => { return true; }
                );
            
            SelectCommand = new MvvmCommand(
                (parameter) => { this.initOnSelecting(parameter); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
               (parameter) => { this.saveReturnRequest(); },
               (parameter) => { return true; }
               );

            IndentDetailsCommand = new MvvmCommand(
             (parameter) => { this.showIndentDetails(parameter); },
             (parameter) => { return true; }
             );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitService _unitService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly UnitHelper _unitHelper;
        private readonly IStoreBillService _storeBillService;
        private readonly ObservableCollection<SubOrderModel> _supplierIndents;

        #endregion

    }
}
