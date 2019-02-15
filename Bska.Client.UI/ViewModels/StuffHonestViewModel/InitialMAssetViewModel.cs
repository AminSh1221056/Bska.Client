
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.AssetViewModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Data.Entity;
    using System.ComponentModel;
    using Domain.Concrete;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using System.Windows.Data;

    public sealed class InitialMAssetViewModel : BaseViewModel
    {
        #region ctor

        public InitialMAssetViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._stuffService = _container.Resolve<IStuffService>();
            this._unitService = _container.Resolve<IUnitService>();

            _subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._stuffList = new ObservableCollection<Stuff>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._storefiristGeneration = new ObservableCollection<StoreTreeViewModel>();
            this._movableAssetCollection = new ObservableCollection<MovableAsset>();
            this._commodityCollection = new ObservableCollection<Commodity>();
            this._firstGeneration = new ObservableCollection<StuffTreeViewModel>();
            _personPermit = new List<RequestPermit>();
            this._storeBills = new HashSet<StoreBill>();
            this._documents = new Dictionary<Location, Document>();
            this._relatedAccounts = new Dictionary<StoreBill, AccountDocumentMaster>();
            this._relatedAccountsDoc = new Dictionary<Document, AccountDocumentMaster>();
            this._relatedAccountDocId = new Dictionary<Document, int>();
            this._relatedAccountId = new Dictionary<StoreBill, int>();
            this.MovableAssetFilteredView = new CollectionViewSource { Source = MovableAssetCollection }.View;
            this._labels = new Dictionary<int, Tuple<int?, string, int?>>();
            this._bLabels = new HashSet<int>();
            this._iLabels = new HashSet<int>();

            IsInStore = false;
            ToLabel = true;
            initalizObj();
            initalizCommand();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;
            set;
        }

        public ObservableCollection<StuffTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public StuffTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
                if (value != null)
                {
                    initStuffAsync();
                }
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

        public List<PersonModel> Persons
        {
            get { return GetValue(() => Persons); }
            set
            {
                SetValue(() => Persons, value);
            }
        }

        public PersonModel SelectedPerson
        {
            get { return GetValue(() => SelectedPerson); }
            set
            {
                SetValue(() => SelectedPerson, value);
                this.GetPersonPermit();
                this.GetParentNode();
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

        public Store SelectedStore
        {
            get { return GetValue(() => SelectedStore); }
            set
            {
                SetValue(() => SelectedStore, value);
                this.GetStoreParentNode();
            }
        }

        public ObservableCollection<Stuff> StuffList
        {
            get { return _stuffList; }
        }

        public ICollectionView MovableAssetFilteredView { get; set; }
        public ObservableCollection<MovableAsset> MovableAssetCollection
        {
            get { return _movableAssetCollection; }
        }

        public ObservableCollection<Commodity> CommodityCollection
        {
            get { return _commodityCollection; }
        }

        public MovableAsset SelectedAsset
        {
            get { return GetValue(() => SelectedAsset); }
            set
            {
                SetValue(() => SelectedAsset, value);
            }
        }

        public Commodity SelectedCommodity
        {
            get { return GetValue(() => SelectedCommodity); }
            set
            {
                SetValue(() => SelectedCommodity, value);
            }
        }

        public Boolean IsInStore
        {
            get { return GetValue(() => IsInStore); }
            set
            {
                SetValue(() => IsInStore, value);
            }
        }

        public ObservableCollection<StoreTreeViewModel> StoreFiristGeneration
        {
            get { return _storefiristGeneration; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> StrategyCollection
        {
            get { return _strategyCollection; }
        }

        public EmployeeDesignTreeViewModel StrategySelected
        {
            get { return GetValue(() => StrategySelected); }
            set
            {
                SetValue(() => StrategySelected, value);
                this.initselecteRequest();
            }
        }

        public EmployeeDesignTreeViewModel OrganizSelected
        {
            get { return GetValue(() => OrganizSelected); }
            set
            {
                SetValue(() => OrganizSelected, value);
                if (value != null)
                {
                    GetStrategyRelated();
                }
            }
        }

        public UnConsumptionViewModel UnConsumptionViewModel
        {
            get;
            private set;
        }

        public CommodityViewModel CommodityViewModel
        {
            get;
            private set;
        }

        public BelongingViewModel BelongingViewModel
        {
            get;
            private set;
        }

        public InCommodityViewModel InCommodityViewModel
        {
            get;
            private set;
        }

        public InstallableViewModel InstallableViewModel
        {
            get;
            private set;
        }

        public StoreBillViewModel StoreBillViewModel
        {
            get { return GetValue(() => StoreBillViewModel); }
            set
            {
                SetValue(() => StoreBillViewModel, value);
            }
        }

        public DocumentViewModel DocumentViewModel
        {
            get { return GetValue(() => DocumentViewModel); }
            set
            {
                SetValue(() => DocumentViewModel, value);
            }
        }

        public Int32 Year
        {
            get { return GetValue(() => Year); }
            set
            {
                SetValue(() => Year, value);
            }
        }

        [Required(ErrorMessage = "تعداد الزامی است")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public int Num
        {
            get { return _num; }
            set
            {
                _num = value;
                ValidateProperty(value);
                OnPropertyChanged("Num");
                if (!IsCommodity)
                {
                    if (value > 200)
                    {
                        _dialogService.ShowAlert("هشدار", "تعداد وارد شده بیش از حد مجاز است");
                        return;
                    }
                    this.generateLabelAsync();
                }
            }
        }

        public Stuff SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
            }
        }

        public Boolean ToLabel
        {
            get { return GetValue(() => ToLabel); }
            set
            {
                SetValue(() => ToLabel, value);
            }
        }

        public StoreTreeViewModel StoreDesignSelected
        {
            get { return GetValue(() => StoreDesignSelected); }
            set
            {
                SetValue(() => StoreDesignSelected, value);
            }
        }

        public List<RequestPermit> RequestPermits
        {
            get { return GetValue(() => RequestPermits); }
            set
            {
                SetValue(() => RequestPermits, value);
            }
        }

        public RequestPermit SelectedRequest
        {
            get { return GetValue(() => SelectedRequest); }
            set
            {
                SetValue(() => SelectedRequest, value);
                if (value != null)
                {
                    this.SelectedPerson = Persons.Find(x => x.PersonId == value.PersonId);
                    this.PerformSearch();
                }
            }
        }
        public string PermitId
        {
            get { return GetValue(() => PermitId); }
            set
            {
                SetValue(() => PermitId, value);
            }
        }

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }
        public Boolean IsCommodity
        {
            get { return GetValue(() => IsCommodity); }
            set
            {
                SetValue(() => IsCommodity, value);
            }
        }

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

        private void initalizObj()
        {
            _storefiristGeneration.Clear();
            _organizCollection.Clear();
            _strategyCollection.Clear();

            Persons = _personService.Queryable().Where(x => x.PersonId != 1).Select(p => new PersonModel
            {
                PersonId = p.PersonId,
                FullName = p.FirstName + " " + p.LastName,
                NationalId = p.NationalId
            }).ToList();

            Stores = _storeService.Queryable().Where(x => x.StoreType != StoreType.Retiring).ToList();

            Units = _unitService.Queryable().ToList();

            StoreBillViewModel = new StoreBillViewModel(new StoreBill()) { Stores = Stores, StoreBillNo = null, AcqTyp = StateOwnership.Purchase, ArrivalDate = PersianDate.Today };
            DocumentViewModel = new DocumentViewModel(new Document()) { Desc1 = null, DocumentDate = PersianDate.Today };
            if (!IsInStore)
            {
                RequestPermits = _personService.GetAllPermits().ToList();

                _allOrganiz = _employeeService.GetParentNode(1).ToList();
                _allStrategy = _employeeService.GetParentNode(2).ToList();
            }
            Year = GlobalClass._Today.PersianDateTime().Year;
        }

        private void movableAssetFilters()
        {
            this.MovableAssetFilteredView.Filter = ((obj) =>
            {
                MovableAsset items = obj as MovableAsset;
                if (items != null)
                    return items.Name.StartsWith(SearchCriteria) || items.Label.ToString() == SearchCriteria;
                return false;
            });
        }

        internal Dictionary<int, string> getAvailableBookTypes()
        {
            Dictionary<int, string> bookDic = new Dictionary<int, string>();
            int bookType = APPSettings.Default.BookType;
            if (bookType == 1001)
            {
                bookDic.Add(1, "نظام نوین");
                bookDic.Add(2, "فهرست موجودی اولیه");
            }
            else if (bookType == 1003)
            {
                bookDic.Add(1, "نظام نوین");
            }
            else if (bookType == 1002)
            {
                bookDic.Add(2, "فهرست موجودی اولیه");
            }

            if (IsInStore)
            {
                bookDic.Add(3, "موجودی اولیه اموال مصرفی");
            }

            return bookDic;
        }

        private void initselecteRequest()
        {
            var rp = RequestPermits.FirstOrDefault(r => r.PersonId == SelectedPerson.PersonId && r.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId
                   && r.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId);
            this.PermitId = rp.RequestPermitId.ToString();
        }

        private void CreateListerner<T>(T ChildviewModel) where T : INotifyPropertyChanged
        {
            ChangeListener.Create(ChildviewModel).PropertyChanged += InitialMAssetViewModel_PropertyChanged;
        }

        private void InitialMAssetViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ArrivalDate":
                    this.Year = StoreBillViewModel.ArrivalDate.Year;
                    this.DocumentViewModel.DocumentDate = this.StoreBillViewModel.ArrivalDate;
                    break;
                case "DocumentDate":
                    if (StoreBillViewModel.ArrivalDate > DocumentViewModel.DocumentDate)
                    {
                        _dialogService.ShowAlert("توجه", "تاریخ حواله انبار نمیتواند از تاریخ قبض انبار کوچکتر باشد");
                        this.DocumentViewModel.DocumentDate = this.StoreBillViewModel.ArrivalDate;
                    }
                    break;
                case "AcqTyp":
                    this.Num = 1;
                    this.StoreBillViewModel.SelectedSeller = null;
                    break;
                case "StoreBillNo":
                    if (!IsCommodity)
                    {
                        int temp = 0;
                        if (!int.TryParse(StoreBillViewModel.StoreBillNo, out temp))
                        {
                            _dialogService.ShowAlert("توجه", "شماره قبض انبار ورودی نامعتبر است");
                            return;
                        }
                        int bookType = APPSettings.Default.BookType;
                        if (bookType == 1001)
                        {
                            if (temp < 5 && !_isOldSystem)
                            {
                                _dialogService.ShowAlert("توجه", "برای نظام نوین حداقل شماره برای قبض انبار از 5 شروع می شود");
                                StoreBillViewModel.StoreBillNo = "5";
                            }
                        }
                    }
                    break;
            }
        }

        private void initStuffAsync()
        {
            _stuffList.Clear();
            Task.Run(() =>
            {
                _stuffService.Query().Include(x => x.Parent)
                      .Select(x => x).Where(x => x.IsStuff == true).ToList().ForEach(x =>
                      {
                          if (SelectedNode != null)
                          {
                              bool isTrue = parentRecovery(x.Parent);
                              if (isTrue)
                              {
                                  DispatchService.Invoke(() =>
                                  {
                                      _stuffList.Add(x);
                                  });
                              }

                              if (SelectedNode.StuffId == 3)
                              {
                                  if (x.StuffType == StuffType.UnConsumption)
                                  {
                                      DispatchService.Invoke(() =>
                                      {
                                         
                                          _stuffList.Add(x);
                                      });
                                  }
                              }
                          }
                          else
                          {
                              DispatchService.Invoke(() =>
                              {
                                  _stuffList.Add(x);
                              });
                          }
                      });
            });
        }

        private Boolean parentRecovery(Stuff parent)
        {
            bool isChild = false;
            if (parent.Equals(SelectedNode.StuffCurrent))
            {
                isChild = true;
            }
            else if (parent.Parent != null)
            {
                isChild = this.parentRecovery(parent.Parent);
            }
            return isChild;
        }

        public StuffTreeViewModel GetStuffLastTreeParent(StuffTreeViewModel parent)
        {
            StuffTreeViewModel currentParent = null;

            if (parent.Parent != null)
            {
                currentParent = this.GetStuffLastTreeParent(parent.Parent);
            }
            else
            {
                currentParent = parent;
            }
            return currentParent;
        }

        private void GetPersonPermit()
        {
            if (SelectedPerson != null)
            {
                _personPermit.Clear();
                if (RequestPermits == null)
                {
                    foreach (var k in _personService.GetPersonPermit(SelectedPerson.PersonId))
                    {
                        _personPermit.Add(k);
                    }
                }
                else
                {
                    RequestPermits.Where(rp => rp.PersonId == SelectedPerson.PersonId).ForEach(rp =>
                    {
                        _personPermit.Add(rp);
                    });
                }
            }
        }

        private void GetStoreParentNode()
        {
            if (SelectedStore != null)
            {
                _storefiristGeneration.Clear();
                var items = _storeService.GetParentNode(SelectedStore.StoreId).Where(x => x.ParentNode == null).ToList();
                foreach (var store in items) _storefiristGeneration.Add(new StoreTreeViewModel(store, null, true));

                if (StoreBillViewModel != null)
                {
                    StoreBillViewModel.SelectedStore = this.SelectedStore;
                }

                var first = _storefiristGeneration.FirstOrDefault();
                if (first != null)
                {
                    first.IsSelected = true;
                }
            }
        }

        private void GetParentNode()
        {
            _organizCollection.Clear();
            _strategyCollection.Clear();
            var organizPermit = _personPermit.Select(x => x.OrganizId);
            var orgItems = _allOrganiz.Where(x => x.ParentNode == null);

            foreach (var org in orgItems)
            {
                _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null, organizPermit));
            }
        }

        private void GetStrategyRelated()
        {
            _strategyCollection.Clear();
            var strategyPermit = _personPermit.Where(x => x.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId)
                .Select(x => x.StrategyId);

            var stgyItems = _allStrategy.Where(x => x.ParentNode == null);
            foreach (var stgy in stgyItems)
            {
                _strategyCollection.Add(new EmployeeDesignTreeViewModel(stgy, null, strategyPermit));
            }
        }

        internal async void stuffGenerationAsync(StuffType stype)
        {
            _stuffList.Clear();
            _firstGeneration.Clear();
            _movableAssetCollection.Clear();
            _storeBills.Clear();
            _documents.Clear();

            this.StoreBillViewModel.Sellers = _sellerService.Queryable().ToList();
            this.StoreBillViewModel.SelectedSeller = this.StoreBillViewModel.Sellers.FirstOrDefault();

            this.StoreBillViewModel.SelectedStore = StoreBillViewModel.Stores.FirstOrDefault();
            this.SelectedStore = StoreBillViewModel.Stores.FirstOrDefault();

            var ts = new Task(() =>
            {
                if (stype == StuffType.Consumable)
                {
                    _stuffService.Query(x => (x.Parent == null || x.IsStuff == true) && x.StuffType == StuffType.Consumable).Include(x => x.Parent)
                         .Select().AsEnumerable().ForEach(s =>
                         {
                             DispatchService.Invoke
                             (() =>
                             {
                                 if (s.IsStuff) _stuffList.Add(s);
                                 else
                                 {
                                     if (s.Parent == null)
                                         _firstGeneration.Add(new StuffTreeViewModel(s, _container));
                                 }
                             });
                         });
                }
                else
                {
                    _stuffService.Query(x => (x.Parent == null || x.IsStuff == true) && x.StuffType != StuffType.Consumable)
                    .Include(x => x.Parent)
                         .Select().AsEnumerable().ForEach(s =>
                         {
                             DispatchService.Invoke
                             (() =>
                             {
                                 if (s.IsStuff) _stuffList.Add(s);
                                 else
                                 {
                                     if (s.Parent == null)
                                         _firstGeneration.Add(new StuffTreeViewModel(s, _container));
                                 }
                             });
                         });
                }
            });
            ts.Start();
            await ts;

            this.CreateListerner<StoreBillViewModel>(StoreBillViewModel);
            this.CreateListerner<DocumentViewModel>(DocumentViewModel);
            if (!_isOldSystem)
            {
                if (stype != StuffType.Consumable)
                {
                    this.StoreBillViewModel.StoreBillNo = await initBillNo(stype);
                }
            }

            this.DocumentViewModel.Desc1 = await initDraftNo();
        }

        internal async void initViewModels(StuffType sType)
        {
            Mouse.SetCursor(Cursors.Wait);
            Num = 0;
            _lastType = sType;

            var units = Units.Where(u => u.StuffId == sType || u.StuffId == StuffType.None).ToList();

            if (sType == StuffType.UnConsumption)
            {
                UnConsumptionViewModel = new UnConsumptionViewModel(new UnConsumption()) { Name = SelectedStuff.Name, UnitId = 0, Cost = 0, Quality = "", Units = units };

                await this.getOldLabelsByFloorAsync();
            }
            else if (sType == StuffType.Consumable)
            {
                IsCommodity = true;
                _isOldSystem = false;
                _subUnits.Clear();
                CommodityViewModel = new CommodityViewModel(new Commodity(), units) { UnitPrice = 0, Cost = 0, UnitId = 0, Num = 0 };
                units.Where(u => u.Parent == null).ForEach(u =>
                {
                    _subUnits.Add(new UnitTreeViewModel(u, _container));
                });
            }
            else if (sType == StuffType.OrderConsumption)
            {
                this.InCommodityViewModel = new InCommodityViewModel(new InCommidity { Name = SelectedStuff.Name, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = 0, Num = 0 }) { Units = units, Cost = 0 };
            }
            else if (sType == StuffType.Installable)
            {
                this.InstallableViewModel = new InstallableViewModel(new Installable { Name = SelectedStuff.Name, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = 0 }) { Units = units, Cost = 0 };
            }
            else if (sType == StuffType.Belonging)
            {
                this.BelongingViewModel = new BelongingViewModel(new Belonging { Name = SelectedStuff.Name, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = 0 }) { Units = units, Cost = 0 };
            }
            PersianDate pDate = new PersianDate(Year, 1, 1);

            if (_isOldSystem)
            {
                if (sType == StuffType.UnConsumption)
                {
                    this.UnConsumptionViewModel.Floor = null;
                    this.UnConsumptionViewModel.OldLabel = null;
                    if (SelectedNode?.StuffId == 3)
                    {
                        this.StoreBillViewModel.StoreBillNo = "2";
                    }
                    else
                    {
                        this.StoreBillViewModel.StoreBillNo = "1";
                    }
                }
                else if (sType == StuffType.OrderConsumption)
                {
                    this.StoreBillViewModel.StoreBillNo = "2";
                }
                else if (sType == StuffType.Installable)
                {
                    this.StoreBillViewModel.StoreBillNo = "3";
                }
                else if (sType == StuffType.Belonging)
                {
                    this.StoreBillViewModel.StoreBillNo = "4";
                }
            }
            else
            {
                string maxBillNo = StoreBillViewModel.StoreBillNo;
                if (sType == StuffType.Consumable)
                {
                    maxBillNo = "0";
                }
                else
                {
                    if (MovableAssetCollection.Count > 0)
                    {
                        switch (sType)
                        {
                            case StuffType.UnConsumption:
                                var items = _movableAssetCollection.OfType<UnConsumption>();
                                if (items.Count() > 0)
                                {
                                    maxBillNo = items.Select(s => s.StoreBill).Last().StoreBillNo;
                                }
                                break;
                            case StuffType.Belonging:
                                var items1 = _movableAssetCollection.OfType<Belonging>();
                                if (items1.Count() > 0)
                                {
                                    maxBillNo = items1.Select(s => s.StoreBill).Last().StoreBillNo;
                                }
                                break;
                            case StuffType.Installable:
                                var items2 = _movableAssetCollection.OfType<Installable>();
                                if (items2.Count() > 0)
                                {
                                    maxBillNo = items2.Select(s => s.StoreBill).Last().StoreBillNo;
                                }
                                break;
                            case StuffType.OrderConsumption:
                                var items3 = _movableAssetCollection.OfType<InCommidity>();
                                if (items3.Count() > 0)
                                {
                                    maxBillNo = items3.Select(s => s.StoreBill).Last().StoreBillNo;
                                }
                                break;
                            case StuffType.Consumable:
                                maxBillNo = "0";
                                break;
                        }
                    }
                }

                StoreBillViewModel.StoreBillNo = maxBillNo;
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private Task<String> initDraftNo()
        {
            var ts = new Task<string>(() =>
            {
                string draftNo = "1";
                int temp;
                IEnumerable<Document> docs;
                if (!_isOldSystem)
                {
                    draftNo = "5";
                    docs = _movableAssetService.GetDocuments(false, true)
                    .Where(d=>d.DocumentType!=DocumentType.InitialBalance).ToList();
                    if (docs.Count() > 0)
                    {
                        int maxVal = docs
                            .Select(d => int.TryParse(d.Desc1, out temp) ? temp : 0).Max();
                        draftNo = (maxVal + 1).ToString();
                    }
                }
                
                return draftNo;
            });
            ts.Start();
            return ts;
        }

        private Task<string> initBillNo(StuffType stype)
        {
            var ts = new Task<string>(() =>
            {
                string billNo = "5";
                int temp;
                var biilsNo = _storeBillService.Queryable().ToList()
                .Where(x => x.ArrivalDate.PersianDateTime().Year == PersianDate.Today.Year).Select(x => x.StoreBillNo);
                if (biilsNo.Count() > 0)
                {
                    int maxVal = biilsNo.Select(x => int.TryParse(x, out temp) ? temp : 0).Max();
                    if (maxVal >= 5)
                    {
                        billNo = (maxVal + 1).ToString();
                    }
                }
               
                return billNo;
            });
            ts.Start();
            return ts;
        }

        private void generateLabelAsync()
        {
            if (Num <= 0) return;
            if (this.StoreBillViewModel == null) return;
            Task.Run(() =>
            {
                if (_lastType == StuffType.Belonging)
                {

                    int maxLabel = 0;

                    if (_bLabels.Count > 0)
                    {
                        maxLabel = _bLabels.Max();
                    }
                    else
                    {
                        var mAsset = _movableAssetService.Queryable().OfType<Belonging>()
                            .Where(x => x.Label.HasValue).AsNoTracking().Select(x => x.Label.Value).ToList();
                        if (mAsset.Count() > 0)
                        {
                            maxLabel = mAsset.Max();
                        }
                    }

                    var labels = new List<int>();
                    if (maxLabel <= 0)
                    {
                        for (int i = 1; i <= Num; i++)
                        {
                            labels.Add(i);
                        }
                    }
                    else
                    {
                        int label = maxLabel + 1;
                        int i = 0;
                        do
                        {
                            labels.Add(label);
                            label++;
                            i++;
                        } while (i != Num);
                    }

                    if (BelongingViewModel == null) return;
                    DispatchService.Invoke(() =>
                    {
                        this.BelongingViewModel.Labels = labels;
                    });
                }
                else if (_lastType == StuffType.Installable)
                {
                    int maxLabel = 0;

                    if (_iLabels.Count > 0)
                    {
                        maxLabel = _iLabels.Max();
                    }
                    else
                    {
                        var mAsset = _movableAssetService.Queryable().OfType<Installable>()
                            .Where(x => x.Label.HasValue).AsNoTracking().Select(x => x.Label.Value).ToList();
                        if (mAsset.Count() > 0)
                        {
                            maxLabel = mAsset.Max();
                        }
                    }

                    var labels = new List<int>();
                    if (maxLabel <= 0)
                    {
                        for (int i = 1; i <= Num; i++)
                        {
                            labels.Add(i);
                        }
                    }
                    else
                    {
                        int label = maxLabel + 1;
                        int i = 0;
                        do
                        {
                            labels.Add(label);
                            label++;
                            i++;
                        } while (i != Num);
                    }

                    if (InstallableViewModel == null) return;
                    DispatchService.Invoke(() =>
                    {
                        this.InstallableViewModel.Labels = labels;
                    });
                }
                else if (_lastType == StuffType.UnConsumption)
                {
                    int maxlable = 1;

                    if (_labels.Count <= 0)
                    {
                        _movableAssetService.Queryable().OfType<UnConsumption>().AsNoTracking().Where(x => x.Label.HasValue)
                        .ToDictionary(x => x.Label.Value, x => new Tuple<int?, string, int?>(x.FloorType, x.Floor, x.OldLabel)).ForEach(x =>
                        {
                            _labels.Add(x.Key, x.Value);
                        });

                        if (_labels.Count > 0)
                        {
                            maxlable = _labels.Max(x => x.Key) + 1;
                        }
                        else
                        {
                            maxlable = 1;
                        }
                    }
                    else
                    {
                        maxlable = _labels.Max(x => x.Key) + 1;
                    }

                    var freeLabels = new List<int>();
                    for (int i = maxlable; i < (maxlable + Num); i++)
                    {
                        freeLabels.Add(i);
                    }

                    DispatchService.Invoke(() =>
                    {
                        this.UnConsumptionViewModel.Labels = freeLabels;
                    });
                }
            });
        }

        private string checkMaxBillNo(StuffType stype)
        {
            string maxBillNo = StoreBillViewModel.CurrentEntity.StoreBillNo;
            int temp;
            if (_storeBills.Count > 0)
            {
                int maxVal = _storeBills.Select(x => int.TryParse(x.StoreBillNo, out temp) ? temp : 0).Max();
                maxBillNo = (maxVal + 1).ToString();
            }
            return maxBillNo;
        }

        private void AddToStoreBill()
        {
            if (!IsInStore)
            {
                if (SelectedPerson == null || OrganizSelected == null || StrategySelected == null)
                {
                    _dialogService.ShowAlert("توجه", "انتخاب پرسنل،منطقه سازمانی و منطقه استراتژیکی الزامی است");
                    return;
                }
            }

            if (SelectedStuff == null)
            {
                _dialogService.ShowAlert("انتخاب مال", "لطفا یک مال را انتخاب کنید");
                return;
            }

            if (StoreBillViewModel.SelectedStore == null)
            {
                _dialogService.ShowAlert("توجه", "انتخاب انبار الزامی است");
                return;
            }

            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            StuffType stuffType = _lastType;
            bool fromUnconsuption = false;

            if (SelectedStuff.StuffType == StuffType.Consumable)
            {
                if (CommodityViewModel.HasErrors || StoreBillViewModel.HasErrors)
                {
                    _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                    return;
                }
            }
            else
            {
                double tempNum = 0;
                if (SelectedStuff.StuffType == StuffType.UnConsumption)
                {
                    if (!UnConsumptionViewModel.CheckErrors())
                    {
                        _dialogService.ShowError("توجه", "لطفا ورودی های خود را کنترل کنید");
                        return;
                    }

                    if (SelectedNode != null)
                    {
                        var lastParent = this.GetStuffLastTreeParent(SelectedNode);
                        if (lastParent.StuffCurrent.StuffType == StuffType.OrderConsumption)
                        {
                            stuffType = StuffType.OrderConsumption;
                            fromUnconsuption = true;
                        }
                    }

                    tempNum = this.Num;
                }
                else if (SelectedStuff.StuffType == StuffType.OrderConsumption)
                {
                    if (InCommodityViewModel.HasErrors)
                    {
                        _dialogService.ShowError("توجه", "لطفا ورودی های خود را کنترل کنید");
                        return;
                    }
                    tempNum = InCommodityViewModel.Num;
                }
                else if (SelectedStuff.StuffType == StuffType.Installable)
                {
                    if (InstallableViewModel.HasErrors)
                    {
                        _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                        return;
                    }
                    tempNum = InstallableViewModel.CurrentEntity.Num;
                }
                else if (SelectedStuff.StuffType == StuffType.Belonging)
                {
                    if (BelongingViewModel.HasErrors)
                    {
                        _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                        return;
                    }

                    if (!IsInStore)
                    {
                        if (BelongingViewModel.CurrentEntity.ParentMAsset == null)
                        {
                            _dialogService.ShowAlert("توجه", "برای خارج از انبار انتخاب مال اصلی الزامی است");
                            return;
                        }
                    }

                    tempNum = BelongingViewModel.CurrentEntity.Num;
                }
                else
                {
                    _dialogService.ShowAlert("توجه", "نوع مال انتخابی نامعتبر می باشد");
                    return;
                }

                if (StoreBillViewModel.HasErrors || DocumentViewModel.HasErrors)
                {
                    _dialogService.ShowError("توجه", "لطفا ورودی های خود را کنترل کنید");
                    return;
                }

                if ((tempNum < 0 || tempNum >= 200))
                {
                    _dialogService.ShowAlert("توجه", "تعداد وارد شده باید کمتر از 200 باشد");
                    return;
                }

                if (_movableAssetCollection.Count >= 200)
                {
                    _dialogService.ShowAlert("توجه", "تعداد آیتم در لیست به حداکثر مقدار خود برای ثبت رسیده است.لطفا ابتدا این آیتم ها را ثبت کرده سپس ادامه عملیات دهید");
                    return;
                }
            }

            Mouse.SetCursor(Cursors.Wait);
            this.SelectedStore = StoreBillViewModel.SelectedStore;
            var docType = DocumentType.StoreInternalDraft;
            string stNo = StoreBillViewModel.StoreBillNo;
            string docNo = DocumentViewModel.Desc1;
            DateTime docDate = DocumentViewModel.DocumentDate.ToDateTime();
            Location docLocation = null;
            PersianDate billdate = StoreBillViewModel.ArrivalDate;
            string transfree = "";
            Document doc = null;
            StoreBill storeBill = null;

            if (SelectedStuff.StuffType != StuffType.Consumable)
            {
                int? _oldLabel = null;

                AccountDocumentMaster accountMaster = null;
                AccountDocumentMaster accountMasterDoc = null;
                int accountMasterId = 0;
                int accountMasterDocId = 0;

                if (IsInStore)
                {
                    docLocation = new Location
                    {
                        StoreId = SelectedStore.StoreId,
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        StoreAddressId = StoreDesignSelected != null ? StoreDesignSelected.StoreDesignCurrent.StoreDesignId : 0,
                        Status = LocationStatus.StoreActive,
                    };
                    transfree = SelectedStore.Name;
                }
                else
                {
                    docLocation = new Location
                    {
                        StoreId = SelectedStore.StoreId,
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        OrganizId = OrganizSelected.BuildingDesignCurrent.BuidldingDesignId,
                        PersonId = SelectedPerson.NationalId,
                        StrategyId = StrategySelected.BuildingDesignCurrent.BuidldingDesignId,
                        Status = LocationStatus.Active,
                    };
                    transfree = SelectedPerson.FullName;
                }

                if (_isOldSystem)
                {
                    if (stuffType == StuffType.UnConsumption)
                    {
                        if (!fromUnconsuption)
                        {
                            if (UnConsumptionViewModel.CurrentEntity.Cost < APPSettings.Default.MStuffPrice)
                            {
                                fromUnconsuption = true;
                                stuffType = StuffType.OrderConsumption;
                                stNo = "2";
                            }
                        }
                        else
                        {
                            stNo = "2";
                        }

                        _oldLabel = UnConsumptionViewModel.CurrentEntity.OldLabel;
                    }

                    docType = DocumentType.InitialBalance;

                    storeBill = _storeBills
                        .Where(x => x.StoreBillNo == stNo
                         && x.AcqType == StoreBillViewModel.AcqTyp && x.ArrivalDate.PersianDateTime().Year == billdate.Year
                         && x.StuffType == stuffType)
                        .FirstOrDefault();

                    if (storeBill == null)
                    {
                        storeBill = _storeBillService.Queryable().Where(x => x.StoreBillNo == stNo
                          && x.AcqType == StoreBillViewModel.AcqTyp).AsEnumerable()
                          .Where(sb => sb.ArrivalDate.PersianDateTime().Year == billdate.Year)
                        .SingleOrDefault();

                        if (storeBill != null)
                        {
                            accountMasterId = _storeBillService.getRelatedAccountMasterId(storeBill.StoreBillId);
                        }
                    }
                    else
                    {
                        if (_relatedAccounts.ContainsKey(storeBill))
                        {
                            accountMaster = _relatedAccounts[storeBill];
                        }
                        else
                        {
                            accountMasterId = _storeBillService.getRelatedAccountMasterId(storeBill.StoreBillId);
                        }
                    }

                    if (_documents.ContainsKey(docLocation))
                    {
                        doc = _documents[docLocation];
                        if (!IsInStore)
                        {
                            if (_relatedAccountsDoc.ContainsKey(doc))
                            {
                                accountMasterDoc = _relatedAccountsDoc[doc];
                            }
                            else
                            {
                                accountMasterDocId = _movableAssetService.GetRelatedAccountDocumentByDoc(doc.DocumentId);
                            }
                        }
                    }
                    else
                    {
                        doc = _movableAssetService.GetInitialBalanceDocumentByLocation(docLocation);
                        if (doc != null && !IsInStore)
                        {
                            accountMasterDocId = _movableAssetService.GetRelatedAccountDocumentByDoc(doc.DocumentId);
                        }
                    }
                }
                else
                {
                    storeBill = _storeBills
                        .Where(x => x.StoreBillNo == stNo
                         && x.AcqType == StoreBillViewModel.AcqTyp && x.ArrivalDate.PersianDateTime().Year == billdate.Year && x.StuffType == stuffType)
                        .FirstOrDefault();

                    if (APPSettings.Default.EnabledNBookStore)
                    {
                        if (storeBill == null)
                        {
                            storeBill = _storeBillService.Queryable().Where(x => x.StoreBillNo == stNo
                             && x.AcqType == StoreBillViewModel.AcqTyp).AsNoTracking()
                            .AsEnumerable().Where(sb => sb.ArrivalDate.PersianDateTime().Year == billdate.Year).SingleOrDefault();

                            if (storeBill != null)
                            {
                                accountMasterId = _storeBillService.getRelatedAccountMasterId(storeBill.StoreBillId);
                            }
                        }
                        else
                        {
                            if (_relatedAccounts.ContainsKey(storeBill))
                            {
                                accountMaster = _relatedAccounts[storeBill];
                            }
                            else
                            {
                                accountMasterId = _storeBillService.getRelatedAccountMasterId(storeBill.StoreBillId);
                            }
                        }

                        doc = _movableAssetService.GetDocument(DocumentViewModel.Desc1);
                        if (doc != null && !IsInStore)
                        {
                            accountMasterDocId = _movableAssetService.GetRelatedAccountDocumentByDoc(doc.DocumentId);
                        }
                    }

                    doc = _documents.Where(x => x.Value.Desc1 == docNo)
                        .Select(x => x.Value).FirstOrDefault();
                }

                if (doc == null)
                {
                    var docEntity = DocumentViewModel.CurrentEntity;
                    doc = new Document
                    {
                        Desc1 = docNo,
                        Desc2 = docEntity.Desc2,
                        Desc3 = docEntity.Desc3,
                        Desc4 = docEntity.Desc4,
                        DocumentDate = docDate,
                        DocumentType = docType,
                        ObjectState = ObjectState.Added,
                        StoreId = StoreBillViewModel.SelectedStore.StoreId,
                        Transferee = transfree,
                    };

                    if(!IsInStore && _isOldSystem)
                    {
                        doc.Desc1 = $"{doc.Desc1}-{docLocation.OrganizId}";
                    }
                }

                if (!IsInStore && accountMasterDoc == null &&
                    accountMasterDocId == 0 && stuffType == StuffType.UnConsumption)
                {
                    accountMasterDoc = new AccountDocumentMaster
                    {
                        AccountDate = GlobalClass._Today,
                        AccountCover = "1",
                        ObjectState = ObjectState.Added,
                        EmployeeId = employee.EmployeeId,
                    };

                    if (doc.AccountDocument == null)
                    {
                        doc.AccountDocument = accountMasterDoc;
                    }
                }

                if (storeBill == null)
                {
                    stNo = checkMaxBillNo(stuffType);
                    var billEntity = StoreBillViewModel.CurrentEntity;
                    storeBill = new StoreBill
                    {
                        AcqType = billEntity.AcqType,
                        ArrivalDate = billdate.ToDateTime(),
                        ObjectState = ObjectState.Added,
                        ModifiedDate = GlobalClass._Today,
                        StoreBillNo = stNo,
                        StoreId = StoreBillViewModel.SelectedStore.StoreId,
                        Desc1 = billEntity.Desc1,
                        Desc2 = billEntity.Desc2,
                        Desc3 = billEntity.Desc3,
                        StuffType = stuffType,
                    };

                    if (StoreBillViewModel.SelectedSeller != null)
                    {
                        StoreBillViewModel.SelectedSeller.ObjectState = ObjectState.Modified;
                        storeBill.SellerId = StoreBillViewModel.SelectedSeller.SellerId;
                    }

                    accountMaster = new AccountDocumentMaster
                    {
                        AccountDate = GlobalClass._Today,
                        AccountCover = "1",
                        ObjectState = ObjectState.Added,
                        EmployeeId = employee.EmployeeId,
                    };
                    accountMaster.StoreBill = storeBill;
                }

                if (storeBill.StuffType != stuffType)
                {
                    _dialogService.ShowError("توجه", "قبض انبار با این شماره نوع اموالی که در اختیار دارد با مال انتخاب شده متفاوت است");
                    return;
                }

                if (stuffType == StuffType.UnConsumption)
                {
                    if (!IsInStore)
                    {
                        if (!_relatedAccountsDoc.ContainsKey(doc))
                        {
                            _relatedAccountsDoc.Add(doc, accountMasterDoc);
                        }

                        if (!_relatedAccountDocId.ContainsKey(doc))
                        {
                            _relatedAccountDocId.Add(doc, accountMasterDocId);
                        }
                    }

                    if (!_relatedAccounts.ContainsKey(storeBill))
                    {
                        _relatedAccounts.Add(storeBill, accountMaster);
                    }

                    if (!_relatedAccountId.ContainsKey(storeBill))
                    {
                        _relatedAccountId.Add(storeBill, accountMasterId);
                    }
                }

                if (storeBill.AcqType == StateOwnership.Trust)
                {
                    PersianDate maxDate;
                    if (!string.IsNullOrWhiteSpace(StoreBillViewModel.Desc2))
                    {
                        if (!PersianDate.TryParse(StoreBillViewModel.Desc2, out maxDate))
                        {
                            _dialogService.ShowAlert("توجه", "تاریخ برگست از امانی اشتباه است");
                            return;
                        }

                        if (maxDate > StoreBillViewModel._trustMaxReturnDate)
                        {
                            _dialogService.ShowAlert("توجه", "نهایت تاریخ برگشت از امانی اسفند سال بعد می باشد");
                            return;
                        }
                    }
                    docType = DocumentType.InternalStoreTrustDraft;
                }

                if (stuffType == StuffType.UnConsumption)
                {
                    var entity = UnConsumptionViewModel.CurrentEntity;
                    entity.Name = SelectedStuff.Name;
                    entity.KalaUid = SelectedStuff.StuffId;
                    entity.KalaNo = SelectedStuff.KalaNo;
                    entity.CurState = MAssetCurState.AtOperation;
                    entity.Num = 1;
                    entity.ObjectState = ObjectState.Added;
                    entity.InsertDate = GlobalClass._Today;
                    entity.ModeifiedDate = GlobalClass._Today;
                    entity.ISCompietion = CompietionState.NotReported;

                    foreach (int l in UnConsumptionViewModel.Labels)
                    {
                        var item = new UnConsumption(entity);
                        item.AssetId = l;

                        if (_isOldSystem)
                        {
                            if (_labels.ContainsKey(l))
                            {
                                if (_labels[l].Item3.HasValue)
                                {
                                    item.OldLabel = _labels[l].Item3;
                                    item.Floor = _labels[l].Item2;
                                    item.FloorType = _labels[l].Item1;
                                }
                            }
                            else
                            {
                                if (_oldLabel.HasValue)
                                {
                                    if (UnConsumptionViewModel._oldLabels.ContainsKey(item.Floor + "*" + item.FloorType))
                                    {
                                        if (!UnConsumptionViewModel._oldLabels[item.Floor + "*" + item.FloorType].Contains(_oldLabel.Value))
                                        {
                                            UnConsumptionViewModel._oldLabels[item.Floor + "*" + item.FloorType].Add(_oldLabel.Value);
                                            item.OldLabel = _oldLabel;
                                        }
                                        else
                                        {
                                            item.OldLabel = null;
                                            item.Floor = null;
                                            item.FloorType = null;
                                        }
                                    }
                                    else
                                    {
                                        UnConsumptionViewModel._oldLabels.Add(item.Floor + "*" + item.FloorType, new List<int> { _oldLabel.Value });
                                    }
                                    _oldLabel++;
                                }
                                else
                                {
                                    item.OldLabel = null;
                                    item.Floor = null;
                                    item.FloorType = null;
                                }
                            }
                        }

                        if (storeBill.AcqType == StateOwnership.Trust)
                        {
                            item.Label = null;
                            item.Locations.Add(new Location
                            {
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.Executive,
                                AccountDocumentType = AccountDocumentType.EscrowToTrust,
                            });

                            if (IsInStore)
                            {
                                if (doc.DocumentType == DocumentType.InitialBalance)
                                    item.Documetns.Add(doc);
                            }

                            docLocation.AccountDocumentType = AccountDocumentType.None;
                            item.Locations.Add(docLocation.Clone());
                        }
                        else
                        {
                            item.Locations.Add(new Location
                            {
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.Executive,
                                AccountDocumentType = AccountDocumentType.ExecutiveToReached
                            });

                            if (IsInStore)
                            {
                                if (ToLabel)
                                {
                                    item.Label = l;

                                    if (!_labels.ContainsKey(l))
                                    {
                                        _labels.Add(l, new Tuple<int?, string, int?>(item.FloorType, item.Floor, item.OldLabel));
                                    }
                                }

                                docLocation.AccountDocumentType = AccountDocumentType.ReachedToStock;
                                docLocation.StoreAddressId = StoreDesignSelected != null ? StoreDesignSelected.StoreDesignCurrent.StoreDesignId : 0;
                                if (doc.DocumentType == DocumentType.InitialBalance)
                                    item.Documetns.Add(doc);
                            }
                            else
                            {
                                item.Label = l;
                                if (!_labels.ContainsKey(l))
                                {
                                    _labels.Add(l, new Tuple<int?, string, int?>(item.FloorType, item.Floor, item.OldLabel));
                                }

                                item.Locations.Add(new Location
                                {
                                    StoreId = SelectedStore.StoreId,
                                    InsertDate = GlobalClass._Today,
                                    ObjectState = ObjectState.Added,
                                    Status = LocationStatus.StoreDeActive,
                                    ReturnDate = GlobalClass._Today,
                                    MovedRequestDate = GlobalClass._Today,
                                    AccountDocumentType = AccountDocumentType.ReachedToStock
                                });

                                docLocation.AccountDocumentType = AccountDocumentType.StockToUnits;
                                item.Documetns.Add(doc);
                            }
                            item.Locations.Add(docLocation.Clone());
                        }
                        item.StoreBill = storeBill;
                        _movableAssetCollection.Add(item);
                        this.SelectedAsset = item;
                    }
                }
                else if (stuffType == StuffType.OrderConsumption)
                {
                    if (fromUnconsuption)
                    {
                        var entity = UnConsumptionViewModel.CurrentEntity;
                        entity.Name = SelectedStuff.Name;
                        entity.KalaUid = SelectedStuff.StuffId;
                        entity.KalaNo = SelectedStuff.KalaNo;
                        entity.CurState = MAssetCurState.AtOperation;
                        entity.Num = 1;
                        entity.ObjectState = ObjectState.Added;
                        entity.InsertDate = GlobalClass._Today;
                        entity.ModeifiedDate = GlobalClass._Today;
                        entity.ISCompietion = CompietionState.NotReported;

                        storeBill.AccountDocument = null;
                        foreach (var l in UnConsumptionViewModel.Labels)
                        {
                            var incommodity = entity.ToInCommidity();
                            incommodity.AssetId = l;
                            if (_isOldSystem)
                            {
                                if (_labels.ContainsKey(l))
                                {
                                    if (_labels[l].Item3.HasValue)
                                    {
                                        incommodity.OldLabel = _labels[l].Item3;
                                        incommodity.Floor = _labels[l].Item2;
                                        incommodity.FloorType = _labels[l].Item1;
                                    }
                                    _labels.Remove(l);
                                }
                                else
                                {
                                    if (_oldLabel.HasValue)
                                    {
                                        if (UnConsumptionViewModel._oldLabels.ContainsKey(incommodity.Floor + "*" + incommodity.FloorType))
                                        {
                                            if (!UnConsumptionViewModel._oldLabels[incommodity.Floor + "*" + incommodity.FloorType].Contains(_oldLabel.Value))
                                            {
                                                UnConsumptionViewModel._oldLabels[incommodity.Floor + "*" + incommodity.FloorType].Add(_oldLabel.Value);
                                                incommodity.OldLabel = _oldLabel;
                                            }
                                            else
                                            {
                                                incommodity.OldLabel = null;
                                                incommodity.Floor = null;
                                                incommodity.FloorType = null;
                                            }
                                        }
                                        else
                                        {
                                            UnConsumptionViewModel._oldLabels.Add(incommodity.Floor + "*" + incommodity.FloorType, new List<int> { _oldLabel.Value });
                                        }
                                        _oldLabel++;
                                    }
                                    else
                                    {
                                        incommodity.OldLabel = null;
                                        incommodity.Floor = null;
                                        incommodity.FloorType = null;
                                    }
                                }
                            }

                            incommodity.Locations.Add(new Location
                            {
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.Executive,
                                AccountDocumentType = AccountDocumentType.ExecutiveToReached
                            });

                            if (IsInStore)
                            {
                                docLocation.AccountDocumentType = AccountDocumentType.ReachedToStock;
                                if (doc.DocumentType == DocumentType.InitialBalance)
                                    incommodity.Documetns.Add(doc);
                            }
                            else
                            {
                                incommodity.Locations.Add(new Location
                                {
                                    StoreId = SelectedStore.StoreId,
                                    InsertDate = GlobalClass._Today,
                                    ObjectState = ObjectState.Added,
                                    Status = LocationStatus.StoreDeActive,
                                    ReturnDate = GlobalClass._Today,
                                    MovedRequestDate = GlobalClass._Today,
                                    AccountDocumentType = AccountDocumentType.ReachedToStock
                                });

                                docLocation.AccountDocumentType = AccountDocumentType.StockToUnits;
                                incommodity.Documetns.Add(doc);
                            }
                            incommodity.Locations.Add(docLocation.Clone());
                            incommodity.StoreBill = storeBill;
                            _movableAssetCollection.Add(incommodity);
                            this.SelectedAsset = incommodity;
                        }
                    }
                    else
                    {
                        var entity = InCommodityViewModel.CurrentEntity;
                        for (int i = 1; i <= InCommodityViewModel.Num; i++)
                        {
                            var item = new InCommidity();
                            item.Cost = entity.Cost;
                            item.Desc1 = entity.Desc1;
                            item.Desc10 = entity.Desc10;
                            item.Desc11 = entity.Desc11;
                            item.Desc2 = entity.Desc2;
                            item.Desc3 = entity.Desc3;
                            item.Desc4 = entity.Desc4;
                            item.Desc5 = entity.Desc5;
                            item.Desc6 = entity.Desc6;
                            item.Desc7 = entity.Desc7;
                            item.Desc8 = entity.Desc8;
                            item.Desc9 = entity.Desc9;
                            item.Description = entity.Description;
                            item.Quality = "A";
                            item.Uid1 = entity.Uid1;
                            item.Uid2 = entity.Uid2;
                            item.Uid3 = entity.Uid3;
                            item.Uid4 = entity.Uid4;
                            item.UnitId = entity.UnitId;
                            item.Name = SelectedStuff.Name;
                            item.KalaUid = SelectedStuff.StuffId;
                            item.KalaNo = SelectedStuff.KalaNo;
                            item.CurState = MAssetCurState.AtOperation;
                            item.Num = 1;
                            item.ObjectState = ObjectState.Added;
                            item.InsertDate = GlobalClass._Today;
                            item.ModeifiedDate = GlobalClass._Today;
                            item.ISCompietion = CompietionState.NotReported;
                            item.ISConfirmed = false;
                            item.Label = null;
                            item.AssetId = i;
                            item.Locations.Add(new Location
                            {
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.Executive,
                                AccountDocumentType = AccountDocumentType.ExecutiveToReached
                            });

                            if (IsInStore)
                            {
                                docLocation.AccountDocumentType = AccountDocumentType.ReachedToStock;
                                if (doc.DocumentType == DocumentType.InitialBalance)
                                    item.Documetns.Add(doc);
                            }
                            else
                            {
                                item.Locations.Add(new Location
                                {
                                    StoreId = SelectedStore.StoreId,
                                    InsertDate = GlobalClass._Today,
                                    ObjectState = ObjectState.Added,
                                    Status = LocationStatus.StoreDeActive,
                                    ReturnDate = GlobalClass._Today,
                                    MovedRequestDate = GlobalClass._Today,
                                    AccountDocumentType = AccountDocumentType.ReachedToStock
                                });

                                docLocation.AccountDocumentType = AccountDocumentType.StockToUnits;
                                item.Documetns.Add(doc);
                            }
                            storeBill.AccountDocument = null;
                            item.Locations.Add(docLocation.Clone());
                            item.StoreBill = storeBill;
                            _movableAssetCollection.Add(item);
                            this.SelectedAsset = item;
                        }
                    }
                }
                else if (SelectedStuff.StuffType == StuffType.Installable)
                {
                    var entity = InstallableViewModel.CurrentEntity;
                    for (int i = InstallableViewModel.Labels.Min(); i <= InstallableViewModel.Labels.Max(); i++)
                    {
                        var item = new Installable();
                        item.AssetId = i;
                        item.Name = SelectedStuff.Name;
                        item.Description = entity.Description;
                        item.UnitId = entity.UnitId;
                        item.KalaNo = SelectedStuff.KalaNo;
                        item.KalaUid = SelectedStuff.StuffId;
                        item.CurState = MAssetCurState.AtOperation;
                        item.Num = 1;
                        item.Quality = "A";
                        item.Cost = entity.Cost;
                        item.Desc3 = entity.Desc3;
                        item.Desc2 = entity.Desc2;
                        item.Desc1 = entity.Desc1;
                        item.Label = entity.Label;
                        item.Desc4 = entity.Desc4;
                        item.Desc5 = entity.Desc5;
                        item.ObjectState = ObjectState.Added;
                        item.InsertDate = GlobalClass._Today;
                        item.Label = i;
                        _iLabels.Add(i);
                        item.ModeifiedDate = GlobalClass._Today;
                        item.ISCompietion = CompietionState.NotReported;
                        item.ISConfirmed = false;
                        item.StoreBill = storeBill;
                        item.Locations.Add(new Location
                        {
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.Executive,
                            AccountDocumentType = AccountDocumentType.ExecutiveToReached
                        });

                        if (IsInStore)
                        {
                            docLocation.AccountDocumentType = AccountDocumentType.ReachedToStock;
                            if (doc.DocumentType == DocumentType.InitialBalance)
                                item.Documetns.Add(doc);
                        }
                        else
                        {
                            item.Locations.Add(new Location
                            {
                                StoreId = SelectedStore.StoreId,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.StoreDeActive,
                                ReturnDate = GlobalClass._Today,
                                MovedRequestDate = GlobalClass._Today,
                                AccountDocumentType = AccountDocumentType.ReachedToStock
                            });

                            docLocation.AccountDocumentType = AccountDocumentType.StockToUnits;
                            item.Documetns.Add(doc);
                        }
                        storeBill.AccountDocument = null;
                        item.Locations.Add(docLocation.Clone());
                        item.StoreBill = storeBill;
                        _movableAssetCollection.Add(item);
                        this.SelectedAsset = item;
                    }
                }
                else if (SelectedStuff.StuffType == StuffType.Belonging)
                {
                    var entity = BelongingViewModel.CurrentEntity;
                    for (int i = BelongingViewModel.Labels.Min(); i <= BelongingViewModel.Labels.Max(); i++)
                    {
                        var item = new Belonging();
                        item.AssetId = i;
                        item.Name = SelectedStuff.Name;
                        item.Description = entity.Description;
                        item.UnitId = entity.UnitId;
                        item.KalaUid = SelectedStuff.StuffId;
                        item.KalaNo = SelectedStuff.KalaNo;
                        item.CurState = MAssetCurState.AtOperation;
                        item.Num = 1;
                        item.Quality = "A";
                        item.Cost = entity.Cost;
                        item.Desc3 = entity.Desc3;
                        item.Desc2 = entity.Desc2;
                        item.Desc1 = entity.Desc1;
                        item.Label = entity.Label;
                        item.Desc4 = entity.Desc4;
                        item.Desc5 = entity.Desc5;
                        item.ObjectState = ObjectState.Added;
                        item.InsertDate = GlobalClass._Today;
                        item.Label = i;
                        _bLabels.Add(i);
                        item.ModeifiedDate = GlobalClass._Today;
                        item.ISCompietion = CompietionState.NotReported;
                        item.ISConfirmed = false;
                        item.StoreBill = storeBill;

                        if (IsInStore)
                        {
                            if (doc.DocumentType == DocumentType.InitialBalance)
                                item.Documetns.Add(doc);
                        }
                        else
                        {
                            item.Documetns.Add(doc);
                        }

                        if (entity.ParentMAsset != null)
                        {
                            item.ParentMAsset = _belongingParent;
                            entity.ParentMAsset.Locations.ForEach(l =>
                            {
                                item.Locations.Add(new Location
                                {
                                    AccountDocumentType = l.AccountDocumentType,
                                    InsertDate = l.InsertDate,
                                    MovedRequestDate = l.MovedRequestDate,
                                    Status = l.Status,
                                    StrategyId = l.StrategyId,
                                    StoreId = l.StoreId,
                                    StoreAddressId = l.StoreAddressId,
                                    ObjectState = ObjectState.Added,
                                    ReturnDate = l.ReturnDate,
                                    OrganizId = l.OrganizId,
                                    PersonId = l.PersonId
                                });
                            });
                        }
                        else
                        {
                            if (IsInStore)
                            {
                                item.Locations.Add(new Location
                                {
                                    InsertDate = GlobalClass._Today,
                                    ObjectState = ObjectState.Added,
                                    Status = LocationStatus.Executive,
                                    AccountDocumentType = AccountDocumentType.ExecutiveToReached
                                });

                                docLocation.AccountDocumentType = AccountDocumentType.ReachedToStock;
                                item.Locations.Add(docLocation.Clone());
                            }
                        }
                        storeBill.AccountDocument = null;
                        item.StoreBill = storeBill;
                        _movableAssetCollection.Add(item);
                        this.SelectedAsset = item;
                    }
                }
                else
                {
                    _dialogService.ShowAlert("توجه", "مال انتخاب شده معتبر نمی باشد");
                    return;
                }

                if (!_documents.ContainsKey(docLocation))
                {
                    _documents.Add(docLocation, doc);
                }

                this.Num = 1;
            }
            else if (stuffType == StuffType.Consumable)
            {
                if (IsInStore)
                {
                    storeBill = _storeBills
                        .Where(x => x.StoreBillNo == stNo
                        && x.StuffType == StuffType.Consumable && x.AcqType == StoreBillViewModel.AcqTyp && x.ArrivalDate.PersianDateTime().Year == billdate.Year)
                       .FirstOrDefault();
                    if (APPSettings.Default.EnabledNBookStore)
                    {
                        if (storeBill == null)
                        {
                            storeBill = _storeBillService.Queryable().Where(x => x.StoreBillNo == stNo
                            && x.AcqType == StoreBillViewModel.AcqTyp).AsNoTracking()
                            .AsEnumerable().Where(sb => sb.ArrivalDate.PersianDateTime().Year == billdate.Year).SingleOrDefault();
                        }
                    }

                    if (storeBill == null)
                    {
                        var billEntity = StoreBillViewModel.CurrentEntity;
                        storeBill = new StoreBill
                        {
                            AcqType = billEntity.AcqType,
                            ArrivalDate = billdate.ToDateTime(),
                            ObjectState = ObjectState.Added,
                            ModifiedDate = GlobalClass._Today,
                            StoreBillNo = stNo,
                            StoreId = StoreBillViewModel.SelectedStore.StoreId,
                            Desc1 = billEntity.Desc1,
                            Desc2 = billEntity.Desc2,
                            Desc3 = billEntity.Desc3,
                            StuffType = StuffType.Consumable,
                        };

                        if (StoreBillViewModel.SelectedSeller != null)
                        {
                            StoreBillViewModel.SelectedSeller.ObjectState = ObjectState.Modified;
                            storeBill.SellerId = StoreBillViewModel.SelectedSeller.SellerId;
                        }
                    }

                    var consumEntity = this.CommodityViewModel.CurrentEntity.Clone();
                    consumEntity.ObjectState = ObjectState.Added;
                    consumEntity.StoreAddress = StoreDesignSelected.StoreDesignCurrent.StoreDesignId.ToString();
                    consumEntity.StoreBill = storeBill;
                    consumEntity.Name = SelectedStuff.Name;
                    consumEntity.KalaUid = SelectedStuff.StuffId;
                    consumEntity.KalaNo = SelectedStuff.KalaNo;
                    consumEntity.InsertDate = GlobalClass._Today;
                    consumEntity.ModeifiedDate = GlobalClass._Today;
                    consumEntity.Quality = "A";
                    _commodityCollection.Add(consumEntity);
                    this.SelectedCommodity = consumEntity;
                }
            }

            if (!_storeBills.Contains(storeBill))
            {
                _storeBills.Add(storeBill);
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private Tuple<string, string> GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";
            string _coding = "";
            if (item.ParentNode != null)
            {
                var getItem = this.GetHirecharyNode(item.ParentNode);
                _nodeName += getItem.Item1;
                _coding += getItem.Item2;
                _nodeName += "**";
            }
            _coding += item.BuidldingDesignId.ToString();
            _nodeName += item.Name;

            return new Tuple<string, string>(_nodeName, _coding);
        }

        private void EditMAsset(object parameter)
        {
            if (IsCommodity)
            {
                var commodity = parameter as Commodity;
                if (commodity == null) return;
                Mouse.SetCursor(Cursors.Wait);
                StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
                this.SelectedCommodity = commodity;
                var cloneAsset = commodity.Clone();
                var viewModel = new CommodityDetailsViewModel(_container, -1, cloneAsset, true, false);
                var window = _navigationService.ShowCommodityDetailsWindow(viewModel);
                if (window.DialogResult == true)
                {
                    int index = _commodityCollection.IndexOf(commodity);
                    if (cloneAsset.AssetId == -1001)
                    {
                        this.deleteMAssets(commodity);
                    }
                    else
                    {
                        _commodityCollection.RemoveAt(index);
                        _commodityCollection.Insert(index, viewModel.CurrentAsset);
                    }
                }
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
                Mouse.SetCursor(Cursors.Arrow);
            }
            else
            {
                var mAsset = parameter as MovableAsset;
                decimal oldcost = mAsset.Cost;
                if (mAsset == null) return;
                this.SelectedAsset = mAsset;
                Mouse.SetCursor(Cursors.Wait);
                StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
                var cloneAsset = mAsset.Clone();
                var viewModel = new MovableAssetDetailsViewModel(_container, -1, cloneAsset, true, false);
                if (cloneAsset is UnConsumption)
                {
                    viewModel.UnConsumptionViewModel._oldLabels = this.UnConsumptionViewModel._oldLabels;
                }
                var window = _navigationService.ShowMAssetDetailsWindow(viewModel);
                int index = _movableAssetCollection.IndexOf(mAsset);
                if (window.DialogResult == true)
                {
                    if (cloneAsset.AssetId == -1001)
                    {
                        this.deleteMAssets(mAsset);
                    }
                    else
                    {
                        if (cloneAsset.Documetns.Any(x => x.DocumentType == DocumentType.InitialBalance))
                        {
                            if (cloneAsset is UnConsumption)
                            {
                                if (cloneAsset.Cost < APPSettings.Default.MStuffPrice)
                                {
                                    var inComodity = ((UnConsumption)cloneAsset).ToInCommidity();
                                    mAsset.Locations.ForEach(l =>
                                    {
                                        inComodity.Locations.Add(l);
                                    });
                                    var inCommodityStoreBill = _storeBills.Where(x => x.StuffType == StuffType.OrderConsumption
                                        && x.StoreBillNo == "2" && x.ArrivalDate.PersianDateTime().Year == mAsset.StoreBill.ArrivalDate.PersianDateTime().Year).AsEnumerable().FirstOrDefault();

                                    if (inCommodityStoreBill == null)
                                    {
                                        inCommodityStoreBill = _storeBillService.Queryable().Where(x => x.StoreBillNo == "2"
                                        && x.StuffType == StuffType.OrderConsumption)
                                        .AsEnumerable().Where(sb => sb.ArrivalDate.PersianDateTime().Year == mAsset.StoreBill.ArrivalDate.PersianDateTime().Year).SingleOrDefault();
                                        if (inCommodityStoreBill == null)
                                        {
                                            var billEntity = StoreBillViewModel.CurrentEntity;
                                            DateTime billdate = StoreBillViewModel.ArrivalDate.ToDateTime();
                                            billEntity.StoreBillNo = "2";
                                            inCommodityStoreBill = new StoreBill
                                            {
                                                AcqType = billEntity.AcqType,
                                                ArrivalDate = billdate,
                                                ObjectState = ObjectState.Added,
                                                ModifiedDate = GlobalClass._Today,
                                                StoreBillNo = billEntity.StoreBillNo,
                                                StoreId = SelectedStore.StoreId,
                                                Desc1 = billEntity.Desc1,
                                                Desc2 = billEntity.Desc2,
                                                Desc3 = billEntity.Desc3,
                                                StuffType = StuffType.OrderConsumption,
                                                SellerId = billEntity.SellerId,
                                            };
                                        }
                                        _storeBills.Add(inCommodityStoreBill);
                                    }
                                    inComodity.StoreBill = inCommodityStoreBill;
                                    mAsset.Documetns.ForEach(d =>
                                    {
                                        inComodity.Documetns.Add(d);
                                    });
                                    if (cloneAsset.Label.HasValue)
                                    {
                                        this.queryGraterLabels(cloneAsset.Label.Value);
                                    }
                                    _movableAssetCollection.RemoveAt(index);
                                    _movableAssetCollection.Insert(index, inComodity);
                                    this.generateLabelAsync();
                                }
                                else
                                {
                                    _movableAssetCollection.RemoveAt(index);
                                    _movableAssetCollection.Insert(index, viewModel.CurrentAsset);
                                }
                            }
                            else if (cloneAsset is InCommidity)
                            {
                                if (cloneAsset.Cost >= APPSettings.Default.MStuffPrice)
                                {
                                    AccountDocumentMaster accountMaster = null;
                                    int accountMasterId = 0;

                                    var unconsumption = ((InCommidity)cloneAsset).ToUnconsumpton();

                                    if (_labels.Count > 0)
                                    {
                                        int label = _labels.Max(x => x.Key) + 1;
                                        unconsumption.Label = label;
                                        _labels.Add(label, new Tuple<int?, string, int?>(unconsumption.FloorType, unconsumption.Floor, unconsumption.OldLabel));
                                    }
                                    else
                                    {
                                        unconsumption.Label = 1;
                                        _labels.Add(1, new Tuple<int?, string, int?>(unconsumption.FloorType, unconsumption.Floor, unconsumption.OldLabel));
                                    }

                                    mAsset.Locations.ForEach(l =>
                                    {
                                        unconsumption.Locations.Add(l);
                                    });
                                    var unconsumptionStoreBill = _storeBills.Where(x => x.StuffType == StuffType.UnConsumption
                                        && x.StoreBillNo == "1" && x.ArrivalDate.PersianDateTime().Year == mAsset.StoreBill.ArrivalDate.PersianDateTime().Year).FirstOrDefault();

                                    if (unconsumptionStoreBill == null)
                                    {
                                        unconsumptionStoreBill = _storeBillService.Queryable().Where(x => x.StoreBillNo == "1"
                                        && x.StuffType == StuffType.UnConsumption)
                                      .AsEnumerable().Where(sb => sb.ArrivalDate.PersianDateTime().Year == mAsset.StoreBill.ArrivalDate.PersianDateTime().Year).SingleOrDefault();

                                        if (unconsumptionStoreBill == null)
                                        {
                                            var employee = _employeeService.Queryable().First();
                                            var billEntity = StoreBillViewModel.CurrentEntity;
                                            DateTime billdate = StoreBillViewModel.ArrivalDate.ToDateTime();
                                            billEntity.StoreBillNo = "1";
                                            unconsumptionStoreBill = new StoreBill
                                            {
                                                AcqType = billEntity.AcqType,
                                                ArrivalDate = billdate,
                                                ObjectState = ObjectState.Added,
                                                ModifiedDate = GlobalClass._Today,
                                                StoreBillNo = "1",
                                                StoreId = SelectedStore.StoreId,
                                                Desc1 = billEntity.Desc1,
                                                Desc2 = billEntity.Desc2,
                                                Desc3 = billEntity.Desc3,
                                                StuffType = StuffType.UnConsumption,
                                                SellerId = billEntity.SellerId,
                                            };

                                            accountMaster = new AccountDocumentMaster
                                            {
                                                AccountDate = GlobalClass._Today,
                                                AccountCover = "1",
                                                ObjectState = ObjectState.Added,
                                                EmployeeId = employee.EmployeeId,
                                            };
                                            accountMaster.StoreBill = unconsumptionStoreBill;
                                        }
                                        else
                                        {
                                            accountMasterId = _storeBillService.getRelatedAccountMasterId(unconsumptionStoreBill.StoreBillId);
                                        }

                                        _storeBills.Add(unconsumptionStoreBill);
                                    }
                                    else
                                    {
                                        if (_relatedAccounts.ContainsKey(unconsumptionStoreBill))
                                        {
                                            accountMaster = _relatedAccounts[unconsumptionStoreBill];
                                        }
                                        else
                                        {
                                            accountMasterId = _storeBillService.getRelatedAccountMasterId(unconsumptionStoreBill.StoreBillId);
                                        }
                                    }

                                    if (!_relatedAccounts.ContainsKey(unconsumptionStoreBill))
                                    {
                                        _relatedAccounts.Add(unconsumptionStoreBill, accountMaster);
                                    }

                                    if (!_relatedAccountId.ContainsKey(unconsumptionStoreBill))
                                    {
                                        _relatedAccountId.Add(unconsumptionStoreBill, accountMasterId);
                                    }

                                    unconsumption.StoreBill = unconsumptionStoreBill;
                                    mAsset.Documetns.ForEach(d =>
                                    {
                                        unconsumption.Documetns.Add(d);
                                    });
                                    _movableAssetCollection.RemoveAt(index);
                                    _movableAssetCollection.Insert(index, unconsumption);
                                    this.generateLabelAsync();
                                }
                                else
                                {
                                    _movableAssetCollection.RemoveAt(index);
                                    _movableAssetCollection.Insert(index, viewModel.CurrentAsset);
                                }
                            }
                            else
                            {
                                _movableAssetCollection.RemoveAt(index);
                                _movableAssetCollection.Insert(index, viewModel.CurrentAsset);
                            }
                        }
                        else
                        {
                            _movableAssetCollection.RemoveAt(index);
                            _movableAssetCollection.Insert(index, viewModel.CurrentAsset);
                        }
                    }
                }
                this.SelectedAsset = mAsset;
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private async void saveMAsset()
        {
            if (!ConnectionHelper.CheckConnection())
            {
                _dialogService.ShowAlert("خطای اتصال", ErrorMessages.Default.NoConnection);
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                var emp = _employeeService.Queryable().First();
                string msg = "";
                if (IsCommodity)
                {
                    if (_commodityCollection.Count <= 0)
                    {
                        _dialogService.ShowError("توجه", "هیچ سطری در لیست موجود نیست");
                        return;
                    }

                    _commodityService.InsertGraphRange(_commodityCollection);
                }
                else
                {
                    if (_movableAssetCollection.Count <= 0)
                    {
                        _dialogService.ShowError("توجه", "هیچ سطری در لیست موجود نیست");
                        return;
                    }

                    _movableAssetCollection.OfType<UnConsumption>().ForEach(item =>
                    {
                        this.setAccountDocDetails(item, emp, _relatedAccounts[item.StoreBill], _relatedAccountId[item.StoreBill]);
                        if (!IsInStore)
                        {
                            var doc = item.Documetns.First();
                            this.setAccountDocDetailsForDoc(item, emp, _relatedAccountsDoc[doc], _relatedAccountDocId[doc]);
                        }
                    });

                    _movableAssetService.InsertGraphRange(_movableAssetCollection);
                }
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    if (IsCommodity)
                    {
                        msg = "ثبت مال مصرفی موجودی اولیه در انبار" + " " + _movableAssetCollection.Count;
                        _commodityCollection.Clear();
                    }
                    else
                    {
                        if (!_isOldSystem)
                        {
                            msg = "ثبت مال از دفاتر به تعداد" + " " + _movableAssetCollection.Count;
                        }
                        else
                        {
                            msg = "ثبت مال از فهرست موجودی اولیه به تعداد" + " " + _movableAssetCollection.Count;
                        }

                        _movableAssetCollection.Clear();
                        _relatedAccountId.Clear();
                        _relatedAccountDocId.Clear();
                        _relatedAccounts.Clear();
                        _relatedAccountsDoc.Clear();
                    }

                    UserLog.UniqueInstance.AddLog(new EventLog
                    {
                        EntryDate = GlobalClass._Today,
                        Key = UserLog.UniqueInstance.LogedUser.FullName,
                        Message = msg,
                        ObjectState = ObjectState.Added,
                        Type = EventType.Update,
                        UserId = UserLog.UniqueInstance.LogedUser.UserId
                    });
                    SelectedStuff = null;
                    if (!_isOldSystem)
                    {
                        this.StoreBillViewModel.StoreBillNo = await initBillNo(_lastType);
                        this.DocumentViewModel.Desc1 = await initDraftNo();
                    }
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("Error", ex.InnerException.InnerException.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void setAccountDocDetails(UnConsumption assets, Employee emp, AccountDocumentMaster accountMaster, int accountMasterId)
        {
            if (assets != null || emp != null || accountMaster != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var ma = assets;
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                ma.Locations.ForEach(l =>
                {
                    string desc = "نامشخص";
                    string code = "0";
                    if (l.AccountDocumentType == AccountDocumentType.EscrowToTrust)
                    {
                        var organ = StoreBillViewModel.Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                        if (organ != null)
                        {
                            code = organ.BudgetNo.ToString();
                            desc = organ.Name;
                        }
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.Escrow),
                           "Creditor", desc, code, ma.Cost, ma));

                        currentAccountCodings.Add(new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(
                            accountCodings.First(x => x.CertainAccountType == CertainAccountsType.TrustOrganizationReciver),
                            "Debtor", emp.Name, emp.BudgetNo.ToString(), ma.Cost, ma));

                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ExecutiveToReached)
                    {

                        if (ma.Label.HasValue)
                        {
                            code = ma.Label.ToString();
                            desc = "برچسب" + code;
                        }
                        currentAccountCodings.Add(
                            new Tuple<AccountDocumentCoding, string, string, string, decimal,
                            UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ExecutiveSequenceLabel),
                            "Creditor", desc, code, ma.Cost, ma));

                        if (_isOldSystem)
                        {
                            desc = "خریداری";
                            if (ma.StoreBill.SellerId.HasValue)
                            {
                                var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                                desc += "**" + seller.Name;
                                code = seller.Coding;
                            }
                            currentAccountCodings.Add(
                             new Tuple<AccountDocumentCoding, string, string, string, decimal,
                             UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                             "Debtor", desc, code, ma.Cost, ma));
                        }
                        else
                        {
                            if (ma.StoreBill.AcqType == StateOwnership.Purchase)
                            {
                                desc = "خریداری";
                                if (ma.StoreBill.SellerId.HasValue)
                                {
                                    var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                                    desc += "**" + seller.Name;
                                    code = seller.Coding;
                                }
                                currentAccountCodings.Add(
                                    new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                    UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                                    "Debtor", desc, code, ma.Cost, ma));
                            }
                            else if (ma.StoreBill.AcqType == StateOwnership.Owned)
                            {
                                desc = "تملیکی**" + ma.StoreBill.Desc1;
                                currentAccountCodings.Add(
                                    new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                    UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetOther),
                                    "Debtor", desc, code, ma.Cost, ma));
                            }
                            else if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived)
                            {
                                var organ = StoreBillViewModel.Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                                if (organ != null)
                                {
                                    desc = "انتفالی**" + organ.Name;
                                    code = organ.BudgetNo.ToString();
                                }

                                currentAccountCodings.Add(
                                   new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                   UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetTransfer),
                                   "Debtor", desc, code, ma.Cost, ma));
                            }
                            else if (ma.StoreBill.AcqType == StateOwnership.Donation)
                            {
                                desc = "اهدایی**" + ma.StoreBill.Desc1;
                                currentAccountCodings.Add(
                                    new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                    UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetDenotion),
                                    "Debtor", desc, code, ma.Cost, ma));
                            }
                        }

                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ReachedToStock)
                    {
                        if (_isOldSystem)
                        {
                            desc = "خریداری";
                            if (ma.StoreBill.SellerId.HasValue)
                            {
                                var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                                desc += "**" + seller.Name;
                                code = seller.Coding;
                            }
                            currentAccountCodings.Add(
                                new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                                "Creditor", desc, code, ma.Cost, ma));
                        }
                        else
                        {
                            if (ma.StoreBill.AcqType == StateOwnership.Purchase)
                            {
                                desc = "خریداری";
                                if (ma.StoreBill.SellerId.HasValue)
                                {
                                    var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                                    desc += "**" + seller.Name;
                                    code = seller.Coding;
                                }
                                currentAccountCodings.Add(
                                   new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                                   "Creditor", desc, code, ma.Cost, ma));
                            }
                            else if (ma.StoreBill.AcqType == StateOwnership.Owned)
                            {
                                desc = "اهدایی**" + ma.StoreBill.Desc1;
                                currentAccountCodings.Add(
                                   new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetOther),
                                   "Creditor", desc, code, ma.Cost, ma));
                            }
                            else if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived)
                            {
                                var organ = StoreBillViewModel.Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                                if (organ != null)
                                {
                                    desc = "انتقالی**" + organ.Name;
                                    code = organ.BudgetNo.ToString();
                                }

                                currentAccountCodings.Add(
                                  new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetTransfer),
                                  "Creditor", desc, code, ma.Cost, ma));
                            }
                            else if (ma.StoreBill.AcqType == StateOwnership.Donation)
                            {
                                desc = "تملیکی**" + ma.StoreBill.Desc1;
                                currentAccountCodings.Add(
                                  new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetDenotion),
                                  "Creditor", desc, code, ma.Cost, ma));
                            }
                        }
                        code = ma.KalaUid.ToString();
                        desc = ma.Name + "**" + ma.Cost.ToString("N0");
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                           "Debtor", desc, code, ma.Cost, ma));
                    }
                });

                string acNo = "";
                decimal deboter = 0;
                decimal creditor = 0;
                string vdesc = "";

                currentAccountCodings.ForEach(adc =>
                {
                    acNo = parentcode.AccountCode + "-" + adc.Item1.Parent.AccountCode + "-" + adc.Item1.AccountCode + "-" + GlobalClass.CheckAccountCode(adc.Item4);

                    if (string.Equals(adc.Item2, "Creditor"))
                    {
                        creditor = adc.Item5;
                        deboter = 0;
                        vdesc = "<<" + adc.Item3 + ">>";
                    }
                    else if (string.Equals(adc.Item2, "Debtor"))
                    {
                        deboter = adc.Item5;
                        creditor = 0;
                        vdesc = "((" + adc.Item3 + "))";
                    }

                    var newDetails = new AccountDocumentDetails
                    {
                        Debtor = deboter,
                        Creditor = creditor,
                        Description = vdesc,
                        AccountNo = acNo,
                        ObjectState = ObjectState.Added,
                    };

                    if (accountMaster != null)
                    {
                        newDetails.AccountDocumentMaster = accountMaster;
                    }
                    else
                    {
                        newDetails.MasterId = accountMasterId;
                    }

                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
            }
        }

        private void setAccountDocDetailsForDoc(UnConsumption assets, Employee emp, AccountDocumentMaster accountMaster, int accountMasterId)
        {
            if (assets != null || emp != null || accountMaster != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var ma = assets;
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                ma.Locations.ForEach(l =>
                {
                    string desc = "نامشخص";
                    string code = "0";

                    if (l.AccountDocumentType == AccountDocumentType.StockToUnits)
                    {
                        code = ma.KalaUid.ToString();
                        desc = ma.Name + "**" + ma.Cost.ToString("N0");
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                           "Creditor", desc, code, ma.Cost, ma));

                        var organization = _employeeService.GetDesignById(l.OrganizId, 1);
                        if (organization != null)
                        {
                            var getItem = this.GetHirecharyNode(organization);
                            desc = getItem.Item1;
                            code = getItem.Item2;
                        }
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.UnitsDeliviry),
                           "Debtor", desc, code, ma.Cost, ma));
                    }
                });

                string acNo = "";
                decimal deboter = 0;
                decimal creditor = 0;
                string vdesc = "";

                currentAccountCodings.ForEach(adc =>
                {
                    acNo = parentcode.AccountCode + "-" + adc.Item1.Parent.AccountCode + "-" + adc.Item1.AccountCode + "-" + GlobalClass.CheckAccountCode(adc.Item4);

                    if (string.Equals(adc.Item2, "Creditor"))
                    {
                        creditor = adc.Item5;
                        deboter = 0;
                        vdesc = "<<" + adc.Item3 + ">>";
                    }
                    else if (string.Equals(adc.Item2, "Debtor"))
                    {
                        deboter = adc.Item5;
                        creditor = 0;
                        vdesc = "((" + adc.Item3 + "))";
                    }

                    var newDetails = new AccountDocumentDetails
                    {
                        Debtor = deboter,
                        Creditor = creditor,
                        Description = vdesc,
                        AccountNo = acNo,
                        ObjectState = ObjectState.Added,
                    };

                    if (accountMaster != null)
                    {
                        newDetails.AccountDocumentMaster = accountMaster;
                    }
                    else
                    {
                        newDetails.MasterId = accountMasterId;
                    }

                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
            }
        }

        private void deleteMAssets(object parameter)
        {
            if (!IsCommodity)
            {
                var mAsset = parameter as MovableAsset;
                if (mAsset == null) return;

                if (_isOldSystem)
                {
                    if (mAsset.OldLabel.HasValue && mAsset is UnConsumption)
                    {
                        string key = mAsset.Floor + "*" + mAsset.FloorType;
                        UnConsumptionViewModel._oldLabels[key].Remove(mAsset.OldLabel.Value);
                        if (UnConsumptionViewModel._oldLabels[key].Count == 0)
                        {
                            UnConsumptionViewModel._oldLabels.Remove(key);
                        }
                    }
                }

                if (mAsset.Label.HasValue)
                {
                    if (mAsset is UnConsumption)
                    {
                        if (_movableAssetCollection.OfType<UnConsumption>().Where(x => x.Label.HasValue).Count() > 0)
                        {
                            this.queryGraterLabels(mAsset.Label.Value);
                        }
                        else
                        {
                            _labels.Remove(mAsset.Label.Value);
                        }
                    }
                    else if (mAsset is Installable)
                    {
                        if (_movableAssetCollection.OfType<Installable>().Where(x => x.Label.HasValue).Count() > 0)
                        {
                            this.queryGraterInstallableLabels(mAsset.Label.Value);
                        }
                        else
                        {
                            _iLabels.Remove(mAsset.Label.Value);
                        }
                    }
                    else if (mAsset is Belonging)
                    {
                        if (_movableAssetCollection.OfType<Belonging>().Where(x => x.Label.HasValue).Count() > 0)
                        {
                            this.queryGraterBelongingLabels(mAsset.Label.Value);
                        }
                        else
                        {
                            _bLabels.Remove(mAsset.Label.Value);
                        }
                    }

                    this.generateLabelAsync();
                }

                _movableAssetCollection.Remove(mAsset);
            }
            else
            {
                var commodity = parameter as Commodity;
                if (commodity == null) return;
                _commodityCollection.Remove(commodity);
            }
        }

        private void queryGraterLabels(int label)
        {
            int currentLabel = label;
            int bigestLabel = _movableAssetCollection.Where(x => x.Label.HasValue).Max(x => x.Label.Value);
            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _movableAssetCollection.OfType<UnConsumption>().SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        int index = _movableAssetCollection.IndexOf(item);
                        item.Label = i;
                        _movableAssetCollection.RemoveAt(index);
                        _movableAssetCollection.Insert(index, item);
                    }
                }
                _labels.Remove(bigestLabel);
            }
            else
            {
                _labels.Remove(currentLabel);
            }
        }

        private void queryGraterBelongingLabels(int label)
        {
            int currentLabel = label;
            int bigestLabel = label;
            var mAssetsCurrent = _movableAssetCollection.OfType<Belonging>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _movableAssetCollection.OfType<Belonging>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        int index = _movableAssetCollection.IndexOf(item);
                        item.Label = i;
                        _movableAssetCollection.RemoveAt(index);
                        _movableAssetCollection.Insert(index, item);
                    }
                }
                _bLabels.Remove(bigestLabel);
            }
            else
            {
                _bLabels.Remove(currentLabel);
            }
        }

        private void queryGraterInstallableLabels(int label)
        {
            int currentLabel = label;
            int bigestLabel = label;
            var mAssetsCurrent = _movableAssetCollection.OfType<Installable>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _movableAssetCollection.OfType<Installable>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        int index = _movableAssetCollection.IndexOf(item);
                        item.Label = i;
                        _movableAssetCollection.RemoveAt(index);
                        _movableAssetCollection.Insert(index, item);
                    }
                }
                _iLabels.Remove(bigestLabel);
            }
            else
            {
                _iLabels.Remove(currentLabel);
            }
        }

        private void showDocWindow(bool isStoreBill)
        {
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            DocumentShowViewModel viewModel = null;
            if (isStoreBill)
            {
                DateTime billDate = StoreBillViewModel.ArrivalDate.ToDateTime();
                var storeBill = _storeBillService.Queryable()
                      .Where(x => x.StoreBillNo == StoreBillViewModel.StoreBillNo && x.ArrivalDate.Year == billDate.Year)
                      .SingleOrDefault();
                viewModel = new DocumentShowViewModel(_container, true);
            }
            else
            {
                var doc = _movableAssetService
                     .GetDocument(DocumentViewModel.Desc1);
                viewModel = new DocumentShowViewModel(_container, false);
            }
            _navigationService.ShowDocumentShowWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showMAssetList()
        {
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            List<MovableAssetModel> mAssets = null;
            int descTyp = 1003;
            if (_isOldSystem)
            {
                mAssets = _movableAssetService.Queryable()
                    .Where(x => x.Documetns.Any(d => d.DocumentType == DocumentType.InitialBalance))
                    .AsEnumerable().Select(x => new MovableAssetModel
                    {
                        AssetId = x.AssetId,
                        CurState = x.CurState,
                        InsertDate = x.InsertDate,
                        Name = x.Name,
                        Num = x.Num,
                        MAssetType = x.ToString("T", null),
                        UnitId = x.UnitId,
                        Label = x.Label,
                        kalaUid = x.KalaUid,
                    }).ToList();
                descTyp = 1003;
            }
            else
            {
                mAssets = _movableAssetService.Queryable().Where(x => x.Documetns.All(d => d.DocumentType != DocumentType.InitialBalance))
                    .AsEnumerable().Select(x => new MovableAssetModel
                    {
                        AssetId = x.AssetId,
                        CurState = x.CurState,
                        InsertDate = x.InsertDate,
                        Name = x.Name,
                        Num = x.Num,
                        MAssetType = x.ToString("T", null),
                        UnitId = x.UnitId,
                        Label = x.Label,
                        kalaUid = x.KalaUid,
                    }).ToList();
                descTyp = 1004;
            }
            var viewModel = new MAssetListViewModel(_container, descTyp, 0);
            viewModel.AssetList = mAssets;
            _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private Task getOldLabelsByFloorAsync()
        {
            var ts = new Task(() =>
            {
                UnConsumptionViewModel._oldLabels = new Dictionary<string, List<int>>();
                _movableAssetService.Queryable()
                      .Where(x => x.OldLabel.HasValue).ToLookup(x => x.Floor + "*" + x.FloorType, p => p.OldLabel.Value).ForEach(x =>
                      {
                          UnConsumptionViewModel._oldLabels.Add(x.Key, x.ToList());
                      });
            });
            ts.Start();
            return ts;
        }

        private void showOldLabelEditWindow()
        {
            if (this.Num <= 0)
            {
                _dialogService.ShowAlert("توجه", "تعداد وارد نشده است");
                return;
            }

            if (this.SelectedStuff == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ مالی انتخاب نشده است");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var oldDic = new Dictionary<string, List<int>>();
            if (UnConsumptionViewModel._oldLabels != null)
            {
                UnConsumptionViewModel._oldLabels.ForEach(d =>
                {
                    oldDic.Add(d.Key, new List<int>());
                    oldDic[d.Key].AddRange(d.Value);
                });
            }

            var items = new List<OldLabelDetailsViewModel>();
            foreach (int i in UnConsumptionViewModel.Labels)
            {
                var oldEdit = new OldLabelDetailsViewModel(new OldLabelModel
                {
                    Floor = UnConsumptionViewModel.Floor,
                    FloorType = UnConsumptionViewModel.FloorType ?? 707,
                    Label = i,
                    OldLabel = 0,
                    Floors = UnConsumptionViewModel.Floors
                });
                oldEdit._oldLabels = oldDic;
                items.Add(oldEdit);
            }
            var viewModel = new OldLabelEditViewModel(_container, items, SelectedStuff.StuffId) { Num = this.Num, StuffName = SelectedStuff.Name };

            var window = _navigationService.ShowOldLabelEditWindow(viewModel);

            if (window.DialogResult == true)
            {
                viewModel.Collection.ForEach(c =>
                {
                    if (!_labels.ContainsKey(c.Label))
                    {
                        if (c.OldLabel > 0 && !string.IsNullOrEmpty(c.Floor))
                        {
                            _labels.Add(c.Label, new Tuple<int?, string, int?>(c.FloorType, c.Floor, c.OldLabel));
                            oldDic.ForEach(dic =>
                            {
                                if (UnConsumptionViewModel._oldLabels.ContainsKey(dic.Key))
                                {
                                    dic.Value.ForEach(dol =>
                                    {
                                        if (!UnConsumptionViewModel._oldLabels[dic.Key].Contains(dol))
                                        {
                                            UnConsumptionViewModel._oldLabels[dic.Key].Add(dol);
                                        }
                                    });
                                }
                                else
                                {
                                    UnConsumptionViewModel._oldLabels.Add(dic.Key, dic.Value);
                                }
                            });
                        }
                    }
                });
            }

            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        async void PerformSearch()
        {
            await this.VerifyMatchingOrganizEnumeratorAsync();
            await this.VerifyMatchingStrategyEnumeratorAsync();
        }

        Task VerifyMatchingOrganizEnumeratorAsync()
        {
            var ts = new Task(() =>
            {
                foreach (var k in _organizCollection)
                {
                    var matches = this.FindMatches(SelectedRequest.OrganizId, k);
                    if (matches.Count() > 0)
                    {
                        DispatchService.Invoke(() =>
                        {
                            OrganizSelected = matches.First();
                            OrganizSelected.IsSelected = true;
                        });
                        break;
                    }
                }
            });

            try
            {
                ts.Start();
                return ts;

            }
            catch (NullReferenceException) { return null; }
            catch (Exception) { throw; }
        }

        Task VerifyMatchingStrategyEnumeratorAsync()
        {
            var ts = new Task(() =>
            {
                foreach (var k in _strategyCollection)
                {
                    var matches = this.FindMatches(SelectedRequest.StrategyId, k);
                    if (matches.Count() > 0)
                    {
                        DispatchService.Invoke(() =>
                        {
                            StrategySelected = matches.First();
                            StrategySelected.IsSelected = true;
                        });
                        break;
                    }
                }
            });

            try
            {
                ts.Start();
                return ts;
            }
            catch (NullReferenceException) { return null; }
            catch (Exception) { throw; }
        }

        IEnumerable<EmployeeDesignTreeViewModel> FindMatches(int id, EmployeeDesignTreeViewModel buildingdesign)
        {
            if (buildingdesign.BuildingDesignCurrent.BuidldingDesignId == id)
                yield return buildingdesign;

            foreach (EmployeeDesignTreeViewModel child in buildingdesign.Children)
                foreach (EmployeeDesignTreeViewModel match in this.FindMatches(id, child))
                    yield return match;
        }

        private void showParentAssetForBelongingWindow()
        {
            ParentAssetForBelongingViewModel viewModel;
            Mouse.SetCursor(Cursors.Wait);
            if (!IsInStore)
            {
                if (SelectedPerson == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ پرسنلی انتخاب نشده است");
                    return;
                }
                viewModel = new ParentAssetForBelongingViewModel(_container, SelectedPerson, null, null);
            }
            else
            {
                if (SelectedStore == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ انباری انتخاب نشده است");
                    return;
                }
                viewModel = new ParentAssetForBelongingViewModel(_container, null, SelectedStore, null);
            }
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var window = _navigationService.ShowParentAssetForBelongingWindow(viewModel);
            if (window.DialogResult == true)
            {
                if (BelongingViewModel != null)
                {
                    BelongingViewModel.Parent = viewModel.Selected;
                    _belongingParent = _movableAssetService.Find(viewModel.Selected.AssetId) as UnConsumption;
                }
            }
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        #endregion

        #region commands

        public ICommand AddListCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand StoreDraftCommand { get; private set; }
        public ICommand StoreBillCommand { get; private set; }
        public ICommand ShowListCommand { get; private set; }
        public ICommand OldLabelEditCommand { get; private set; }
        public ICommand ParentAssetForBelongingCommand { get; private set; }
        private void initalizCommand()
        {
            AddListCommand = new MvvmCommand(
                (parameter) => { this.AddToStoreBill(); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveMAsset(); },
                (parameter) => { return true; }
                );

            EditCommand = new MvvmCommand(
                (parameter) => { this.EditMAsset(parameter); },
                (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.deleteMAssets(parameter);
                },
                (parameter) => { return true; }
                );

            StoreBillCommand = new MvvmCommand(
                (paramter) => { this.showDocWindow(true); },
                (paramter) => { return true; }
                );

            StoreDraftCommand = new MvvmCommand(
                (paramter) => { this.showDocWindow(false); },
                (paramter) => { return true; }
                );

            ShowListCommand = new MvvmCommand(
                (parameter) => { this.showMAssetList(); },
                (parameter) => { return true; }
                );

            OldLabelEditCommand = new MvvmCommand(
                (parameter) => { this.showOldLabelEditWindow(); },
                (parameter) => { return true; }
                );

            ParentAssetForBelongingCommand = new MvvmCommand(
               (parameter) => { this.showParentAssetForBelongingWindow(); },
               (parameter) => { return true; }
               );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly ISellerService _sellerService;
        private readonly IStuffService _stuffService;
        private readonly IStoreService _storeService;
        private readonly IEmployeeService _employeeService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IStoreBillService _storeBillService;
        private readonly IUnitService _unitService;
        private readonly ObservableCollection<Stuff> _stuffList;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private readonly ObservableCollection<StoreTreeViewModel> _storefiristGeneration;
        private readonly ObservableCollection<MovableAsset> _movableAssetCollection;
        private readonly ObservableCollection<Commodity> _commodityCollection;
        private readonly HashSet<StoreBill> _storeBills;
        private readonly ObservableCollection<StuffTreeViewModel> _firstGeneration;
        private readonly Dictionary<Location, Document> _documents;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private readonly Dictionary<StoreBill, AccountDocumentMaster> _relatedAccounts;
        private readonly Dictionary<StoreBill, int> _relatedAccountId;
        private readonly Dictionary<Document, AccountDocumentMaster> _relatedAccountsDoc;
        private readonly Dictionary<Document, int> _relatedAccountDocId;

        private readonly Dictionary<int, Tuple<int?, string, int?>> _labels;
        private readonly HashSet<int> _bLabels;
        private readonly HashSet<int> _iLabels;

        private List<RequestPermit> _personPermit;
        private List<EmployeeDesign> _allOrganiz;
        private List<EmployeeDesign> _allStrategy;
        private StuffType _lastType = StuffType.UnConsumption;
        internal Boolean _isOldSystem = true;
        int _num;
        private UnConsumption _belongingParent = null;

        #endregion
    }
}
