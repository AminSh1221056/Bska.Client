
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Data.Service;
    using Microsoft.Practices.Unity;
    using Repository.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using AssetViewModel;

    public sealed class MAssetListViewModel : BaseViewModel
    {
        #region ctor

        public MAssetListViewModel(IUnityContainer container,int descTyp,Int64 targetId,string itemDesc1="",int itemDesc2=0)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._descTyp = descTyp;
            this._targerId = targetId;
            this._itemDesc1 = itemDesc1;
            this._itemDesc2 = itemDesc2;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public String Description
        {
            get { return GetValue(() => Description); }
            set
            {
                SetValue(() => Description, value);
            }
        }

        public List<MovableAssetModel> AssetList
        {
            get { return GetValue(() => AssetList); }
            set
            {
                SetValue(() => AssetList, value);
                if(AssetList!=null)
                this.Assets = new CollectionViewSource { Source = AssetList }.View;
            }
        }

        public MovableAssetModel Selected
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

        public ICollectionView Assets { get; private set; }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.movableAssetFilters();
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            if (_descTyp == 1001)
            {
                Description = "لیست اموال ثبت شده برای صورت جلسه";
                _queryLeverl = 701;
            }
            else if (_descTyp == 1002)
            {
                Description = "لیست اموال ثبت شده برای قبض انبار با شماره:"+_itemDesc1;
                _queryLeverl = 601;
            }
            else if (_descTyp == 1003)
            {
                Description = "لیست اموال ثبت شده برای فهرست های موجودی اولیه";
                _queryLeverl = 501;
            }
            else if (_descTyp == 1004)
            {
                Description = "لیست اموال ثبت شده برای نظام نوین";
                _queryLeverl = 401;
            }
            else if (_descTyp == 1005)
            {
                _queryLeverl = 801;
            }
            else if (_descTyp == 1006)
            {
                Description = "لیست اموال ثبت شده برای سند با شماره:"+_itemDesc1;
                _queryLeverl = 901;
            }
            this.Units = _unitService.Queryable().ToList();
        }

        private void movableAssetFilters()
        {
            Assets.Filter = (obj) =>
            {
                var asset = obj as MovableAssetModel;
                return asset.Name.Contains(SearchCriteria) || asset.Label.ToString() == SearchCriteria;
            };
        }

        private void showMAssetDetails(IList<object> parameters)
        {
            var mAsset = parameters[0] as MovableAssetModel;
            if (mAsset == null) return;
            var window = parameters[1] as Window;
            if (window == null) return;
            this.Selected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            if (string.Equals(mAsset.MAssetType, "مصرفی"))
            {
                var viewModel = new CommodityDetailsViewModel(_container, mAsset.AssetId, null);
                _navigationService.ShowCommodityDetailsWindow(viewModel);
            }
            else
            {
                var viewModel = new MovableAssetDetailsViewModel(_container, mAsset.AssetId);
                _navigationService.ShowMAssetDetailsWindow(viewModel);
            }
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void report()
        {
            Mouse.SetCursor(Cursors.Wait);
            string searchItem = SearchCriteria;
            var viewModel = new ReportViewModel();
            string desc3 = "امین اموال : نامشخص";
            string desc4 = "انباردار : نامشخس";
            string desc5 = "ذی حساب : نامشخص";

            var stuffHUser = _personService.GetUniqueUserToPersonByPermission(Common.PermissionsType.StuffHonest);
            if (stuffHUser != null)
            {
                desc3 = $"امین اموال : {stuffHUser.FullName}";
            }

            var accounting = _personService.GetUniqueUserToPersonByPermission(Common.PermissionsType.Accountant);
            if (accounting != null)
            {
                desc5 = $"ذی حساب:{accounting.FullName}";
            }
            if (_queryLeverl == 601)
            {
                var bill = _storeBillService.Query(sb => sb.StoreBillId == _targerId)
                    .Include(sb => sb.SupplierIndents).Select().Single();

                var masterId = _storeBillService.getRelatedAccountMasterId(bill.StoreBillId);
                string spName = "نامشخص";
                string desc2 = "";

                if (bill.SupplierIndents.Count>0)
                {
                    var supplier = _personService.GetUser(bill.SupplierIndents.First().SupplierId);
                    spName = $"کارپرداز: {supplier.FullName}";
                }
               

                if (bill.AcqType == Common.StateOwnership.Purchase)
                {
                    var seller = _sellerService.Find(bill.SellerId);
                    if (seller != null)
                    {
                        desc2 = $"فروشنده : {seller.Name} {seller.Lastname}";
                    }
                }

                if (bill.StoreId.HasValue)
                {
                    var storeLeader = _storeService.GetUserForStore(bill.StoreId.Value);
                    if (storeLeader != null)
                    {
                        desc4 = $"انباردار:{storeLeader.FullName}";
                    }
                }

                viewModel.StoreBillReport(_itemDesc1,searchItem,spName,desc2,masterId,desc3,desc4,desc5);
            }
            else if (_queryLeverl == 901 || _queryLeverl == 501)
            {
                int docId =Convert.ToInt32(_targerId);
                var doc = _movableAssetService.GetDocumentToRelatedAccount(docId);
                int masterId = 0;
                if (doc.AccountDocument != null)
                {
                    masterId = doc.AccountDocument.ID;
                }
                else
                {
                    var ac = _movableAssetService.GetRelatedRetiringBillAccount(docId);
                    if (ac != null)
                    {
                        masterId = ac.ID;
                    }
                }
                if (doc.StoreId.HasValue)
                {
                    var storeLeader = _storeService.GetUserForStore(doc.StoreId.Value);
                    if (storeLeader != null)
                    {
                        desc4 = $"انباردار:{storeLeader.FullName}";
                    }
                }
                string transfree = $"تحویل گیرنده : {doc.Transferee}";
                viewModel.DocumentReport(_itemDesc1, _itemDesc2,searchItem,transfree,masterId
                    ,desc3,desc4,desc5);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchItem))
                {
                    searchItem = null;
                }

                viewModel.MAssetInitialListReport(Description, searchItem, _queryLeverl);
            }
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand DetailsCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        private void initializCommands()
        {
            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter as IList<object>); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.report(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields
        private readonly IUnityContainer _container;
        private readonly INavigationService _navigationService;
        private readonly IUnitService _unitService;
        private readonly IStoreBillService _storeBillService;
        private readonly ISellerService _sellerService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IPersonService _personService;
        private readonly IStoreService _storeService;
        private readonly int _descTyp;
        private int _queryLeverl;
        private Int64 _targerId;
        private string _itemDesc1;
        private Int32 _itemDesc2;

        #endregion

    }
}
