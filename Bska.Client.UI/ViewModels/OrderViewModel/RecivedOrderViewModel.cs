
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.PersonDetailsInfoViewModels;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    public sealed class RecivedOrderViewModel : BaseViewModel
    {
        #region ctor

        public RecivedOrderViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository",_unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._orderCollection = new ObservableCollection<OrderModel>();
            this._unitService = _container.Resolve<IUnitService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._personService = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this.OrderFilteredView = new CollectionViewSource { Source = OrderCollection }.View;
            this._selectedAssets = new HashSet<MovableAsset>();
            this._collection = new ObservableCollection<OrderDetails>();
            this._allOrganiz = new List<EmployeeDesign>();
            this._unitHelper = new UnitHelper();
            this._recivedTypes = new Dictionary<string, object>();
            this.initializObj();
            this.initializCommands();
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

        public String Description
        {
            get { return GetValue(() => Description); }
            set
            {
                SetValue(() => Description, value);
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

        public ObservableCollection<OrderModel> OrderCollection
        {
            get { return _orderCollection; }
        }

        public ICollectionView OrderFilteredView { get; set; }

        public OrderModel OMSelected
        {
            get { return GetValue(() => OMSelected); }
            set
            {
                SetValue(() => OMSelected, value);
                this.getOrderDetails();
            }
        }

        public ObservableCollection<OrderDetails> Collection
        {
            get { return _collection; }
        }

        public List<MovableAsset> DisCollection
        {
            get { return GetValue(() => DisCollection); }
            set
            {
                SetValue(() => DisCollection, value);
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

        public List<OrderUserHistory> OrderUserHistories
        {
            get { return GetValue(() => OrderUserHistories); }
            set
            {
                SetValue(() => OrderUserHistories, value);
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
        public MovableAsset DisSelected
        {
            get { return GetValue(() => DisSelected); }
            set
            {
                SetValue(() => DisSelected, value);
            }
        }

        public OrderDetails Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
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

        public Dictionary<string, object> RecivedTypes
        {
            get
            {
                return _recivedTypes;
            }
            set
            {
                _recivedTypes = value;
                OnPropertyChanged("RecivedTypes");
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

        public Boolean IsEditableOrder
        {
            get { return GetValue(() => IsEditableOrder); }
            set
            {
                SetValue(() => IsEditableOrder, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            SelectedRecivedType = new Dictionary<string, object>();
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                _recivedTypes.Add("درخواست های رسیده", "A001");
            }
            else
            {
                _recivedTypes.Add("درخواست های رسیده", "A001");
                _recivedTypes.Add("درخواست های در راه", "A002");
            }

            this.Units = _unitService.Queryable().ToList();
            SelectedRecivedType.Add("درخواست های رسیده", "A001");
            InternalVisible = true;
            this.initOnRecivedTypes();
        }
        
        private async void initOnRecivedTypes()
        {
            _orderCollection.Clear();

            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                await Task.Run(() =>
                {
                    _orderService.GetRecivedOrders(true, 0, false, OrderStatus.None).ToList().ForEach(o =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _orderCollection.Add(o);
                        });
                    });
                });

            }
            else
            {
                await Task.Run(() =>
                {
                    _orderService.GetRecivedOrganizManageOrders(UserLog.UniqueInstance.LogedUser.UserId).ForEach(o =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            _orderCollection.Add(o);
                        });
                    });
                });
            }

            filterOnRecivedType("درخواست های رسیده", true);
        }

        internal void filterOnRecivedType(string key, bool isChecked)
        {
            if (this.SelectedRecivedType.Count==_recivedTypes.Count)
            {
                this.OrderFilteredView.Filter = null;
                this.SearchCriteria = "";
            }
            else if (this.SelectedRecivedType.Count <= 0)
            {
                this.OrderFilteredView.Filter = (obj) =>
                {
                    var order = obj as OrderModel;
                    return order.NationalId=="-1";
                };
            }
            else
            {
                if (this.SelectedRecivedType.ContainsValue("A001"))
                {
                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = obj as OrderModel;
                        return order.IsEditable;
                    };
                }
                else if (this.SelectedRecivedType.ContainsKey("A002"))
                {
                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = obj as OrderModel;
                        return !order.IsEditable;
                    };
                }
            }
        }

        private void SearchOrder()
        {
            this.OrderFilteredView.Filter = ((obj) =>
            {
                var order = obj as OrderModel;
                if (order != null)
                {
                    return order.OrderId.ToString().StartsWith(SearchCriteria)
                        || order.PersonName.Contains(SearchCriteria);
                }
                return false;
            });
        }

        private void getOrderDetails()
        {
            if (OMSelected != null)
            {
                Mouse.SetCursor(Cursors.Wait);
                this.OrderUserHistories = null;
                Boolean ismanager = false;
                if (Thread.CurrentPrincipal.IsInRole("Manager"))
                {
                    ismanager = true;
                }

                if (OMSelected.OrderType == OrderType.InternalRequest)
                {
                    _collection.Clear();
                    DisplacementVisible = false;
                    InternalVisible = true;
                    _currentOrder = _orderService.Find(OMSelected.OrderId);
                    _orderService.GetRecivedOrderDetails(OMSelected.OrderId, UserLog.UniqueInstance.LogedUser.UserId, ismanager)
                        .ToList().ForEach(od =>
                    {
                        _collection.Add(od);
                    });
                }
                else
                {
                    _currentOrder = _orderService.Query(x => x.OrderId == OMSelected.OrderId).Include(o => o.OrderDetails)
                        .Include(o => o.MovableAssets).Select().Single();
                    DisCollection = _currentOrder.MovableAssets.ToList();
                    OrderUserHistories = _orderService.GetUserHistories(_currentOrder.OrderDetails.First()
                        .OrderDetialsId).Where(ou => ou.UserDecision).ToList();
                    DisplacementVisible = true;
                    InternalVisible = false;

                }
                this._allOrganiz = _employeeService.GetParentNode(1).ToList();
                if (OMSelected.IsEditable)
                {
                    this.IsEditableOrder = true;
                }
                else
                {
                    this.IsEditableOrder = false;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAsset;
            if (mAsset == null) return;
            this.DisSelected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showOrderDetailsWindow(object parameter)
        {
            var orderDetails = parameter as OrderDetails;
            if (orderDetails == null) return;
            this.Selected = orderDetails;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new OrderDetailsManageViewModel(_container, true);
            viewModel.Units = this.Units;
            viewModel.CurrentOrderDetails =(OrderDetails)orderDetails.Clone();
            viewModel.AllOrderDetails =new List<OrderDetails>();
            viewModel.AllOrderDetails.Add(viewModel.CurrentOrderDetails);
            var itemDic = new Dictionary<long, double>();
            itemDic.Add(orderDetails.OrderDetialsId, orderDetails.Num);
            _collection.Where(od=>od.OrderDetialsId!=orderDetails.OrderDetialsId).ForEach(od =>
            {
                viewModel.AllOrderDetails.Add(od.Clone() as OrderDetails);
                itemDic.Add(od.OrderDetialsId, od.Num);
            });
            var window= _navigationService.ShowOrderDetailsWindow(viewModel);
            if (window.DialogResult == true)
            {
                viewModel.AllOrderDetails.ForEach(od =>
                {
                    double oldOdNum = itemDic[od.OrderDetialsId];
                    if (od.Num != oldOdNum)
                    {
                        var item = _collection.First(x => x.OrderDetialsId == od.OrderDetialsId);
                        var index = _collection.IndexOf(item);
                        item.Num = od.Num;
                        _collection.RemoveAt(index);
                        _collection.Insert(index, item);
                    }
                });
            }
            this.Selected = orderDetails;
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void selectMAssets(object parameter)
        {
            var ch = parameter as CheckBox;
            var mAsset = ch.Tag as MovableAsset;
            if (mAsset == null) return;
            DisSelected = mAsset;
            if (ch.IsChecked == true)
            {
                _selectedAssets.Add(mAsset);
            }
            else
            {
                if (_selectedAssets.Contains(mAsset))
                {
                    _selectedAssets.Remove(mAsset);
                }
            }
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
            _collection.Insert(index,od);
            this.Selected = od;
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
                _currentOrder.ObjectState = ObjectState.Modified;
                _currentOrder.ModifiedDate = GlobalClass._Today;
                Mouse.SetCursor(Cursors.Wait);
               
                foreach (var od in _currentOrder.OrderDetails)
                {
                    od.ObjectState = ObjectState.Modified;
                    var userHistory = od.OrderUserHistories.First(o=>o.IsCurrent);
                    userHistory.ObjectState = ObjectState.Modified;
                   
                    var nextHistory = od.OrderUserHistories.GetNext(userHistory);

                    if (nextHistory != null)
                    {
                        nextHistory.ObjectState = ObjectState.Modified;
                        nextHistory.IsCurrent = true;
                        od.OrderUserHistories.Add(nextHistory);
                        if (string.Equals(nextHistory.Identity, "OrganManager"))
                        {
                            od.State = OrderDetailsState.ManagerConfirm;
                        }
                        else if (string.Equals(nextHistory.Identity, "StuffHonest"))
                        {
                            od.State = OrderDetailsState.StuffHonest;
                        }
                    }

                    userHistory.UserDecision = true;
                    userHistory.IsCurrent = false;
                    string str = "تایید درخواست";
                    if (od.IsReject)
                    {
                       str = "رد درخواست";
                    }
                    userHistory.Description = str + " " + "توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                  "در تاریخ:" + " " + GlobalClass._Today.PersianDateString() + ".به تعداد "+od.Num;
                    od.OrderUserHistories.Add(userHistory);
                }

                if (_currentOrder.OrderType != OrderType.InternalRequest
                && _currentOrder.OrderType != OrderType.Store)
                {
                    foreach (var mAsset in _selectedAssets)
                    {
                        var location = _movableAssetService.GetLocation(mAsset.AssetId, false);
                        location.Status = LocationStatus.Active;
                        location.ObjectState = ObjectState.Modified;

                        var unconsum = mAsset as UnConsumption;
                        if (unconsum != null)
                        {
                            var belongs = _movableAssetService.GetBelongingsToLocation(unconsum.AssetId);
                            if (belongs.Count() > 0)
                            {
                                foreach (var b in belongs)
                                {
                                    var bloc = b.Locations.SingleOrDefault(x => x.Status == LocationStatus.MovedRequest);
                                    if (bloc != null)
                                    {
                                        bloc.Status = LocationStatus.Active;
                                        bloc.ObjectState = ObjectState.Modified;
                                        b.Locations.Add(bloc);
                                    }
                                    unconsum.Belongings.Add(b);
                                }
                            }
                            unconsum.Locations.Add(location);
                            _movableAssetService.InsertOrUpdateGraph(unconsum);
                        }
                        else
                        {
                            mAsset.Locations.Add(location);
                            _movableAssetService.InsertOrUpdateGraph(mAsset);
                        }
                        _currentOrder.MovableAssets.Remove(mAsset);
                    }

                    if (_currentOrder.MovableAssets.Count == 0)
                    {
                        _currentOrder.OrderDetails.ForEach(od =>
                        {
                            od.IsReject = true;
                        });
                    }
                }

                var order = _orderService.Queryable().Where(o => o.OrderId == _currentOrder.OrderId)
                    .Include(o => o.OrderDetails).AsNoTracking().Single();

                order.OrderDetails.ForEach(od =>
                {
                    _currentOrder.OrderDetails.ForEach(odn =>
                    {
                        if (odn.OrderDetialsId == od.OrderDetialsId)
                        {
                            od.State = odn.State;
                            od.IsReject = odn.IsReject;
                        }
                    });
                });

                if (order.OrderDetails.Where(od=>!od.IsReject).Any(od => od.State == OrderDetailsState.OrganizManagerConfirm))
                {
                    _currentOrder.Status = OrderStatus.OrganizManagerConfirm;
                }
                else if (order.OrderDetails.Where(od => !od.IsReject).Any(od => od.State == OrderDetailsState.ManagerConfirm))
                {
                    _currentOrder.Status = OrderStatus.ManagerConfirm;
                }
                else
                {
                    if (order.OrderDetails.All(o => o.IsReject))
                    {
                        _currentOrder.Status = OrderStatus.Reject;
                    }
                    else
                    {
                        _currentOrder.Status = OrderStatus.StuffHonest;
                    }
                }

                _orderService.InsertOrUpdateGraph(_currentOrder);

                try
                {
                    _currentOrder.RowVersion = order.RowVersion;
                    _unitOfWork.TrackConcurrency<Order>(_currentOrder, _currentOrder.RowVersion);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.OrderCollection.Remove(OMSelected);
                    DisCollection=null;
                    _collection.Clear();
                    this.Selected = null;
                    _currentOrder = null;
                    this.DisSelected = null;
                    this.OMSelected = null;
                    this.OrderUserHistories = null;
                }
                catch (DbUpdateConcurrencyException)
                {
                    _dialogService.ShowError("خطا", "این درخواستی که شما میخواهید حذف کنید به وسیله کاربر دیگری چند لحظه پیش ویرایش شده است.لطفا پنجره را بسته دوباره وارد شوید"); 
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
        
        private void ShowPersonDetails()
        {
            if (OMSelected == null)
            {
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            var person = _personService.Find(OMSelected.PersonId);
            if (person == null) return;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new PersonDetailsInfoViewModel(person.PersonId);
            _navigationService.ShowPersonDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showStoreUseableAsset(object parameter)
        {
            if (OMSelected == null || _collection.Count<=0)
            {
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var requestStuffs = new List<Tuple<string, StuffType,int, string>>();
            if (OMSelected.OrderType == OrderType.InternalRequest)
            {
                _collection.GroupBy(x => new { x.kalaNo }).ForEach(od =>
                {
                    requestStuffs.Add(new Tuple<string, StuffType, int, string>(od.First().StuffName, 
                        od.First().StuffType, od.First().KalaUid, od.Key.kalaNo));
                });
            }
            else
            {
                StuffType sType = StuffType.None;
                DisCollection.GroupBy(x => new { x.KalaNo }).ForEach(ds =>
                {
                    if (ds is Belonging) sType = StuffType.Belonging;
                    else if (ds is UnConsumption) sType = StuffType.UnConsumption;
                    else if (ds is InCommidity) sType = StuffType.OrderConsumption;
                    else if (ds is Installable) sType = StuffType.Installable;
                    requestStuffs.Add(new Tuple<string, StuffType, int, string>(ds.First().Name, sType, 
                        ds.First().KalaUid, ds.Key.KalaNo));
                });
            }
            var analizM = parameter as AnalizModel;
            if (analizM == null)
            {
                analizM = new AnalizModel();
            }
            var viewmodel = new StoreAssetDetailsViewModel(_container, requestStuffs,analizM);

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
                                                 isOrderChild =_unitHelper.mainparentRecovery(coU);
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
                            
                            items.Add(_storeBillService.GetStoreBillAnalized(kalaUid,Selected.StuffType, fromDate, toDate, true));
                            items.Add(_commodityService.GetInternalDocAnaliz(kalaUid, fromDate, toDate));
                            
                            _orderService.GetOrderDetailsByKalaUid(kalaUid,Selected.StuffType, fromDate, toDate,false)
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
                            items.Add(_movableAssetService.GetCurrentStoreByKalaUid(kalaUid,Selected.StuffType,false));
                            items.Add(_storeBillService.GetStoreBillAnalized(kalaUid,Selected.StuffType, fromDate, toDate,false));
                            items.Add(_movableAssetService.GetStoreInternalDocAnaliz(kalaUid,Selected.StuffType, fromDate, toDate,false));
                            _orderService.GetOrderDetailsByKalaUid(kalaUid,Selected.StuffType, fromDate, toDate,false)
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
        
        private void rejectOrder()
        {
            if (_currentOrder == null || OMSelected==null)
            {
                _dialogService.ShowError("انتخاب درخواست", "هیچ درخواستی انتخاب نشده است");
                return;
            }

            bool isConfirm = _dialogService.AskConfirmation("توجه", ErrorMessages.Default.AskConfrimation);
            if (isConfirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                _currentOrder.ObjectState = ObjectState.Modified;
                _currentOrder.ModifiedDate = GlobalClass._Today;
                _currentOrder.Status = OrderStatus.Reject;
                _orderService.Update(_currentOrder);
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

        #endregion

        #region commands

        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand PersonDetailsCommand { get; private set; }
        public ICommand StoreDetailsCommand { get; private set; }
        public ICommand OrderDetailsHistoryCommand { get; private set; }
        public ICommand AnalizCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand DoubleClickListBoxItemCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        private void initializCommands()
        {
            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
               (parameter) => { this.selectMAssets(parameter); },
               (parameter) => { return true; }
               );

            OrderDetailsCommand = new MvvmCommand(
               (parameter) => { this.showOrderDetailsWindow(parameter); },
               (parameter) => { return true; }
               );

            RemoveCommand = new MvvmCommand(
                (parameter) => { this.initSelectedDetails(parameter); },
                (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confirmOrder(); },
                 (parameter) => { return IsEditableOrder; }
                ).AddListener<RecivedOrderViewModel>(this, th => th.IsEditableOrder);

            PersonDetailsCommand = new MvvmCommand(
             (parameter) => { this.ShowPersonDetails(); },
             (parameter) => { return OMSelected!=null; }).AddListener<RecivedOrderViewModel>(this, x => x.OMSelected);

            StoreDetailsCommand = new MvvmCommand(
             (parameter) => { this.showStoreUseableAsset(null); },
             (parameter) => { return OMSelected!=null; }).AddListener<RecivedOrderViewModel>(this, x => x.OMSelected);

            OrderDetailsHistoryCommand = new MvvmCommand(
                (parameter) => { this.getOrderUserHistory(parameter); },
                (parameter) => { return true; }
                );

            AnalizCommand = new MvvmCommand(
                 (parameter) => { this.analizationOnOrderAsync(parameter); },
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

            RejectCommand = new MvvmCommand(
                (parameter) => { this.rejectOrder(); },
                (parameter) => { return IsEditableOrder; }
                ).AddListener<RecivedOrderViewModel>(this,th=>th.IsEditableOrder);


            RefreshCommand = new MvvmCommand(
             (parameter) =>
             {
                this.initOnRecivedTypes();
             },
             (parameter) => { return true; }
             );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IStoreBillService _storeBillService;
        private readonly ObservableCollection<OrderModel> _orderCollection;
        private HashSet<MovableAsset> _selectedAssets;
        private readonly ObservableCollection<OrderDetails> _collection;
        private List<EmployeeDesign> _allOrganiz;
        private readonly UnitHelper _unitHelper;
        private Order _currentOrder;
        private Dictionary<string, object> _recivedTypes;

        #endregion
    }
}
