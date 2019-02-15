
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using Bska.Client.UI.Helper;
    using System.Windows.Input;
    using Bska.Client.UI.API;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using AssetViewModel;
    using System.Threading;

    public sealed class StoreMAssetManageViewModel : BaseViewModel
    {
        #region ctor

        public StoreMAssetManageViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._personService = _container.Resolve<IPersonService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._allStoreId = new List<int>();
            _collection = new ObservableCollection<MovableAssetModel>();
            this._storefiristGeneration = new ObservableCollection<StoreTreeViewModel>();
            this.initializObj();
            this.initializCommand();
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

        public Dictionary<string, object> StuffTypes
        {
            get { return GetValue(() => StuffTypes); }
            set
            {
                SetValue(() => StuffTypes, value);
            }
        }

        public Dictionary<string, object> SelectedStuffType
        {
            get { return GetValue(() => SelectedStuffType); }
            set
            {
                SetValue(() => SelectedStuffType, value);
            }
        }

        public Boolean DateEnabled
        {
            get { return GetValue(() => DateEnabled); }
            set
            {
                SetValue(() => DateEnabled, value);
                if (value)
                {
                    FromDate = GlobalClass._Today.AddMonths(-6).PersianDateTime();
                    ToDate = GlobalClass._Today.PersianDateTime();
                }
                else
                {
                    FromDate = null;
                    ToDate = null;
                }
            }
        }
        public PersianDate? FromDate
        {
            get { return GetValue(() => FromDate); }
            set
            {
                SetValue(() => FromDate, value);
            }
        }

        public PersianDate? ToDate
        {
            get { return GetValue(() => ToDate); }
            set
            {
                SetValue(() => ToDate, value);
            }
        }
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set { SetValue(() => Units, value); }
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
                this.GetParentNode();
            }
        }

        public Boolean NestPropertyView
        {
            get { return GetValue(() => NestPropertyView); }
            set
            {
                SetValue(() => NestPropertyView, value);
            }
        }

        public StoreTreeViewModel StoreDesignSelected
        {
            get { return GetValue(() => StoreDesignSelected); }
            set
            {
                SetValue(() => StoreDesignSelected, value);
                if (value != null)
                {
                    _allStoreId.Clear();
                    GetAllStoreDesignId(StoreDesignSelected.StoreDesignCurrent);
                    FilterByStoreDesign();
                }
            }
        }
        public Boolean ChGroupView
        {
            get { return GetValue(() => ChGroupView); }
            set
            {
                SetValue(() => ChGroupView, value);
                if (value)
                {
                    this.StoreGroupingFiltering();
                }
                else
                {
                    this.StoreNormalFilteringAsync();
                }
            }
        }
        public ObservableCollection<MovableAssetModel> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged("Collection");
            }
        }

        public ObservableCollection<StoreTreeViewModel> StoreFiristGeneration
        {
            get { return _storefiristGeneration; }
        }

        public String SearchText
        {
            get { return GetValue(() => SearchText); }
            set
            {
                SetValue(() => SearchText, value);
                _matchingStoreEnumerator = null;
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

        public Boolean IsCommodityToNum
        {
            get { return GetValue(() => IsCommodityToNum); }
            set
            {
                SetValue(() => IsCommodityToNum, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            Units = _unitService.Queryable().ToList();

            if (Thread.CurrentPrincipal.IsInRole("Manager")
                    || Thread.CurrentPrincipal.IsInRole("GeneralManager"))
            {
                Stores = _storeService.Queryable().ToList();
            }
            else if (Thread.CurrentPrincipal.IsInRole("StoreLeader"))
            {
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                    .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                Stores = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId)).ToList();
            }

            StuffTypes = new Dictionary<string, object> { { StuffType.Consumable.GetDescription(),StuffType.Consumable}, { StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};

            SelectedStuffType = new Dictionary<string, object> {{ StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};

            SelectedStore = Stores.FirstOrDefault();
        }

        private async void getMAssetForStore()
        {
            if (SelectedStore == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ انباری انتخاب نشده است");
                return;
            }
            DateTime? date1 = default(DateTime?);
            DateTime? date2 = default(DateTime?);
            if (FromDate != null && ToDate != null)
            {
                date1 = FromDate.Value.ToDateTime();
                DateTime dt = ToDate.Value.ToDateTime();
                date2 = new DateTime(dt.Year, dt.Month, dt.Day, GlobalClass._Today.Hour, GlobalClass._Today.Minute, GlobalClass._Today.Second);
            }
            Mouse.SetCursor(Cursors.Wait);
            if (SelectedStore.StoreType == StoreType.Retiring)
            {
               await Task.Run(() =>
                {
                   _allAsset= _movableAssetService.GetStoreMovableAssetInRetiringStores(date1, date2, SelectedStore.StoreId)
                         .Where(x => SelectedStuffType.ContainsKey(x.MAssetType));
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    var items = new List<MovableAssetModel>();
                    _movableAssetService.GetStoreMovableAssetByLocation(date1, date2, SelectedStore.StoreId)
                        .ForEach(mo =>
                         {
                             items.Add(mo);
                         });

                    if (SelectedStuffType.ContainsKey("مصرفی"))
                    {
                        _commodityService.GetCommodities(date1,date2).ForEach(co =>
                        {
                            double memo = co.Num;
                            if (co.PlaceOfUses.Count() > 0)
                            {
                                var coU = Units.First(v => v.UnitId == co.UnitId);
                                co.PlaceOfUses.ForEach(cop =>
                                {
                                    var copU = Units.First(u => u.UnitId == cop.UnitId);
                                    double coMemo = 0;
                                    if (coU.Equals(copU))
                                    {
                                        coMemo = cop.Num;
                                    }
                                    else
                                    {
                                        Unit isOrderChild = null;
                                        Unit IsPropertyChild = null;

                                        if (coU != null)
                                        {
                                            isOrderChild = mainparentRecovery(coU);
                                        }

                                        if (copU != null)
                                        {
                                            IsPropertyChild = mainparentRecovery(copU);
                                        }

                                        if (isOrderChild.Equals(IsPropertyChild))
                                        {
                                            Double orderval = CalculateUnitNum(copU, cop.Num);
                                            double propertyVal = ReverseCalculateUnitNum(coU, orderval);
                                            coMemo = propertyVal;
                                        }
                                    }
                                    memo -= coMemo;
                                });
                            }

                            if (IsCommodityToNum)
                            {
                                if (memo>0)
                                {
                                    var coItem = new MovableAssetModel
                                    {
                                        AcqType = StateOwnership.Purchase,
                                        AssetId = co.AssetId,
                                        InsertDate = co.InsertDate,
                                        CurState = MAssetCurState.AtOperation,
                                        IsCompietion = CompietionState.NotReported,
                                        IsConfirmed = false,
                                        IsInStore = true,
                                        kalaUid = co.KalaUid,
                                        KalaNo = co.KalaNo,
                                        Label = null,
                                        MAssetType = "مصرفی",
                                        Name = co.Name,
                                        Num = memo,
                                        UnitId = co.UnitId,
                                        PersianInsertDate = co.InsertDate.PersianDateTime()
                                    };
                                    items.Add(coItem);
                                }
                            }
                            else
                            {
                                var coItem = new MovableAssetModel
                                {
                                    AcqType = StateOwnership.Purchase,
                                    AssetId = co.AssetId,
                                    InsertDate = co.InsertDate,
                                    CurState = MAssetCurState.AtOperation,
                                    IsCompietion = CompietionState.NotReported,
                                    IsConfirmed = false,
                                    IsInStore = true,
                                    kalaUid = co.KalaUid,
                                    KalaNo = co.KalaNo,
                                    Label = null,
                                    MAssetType = "مصرفی",
                                    Name = co.Name,
                                    Num = memo,
                                    UnitId = co.UnitId,
                                    PersianInsertDate=co.InsertDate.PersianDateTime()
                                };
                                items.Add(coItem);
                            }
                          });
                    }
                    _allAsset = items.Where(x => SelectedStuffType.ContainsKey(x.MAssetType));
                });

            }

            this.FilterByStoreDesign();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void GetAllStoreDesignId(StoreDesign selectedItem)
        {
            if (selectedItem.ChildNode.Count > 0)
            {
                foreach (var k in selectedItem.ChildNode.AsParallel<StoreDesign>())
                {
                    this.GetAllStoreDesignId(k);
                }
            }
            _allStoreId.Add(selectedItem.StoreDesignId);
        }

        private void FilterByStoreDesign()
        {
            if (_allAsset == null) return;
            if (NestPropertyView)
            {
                _storeFilterNo = 1;
            }
            else
            {
                if (_allStoreId.Count > 0) _storeFilterNo = 2;
                else _storeFilterNo = 3;
            }

            if (ChGroupView) this.StoreGroupingFiltering();
            else this.StoreNormalFilteringAsync();
        }

        private async void StoreNormalFilteringAsync()
        {
            Mouse.SetCursor(Cursors.Wait);

            var _globalTask = Task.Factory.StartNew(() =>
            {
                int i = 0;
                switch (_storeFilterNo)
                {
                    case 1:
                        _allAsset.Where(x => x.StoreDesignId == StoreDesignSelected.StoreDesignCurrent.StoreDesignId).AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 2:
                        _allAsset.Where(x => _allStoreId.Contains(x.StoreDesignId)).AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    default:
                        _allAsset.AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                }
            });
            try
            {
                this.Collection = new ObservableCollection<MovableAssetModel>(new List<MovableAssetModel>
                { new MovableAssetModel() });
                _collection.Clear();
                await _globalTask;
            }
            catch (ArgumentOutOfRangeException) { }
            catch (Exception) { throw; }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void StoreGroupingFiltering()
        {
            Mouse.SetCursor(Cursors.Wait);
            List<MovableAssetModel> Commoditygroup = null;
            switch (_storeFilterNo)
            {
                case 1:
                    Commoditygroup = (from c in _allAsset
                                      where (c.StoreDesignId == StoreDesignSelected.StoreDesignCurrent.StoreDesignId)
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          StoreDesignId = g.First().StoreDesignId,
                                          StoreId = SelectedStore != null ? SelectedStore.StoreId : 0,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      }).ToList();
                    break;
                case 2:
                    Commoditygroup = (from c in _allAsset
                                      where (_allStoreId.Contains(c.StoreDesignId))
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          StoreDesignId = g.First().StoreDesignId,
                                          StoreId = SelectedStore != null ? SelectedStore.StoreId : 0,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      }).ToList();
                    break;
                default:
                    Commoditygroup = (from c in _allAsset
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          StoreDesignId = g.First().StoreDesignId,
                                          StoreId = SelectedStore != null ? SelectedStore.StoreId : 0,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      }).ToList();
                    break;
            }

            if (Commoditygroup != null)
            {
                _collection.Clear();
                Commoditygroup.ForEach(x =>
                  {
                      _collection.Add(x);
                  });
            }

            Mouse.SetCursor(Cursors.Arrow);
        }
        void PerformSearch()
        {
            if (_matchingStoreEnumerator == null || !_matchingStoreEnumerator.MoveNext())
            {
                this.VerifyMatchingPeopleEnumerator();
            }

            var store = _matchingStoreEnumerator.Current;

            if (store == null)
                return;

            // Ensure that this person is in view.
            if (store.Parent != null)
            {
                store.Parent.IsExpanded = true;
            }

            store.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            try
            {
                var matches = this.FindParnetMatches(SearchText, _storefiristGeneration);

                _matchingStoreEnumerator = matches.GetEnumerator();

                if (!_matchingStoreEnumerator.MoveNext())
                {
                    _dialogService.ShowAlert("دوباره سعی کنید", "هیچ شاخه ای پیدا نشد");
                }
            }

            catch (NullReferenceException) { }
            catch (Exception) { throw; }
        }

        IEnumerable<StoreTreeViewModel> FindParnetMatches(string searchText, IEnumerable<StoreTreeViewModel> allstore)
        {
            foreach (var store in allstore)
            {
                if (store.NameContainsText(searchText))
                    yield return store;

                foreach (StoreTreeViewModel child in store.Children)
                    foreach (StoreTreeViewModel match in this.FindMatches(searchText, child))
                        yield return match;
            }
        }

        IEnumerable<StoreTreeViewModel> FindMatches(string searchText, StoreTreeViewModel store)
        {

            if (store.NameContainsText(searchText))
                yield return store;

            foreach (StoreTreeViewModel child in store.Children)
                foreach (StoreTreeViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        private void GetParentNode()
        {
            if (SelectedStore == null) return;
            _collection.Clear();
            var items = _storeService.GetParentNode(SelectedStore.StoreId).Where(x => x.ParentNode == null).ToList();

            _storefiristGeneration.Clear();
            foreach (var store in items) _storefiristGeneration.Add(new StoreTreeViewModel(store, null, true));
        }
        
        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAssetModel;
            if (mAsset == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            this.Selected = mAsset;
            Window window = null;
            if (string.Equals(mAsset.MAssetType, "مصرفی"))
            {
                var asset=_commodityService.Query(ma => ma.AssetId == mAsset.AssetId)
                    .Include(ma => ma.StoreBill).Include(ma=>ma.PlaceOfUses).Select().Single();
                var viewModel = new CommodityDetailsViewModel(_container,asset.AssetId,null);
                window = _navigationService.ShowCommodityDetailsWindow(viewModel);
            }
            else
            {
                var asset = _movableAssetService.Query(ma => ma.AssetId == mAsset.AssetId)
                    .Include(ma => ma.StoreBill).Include(ma => ma.Locations).Include(ma => ma.Documetns).Select().Single();

                var viewModel = new MovableAssetDetailsViewModel(_container, asset.AssetId);
                window = _navigationService.ShowMAssetDetailsWindow(viewModel);
            }

            if (window.DialogResult == true)
            {
                this.getMAssetForStore();
            }
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private String GetHirecharyStoreNode(StoreDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyStoreNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private void storeListReport()
        {
            Mouse.SetCursor(Cursors.Wait);

            if (SelectedStore == null)
            {
                _dialogService.ShowAlert("انتخاب انبار", "هیچ انباری انتخاب نشده است");
                return;
            }
            var viewModel = new ReportViewModel();
            string[] Type = new string[5];
            DateTime? pDate1 = default(DateTime?);
            DateTime? pDate2 = default(DateTime?);
            if (SelectedStuffType.ContainsKey("مصرفی"))
            {
                Type[0] = "11001";
            }

            if (SelectedStuffType.ContainsKey("غیرمصرفی"))
            {
                Type[1] = "11002";
            }

            if (SelectedStuffType.ContainsKey("در حکم مصرف"))
            {
                Type[2] = "11003";
            }

            if (SelectedStuffType.ContainsKey("قابل نصب در بنا"))
            {
                Type[3] = "11004";
            }

            if (SelectedStuffType.ContainsKey("متعلقات"))
            {
                Type[4] = "11005";
            }

            string storePath = "";
            int queryLevel = _storeFilterNo;
            int stdId = 0;
            if (StoreDesignSelected != null)
            {
                storePath = GetHirecharyStoreNode(StoreDesignSelected.StoreDesignCurrent);
                stdId = StoreDesignSelected.StoreDesignCurrent.StoreDesignId;
            }
            if (DateEnabled)
            {
                pDate1 = FromDate.Value.ToDateTime();
                pDate2 = ToDate.Value.ToDateTime();
            }
            viewModel.MAssetStoreListReport(queryLevel, SelectedStore.StoreId,
                stdId, SelectedStore.Name, storePath, Type, pDate1, pDate2, "", ChGroupView);

            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private Unit mainparentRecovery(Unit parent)
        {
            Unit mparent = parent;

            if (parent.Parent != null)
            {
                mparent = this.mainparentRecovery(parent.Parent);
            }
            return mparent;
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

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand StoreTreeSearchCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        private void initializCommand()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.getMAssetForStore(); },
                (parameter) => { return true; }
                );

            StoreTreeSearchCommand = new MvvmCommand(
               (ParameterOverride) =>
               {
                   this.PerformSearch();
               },
               (parameter) =>
               {
                   return true;
               }
               );

            EditCommand = new MvvmCommand(
               (parameter) => { this.showMAssetDetails(parameter); },
               (parameter) => { return true; }
               );

            ReportCommand = new MvvmCommand(
              (parameter) =>
              {
                this.storeListReport();
              },
              (parameter) => { return true; }
              );

            DoubleClickListViewItemCommand = new MvvmCommand(
            (parameter) => { this.showMAssetDetails(parameter); },
            (parameter) => { return true; }
            );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitService _unitService;
        private readonly IStoreService _storeService;
        private readonly IPersonService _personService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        IEnumerable<MovableAssetModel> _allAsset;
        List<int> _allStoreId;
        int _storeFilterNo = 3;
        private ObservableCollection<MovableAssetModel> _collection;
        private readonly ObservableCollection<StoreTreeViewModel> _storefiristGeneration;
        IEnumerator<StoreTreeViewModel> _matchingStoreEnumerator;

        #endregion
    }
}
