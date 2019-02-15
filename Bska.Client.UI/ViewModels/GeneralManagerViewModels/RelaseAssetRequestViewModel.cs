
namespace Bska.Client.UI.ViewModels.GeneralManagerViewModels
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.Services;
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using System.Linq;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Threading.Tasks;
    using Bska.Client.UI.API;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using System.Data.Entity;
    using Bska.Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;

    public sealed class RelaseAssetRequestViewModel : BaseViewModel
    {
        #region ctor

        public RelaseAssetRequestViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._moableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository",_unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._orderService = _container.Resolve<IOrderService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._movableAssetCollection = new ObservableCollection<PortableAsset>();
            this.RelaseRequestFilteredView = new CollectionViewSource { Source = MovableAssetCollection }.View;
            this.initalizObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;set;
        }

        public List<string> RequestHistories
        {
            get { return GetValue(() => RequestHistories); }
            set
            {
                SetValue(() => RequestHistories ,value);
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

        public ObservableCollection<PortableAsset> MovableAssetCollection
        {
            get { return _movableAssetCollection; }
        }

        public ICollectionView RelaseRequestFilteredView { get; set; }

        public PortableAsset SelectedAsset
        {
            get { return GetValue(() => SelectedAsset); }
            set
            {
                SetValue(() => SelectedAsset, value);
            }
        }
        #endregion

        #region methods

        private async void initalizObj()
        {
            this.Units = _unitService.Queryable().ToList();
            await this.getRequestedAssetAsync();
        }

        private Task getRequestedAssetAsync()
        {
            _movableAssetCollection.Clear();
            Task ts = new Task(() =>
              {
                  _commodityService.Queryable().Where(co => co.CommodityAssetReserveHistories
                  .Any(rs => rs.Status == MAssetReserveStatus.UnReservedRequested))
                  .Include(co=>co.CommodityAssetReserveHistories)
                  .ForEach(co =>
                  {
                      DispatchService.Invoke(() =>
                      {
                          _movableAssetCollection.Add(co);
                      });
                  });

                  _moableAssetService.Queryable().Where(ma => ma.MovableAssetReserveHistories
                  .Any(rs => rs.Status == MAssetReserveStatus.UnReservedRequested))
                     .Include(ma => ma.MovableAssetReserveHistories)
                  .ForEach(ma =>
                  {
                      DispatchService.Invoke(() =>
                      {
                          _movableAssetCollection.Add(ma);
                      });
                  });
              });
            ts.Start();
            return ts;
        }

        private void showOrderDetails(object parameter)
        {
            var asset = parameter as PortableAsset;
            if (asset == null)
            {
                return;
            }
            this.SelectedAsset = asset;
           var oDetails= _orderService.Queryable()
                .Where(o => o.MovableAssets.Any(ma => ma.AssetId == asset.AssetId) || o.Commodities.Any(co => co.AssetId == asset.AssetId))
                .SelectMany(o => o.OrderDetails).FirstOrDefault(od => od.kalaNo == asset.KalaNo);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            viewModel.Units = this.Units;
            viewModel.CurrentOrderDetails = oDetails;
            _navigationService.ShowOrderDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void showHitories(object parameter)
        {
            var asset = parameter as PortableAsset;
            if (asset == null)
            {
                return;
            }
            this.SelectedAsset = asset;
            if(asset is Commodity)
            {
                var reservers = ((Commodity)asset).CommodityAssetReserveHistories.First();
                this.RequestHistories = reservers.Description.Split('/').ToList();
            }
            else
            {
                var reservers = ((MovableAsset)asset).MovableAssetReserveHistories.First();
                this.RequestHistories = reservers.Description.Split('/').ToList();
            }
        }

        private async void confirmRequest(object parameter,MAssetReserveStatus status)
        {
            var asset = parameter as PortableAsset;
            if (asset == null)
            {
                return;
            }

            this.SelectedAsset = asset;
            bool confirm= _dialogService.AskConfirmation("", ErrorMessages.Default.AskConfrimation);
            string msg = "";
            if (status == MAssetReserveStatus.UnReservedToStore)
            {
                msg = "تایید درخواست آزاد کردن مال و موجودی قطعی برای انبار";
            }
            else
            {
                msg = "رد درخواست آزاد کردن مال";
            }

            if (confirm)
            {
                if (asset is Commodity)
                {
                    var commdity = asset as Commodity;
                    var reservers = commdity.CommodityAssetReserveHistories.First();
                    reservers.Status = status;
                    reservers.Description += "/" + msg;
                    reservers.ObjectState = ObjectState.Modified;
                    commdity.CommodityAssetReserveHistories.Add(reservers);
                }
                else
                {
                    var mAsset = asset as MovableAsset;
                    var reservers = mAsset.MovableAssetReserveHistories.First();
                    reservers.Status = status;
                    reservers.Description += "/" + msg;
                    reservers.ObjectState = ObjectState.Modified;
                    mAsset.MovableAssetReserveHistories.Add(reservers);
                }

                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    await this.getRequestedAssetAsync();
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

        #endregion

        #region commands

        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand OrderDetailsHistoryCommand { get; private set; }

        private void initializCommands()
        {
            OrderDetailsCommand = new MvvmCommand(
                 (parameter) => { this.showOrderDetails(parameter); },
                (parameter) => { return true; }
                );

            OrderDetailsHistoryCommand= new MvvmCommand(
                 (parameter) => { this.showHitories(parameter); },
                (parameter) => { return true; }
                );

            RejectCommand = new MvvmCommand(
                 (parameter) => { this.confirmRequest(parameter,MAssetReserveStatus.Reserved); },
                (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
               (parameter) => { this.confirmRequest(parameter, MAssetReserveStatus.UnReservedToStore); },
              (parameter) => { return true; }
              );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMovableAssetService _moableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<PortableAsset> _movableAssetCollection;
       
        #endregion

    }
}
