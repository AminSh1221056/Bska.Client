
namespace Bska.Client.UI.ViewModels
{
    using System;
    using Microsoft.Practices.Unity;
    using Repository.Model;
    using Domain.Entity;
    using Data.Service;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using UI.Helper;
    using Client.API.UnitOfWork;
    using Services;
    using System.Collections.ObjectModel;
    using Domain.Entity.OrderEntity;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using API;
    using Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;
    using MunitionViewModel;
    using System.Windows;

    public sealed class SupplierIndentViewModel : BaseViewModel
    {
        #region ctor

        public SupplierIndentViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._subOrderCollection = new ObservableCollection<SupplierIndent>();
            this.initializObj();
            this.initializCommand();
        }

        #endregion

        #region properties
        public Double CurrentNum
        {
            get { return GetValue(() => CurrentNum); }
            set
            {
                SetValue(() => CurrentNum, value);
            }
        }
        public Unit SelectedUnit
        {
            get { return GetValue(() => SelectedUnit); }
            set
            {
                SetValue(() => SelectedUnit, value);
            }
        }
        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        public ObservableCollection<SupplierIndent> SubOrdersCollection
        {
            get { return _subOrderCollection; }
        }

        public List<SubOrderModel> SubOrders
        {
            get { return GetValue(() => SubOrders); }
            set
            {
                SetValue(() => SubOrders, value);
            }
        }

        public SubOrderModel CurrentSubOrder
        {
            get { return GetValue(() => CurrentSubOrder); }
            set
            {
                SetValue(() => CurrentSubOrder, value);
                this.initOnSubOrderAsync();
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

        public Users SelectedSupplier
        {
            get { return GetValue(() => SelectedSupplier); }
            set
            {
                SetValue(() => SelectedSupplier, value);
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

        public Seller SelectedSeller
        {
            get { return GetValue(() => SelectedSeller); }
            set
            {
                SetValue(() => SelectedSeller, value);
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

        public Unit Unit
        {
            get { return GetValue(() => Unit); }
            set
            {
                SetValue(() => Unit, value);
            }
        }
        public Double SubOrdersNum
        {
            get { return GetValue(() => SubOrdersNum); }
            set
            {
                SetValue(() => SubOrdersNum, value);
            }
        }

        public Double RemainNum
        {
            get { return GetValue(() => RemainNum); }
            set
            {
                SetValue(() => RemainNum, value);
            }
        }

        public Boolean Deleteable
        {
            get { return GetValue(() => Deleteable); }
            set
            {
                SetValue(() => Deleteable, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            Units = _unitService.Queryable().ToList();
            Suppliers = _personService.GetUsersByPermission(PermissionsType.Supplier).ToList();
            Sellers = _sellerService.Queryable().ToList();
            SelectedSupplier = Suppliers.FirstOrDefault();
            SelectedSeller = Sellers.FirstOrDefault();
        }

        private async void initOnSubOrderAsync()
        {
            if (CurrentSubOrder == null) return;

            _subOrderCollection.Clear();
            _orderService.GetSupplierIndents(CurrentSubOrder.SubOrderId).ForEach(sp =>
            {
                _subOrderCollection.Add(sp);
            });

            Unit = SelectedUnit = Units.SingleOrDefault(u => u.UnitId == CurrentSubOrder.UnitId);
            _subUnits.Clear();
            var mParent = unitParentRecovery(SelectedUnit);
            _subUnits.Add(new UnitTreeViewModel(mParent, _container));
            SubOrdersNum = await calculateSubOrdersNum();
            RemainNum = CurrentSubOrder.Num - SubOrdersNum;
            if (RemainNum > 0)
            {
                this.Deleteable = true;
            }
            else
            {
                this.Deleteable = false;
            }
        }

        private Unit unitParentRecovery(Unit current)
        {
            Unit mparent = current;

            if (mparent.Parent != null)
            {
                mparent = this.unitParentRecovery(mparent.Parent);
            }
            return mparent;
        }

        private Task<double> calculateSubOrdersNum()
        {
            var ts = new Task<double>(() =>
            {
                double allNum = 0;
                _subOrderCollection.ForEach(so =>
                {
                    var soUnit = Units.Find(u => u.UnitId == so.UnitId);
                    if (!Unit.Equals(soUnit))
                    {
                        Double val = CalculateUnitNum(soUnit, so.Num);
                        Double mVal = ReverseCalculateUnitNum(Unit, val);
                        allNum += mVal;
                    }
                    else
                    {
                        allNum += so.Num;
                    }
                });
                return allNum;
            });
            ts.Start();
            return ts;
        }

        private Double CalculateUnitNum(Unit unit, Double val)
        {
            switch (unit.MathType)
            {
                case UnitMathType.Multiple:
                    val = (val * unit.MathNum.Value);
                    break;
                case UnitMathType.Divide:
                    val = (val / unit.MathNum.Value);
                    break;
            }
            if (unit.Parent != null)
            {
                val = CalculateUnitNum(unit.Parent, val);
            }
            return val;
        }

        private Double ReverseCalculateUnitNum(Unit unit, Double val)
        {
            switch (unit.MathType)
            {
                case UnitMathType.Divide:
                    val = (val * unit.MathNum.Value);
                    break;
                case UnitMathType.Multiple:
                    val = (val / unit.MathNum.Value);
                    break;
            }
            if (unit.Parent != null)
            {
                val = ReverseCalculateUnitNum(unit.Parent, val);
            }
            return val;
        }

        private void confirmSubOrder()
        {
            string errMsg = "";
            if (CurrentNum <= 0)
            {
                errMsg = "مقدار وارد شده صحیح نیست"+Environment.NewLine;
            }

            if (SelectedSeller == null)
            {
                errMsg = "فروشنده انتخاب نشده است" + Environment.NewLine;
            }

            if (SelectedSupplier == null)
            {
                errMsg = "کارپرداز انتخاب نشده است" + Environment.NewLine;
            }

            if (errMsg.Length > 0)
            {
                _dialogService.ShowAlert("توجه", errMsg);
                return;
            }

            Double subtractVal = 0;
            if (!SelectedUnit.Equals(Unit))
            {
                var selectedUnit = Units.Find(u => u.UnitId == SelectedUnit.UnitId);
                double detailsVal = CalculateUnitNum(Unit, RemainNum);
                double subOrderVal = CalculateUnitNum(selectedUnit, CurrentNum);
                if (subOrderVal > detailsVal)
                {
                    _dialogService.ShowAlert("توجه", "تعداد وارد شده برای این سفارش بیش از مقدار درخواست شده است");
                    return;
                }
                subtractVal = ReverseCalculateUnitNum(Unit, subOrderVal);
            }
            else
            {
                if (CurrentNum > RemainNum)
                {
                    _dialogService.ShowAlert("توجه", "تعداد وارد شده برای این سفارش بیش از مقدار درخواست شده است");
                    return;
                }
                subtractVal = CurrentNum;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش",ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                var order = _orderService.Query(x => x.OrderId == CurrentSubOrder.OrderId)
                  .Include(o => o.OrderDetails).Select().First();
                var orderDetails = order.OrderDetails.First(od => od.OrderDetialsId == CurrentSubOrder.OrderDetailsId);
                var subOrder = _orderService.GetSubOrders(CurrentSubOrder.OrderDetailsId).First(x => x.SubOrderId == CurrentSubOrder.SubOrderId);
                subOrder.ObjectState = ObjectState.Modified;

                var newIndent = new SupplierIndent
                {
                    Num=CurrentNum,
                    ObjectState=ObjectState.Added,
                    Seller=SelectedSeller,
                    State=SupplierIndentState.Ongoing,
                    SupplierId=SelectedSupplier.UserId,
                    Remain=CurrentNum,
                    UnitId=SelectedUnit.UnitId
                };
                
                SubOrdersNum += subtractVal;
                RemainNum -= subtractVal;

                if (RemainNum == 0)
                {
                    this.Deleteable = false;
                }
                subOrder.SupplierIndents.Add(newIndent);
                orderDetails.SubOrders.Add(subOrder);
                order.OrderDetails.Add(orderDetails);

                _orderService.InsertOrUpdateGraph(order);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.CurrentNum = 0;
                    _subOrderCollection.Add(newIndent);
                    Mouse.SetCursor(Cursors.Arrow);
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

        private void deleteIndent(object parameter)
        {
            var spi = parameter as SupplierIndent;
            if (spi == null) return;

            bool confirm = _dialogService.AskConfirmation("", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                spi.ObjectState = ObjectState.Deleted;
          
                var soUnit = Units.Find(u => u.UnitId == spi.UnitId);
                Double subtractVal = 0;

                if (!Unit.Equals(soUnit))
                {
                    double subOrderVal = CalculateUnitNum(soUnit, spi.Num);
                    subtractVal = ReverseCalculateUnitNum(Unit, subOrderVal);
                }
                else
                {
                    subtractVal = spi.Num;
                }

                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    _subOrderCollection.Remove(spi);
                    SubOrdersNum -= subtractVal;
                    RemainNum += subtractVal;
                    this.Deleteable = true;
                    Mouse.SetCursor(Cursors.Arrow);
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

        private void showSupplierWindow(object parameter)
        {
            if (SelectedSupplier == null)
            {
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            var window = parameter as Window;
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new SupplierHistoryViewModel(_container, SelectedSupplier);
            _navigationService.ShowSupplierWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand ConfirmCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SupplierDetailsCommand { get; private set; }
        private void initializCommand()
        {
            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confirmSubOrder(); },
                (parameter) => { return CurrentSubOrder.Remain>0; }
                ).AddListener<SupplierIndentViewModel>(this,x=>x.CurrentSubOrder);

            DeleteCommand = new MvvmCommand(
                (parameter) => { this.deleteIndent(parameter); },
                (parameter) => { return true; }
                );

            SupplierDetailsCommand = new MvvmCommand(
                (parameter) => { this.showSupplierWindow(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly ISellerService _sellerService;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private readonly ObservableCollection<SupplierIndent> _subOrderCollection;

        #endregion

    }
}
