
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
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.API.UnitOfWork;
    using System.Windows.Controls;
    using Bska.Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;

    public sealed class ReturnSupplierIndentViewModel : BaseViewModel
    {
        #region ctor

        public ReturnSupplierIndentViewModel(IUnityContainer container)
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
            this._returnRequestCollection = new ObservableCollection<ReturnIndentRequest>();
            this.initializObj();
            this.initializCommands();
        }
        #endregion

        #region properties

        public Window Window
        {
            get;set;
        }
        
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
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
        public GlobalRequestStatus CurrentEditStatus
        {
            get { return GetValue(() => CurrentEditStatus); }
            set
            {
                SetValue(() => CurrentEditStatus, value);
                this.getReturnRequstsAsync();
            }
        }

        public ObservableCollection<ReturnIndentRequest> ReturnRequestCollection
        {
            get {return _returnRequestCollection; }
        }

        public ReturnIndentRequest SelectedRequest
        {
            get { return GetValue(() => SelectedRequest); }
            set
            {
                SetValue(() => SelectedRequest, value);
            }
        }

        public Boolean IsEnabledRequest
        {
            get { return GetValue(() => IsEnabledRequest); }
            set
            {
                SetValue(() => IsEnabledRequest, value);
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
            suppliers.Insert(0, new Users { FullName="کل سفارش های باقیمانده",UserId=0});
            CurrentEditStatus = GlobalRequestStatus.Confirmed;
            this.Units = _unitService.Queryable().ToList();
        }
        
        private void openHelpDoc()
        {
           // var viewModel = new HelpViewModel(HelpPageingAddress.Default.HelpFileName, HelpPageingAddress.Default.returnSupplierIndentHelpPage);
           // _navigationService.ShowHelpWindow(viewModel);
        }

        private async void getReturnRequstsAsync()
        {
            Mouse.SetCursor(Cursors.Wait);
            _returnRequestCollection.Clear();
            _supplierIndents.Clear();
            await Task.Run(() =>
            {
                _employeeService.GetReturnRequestByStatus(CurrentEditStatus,false).ForEach(rq =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _returnRequestCollection.Add(rq);
                    });
                });
            });
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void getindentInRequest(object parameter)
        {
            var sqR = parameter as ReturnIndentRequest;
            if (sqR == null) return;
            Mouse.SetCursor(Cursors.Wait);
            SelectedRequest = sqR;
            if (SelectedRequest.Status == GlobalRequestStatus.Confirmed)
            {
                IsEnabledRequest = true;
            }
            else
            {
                IsEnabledRequest = false;
            }

            this.Stores = _storeService.Queryable().Where(st=>st.StoreType!=StoreType.Retiring).ToList();
            _supplierIndents.Clear();
            _orderService.GetSupplierIndentByRequest(SelectedRequest.Id).ToList()
                .ForEach(sp =>
                {
                    sp.Identity = "";
                    _supplierIndents.Add(sp);
                });
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void analizationOnOrderAsync(object parameter)
        {
            var od = parameter as SubOrderModel;
            if (od == null) return;
            SelectedIndent = od;
            Analizes = null;
            Task ts = null;
            DateTime fromDate = GlobalClass._Today.AddMonths(-6);
            DateTime toDate = GlobalClass._Today;
            if (SelectedIndent != null)
            {
                double allMemo = 0;
                int kalaUid = SelectedIndent.KalaUid;
                var items = new List<AnalizModel>();
                var ounit = Units.First(u => u.UnitId == SelectedIndent.UnitId);
                if (SelectedIndent.StuffType == StuffType.Consumable)
                {
                    ts = new Task(() =>
                    {
                        _commodityService.Queryable().Where(co => co.KalaUid == SelectedIndent.KalaUid &&
                        (co.InsertDate > fromDate && co.InsertDate <= toDate))
                         .Select(x => new
                         {
                             x.PlaceOfUses,
                             x.Num,
                             x.UnitId
                         }).ToList().ForEach(co =>
                         {
                             var coU = Units.First(v => v.UnitId == co.UnitId);
                             double coMemo = co.Num;
                             if (co.PlaceOfUses.Count() > 0)
                             {
                                 co.PlaceOfUses.ForEach(cop =>
                                 {
                                     double memo = 0;
                                     var copU = Units.First(u => u.UnitId == cop.UnitId);
                                     if (coU.Equals(copU))
                                     {
                                         memo += cop.Num;
                                     }
                                     else
                                     {
                                         Unit isOrderChild = null;
                                         Unit IsPropertyChild = null;

                                         if (coU != null)
                                         {
                                             isOrderChild = _unitHelper.mainparentRecovery(coU);
                                         }

                                         if (copU != null)
                                         {
                                             IsPropertyChild = _unitHelper.mainparentRecovery(copU);
                                         }

                                         if (isOrderChild.Equals(IsPropertyChild))
                                         {
                                             Double orderval = _unitHelper.CalculateUnitNum(copU, cop.Num);
                                             double propertyVal = _unitHelper.ReverseCalculateUnitNum(coU, orderval);
                                             memo = propertyVal;

                                         }
                                     }
                                     coMemo -= memo;
                                 });
                             }

                             if (coU.Equals(ounit))
                             {
                                 allMemo += coMemo;
                             }
                             else
                             {
                                 Unit isOrderChild = null;
                                 Unit IsPropertyChild = null;

                                 if (coU != null)
                                 {
                                     isOrderChild = _unitHelper.mainparentRecovery(coU);
                                 }

                                 if (ounit != null)
                                 {
                                     IsPropertyChild = _unitHelper.mainparentRecovery(ounit);
                                 }

                                 if (isOrderChild.Equals(IsPropertyChild))
                                 {
                                     Double orderval = _unitHelper.CalculateUnitNum(coU, coMemo);
                                     double propertyVal = _unitHelper.ReverseCalculateUnitNum(ounit, orderval);
                                     allMemo += propertyVal;
                                 }
                             }
                         });

                        items.Add(new AnalizModel
                        {
                            Description = "موجودی حال حاضر",
                            Num = allMemo,
                            UnitName = ounit.Name,
                            Identity = AnalizModelIdentity.Stock
                        });

                        items.Add(_storeBillService.GetStoreBillAnalized(kalaUid, SelectedIndent.StuffType, fromDate, toDate, true));
                        items.Add(_commodityService.GetInternalDocAnaliz(kalaUid, fromDate, toDate));

                        _orderService.GetOrderDetailsByKalaUid(kalaUid, SelectedIndent.StuffType, fromDate, toDate, false)
                         .GroupBy(g => new { g.KalaUid, g.Status }).Where(g => g.Count() > 0).ForEach(og =>
                         {
                             items.Add(new AnalizModel
                             {
                                 Description = "درخواست ها با وضعیت " + og.Key.Status.GetDescription(),
                                 Num = og.Sum(b =>
                                 {
                                     double memo = 0;
                                     var punit = Units.First(t => t.UnitId == b.UnitId);
                                     if (punit.Equals(ounit))
                                     {
                                         memo = b.Num;
                                     }
                                     else
                                     {
                                         Unit isOrderChild = null;
                                         Unit IsPropertyChild = null;

                                         if (punit != null)
                                         {
                                             isOrderChild = _unitHelper.mainparentRecovery(punit);
                                         }

                                         if (ounit != null)
                                         {
                                             IsPropertyChild = _unitHelper.mainparentRecovery(ounit);
                                         }

                                         if (isOrderChild.Equals(IsPropertyChild))
                                         {
                                             Double orderval = _unitHelper.CalculateUnitNum(punit, b.Num);
                                             double propertyVal = _unitHelper.ReverseCalculateUnitNum(ounit, orderval);
                                             memo = propertyVal;
                                         }
                                     }
                                     return memo;
                                 }),

                                 UnitName = ounit.Name,
                                 Identity = AnalizModelIdentity.Order,
                                 OrderStatus = og.Key.Status
                             });
                         });

                        DispatchService.Invoke(() =>
                        {
                            Analizes = items;
                        });
                    });
                }
                else
                {
                    ts = new Task(() =>
                    {
                        items.Add(_movableAssetService.GetCurrentStoreByKalaUid(kalaUid, SelectedIndent.StuffType, false));
                        items.Add(_storeBillService.GetStoreBillAnalized(kalaUid, SelectedIndent.StuffType, fromDate, toDate, false));
                        items.Add(_movableAssetService.GetStoreInternalDocAnaliz(kalaUid, SelectedIndent.StuffType, fromDate, toDate, false));
                        _orderService.GetOrderDetailsByKalaUid(kalaUid, SelectedIndent.StuffType, fromDate, toDate, false)
                        .GroupBy(g => new { g.KalaUid, g.UnitId, g.Status }).Where(g => g.Count() > 0).ForEach(og =>
                        {
                            var u = Units.FirstOrDefault(un => un.UnitId == og.Key.UnitId);
                            items.Add(new AnalizModel
                            {
                                Description = "درخواست ها با وضعیت " + og.Key.Status.GetDescription(),
                                Num = og.Sum(b => b.Num),
                                UnitName = u?.Name,
                                Identity = AnalizModelIdentity.Order,
                                OrderStatus = og.Key.Status
                            });
                        });

                        DispatchService.Invoke(() =>
                        {
                            Analizes = items;
                        });
                    });
                }

            }

            if (ts != null)
            {
                ts.Start();
                await ts;
            }
        }

        private void showStoreUseableAsset(object parameter)
        {
            if (SelectedIndent == null)
            {
                _dialogService.ShowAlert("", "هیچ درخواستی انتخاب نشده است");
                return;
            }

            var analizM = parameter as AnalizModel;
            if (analizM == null)
            {
                analizM = new AnalizModel();
            }
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var requestStuffs = new List<Tuple<string, StuffType, int, string>>();

            _supplierIndents.GroupBy(x => new { x.KalaNo }).ForEach(od =>
            {
                requestStuffs.Add(new Tuple<string, StuffType, int, string>(od.First().StuffName, od.First().StuffType, od.First().KalaUid, od.Key.KalaNo));
            });
            var viewmodel = new StoreAssetDetailsViewModel(_container, requestStuffs, analizM);

            viewmodel.SelectedStuff = requestStuffs.FirstOrDefault(v => v.Item3 == SelectedIndent.KalaUid);
            _navigationService.ShowStoreAssetDetailsWindow(viewmodel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
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

        private void showAddWindow()
        {
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new AddReturnIndentRequestViewModel(_container);
            _navigationService.ShowAddIndentReturnRequestWindow(viewModel);
            this.getReturnRequstsAsync();
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void addDisplacementToStore(object parameter)
        {
            var comboBox = parameter as ComboBox;
            if (comboBox == null) return;
            if (comboBox.SelectedIndex == -1) return;
            var sb = comboBox.Tag as SubOrderModel;
            if (sb == null) return;
            Store selectedStore = comboBox.SelectedItem as Store;
            if (selectedStore != null)
            {
                sb.Identity = selectedStore.StoreId.ToString();
            }
        }

        private void confirmRRequest()
        {
            if (_supplierIndents.Any(sb => string.IsNullOrWhiteSpace(sb.Identity)))
            {
                _dialogService.ShowAlert("توجه", "انتخاب انبار برای همه سفارش ها الزامی است");
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
                Mouse.SetCursor(Cursors.Wait);
                Order currentOrder = null;
                var orderList = new List<Order>();
                _supplierIndents.ForEach(spi =>
                {
                    var spIndent = _orderService.GetSupplierIndent(spi.SupplierIndentId);
                    var subOrder = _orderService.GetSubOrderBySupplierIndent(spi.SupplierIndentId);
                    var subOrders = _orderService.GetSubOrders(subOrder.OrderDetailsId.Value);
                    var currentOrderDetails = _orderService.GetOrderDetails(subOrder.OrderDetailsId.Value);
                    var order = _orderService.Query(o => o.OrderId == currentOrderDetails.OrderId)
                      .Include(o => o.OrderDetails).Include(p => p.Person).Select().Single();

                    if (currentOrder == null)
                    {
                        currentOrder = order;
                    }
                    else if (currentOrder.OrderId != order.OrderId)
                    {
                        currentOrder = order;
                    }

                    var newSb = new SubOrder
                    {
                        Identity=spi.Identity,
                        Num=spIndent.Remain,
                        ObjectState=ObjectState.Added,
                        UnitId= spIndent.UnitId,
                        Type=SubOrderType.Store,
                        State=SubOrderState.None,
                        Remain=spIndent.Remain,
                    };

                    spIndent.State = SupplierIndentState.Delivery;
                    spIndent.Remain = 0;
                    spIndent.Num = (spi.Num - spi.Remain);
                    spIndent.ObjectState = ObjectState.Modified;
                    subOrder.SupplierIndents.Add(spIndent);

                    subOrder.Remain -= spi.Remain;
                    subOrder.Num -= spi.Remain;
                    if (subOrder.Remain <= 0
                       && subOrder.State != SubOrderState.Deliviry)
                    {
                        subOrder.State = SubOrderState.Deliviry;
                    }

                    subOrder.ObjectState = ObjectState.Modified;
                    currentOrderDetails.SubOrders.Add(subOrder);

                    currentOrderDetails.SubOrders.Add(newSb);
                    currentOrder.OrderDetails.Add(currentOrderDetails);
                    if (!orderList.Contains(currentOrder))
                    {
                        orderList.Add(currentOrder);
                    }
                });

                SelectedRequest.ObjectState = ObjectState.Modified;
                SelectedRequest.Status = GlobalRequestStatus.Completed;
                employee.ReturnIndentRequests.Add(SelectedRequest);

                try
                {
                    _employeeService.InsertOrUpdateGraph(employee);
                    _orderService.InsertGraphRange(orderList);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.getReturnRequstsAsync();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region commands
        
        public ICommand ReportCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand AnalizCommand { get; private set; }
        public ICommand DoubleClickListBoxItemCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ReturnRequestDetailsCommand { get; private set; }
        public ICommand IndentDetailsCommand { get; private set; }
        public ICommand AddRequestIndentCommand { get; private set; }
        public ICommand StoreSelectCommand { get; private set; }
        private void initializCommands()
        {

            HelpCommand = new MvvmCommand(
               (parameter) => { this.openHelpDoc(); },
               (parameter) => { return true; }
              );


            AnalizCommand = new MvvmCommand(
                 (parameter) => { this.analizationOnOrderAsync(parameter); },
                (parameter) => { return true; }
                );

            DoubleClickListBoxItemCommand = new MvvmCommand(
               (parameter) => { this.showStoreUseableAsset(parameter); },
               (parameter) => { return true; }
              );
            
            IndentDetailsCommand = new MvvmCommand(
             (parameter) => { this.showIndentDetails(parameter); },
             (parameter) => { return true; }
             );

            AddRequestIndentCommand = new MvvmCommand(
           (parameter) => { this.showAddWindow(); },
           (parameter) => { return true; }
           );

            ReturnRequestDetailsCommand = new MvvmCommand(
           (parameter) => { this.getindentInRequest(parameter); },
           (parameter) => { return true; }
           );

            StoreSelectCommand = new MvvmCommand(
            (parameter) => { this.addDisplacementToStore(parameter); },
            (parameter) => { return IsEnabledRequest; }
            ).AddListener<ReturnSupplierIndentViewModel>(this,th=>th.IsEnabledRequest);

            SaveCommand= new MvvmCommand(
            (parameter) => { this.confirmRRequest(); },
            (parameter) => { return IsEnabledRequest; }
            ).AddListener<ReturnSupplierIndentViewModel>(this, th => th.IsEnabledRequest);
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
        private readonly ObservableCollection<ReturnIndentRequest> _returnRequestCollection;

        #endregion

    }
}
