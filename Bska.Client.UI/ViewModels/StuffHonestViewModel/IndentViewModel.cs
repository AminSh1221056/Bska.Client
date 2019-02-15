
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Common;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Windows;
    using Bska.Client.Repository.Model;

    public sealed class IndentViewModel : BaseViewModel
    {
        #region ctor

        public IndentViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._storeService = _container.Resolve<IStoreService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._subOrderCollection = new ObservableCollection<SubOrder>();
            ToStore = true;
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

        public List<OrderDetails> CurrentOrders
        {
            get { return GetValue(() => CurrentOrders); }
            set
            {
                SetValue(() => CurrentOrders, value);
            }
        }

        public OrderDetails CurrentOrderDetails
        {
            get { return GetValue(() => CurrentOrderDetails); }
            set
            {
                SetValue(() => CurrentOrderDetails, value);
                this.initializObj();
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

        public Unit SelectedUnit
        {
            get { return GetValue(() => SelectedUnit); }
            set
            {
                SetValue(() => SelectedUnit, value);
            }
        }

        public Boolean ToStore
        {
            get { return GetValue(() => ToStore); }
            set
            {
                SetValue(() => ToStore, value);
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

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        public ObservableCollection<SubOrder> SubOrdersCollection
        {
            get { return _subOrderCollection; }
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

        public Dictionary<string,string> IndentItems
        {
            get { return GetValue(() => IndentItems); }
            set
            {
                SetValue(() => IndentItems, value);
            }
        }
        
        public string SelectedIndentType
        {
            get { return GetValue(() => SelectedIndentType); }
            set
            {
                SetValue(() => SelectedIndentType, value);
                initOnSelectedIndent();
            }
        }

        public List<PersonModel> TrenderUsers
        {
            get { return GetValue(() => TrenderUsers); }
            set
            {
                SetValue(() => TrenderUsers, value);
            }
        }

        public PersonModel SelectedUser
        {
            get { return GetValue(() => SelectedUser); }
            set
            {
                SetValue(() => SelectedUser, value);
            }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
            this.Units = _unitService.Query().Include(u => u.Parent).Select().ToList();
            this.Unit = this.SelectedUnit = Units.Find(x => x.UnitId == CurrentOrderDetails.UnitId);
            this.IndentItems = new Dictionary<string, string> { {"A001","انبار" }, { "A002","خرید"},
                { "A003","مناقصه"}, { "A004","جابه جایی داخلی"} };
            
            _subUnits.Clear();
            var mParent = unitParentRecovery(SelectedUnit);
            _rootUnit = new UnitTreeViewModel(mParent, _container);
            _subUnits.Add(_rootUnit);
            _subOrderCollection.Clear();
            _orderService.GetSubOrders(CurrentOrderDetails.OrderDetialsId).ToList().ForEach(so =>
            {
                _subOrderCollection.Add(so);
            });
            CurrentNum=RemainNum =CurrentOrderDetails.Num-(SubOrdersNum=await calculateSubOrdersNum());
            if (RemainNum > 0)
            {
                this.Deleteable = true;
            }
            else
            {
                this.Deleteable = false;
            }
        }

        private void initOnSelectedIndent()
        {
            if (!string.IsNullOrEmpty(SelectedIndentType))
            {
                ToStore = false;
                switch (SelectedIndentType)
                {
                    case "A001":
                        ToStore = true;
                        Stores = _storeService.Queryable().Where(x => x.StoreType != StoreType.Retiring).ToList();
                        break;
                    case "A003":
                        this.TrenderUsers = _personService.GetAllUserToOrganizRoles().ToList();
                        break;
                }
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
                    var soUnit = Units.Find(u => u.UnitId==so.UnitId);
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

        private void showInternalTransferIndent(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            if (CurrentOrderDetails.StuffType == StuffType.Consumable)
            {
                _dialogService.ShowAlert("توجه", "اموال مصرفی قابلیت انتقال داخلی را ندارد");
                ToStore = true;
                return;
            }

            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new InternalTransferViewModel(_container);
            viewModel.CurrentOrderDetails = CurrentOrderDetails;
            viewModel.IndentNum = SubOrdersNum;
            viewModel.Remain = RemainNum;
            var diswindow = _navigationService.ShowDisplacementIndentWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            if (diswindow.DialogResult == true)
            {
                viewModel._suborders.ForEach(so =>
                {
                    _subOrderCollection.Add(so);
                    SubOrdersNum += 1;
                    RemainNum -= 1;
                });
            }

            if (RemainNum == 0)
            {
                this.Deleteable = false;
            }
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
            if (string.IsNullOrEmpty(SelectedIndentType))
            {
                _dialogService.ShowError("خطا", "هیچ نوع سفارشی انتخاب نشده است");
                return;
            }

            if (CurrentNum <= 0)
            {
                _dialogService.ShowError("خطا", "مقدار وارد شده صحیح نیست");
                return;
            }

            var order = _orderService.Query(x=>x.OrderId==CurrentOrderDetails.OrderId)
                .Include(o=>o.OrderDetails).Select().Single();
            var orderDetails = order.OrderDetails.Single(od => od.OrderDetialsId == CurrentOrderDetails.OrderDetialsId);

            order.ObjectState = ObjectState.Modified;
            orderDetails.ObjectState = ObjectState.Modified;
            var subOrder = new SubOrder
            {
                Num=CurrentNum,
                Remain=CurrentNum,
                ObjectState=ObjectState.Added,
                State=SubOrderState.None,
            };

            if (string.Equals("A001",SelectedIndentType))
            {
                if (SelectedStore == null)
                {
                    _dialogService.ShowAlert("انتخاب انبار", "هیچ انباری انتخاب نشده است");
                    return;
                }
                subOrder.Identity = SelectedStore.StoreId.ToString();
                subOrder.Type = SubOrderType.Store;
            }
            else if (string.Equals("A002", SelectedIndentType))
            {
                subOrder.Type = SubOrderType.Supplier;
                subOrder.Identity = "Buy";
            }
            else if (string.Equals("A003", SelectedIndentType))
            {
                if (SelectedUser == null)
                {
                    _dialogService.ShowAlert("انتخاب انبار", "هیچ مدیری انتخاب نشده است");
                    return;
                }
                subOrder.Type = SubOrderType.TenderOffer;
                subOrder.Identity = SelectedUser.PersonId.ToString();
            }
            else
            {
                subOrder.Type = SubOrderType.Displacement;
            }

            Double subtractVal = 0;

            if (!SelectedUnit.Equals(Unit))
            {
                var selectedUnit = Units.Find(u => u.UnitId == SelectedUnit.UnitId);
                double detailsVal = CalculateUnitNum(Unit, RemainNum);
                double subOrderVal = CalculateUnitNum(selectedUnit, subOrder.Num);
                if (subOrderVal > detailsVal)
                {
                    _dialogService.ShowAlert("توجه", "تعداد وارد شده برای این سفارش بیش از مقدار درخواست شده است");
                    return;
                }
                subOrder.UnitId = SelectedUnit.UnitId;
                subtractVal = ReverseCalculateUnitNum(Unit, subOrderVal);
            }
            else
            {
                if (subOrder.Num > RemainNum)
                {
                    _dialogService.ShowAlert("توجه", "تعداد وارد شده برای این سفارش بیش از مقدار درخواست شده است");
                    return;
                }
                subOrder.UnitId = SelectedUnit.UnitId;
                subtractVal = CurrentNum;
            }

            SubOrdersNum += subtractVal;
            RemainNum -= subtractVal;

            if (RemainNum == 0)
            {
                orderDetails.State = OrderDetailsState.SubOrder;
                this.Deleteable = false;
                var oldHistory = _orderService.GetUserHistories(CurrentOrderDetails.OrderDetialsId).SingleOrDefault(ou =>! ou.UserDecision);
                if (oldHistory != null)
                {
                    oldHistory.ObjectState = ObjectState.Modified;
                    oldHistory.Description = "سفارش درخواست توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                              "در تاریخ:" + " " + GlobalClass._Today.PersianDateString();
                    oldHistory.Identity = "Indent";
                    oldHistory.UserDecision = true;
                    orderDetails.OrderUserHistories.Add(oldHistory);
                }
            }

            if (order.OrderDetails.All(od => od.State == OrderDetailsState.SubOrder || od.State==OrderDetailsState.Deliviry
            || od.State==OrderDetailsState.ToOther))
            {
                order.Status = OrderStatus.SubOrder;
            }

            orderDetails.SubOrders.Add(subOrder);
            _orderService.InsertOrUpdateGraph(order);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                this.CurrentNum = 0;
                _subOrderCollection.Add(subOrder);
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

        private void deleteSubOrder(object parameter)
        {
            var subOrder = parameter as SubOrder;
            if (subOrder == null) return;
            subOrder.ObjectState = ObjectState.Deleted;
            var order = _orderService.Query(x => x.OrderId == CurrentOrderDetails.OrderId)
               .Include(o => o.OrderDetails).Select().Single();
            var orderDetails = order.OrderDetails.Single(od => od.OrderDetialsId == CurrentOrderDetails.OrderDetialsId);
            var soUnit = Units.Find(u => u.UnitId == subOrder.UnitId);
            Double subtractVal = 0;

            if (!Unit.Equals(soUnit))
            {
                double subOrderVal = CalculateUnitNum(soUnit, subOrder.Num);
                subtractVal = ReverseCalculateUnitNum(Unit, subOrderVal);
            }
            else
            {
                subtractVal = subOrder.Num;
            }

            orderDetails.SubOrders.Remove(subOrder);
            _orderService.InsertOrUpdateGraph(order);

            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                _subOrderCollection.Remove(subOrder);
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

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand DiplacementIndentCommand { get; private set; }
        private void initializCommand()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.confirmSubOrder(); },
                (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) => { deleteSubOrder(parameter); },
                (parameter) => { return this.Deleteable; }
                ).AddListener<IndentViewModel>(this,s=>s.Deleteable);

            DiplacementIndentCommand = new MvvmCommand(
                (parameter) => { showInternalTransferIndent(parameter); },
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
        private readonly IStoreService _storeService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private readonly ObservableCollection<SubOrder> _subOrderCollection;
        private UnitTreeViewModel _rootUnit;

        #endregion
    }
}
