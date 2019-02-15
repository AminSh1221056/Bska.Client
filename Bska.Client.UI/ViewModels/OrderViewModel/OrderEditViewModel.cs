
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.UI.Helper;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.Data.Service;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Data.Entity.Infrastructure;
    using System.Windows;
    using System.Threading;
    public sealed class OrderEditViewModel : BaseViewModel
    {
        #region ctor

        public OrderEditViewModel(IUnityContainer container,Order currentOrder)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._unitService = _container.Resolve<IUnitService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._navigationService = _container.Resolve<INavigationService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._collection = new ObservableCollection<OrderDetails>();
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._selectedAssets = new HashSet<MovableAsset>();
            this._selectedDetails = new HashSet<OrderDetails>();
            this._dialogService = _container.Resolve<IDialogService>();
            this.CurrentOrder = currentOrder;
            this.EnableEdit = true;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;set;
        }

        public Order CurrentOrder
        {
            get { return GetValue(() => CurrentOrder); }
            set
            {
                SetValue(() => CurrentOrder, value);
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
        public MovableAsset DisSelected
        {
            get { return GetValue(() => DisSelected); }
            set
            {
                SetValue(() => DisSelected, value);
            }
        }

        public InternalOrderDetailsViewModel InternalOrderDetails
        {
            get
            {
                return GetValue(() => InternalOrderDetails);
            }
            private set
            {
                SetValue(() => InternalOrderDetails, value);
            }
        }

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        public Boolean EnableEdit
        {
            get { return GetValue(() => EnableEdit); }
            set
            {
                SetValue(() => EnableEdit, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            if (CurrentOrder.OrderType == OrderType.InternalRequest
                || CurrentOrder.OrderType==OrderType.Store)
            {
                this._collection.Clear();
                CurrentOrder.OrderDetails.ForEach(od =>
                {
                    _collection.Add(od);
                });
            }
            else
            {
                this.DisCollection = CurrentOrder.MovableAssets.ToList();
            }

            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                if (CurrentOrder.Status == OrderStatus.Reject
               || CurrentOrder.Status == OrderStatus.Deliviry
               || CurrentOrder.Status == OrderStatus.SubOrder)
                {
                    EnableEdit = false;
                }
            }
            else
            {
                CurrentOrder.OrderDetails.ForEach(od =>
                {
                    if (_orderService.GetUserHistories(od.OrderDetialsId).Where(ou=>ou.UserDecision).Count() > 1)
                    {
                        EnableEdit = false;
                    }
                });
            }

            this.Units = _unitService.Queryable().ToList();
        }

        private void showOrderDetails(object parameter)
        {
            var orderDetails = parameter as OrderDetails;
            if (orderDetails == null) return;
            this.Selected = orderDetails;
            Mouse.SetCursor(Cursors.Wait);
            InternalOrderDetails = new InternalOrderDetailsViewModel(orderDetails);
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                if(CurrentOrder.Status==OrderStatus.Deliviry
                    || CurrentOrder.Status==OrderStatus.Reject
                    || CurrentOrder.Status == OrderStatus.SubOrder)
                {
                    InternalOrderDetails.IsEditable = false;
                }
            }
            else
            {
                var histories = _orderService.GetUserHistories(orderDetails.OrderDetialsId);
                if (histories.Count(oh => oh.UserDecision) >= 2)
                {
                    InternalOrderDetails.IsEditable = false;
                }
            }

            this.GetUnits(orderDetails.StuffType);
            if(!_selectedDetails.Contains(orderDetails))
            _selectedDetails.Add(orderDetails);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void GetUnits(StuffType sId)
        {
            _subUnits.Clear();
            Units.Where(x => x.Parent == null && (x.StuffId == StuffType.None || x.StuffId == sId)).ForEach(x =>
            {
                _subUnits.Add(new UnitTreeViewModel(x, _container));
            });
        }

        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAsset;
            if (mAsset == null) return;
            this.DisSelected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
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
            _collection.Insert(index, od);
            if (!_selectedDetails.Contains(od))
            {
                _selectedDetails.Add(od);
            }
            this.Selected = od;
        }

        private void editOrder(object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);
            var order = _orderService.Query(x => x.OrderId == CurrentOrder.OrderId).Include(x=>x.OrderDetails)
                .Include(x => x.MovableAssets).Select().Single();
            if (CurrentOrder.OrderType != OrderType.InternalRequest
                && CurrentOrder.OrderType!=OrderType.Store)
            {
                if (_selectedAssets.Count <= 0)
                {
                    _dialogService.ShowAlert("انتخاب اموال", "هیچ مالی انتخاب نشده است");
                    return;
                }

                foreach (var item in _selectedAssets)
                {
                    var loc = _movableAssetService.GetLocation(item.AssetId, false);
                    var updateItem = order.MovableAssets.Single(ma => ma.AssetId == item.AssetId);
                    loc.Status = LocationStatus.Active;
                    loc.ObjectState = ObjectState.Modified;
                    updateItem.Locations.Add(loc);
                    _movableAssetService.InsertOrUpdateGraph(updateItem);
                    order.MovableAssets.Remove(updateItem);
                }

                if (order.MovableAssets.Count == 0)
                {
                    order.Status = OrderStatus.Reject;
                }
            }
            else
            {
                if (_selectedDetails.Count <= 0)
                {
                    _dialogService.ShowAlert("انتخاب اموال", "هیچ مالی انتخاب نشده است");
                    return;
                }

                foreach (var item in order.OrderDetails.ToList())
                {
                    var editItem = _selectedDetails.FirstOrDefault(x => x.OrderDetialsId == item.OrderDetialsId);
                    if (editItem != null)
                    {
                        item.Num = editItem.Num;
                        item.UsingLocation = editItem.UsingLocation;
                        item.UnitId = editItem.UnitId;
                        item.OfferSpecification = editItem.OfferSpecification;
                        item.OfferQuality = editItem.OfferQuality;
                        item.ImportantDegree = editItem.ImportantDegree;
                        item.EstimatePrice = editItem.EstimatePrice;
                        item.ObjectState = ObjectState.Modified;
                        item.IsReject = editItem.IsReject;
                        order.OrderDetails.Add(item);
                    }
                }
            }

            Boolean confrim = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confrim)
            {
                order.ModifiedDate = GlobalClass._Today;
                order.ObjectState = ObjectState.Modified;
                _orderService.Update(order);
                try
                {
                    order.RowVersion = CurrentOrder.RowVersion;
                    _unitOfWork.TrackConcurrency<Order>(order, order.RowVersion);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    ((Window)parameter).DialogResult = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    _dialogService.ShowError("توجه", "این درخواستی که شما میخواهید ویرایش کنید به وسیله کاربر دیگری چند لحظه پیش ویرایش شده است.با تایید این پیغام پنجره بسته خواهد شد لطفا دوباره سعی کنید");
                    ((Window)parameter).DialogResult = true;
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

        private void storeOrderMoved(object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);
            var order = _orderService.Query(x => x.OrderId == CurrentOrder.OrderId).Include(x => x.OrderDetails).Select().Single();
            if (order.OrderType != OrderType.Store)
            {
                return;
            }
            string msg = "";
            OrderStatus oState = OrderStatus.None;
            OrderDetailsState odState = OrderDetailsState.None;
            if (order.Status == OrderStatus.ManagerConfirm)
            {
                msg = "کل درخواست تایید شده و به تدارکات فرستاده می شود.آیا میخواهید ادامه دهید";
                oState = OrderStatus.SubOrder;
                odState = OrderDetailsState.SubOrder;
            }
            else if (order.Status == OrderStatus.StuffHonest)
            {
                msg = "کل درخواست تایید شده و به مدیریت فرستاده می شود.آیا میخواهید ادامه دهید";
                oState = OrderStatus.ManagerConfirm;
                odState = OrderDetailsState.ManagerConfirm;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", msg);
            if (confirm)
            {
                order.Status = oState;
                order.ModifiedDate = GlobalClass._Today;
                order.ObjectState = ObjectState.Modified;
                order.OrderDetails.ToList().ForEach(od =>
                {
                    var editItem = _selectedDetails.FirstOrDefault(x => x.OrderDetialsId == od.OrderDetialsId);
                    if (editItem != null)
                    {
                        od.Num = editItem.Num;
                        od.UsingLocation = editItem.UsingLocation;
                        od.UnitId = editItem.UnitId;
                        od.OfferSpecification = editItem.OfferSpecification;
                        od.OfferQuality = editItem.OfferQuality;
                        od.ImportantDegree = editItem.ImportantDegree;
                        od.EstimatePrice = editItem.EstimatePrice;
                        od.IsReject = editItem.IsReject;
                    }
                    od.State = odState;
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
                    }

                    if (odState == OrderDetailsState.SubOrder)
                    {
                        od.SubOrders.Add(new SubOrder
                        {
                            Identity = "Buy",
                            Num=od.Num,
                            ObjectState=ObjectState.Added,
                            State=SubOrderState.None,
                            Type=SubOrderType.Supplier,
                            UnitId=od.UnitId,
                        });
                    }
                    order.OrderDetails.Add(od);
                });

                _orderService.Update(order);
                try
                {
                    order.RowVersion = CurrentOrder.RowVersion;
                    _unitOfWork.TrackConcurrency<Order>(order, order.RowVersion);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    ((Window)parameter).DialogResult = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    _dialogService.ShowError("توجه", "این درخواستی که شما میخواهید ویرایش کنید به وسیله کاربر دیگری چند لحظه پیش ویرایش شده است.با تایید این پیغام پنجره بسته خواهد شد لطفا دوباره سعی کنید");
                    ((Window)parameter).DialogResult = true;
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
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showUserHistory(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", this.Window);
            var viewModel = new OrderUserHistoryViewModel(_container);
            viewModel.CurrentOrder = od;
            this.Selected = od;
            viewModel.OrderUserHistories = _orderService.GetUserHistories(od.OrderDetialsId).Where(ou=>ou.UserDecision).ToList();
            _navigationService.ShowOrderUserHistoryWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void openHelpDoc()
        {
            //var viewModel = new HelpViewModel(HelpPageingAddress.Default.HelpFileName, HelpPageingAddress.Default.mainWinHelpPage);
            //_navigationService.ShowHelpWindow(viewModel);
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110001-5");
            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand StoreOrderFinalCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand OrderDetailsHistoryCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initializCommands()
        {
            OrderDetailsCommand = new MvvmCommand(
             (parameter) => { this.showOrderDetails(parameter); },
             (parameter) => { return true; }
             );

            DetailsCommand = new MvvmCommand(
               (parameter) => { this.showMAssetDetails(parameter); },
               (parameter) => { return true; }
               );

            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.editOrder(parameter); },
                (parameter) => { return EnableEdit; }).AddListener<OrderEditViewModel>(this, x => x.EnableEdit);

            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectMAssets(parameter); },
                (parameter) => { return true; }
                );

            RemoveCommand = new MvvmCommand(
               (parameter) => { this.initSelectedDetails(parameter); },
               (parameter) => { return true; }
               );

            StoreOrderFinalCommand = new MvvmCommand(
                (parameter) => { this.storeOrderMoved(parameter); },
                (parameter) => { return true; }
                );

            OrderDetailsHistoryCommand = new MvvmCommand(
               (parameter) => { this.showUserHistory(parameter); },
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
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<OrderDetails> _collection;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private HashSet<MovableAsset> _selectedAssets;
        private HashSet<OrderDetails> _selectedDetails;
        private readonly IUnitOfWorkAsync _unitOfWork;

        #endregion
    }
}
