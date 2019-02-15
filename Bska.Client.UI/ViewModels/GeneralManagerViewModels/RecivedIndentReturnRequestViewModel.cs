
namespace Bska.Client.UI.ViewModels.GeneralManagerViewModels
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.Common;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using System.Threading.Tasks;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using System.Windows.Controls;
    using Bska.Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;

    public sealed class RecivedIndentReturnRequestViewModel : BaseViewModel
    {
        #region cror
        public RecivedIndentReturnRequestViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._supplierIndents = new ObservableCollection<SubOrderModel>();
            this.SupplierIndentFilteredView = new CollectionViewSource { Source = SupplierIndents }.View;
            this._unitHelper = new UnitHelper();
            this._selectedItems = new HashSet<long>();
            this.initializCommands();
            this.initialzObj();
        }

        #endregion

        #region properties

        public Window Window { get; set; }

        public ObservableCollection<SubOrderModel> SupplierIndents
        {
            get { return _supplierIndents; }
        }

        public ICollectionView SupplierIndentFilteredView { get; set; }

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
                this.getReturnRequsts();
            }
        }

        public List<ReturnIndentRequest> RIndents
        {
            get { return GetValue(() => RIndents); }
            set
            {
                SetValue(() => RIndents, value);
            }
        }

        public ReturnIndentRequest SelectedRequest
        {
            get { return GetValue(() => SelectedRequest); }
            set
            {
                SetValue(() => SelectedRequest, value);
                this.getSupplierIndent();
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

        private void initialzObj()
        {
            this.CurrentEditStatus = GlobalRequestStatus.Pending;
            this.Units = _unitService.Queryable().ToList();
            IsEnabledRequest = false;
        }

        private void getReturnRequsts()
        {
            Mouse.SetCursor(Cursors.Wait);
            _supplierIndents.Clear();
            this.RIndents = _employeeService.GetReturnRequestByStatus(CurrentEditStatus,true).ToList();
            this.SelectedRequest = null;
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void SearchOrder()
        {
            SupplierIndentFilteredView.Filter = (obj) =>
            {
                var model = obj as SubOrderModel;
                return model.StuffName.StartsWith(SearchCriteria)
                    || model.SubOrderId.ToString() == SearchCriteria;
            };
        }

        private void getSupplierIndent()
        {
            if (SelectedRequest == null) return;
            Mouse.SetCursor(Cursors.Wait);
            _supplierIndents.Clear();
            _orderService.GetSupplierIndentByRequest(SelectedRequest.Id).ForEach(sp =>
            {
                sp.IsSelected = true;
                _supplierIndents.Add(sp);
            });

            if (SelectedRequest.Status == GlobalRequestStatus.Pending)
            {
                IsEnabledRequest = true;
            }
            else
            {
                IsEnabledRequest = false;
            }

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
        private void initOnSelecting(object parameter)
        {
            var ch = parameter as CheckBox;
            var sbm = ch.Tag as SubOrderModel;
            if (sbm != null)
            {
                if (ch.IsChecked == false)
                {
                    if (!_selectedItems.Contains(sbm.SupplierIndentId))
                    {
                        _selectedItems.Add(sbm.SupplierIndentId);
                    }
                }
                else
                {
                    _selectedItems.Remove(sbm.SupplierIndentId);
                }
            }
        }

        private void confirmRequest(GlobalRequestStatus status)
        {
            if (SelectedRequest == null)
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
                if (_selectedItems.Count > 0)
                {
                    _selectedItems.ForEach(si =>
                    {
                        var sp = _orderService.GetSupplierIndent(si);
                        SelectedRequest.SupplierIndents.Remove(sp);
                    });
                }

                SelectedRequest.Description += string.Format("{0} تایید درخواست عودت خرید در تاریخ {1}", Environment.NewLine,GlobalClass._Today.PersianDateString());
                SelectedRequest.ObjectState =ObjectState.Modified;
                SelectedRequest.Status = status;
                employee.ReturnIndentRequests.Add(SelectedRequest);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _employeeService.InsertOrUpdateGraph(employee);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    IsEnabledRequest = false;
                    this._supplierIndents.Clear();
                    this.getReturnRequsts();
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
        
        #endregion

        #region commands

        public ICommand ConfirmCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand AnalizCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand IndentDetailsCommand { get; private set; }
        public ICommand DoubleClickListBoxItemCommand { get; private set; }
        private void initializCommands()
        {
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

            SelectCommand = new MvvmCommand(
              (parameter) => { this.initOnSelecting(parameter); },
              (parameter) => { return IsEnabledRequest; }
                ).AddListener<RecivedIndentReturnRequestViewModel>(this, x => x.IsEnabledRequest);

            RejectCommand = new MvvmCommand(
                (parameter) => { this.confirmRequest(GlobalRequestStatus.Rejected); },
                (parameter) => { return IsEnabledRequest; }
                ).AddListener<RecivedIndentReturnRequestViewModel>(this,x=>x.IsEnabledRequest);

            ConfirmCommand = new MvvmCommand(
              (parameter) => { this.confirmRequest(GlobalRequestStatus.Confirmed); },
              (parameter) => { return IsEnabledRequest; }
              ).AddListener<RecivedIndentReturnRequestViewModel>(this, x => x.IsEnabledRequest);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly UnitHelper _unitHelper;
        private readonly IStoreBillService _storeBillService;
        private readonly ObservableCollection<SubOrderModel> _supplierIndents;
        private readonly HashSet<Int64> _selectedItems;

        #endregion

    }
}
