
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Microsoft.Practices.Unity;
    using System;
    using System.Windows.Media;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Windows.Input;
    using Bska.Client.UI.Helper;
    using System.Windows;
    using Bska.Client.UI.Services;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;
    public sealed class OrderTrackViewModel : BaseViewModel
    {
        #region ctor

        public OrderTrackViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork=_container.Resolve<IUnitOfWorkAsync>();
            this._unitService = _container.Resolve<IUnitService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository",_unitOfWork.Repository<Order>()));
            this._navigationService = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this.initializCommand();
        }

        #endregion

        #region properties

        public Order CurrentOrder
        {
            get { return GetValue(() => CurrentOrder); }
            set
            {
                SetValue(() => CurrentOrder, value);
                this.initalizObj();
            }
        }

        public Color Color
        {
            get { return GetValue(() => Color); }
            set
            {
                SetValue(() => Color, value);
            }
        }

        public Decimal AllPrice
        {
            get { return GetValue(() => AllPrice); }
            set
            {
                SetValue(() => AllPrice, value);
            }
        }

        public Decimal AllNum
        {
            get { return GetValue(() => AllNum); }
            set
            {
                SetValue(() => AllNum, value);
            }
        }

        public OrderStatus Status
        {
            get { return GetValue(() => Status); }
            set
            {
                SetValue(() => Status, value);
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

        public List<OrderDetails> Collection
        {
            get { return GetValue(() => Collection); }
            set
            {
                SetValue(() => Collection, value);
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

        public List<MovableAsset> DisCollection
        {
            get { return GetValue(() => DisCollection); }
            set
            {
                SetValue(() => DisCollection, value);
            }
        }

        #endregion

        #region methods

        private async void initalizObj()
        {
            if (CurrentOrder == null) return;
            Color = BOT.ParseHexColor("#" + ((OrderStatusColor)CurrentOrder.Status).ToString());
            Status = CurrentOrder.Status;
            if (CurrentOrder.OrderType == OrderType.InternalRequest
                || CurrentOrder.OrderType==OrderType.Store)
            {
                this.Collection = CurrentOrder.OrderDetails.ToList();
                AllNum = CurrentOrder.OrderDetails.Count;
            }
            else
            {
                this.DisCollection = CurrentOrder.MovableAssets.ToList();
                this.OrderUserHistories = _orderService.GetUserHistories(CurrentOrder.OrderDetails.First().OrderDetialsId)
                    .Where(ou => ou.UserDecision).ToList();
                AllNum = CurrentOrder.MovableAssets.Count;
            }

            this.AllPrice = await estimateOrderPrice();
            this.Units = _unitService.Queryable().ToList();
        }

        private Task<Decimal> estimateOrderPrice()
        {
            Task<Decimal> ts = new Task<Decimal>(() =>
            {
                Decimal val = 0;
                if (CurrentOrder.OrderType == OrderType.InternalRequest)
                {
                    foreach (var od in CurrentOrder.OrderDetails)
                    {
                        val += Convert.ToDecimal(od.Num) * od.EstimatePrice;
                    }
                }
                else
                {
                    foreach (var od in CurrentOrder.MovableAssets)
                    {
                        val += Convert.ToDecimal(od.Num) * od.Cost;
                    }
                }
                return val;
            });
            ts.Start();
            return ts;
        }

        private void showUserHistory(IList<object> parameters)
        {
            var od = parameters[0] as OrderDetails;
            var winsow = parameters[1] as Window;
            if (winsow == null || od == null) return;
            StoryboardManager.PlayStoryboard("OrderTrackContractingStoryboard", winsow);
            Mouse.SetCursor(Cursors.Wait);
            this.OrderUserHistories = _orderService.GetUserHistories(od.OrderDetialsId).Where(ou=>ou.UserDecision).ToList();
            Mouse.SetCursor(Cursors.Arrow);
        }

        internal Task<List<SubOrder>> AllSubOrders()
        {
            var ts = new Task<List<SubOrder>>(() =>
            {
                var subOrders = new List<SubOrder>();
                foreach (var od in Collection)
                {
                    _orderService.GetSubOrders(od.OrderDetialsId).ToList().ForEach(sOd =>
                    {
                        subOrders.Add(sOd);
                    });
                }
                return subOrders;
            });
            ts.Start();
            return ts;
        }

        private void DeleteOrder(Window window)
        {
            bool IsConfirm = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.ConfirmDelete);
            if (IsConfirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                var order = _orderService
                    .Query(x => x.OrderId == CurrentOrder.OrderId)
                    .Include(x => x.MovableAssets).Select().Single();
                if (CurrentOrder.OrderType != OrderType.InternalRequest
                && CurrentOrder.OrderType != OrderType.Store)
                {
                    foreach (var mAsset in order.MovableAssets)
                    {
                        var location = _movableAssetService.GetLocation(mAsset.AssetId,false);
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
                    }
                }
                _orderService.Delete(order);

                try
                {
                    order.RowVersion = CurrentOrder.RowVersion;
                    _unitOfWork.TrackConcurrency<Order>(order, order.RowVersion);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    if (window != null)
                    {
                        window.DialogResult = true;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    _dialogService.ShowError("توجه", "این درخواستی که شما میخواهید حذف کنید به وسیله کاربر دیگری چند لحظه پیش ویرایش شده است.با تایید این پیغام پنجره بسته خواهد شد لطفا دوباره سعی کنید");
                    window.DialogResult = true;
                }
                catch (Exception)
                {
                    throw;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);
            var managerRole = _personService.GetSpecificRole(PermissionsType.GeneralManager);
            var stuffHonestRole = _personService.GetSpecificRole(PermissionsType.StuffHonest);
            string managerName = "Unknown";
            String stuffHonestName = "Unknown";
            if (managerRole != null)
            {
                managerName = _personService.GetUser(managerRole.UserId.Value).FullName;
            }

            if (stuffHonestRole != null)
            {
                stuffHonestName = _personService.GetUser(stuffHonestRole.UserId.Value).FullName;
            }
            var viewModel = new ReportViewModel();
            viewModel.OrderConfirmReport(CurrentOrder.OrderId,managerName, stuffHonestName, CurrentOrder.OrderType.GetDescription());
            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand DeleteCommand { get; private set; }
        public ICommand UserHistoryCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        private void initializCommand()
        {
            DeleteCommand = new MvvmCommand(
                (parameter) => { this.DeleteOrder(parameter as Window); },
                (parameter) =>
                {
                    if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager) return true;
                    else
                    {
                        return false;
                    }
                });

            UserHistoryCommand = new MvvmCommand(
                (parameter) => { this.showUserHistory(parameter as IList<object>); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.reports(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IUnitService _unitService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;

        #endregion
    }
}
