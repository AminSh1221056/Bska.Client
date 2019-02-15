
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using API;
    using Client.API.UnitOfWork;
    using Common;
    using Data.Service;
    using Domain.Entity;
    using Domain.Entity.AssetEntity;
    using Helper;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public sealed class CommodityDetailsViewModel : BaseViewModel
    {
        #region ctor

        public CommodityDetailsViewModel(IUnityContainer container, Int64 assetId, Commodity asset = null,
            Boolean isEditable = false, Boolean isRealAsset = true)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._isEditable = isEditable;
            this._isRealasset = isRealAsset;
            this.CurrentAsset = asset;
            this.initalizObj(assetId);
            this.initializCommands();
        }

        #endregion

        #region properties

        public CommodityViewModel CommodityVM
        {
            get { return GetValue(() => CommodityVM); }
            private set
            {
                SetValue(() => CommodityVM, value);
            }
        }

        public string StoreBillNo
        {
            get { return GetValue(() => StoreBillNo); }
            set
            {
                SetValue(() => StoreBillNo, value);
            }
        }

        public string StoreBillType
        {
            get { return GetValue(() => StoreBillType); }
            set
            {
                SetValue(() => StoreBillType, value);
            }
        }

        public String StoreBillDate
        {
            get { return GetValue(() => StoreBillDate); }
            set
            {
                SetValue(() => StoreBillDate, value);
            }
        }

        public Commodity CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            set
            {
                SetValue(() => CurrentAsset, value);
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

        public List<Stuff> Stuffs
        {
            get { return GetValue(() => Stuffs); }
            set
            {
                SetValue(() => Stuffs, value);
            }
        }

        public Stuff SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
                if (value != null)
                {
                    CurrentAsset.Name = SelectedStuff.Name;
                    CurrentAsset.KalaUid = SelectedStuff.StuffId;
                }
            }
        }

        public Int32 PStuffId
        {
            get;
            set;
        }

        public Boolean IsEditableAsset
        {
            get { return GetValue(() => IsEditableAsset); }
            set
            {
                SetValue(() => IsEditableAsset, value);
            }
        }

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        private void showOrderHistoryWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new OrderHistoryViewModel(_container);

            viewModel.CurrentAsset = new Repository.Model.MovableAssetModel
            {
                AssetId = CurrentAsset.AssetId,
                Name = CurrentAsset.Name,
                kalaUid = CurrentAsset.KalaUid,
            };

            viewModel.Orders = _commodityService.GetOrders(CurrentAsset.AssetId).ToList();
            _navigationService.ShowOrderHistoryWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        //private void showDocumentHistoryWindow(object parameter)
        //{
        //    var window = parameter as Window;
        //    if (window == null) return;
        //    Mouse.SetCursor(Cursors.Wait);
        //    StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
        //    var viewModel = new AccountHistoryViewModel(_container, CurrentAsset);
        //    _navigationService.ShowDocumentHistoryWindow(viewModel);
        //    StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
        //    Mouse.SetCursor(Cursors.Arrow);
        //}

        private void showSplitWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new CommoditySplitViewModel(_container, CurrentAsset);
            _navigationService.ShowCommoditySplitWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region methods

        private async void initalizObj(Int64 assetId)
        {
            if (assetId > 0)
            {
                CurrentAsset = _commodityService.Query(ma => ma.AssetId == assetId)
                   .Include(ma => ma.StoreBill).Include(ma => ma.PlaceOfUses).Select().SingleOrDefault();
            }

            if (CurrentAsset == null) return;

            var stuff = _stuffService.Query(x => x.KalaNo == CurrentAsset.KalaNo)
                     .Include(x => x.Parent).Select().SingleOrDefault();
            if (stuff != null)
            {
                if (stuff.Parent != null)
                {
                    PStuffId = stuff.Parent.StuffId;
                }
            }

            this.Units = _unitService.Queryable().Where(x => x.StuffId == StuffType.Consumable || x.StuffId == StuffType.None).ToList();
            CommodityVM = new CommodityViewModel(CurrentAsset, Units) {Cost=CurrentAsset.Cost };
           
            if (CurrentAsset.StoreBill != null)
            {
                StoreBillNo = "شماره قبض انبار : " + CurrentAsset.StoreBill.StoreBillNo.ToString();
                StoreBillType = "نوع قبض انبار : " + CurrentAsset.StoreBill.AcqType.GetDescription();
                StoreBillDate = "تاریخ : " + CurrentAsset.StoreBill.ArrivalDate.PersianDateString();
            }

            SelectedStuff = stuff;
            if (_isEditable)
            {
                IsEditableAsset = true;
                Stuffs = await this.getStuffAsync(StuffType.Consumable);
                Units.Where(u=>u.Parent==null).ForEach(u =>
                {
                    _subUnits.Add(new UnitTreeViewModel(u, _container));
                });
            }
            else
            {
                IsEditableAsset = false;
                Stuffs = new List<Stuff> { SelectedStuff };
            }
        }

        private Task<List<Stuff>> getStuffAsync(StuffType sType)
        {
            var ts = new Task<List<Stuff>>(() =>
            {
                return _stuffService.Query(s => s.StuffType == sType && s.IsStuff == true).Include(s => s.Parent).Select().ToList();
            });
            ts.Start();
            return ts;
        }

        private void editCurrentAsset(object parameter)
        {
            if (CurrentAsset == null) return;
            var window = parameter as Window;
            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (_isRealasset)
                {

                }
                else
                {
                    window.DialogResult = true;
                }
            }
        }

        private void deleteMasset(object parameter)
        {
            Window window = parameter as Window;
            if (window == null) return;
            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (_isRealasset)
                {

                }
                else
                {
                    CurrentAsset.AssetId = -1001;
                }

                window.DialogResult = true;
            }
        }
        #endregion

        #region commands

        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand OrderHistoryCommand { get; private set; }
        public ICommand DocumentHistoryCommand { get; private set; }
        public ICommand SplitCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        private void initializCommands()
        {
            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.deleteMasset(parameter);
                },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.editCurrentAsset(parameter); },
                (parameter) =>
                {
                    return true;
                }
                );

            OrderHistoryCommand = new MvvmCommand(
              (parameter) => { this.showOrderHistoryWindow(parameter); },
              (paramter) => { return true; }
              );

            //DocumentHistoryCommand = new MvvmCommand(
            //    (parameter) => { //this.showDocumentHistoryWindow(parameter); },
            //    (paramter) => { return true; }
            //    );

            SplitCommand= new MvvmCommand(
                (parameter) => { this.showSplitWindow(parameter); },
                (paramter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStuffService _stuffService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IUnitService _unitService;
        private readonly Boolean _isEditable = false;
        private readonly Boolean _isRealasset = true;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;

        #endregion

    }
}
