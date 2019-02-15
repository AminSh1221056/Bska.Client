
namespace Bska.Client.UI.ViewModels.GeneralManagerViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Data.Entity;

    public class InternalOrderRecivedViewModel : BaseViewModel
    {
        #region ctor

        public InternalOrderRecivedViewModel(IUnityContainer container,string currentManager)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._orderCollection = new ObservableCollection<OrderModel>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            _collection = new ObservableCollection<OrderDetails>();
            this.OrderFilteredView = new CollectionViewSource { Source = OrderCollection }.View;
            this._unitHelper = new UnitHelper();
            this._currentManager = currentManager;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties
        public Window Window
        {
            get;
            set;
        }

        public ObservableCollection<OrderModel> OrderCollection
        {
            get { return _orderCollection; }
        }

        public OrderModel OMSelected
        {
            get { return GetValue(() => OMSelected); }
            set
            {
                SetValue(() => OMSelected, value);
                this.getOrderDetails();
            }
        }

        public ICollectionView OrderFilteredView { get; set; }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchOrder();
            }
        }

        public ObservableCollection<OrderDetails> Collection
        {
            get { return _collection; }
        }

        public OrderDetails Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
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
        public Boolean InternalVisible
        {
            get { return GetValue(() => InternalVisible); }
            set
            {
                SetValue(() => InternalVisible, value);
            }
        }

        public Boolean DisplacementVisible
        {
            get { return GetValue(() => DisplacementVisible); }
            set
            {
                SetValue(() => DisplacementVisible, value);
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
        public Boolean OrderHistoryVisible
        {
            get { return GetValue(() => OrderHistoryVisible); }
            set
            {
                SetValue(() => OrderHistoryVisible, value);
            }
        }

        public List<OrderUserHistory> OrderUserHistories
        {
            get { return GetValue(() => OrderUserHistories); }
            set
            {
                SetValue(() => OrderUserHistories, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            _orderCollection.Clear();
            if (string.Equals(_currentManager, "MunitionManager"))
            {
                _orderService.GetRecivedOrders(true, 0, true, OrderStatus.OrganizManagerConfirm).ToList().ForEach(o =>
                        {
                            _orderCollection.Add(o);
                        });
            }
            else
            {
                _orderService.GetRecivedOrders(true, 0, true, OrderStatus.ManagerConfirm).ToList().ForEach(o =>
                         {
                             _orderCollection.Add(o);
                         });
            }

            this.Units = _unitService.Queryable().ToList();
            InternalVisible = true;
        }

        private void searchOrder()
        {
            OrderFilteredView.Filter = (obj) =>
            {
                var od = obj as Order;
                return od.OrderId.ToString().StartsWith(SearchCriteria);
            };
        }

        private void getOrderDetails()
        {
            if (OMSelected == null) return;
            _collection.Clear();
            _orderService.Queryable().Where(x=>x.OrderId==OMSelected.OrderId)
                .SelectMany(x=>x.OrderDetails).Include(x=>x.OrderUserHistories).ForEach(od =>
            {
                _collection.Add(od);
            });
        }

        private void confirmOrder()
        {
            if (OMSelected == null)
            {
                _dialogService.ShowError("انتخاب درخواست", "هیچ درخواستی انتخاب نشده است");
                return;
            }

            bool isConfirm = _dialogService.AskConfirmation("توجه", ErrorMessages.Default.AskConfrimation);
            if (isConfirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                var order = _orderService.Query(x => x.OrderId == OMSelected.OrderId).Include(o => o.OrderDetails).Select().Single();
                order.ObjectState = ObjectState.Modified;
                order.ModifiedDate = GlobalClass._Today;
                if (string.Equals(_currentManager, "GeneralManager"))
                {
                    order.Status = OrderStatus.SubOrder;
                }
                else
                {
                    order.Status = OrderStatus.ManagerConfirm;
                }

                order.OrderDetails.ToList().ForEach(od =>
                {
                    od.ObjectState = ObjectState.Modified;

                    if (od.IsReject)
                    {
                        od.OrderUserHistories.Add(new OrderUserHistory
                        {
                            Description = "رد درخواست برای انبار توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                            "در تاریخ:" + " " + GlobalClass._Today.PersianDateString() + ".",
                            ObjectState = ObjectState.Added,
                            UserDecision = true,
                            UserId = UserLog.UniqueInstance.LogedUser.UserId
                        });
                    }
                    else
                    {
                        od.OrderUserHistories.Add(new OrderUserHistory
                        {
                            Description = "تایید درخواست برای انبار توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                            "در تاریخ:" + " " + GlobalClass._Today.PersianDateString() + ".",
                            ObjectState = ObjectState.Added,
                            UserDecision = true,
                            UserId = UserLog.UniqueInstance.LogedUser.UserId
                        });

                        if (string.Equals(_currentManager, "GeneralManager"))
                        {
                            od.State = OrderDetailsState.SubOrder;
                            od.SubOrders.Add(new SubOrder
                            {
                                Identity = "Buy",
                                Num = od.Num,
                                Remain = od.Num,
                                ObjectState = ObjectState.Added,
                                State = SubOrderState.None,
                                Type = SubOrderType.Supplier,
                                UnitId = od.UnitId,
                            });
                        }
                        else
                        {
                            od.State = OrderDetailsState.ManagerConfirm;
                        }
                       
                    }
                    order.OrderDetails.Add(od);
                });

                if (order.OrderDetails.All(x => x.IsReject))
                {
                    order.Status = OrderStatus.Reject;
                }

                _orderService.InsertOrUpdateGraph(order);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    _orderCollection.Remove(OMSelected);
                    _collection.Clear();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void rejectOrder()
        {
            if (OMSelected == null)
            {
                _dialogService.ShowError("انتخاب درخواست", "هیچ درخواستی انتخاب نشده است");
                return;
            }

            bool isConfirm = _dialogService.AskConfirmation("توجه", ErrorMessages.Default.AskConfrimation);
            if (isConfirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                var order = _orderService.Query(x => x.OrderId == OMSelected.OrderId).Select().Single();
                order.ObjectState = ObjectState.Modified;
                order.ModifiedDate = GlobalClass._Today;
                order.Status = OrderStatus.Reject;
                _orderService.Update(order);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    _orderCollection.Remove(OMSelected);
                    _collection.Clear();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void showOrderDetailsWindow(object parameter)
        {
            var orderDetails = parameter as OrderDetails;
            if (orderDetails == null) return;
            this.Selected = orderDetails;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new OrderDetailsManageViewModel(_container,true);
            viewModel.Units = this.Units;
            viewModel.CurrentOrderDetails = orderDetails;
            viewModel.AllOrderDetails = _collection.ToList();
            _navigationService.ShowOrderDetailsWindow(viewModel);
            _collection.Clear();
            viewModel.AllOrderDetails.ForEach(x =>
            {
                _collection.Add(x);
            });
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }
        
        private void initSelectedDetails(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            if (od.IsReject)
            {
                od.IsReject = false;
            }
            else
            {
                od.IsReject = true;
            }
            int index = _collection.IndexOf(od);
            _collection.RemoveAt(index);
            _collection.Insert(index, od);
            this.Selected = od;
        }

        private async void analizationOnOrderAsync(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            Selected = od;
            Analizes = null;
            OrderHistoryVisible = false;
            Task ts = null;
            DateTime fromDate = GlobalClass._Today.AddMonths(-6);
            DateTime toDate = GlobalClass._Today;
            if (OMSelected.OrderType == OrderType.InternalRequest
                || OMSelected.OrderType == OrderType.Store)
            {
                if (Selected != null)
                {
                    int kalaUid = Selected.KalaUid;
                    var items = new List<AnalizModel>();
                    var ounit = Units.First(u => u.UnitId == Selected.UnitId);
                    if (Selected.StuffType == StuffType.Consumable)
                    {
                        ts = new Task(() =>
                        {
                            double allMemo = 0;
                            _commodityService.Queryable().Where(co => co.KalaUid == Selected.KalaUid &&
                            (co.InsertDate > fromDate && co.InsertDate <= toDate))
                             .Include(co => co.PlaceOfUses).Select(x => new
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

                            items.Add(_storeBillService.GetStoreBillAnalized(kalaUid, Selected.StuffType, fromDate, toDate, true));
                            items.Add(_commodityService.GetInternalDocAnaliz(kalaUid, fromDate, toDate));

                            _orderService.GetOrderDetailsByKalaUid(kalaUid, Selected.StuffType, fromDate, toDate, false)
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
                            items.Add(_movableAssetService.GetCurrentStoreByKalaUid(kalaUid, Selected.StuffType, false));
                            items.Add(_storeBillService.GetStoreBillAnalized(kalaUid, Selected.StuffType, fromDate, toDate, false));
                            items.Add(_movableAssetService.GetStoreInternalDocAnaliz(kalaUid, Selected.StuffType, fromDate, toDate, false));
                            _orderService.GetOrderDetailsByKalaUid(kalaUid, Selected.StuffType, fromDate, toDate, false)
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
            }

            if (ts != null)
            {
                ts.Start();
                await ts;
            }
        }

        private void showStoreUseableAsset(object parameter)
        {
            if (OMSelected == null || _collection.Count <= 0)
            {
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var requestStuffs = new List<Tuple<string, StuffType, int, string>>();
            _collection.GroupBy(x => new { x.kalaNo }).ForEach(od =>
            {
                requestStuffs.Add(new Tuple<string, StuffType, int, string>(od.First().StuffName,
                    od.First().StuffType, od.First().KalaUid, od.Key.kalaNo));
            });
            var analizM = parameter as AnalizModel;
            if (analizM == null)
            {
                analizM = new AnalizModel();
            }
            var viewmodel = new StoreAssetDetailsViewModel(_container, requestStuffs, analizM);

            if (Selected != null)
            {
                viewmodel.SelectedStuff = requestStuffs.FirstOrDefault(v => v.Item3 == Selected.KalaUid);
            }
            _navigationService.ShowStoreAssetDetailsWindow(viewmodel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void getOrderUserHistory(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od != null)
            {
                Mouse.SetCursor(Cursors.Wait);
                this.Selected = od;
                this.OrderHistoryVisible = true;
                this.OrderUserHistories = od.OrderUserHistories.Where(ou => ou.UserDecision).ToList();
                Mouse.SetCursor(Cursors.Arrow);
            }
        }
        #endregion

        #region commands

        public ICommand RemoveCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand AnalizCommand { get; private set; }
        public ICommand StoreDetailsCommand { get; private set; }
        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand OrderDetailsHistoryCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand DoubleClickListBoxItemCommand { get; private set; }

        private void initializCommands()
        {
            RejectCommand = new MvvmCommand(
             (parameter) => { this.rejectOrder(); },
             (parameter) => { return true; }
             );

            ConfirmCommand = new MvvmCommand(
             (parameter) => { this.confirmOrder(); },
             (parameter) => { return OMSelected != null; }
             ).AddListener<RecivedOrderViewModel>(this, x => x.OMSelected);

            RemoveCommand = new MvvmCommand(
               (parameter) => { this.initSelectedDetails(parameter); },
               (parameter) => { return true; }
               );

            AnalizCommand = new MvvmCommand(
               (parameter) => { this.analizationOnOrderAsync(parameter); },
              (parameter) => { return true; }
              );

            StoreDetailsCommand = new MvvmCommand(
           (parameter) => { this.showStoreUseableAsset(null); },
           (parameter) => { return OMSelected != null; }).AddListener<RecivedOrderViewModel>(this, x => x.OMSelected);

            OrderDetailsCommand = new MvvmCommand(
             (parameter) => { this.showOrderDetailsWindow(parameter); },
             (parameter) => { return true; }
             );

            OrderDetailsHistoryCommand = new MvvmCommand(
              (parameter) => { this.getOrderUserHistory(parameter); },
              (parameter) => { return true; }
              );

            DoubleClickListViewItemCommand = new MvvmCommand(
              (parameter) => { this.analizationOnOrderAsync(parameter); },
              (parameter) => { return true; }
              );

            DoubleClickListBoxItemCommand = new MvvmCommand(
                 (parameter) => { this.showStoreUseableAsset(parameter); },
             (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ObservableCollection<OrderModel> _orderCollection;
        private readonly IMovableAssetService _movableAssetService;
        private readonly UnitHelper _unitHelper;
        private readonly IOrderService _orderService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IStoreBillService _storeBillService;
        private readonly IUnitService _unitService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<OrderDetails> _collection;
        private string _currentManager = "GeneralManager";

        #endregion
    }
}
