
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Microsoft.Practices.Unity;
    using AssetViewModel;
    using Client.API.UnitOfWork;
    using Common;
    using CustomAttributes;
    using Data.Service;
    using Domain.Entity;
    using Domain.Entity.AssetEntity;
    using Domain.Entity.OrderEntity;
    using Helper;
    using Repository.Model;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using System.Windows.Input;
    using API;
    using Client.API.Infrastructure;
    using Domain.Entity.AssetEntity.CommodityAsset;
    using System.Windows;
    using System.Data.Entity.Infrastructure;

    public sealed class AddBuyAssetViewModel : BaseViewModel
    {
        #region ctor

        public AddBuyAssetViewModel(IUnityContainer container,IEnumerable<SupplierIndentModel> indents,Boolean isdocumentIssue,Users currentSupplier)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._storeBillService = _container.Resolve<IStoreBillService>(new ParameterOverride("repository", _unitOfWork.Repository<StoreBill>()));
            this._personService = _container.Resolve<IPersonService>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._relatedAccounts = new Dictionary<StoreBill, AccountDocumentMaster>();
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._storeBills = new HashSet<StoreBill>();
            this._documents = new HashSet<Document>();
            this._labels = new HashSet<int>();
            this._bLabels = new HashSet<int>();
            this._iLabels = new HashSet<int>();
            this._collection = new ObservableCollection<SupplierIndentModel>();
            this._assetItems = new ObservableCollection<Tuple<long, PortableAsset>>();
            this._isDocumentIssue = isdocumentIssue;
            this._currentSupplier = currentSupplier;
            this._unitHelper = new UnitHelper();
            this.initializObj(indents);
            this.initializCommands();
        }
        #endregion

        #region properties

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        [Required(ErrorMessage = "تعداد الزامی است")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public Double Num
        {
            get { return _num; }
            set
            {
                _num = value;
                ValidateProperty(value);
                OnPropertyChanged("Num");
                this.generateLabelAsync();
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
            get;
            private set;
        }

        public DocumentViewModel DocumentViewModel
        {
            get;
            private set;
        }
        public List<Store> Stores
        {
            get { return GetValue(() => Stores); }
            set
            {
                SetValue(() => Stores, value);
            }
        }

        public ObservableCollection<SupplierIndentModel> Collection
        {
            get { return _collection; }
        }

        public SupplierIndentModel CurrentIndent
        {
            get { return GetValue(() => CurrentIndent); }
            set
            {
                SetValue(() => CurrentIndent, value);
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

        public Unit Unit
        {
            get { return GetValue(() => Unit); }
            set
            {
                SetValue(() => Unit, value);
            }
        }

        public PersonModel Person
        {
            get { return GetValue(() => Person); }
            set
            {
                SetValue(() => Person, value);
            }
        }

        public double Remain
        {
            get { return GetValue(() => Remain); }
            set
            {
                SetValue(() => Remain, value);
            }
        }

        public int AssetCount
        {
            get { return GetValue(() => AssetCount); }
            set
            {
                SetValue(() => AssetCount, value);
            }
        }
        
        public ObservableCollection<Tuple<long,PortableAsset>> InsertedAssets
        {
            get { return _assetItems; }
        }

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
        private async void initializObj(IEnumerable<SupplierIndentModel> indents)
        {
            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager ||
             UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest)
            {
                Stores = _storeService.Queryable().Where(x => x.StoreType != StoreType.Retiring).ToList();
            }
            else if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
            {
                var user = _personService.GetUser(UserLog.UniqueInstance.LogedUser.UserId);
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                        .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                Stores = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId) && x.StoreType != StoreType.Retiring).ToList();
            }
            Units = _unitService.Queryable().ToList();
            _collection.Clear();
            indents.ForEach(i =>
            {
                _collection.Add(i);
            });

            CurrentIndent = _collection.FirstOrDefault();
            this.StoreBillViewModel = new StoreBillViewModel(new StoreBill()) { Stores = Stores, AcqTyp = StateOwnership.Purchase, ArrivalDate = PersianDate.Today };
            this.DocumentViewModel = new DocumentViewModel(new Document()) { DocumentDate = PersianDate.Today };
            this.StoreBillViewModel.Desc1 = null;
            this.StoreBillViewModel.SelectedStore = null;
            this.StoreBillViewModel.Sellers = _sellerService.Queryable().ToList();
            await this.initDraftNoAsync();
            await this.initBillNoAsync();
        }

        internal int initOnIndent()
        {
            if (CurrentIndent == null) return -1;
            Remain = CurrentIndent.Remain;
            Person = _personService.Query(p => p.NationalId == CurrentIndent.NationalId)
                .Select(p => new PersonModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    NationalId = p.NationalId,
                    PersonId = p.PersonId
                }).FirstOrDefault();
            var stuff = _stuffService.Query(s => s.StuffId == CurrentIndent.KalaUid)
                .Include(s => s.Parent).Select().FirstOrDefault();
            Unit = Units.FirstOrDefault(u => u.UnitId == CurrentIndent.UnitId);
            var seller = StoreBillViewModel.Sellers.FirstOrDefault(s => s.SellerId == CurrentIndent.SellerId);
            StoreBillViewModel.SelectedSeller = seller;
            var units = this.Units.Where(u => u.StuffId == StuffType.None || u.StuffId == CurrentIndent.StuffType).ToList();
            switch (CurrentIndent.StuffType)
            {
                case StuffType.Belonging:
                    this.BelongingViewModel = new BelongingViewModel(new Belonging { Name = CurrentIndent.StuffName, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = Unit.UnitId }) { Units=units,Cost=0};
                    this.Num = CurrentIndent.Remain;
                    var parent = _orderService.GetParentBelongingAsstBySupllierIndent(CurrentIndent.IndentId);
                    BelongingViewModel.Parent = parent;
                    break;
                case StuffType.UnConsumption:
                    this.UnConsumptionViewModel = new UnConsumptionViewModel(new UnConsumption { Name = CurrentIndent.StuffName, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = Unit.UnitId }) { Units=units, Cost = 0 };
                    this.Num = CurrentIndent.Remain;
                    break;
                case StuffType.Consumable:
                    this.CommodityViewModel = new CommodityViewModel(new Commodity { Name = CurrentIndent.StuffName, Cost = 0, Quality = "A", UnitId = Unit.UnitId, Num = CurrentIndent.Remain }, this.Units) { Cost=0,UnitPrice=0};
                    var mParent = _unitHelper.mainparentRecovery(Unit);
                    _subUnits.Clear();
                    var rootUnit = new UnitTreeViewModel(mParent, _container);
                    _subUnits.Add(rootUnit);
                    break;
                case StuffType.Installable:
                    this.InstallableViewModel = new InstallableViewModel(new Installable { Name = CurrentIndent.StuffName, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = Unit.UnitId }) { Units = units, Cost = 0 };
                    this.Num = CurrentIndent.Remain;
                    break;
                case StuffType.OrderConsumption:
                    this.InCommodityViewModel = new InCommodityViewModel(new InCommidity { Name = CurrentIndent.StuffName, Cost = 0, Quality = "A", CurState = MAssetCurState.AtOperation, UnitId = Unit.UnitId, Num = this.CurrentIndent.Remain }) { Units = units, Cost =0};
                    break;
            }
            return stuff.Parent!=null?stuff.Parent.StuffId:stuff.StuffId;
        }

        private Task initDraftNoAsync()
        {
            var ts = new Task(() =>
            {
                string draftNo = "1";
                int temp;
                var docs = _movableAssetService.GetDocuments(false).ToList();
                if (docs.Count() > 0)
                {
                    int maxVal = docs
                        .Select(d => int.TryParse(d.Desc1, out temp) ? temp : 0).Max();
                    draftNo = (maxVal + 1).ToString();
                }
                DispatchService.Invoke(() =>
                {
                    this.DocumentViewModel.Desc1 = draftNo;
                });
            });
            ts.Start();
            return ts;
        }

        private Task initBillNoAsync()
        {
            var ts = new Task(() =>
            {
                string billNo = "5";
                int temp;
                var biilsNo = _storeBillService.Queryable().ToList()
                    .Where(x => x.ArrivalDate.PersianDateTime().Year == PersianDate.Today.Year).Select(x => x.StoreBillNo);
                if (biilsNo.Count() > 0)
                {
                    int maxVal = biilsNo.Select(x => int.TryParse(x, out temp) ? temp : 0).Max();
                    if (maxVal >= 4)
                    {
                        billNo = (maxVal + 1).ToString();
                    }
                }
                DispatchService.Invoke(() =>
                {
                    this.StoreBillViewModel.StoreBillNo = billNo;
                });
            });
            ts.Start();
            return ts;
        }

        private void generateLabelAsync()
        {
            if (Num <= 0) return;
            if (CurrentIndent == null) return;
            
            Task.Run(() =>
            {
                if (CurrentIndent.StuffType==StuffType.Belonging)
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
                else if (CurrentIndent.StuffType == StuffType.Installable)
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
                else if (CurrentIndent.StuffType == StuffType.UnConsumption)
                {
                    int maxLabel = 0;

                    if (_labels.Count > 0)
                    {
                        maxLabel = _labels.Max();
                    }
                    else
                    {
                        var mAsset = _movableAssetService.Queryable().OfType<UnConsumption>()
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

                    if (UnConsumptionViewModel == null) return;
                    DispatchService.Invoke(() =>
                    {
                        this.UnConsumptionViewModel.Labels = labels;
                    });
                }
            });
        }
        
        private void addAssetToList()
        {
            if (CurrentIndent == null)
            {
                _dialogService.ShowAlert("توجه", "هیج سفارشی انتخاب نشده است");
                return;
            }
            
            bool inputInvalid = true;
            StuffType sType = CurrentIndent.StuffType;
            double tempNum = 0;
            bool fromUnconsuption = false;
            if (this.StoreBillViewModel.HasErrors || this.DocumentViewModel.HasErrors)
            {
                inputInvalid = false;
            }

            if (sType == StuffType.UnConsumption)
            {
                if(!UnConsumptionViewModel.CheckErrors() || this.HasErrors)
                {
                    inputInvalid = false;
                }
   
                if (UnConsumptionViewModel.Cost < APPSettings.Default.MStuffPrice)
                {
                    sType = StuffType.OrderConsumption;
                    fromUnconsuption = true;
                }
                tempNum = this.Num;
            }
            else if (sType == StuffType.Installable)
            {
                if (InstallableViewModel.HasErrors)
                {
                    inputInvalid = false;
                }
                tempNum = this.Num;
            }
            else if (sType == StuffType.Belonging)
            {
                if (BelongingViewModel.HasErrors)
                {
                    inputInvalid = false;
                }
                tempNum = this.Num;
            }
            else if (sType == StuffType.Consumable)
            {
                if (CommodityViewModel.HasErrors)
                {
                    inputInvalid = false;
                }

                if (CurrentIndent.UnitId != CommodityViewModel.UnitId)
                {
                    var selectedUnit = Units.Find(u => u.UnitId == CommodityViewModel.UnitId);
                    var realUnit = Units.Find(u => u.UnitId == CurrentIndent.UnitId);
                    double subOrderVal = _unitHelper.CalculateUnitNum(selectedUnit, CommodityViewModel.Num);
                    tempNum = _unitHelper.ReverseCalculateUnitNum(realUnit, subOrderVal);
                }
                else
                {
                    tempNum = CommodityViewModel.Num;
                }
            }
            else if (sType == StuffType.OrderConsumption)
            {
                if (InCommodityViewModel.HasErrors)
                {
                    inputInvalid = false;
                }
                tempNum = InCommodityViewModel.Num;
            }

            if (!inputInvalid)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (tempNum > CurrentIndent.Remain || tempNum<=0)
            {
                _dialogService.ShowAlert("توجه", "مقدار باقی مانده صفر یا مجاز نیست");
                return;
            }

            StoreBillViewModel.CurrentEntity.StuffType = sType;

            StoreBill storeBill = _storeBills
              .Where(x => x.ArrivalDate.Year == StoreBillViewModel.ArrivalDate.ToDateTime().Year
              && x.StuffType == sType)
              .SingleOrDefault();

            if (storeBill == null)
            {
                storeBill = new StoreBill
                {
                    AcqType = StoreBillViewModel.AcqTyp,
                    ArrivalDate = StoreBillViewModel.ArrivalDate.ToDateTime(),
                    ObjectState = ObjectState.Added,
                    ModifiedDate = GlobalClass._Today,
                    StoreBillNo = checkMaxBillNo(),
                    StoreId = StoreBillViewModel.SelectedStore.StoreId,
                    Desc1 = StoreBillViewModel.Desc1,
                    Desc2 = StoreBillViewModel.Desc2,
                    Desc3 = StoreBillViewModel.Desc3,
                    StuffType = sType
                };

                if (StoreBillViewModel.SelectedSeller != null)
                {
                    StoreBillViewModel.SelectedSeller.ObjectState = ObjectState.Modified;
                    storeBill.SellerId = StoreBillViewModel.SelectedSeller.SellerId;
                }
                _storeBills.Add(storeBill);
            }
            else
            {
                if(_relatedAccounts.Count>0 && CurrentIndent.StuffType==StuffType.UnConsumption)
                accountMaster = _relatedAccounts[storeBill];
            }

            Document document = null;
            if (_isDocumentIssue)
            {
                document = _documents.Where(d => d.Desc1 == DocumentViewModel.Desc1)
                             .SingleOrDefault();
                if (document == null)
                {
                    document = new Document
                    {
                        Desc1 = DocumentViewModel.Desc1,
                        DocumentDate = DocumentViewModel.DocumentDate.ToDateTime(),
                        DocumentType = DocumentType.StoreInternalDraft,
                        ObjectState = ObjectState.Added,
                        StoreId = StoreBillViewModel.SelectedStore.StoreId,
                        Transferee = CurrentIndent.PersonName
                    };
                    _documents.Add(document);
                }
            }

            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            if (sType == StuffType.UnConsumption)
            {
                var entity = UnConsumptionViewModel.CurrentEntity;
                entity.Name = CurrentIndent.StuffName;
                entity.KalaUid =CurrentIndent.KalaUid;
                entity.KalaNo = CurrentIndent.kalaNo;
                entity.CurState = MAssetCurState.AtOperation;
                entity.Num = 1;
                entity.ObjectState = ObjectState.Added;
                entity.InsertDate = GlobalClass._Today;
                entity.ModeifiedDate = GlobalClass._Today;
                entity.ISCompietion = CompietionState.NotReported;
                entity.ISConfirmed = true;

                for (int i = UnConsumptionViewModel.Labels.Min(); i <= UnConsumptionViewModel.Labels.Max(); i++)
                {
                    var item = new UnConsumption(entity);
                    item.AssetId = i;
                    item.Label = i;
                    if (!_labels.Contains(i))
                    {
                        _labels.Add(i);
                    }

                    item.Locations.Add(new Location
                    {
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        Status = LocationStatus.Executive,
                        AccountDocumentType = AccountDocumentType.ExecutiveToReached
                    });

                    if (document != null)
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreDeActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock
                        });

                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            OrganizId = CurrentIndent.OrganizId??0,
                            PersonId = CurrentIndent.NationalId,
                            StrategyId = CurrentIndent.StrategyId??0,
                            Status = LocationStatus.Active,
                            AccountDocumentType = AccountDocumentType.StockToUnits
                        });

                        item.Documetns.Add(document);
                    }
                    else
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = CurrentIndent.StoreId?? storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = CurrentIndent.StoreAddressId??0
                        });
                    }

                    item.StoreBill = storeBill;
                    if (accountMaster == null)
                    {
                        accountMaster = new AccountDocumentMaster
                        {
                            AccountDate = GlobalClass._Today,
                            AccountCover = "1",
                            ObjectState = ObjectState.Added,
                            EmployeeId = employee.EmployeeId,
                        };

                        accountMaster.StoreBill = storeBill;

                        _relatedAccounts.Add(storeBill, accountMaster);

                        if (document != null)
                        {
                            if (accountMasterDoc == null)
                            {
                                accountMasterDoc = new AccountDocumentMaster
                                {
                                    AccountDate = GlobalClass._Today,
                                    AccountCover = "1",
                                    ObjectState = ObjectState.Added,
                                    EmployeeId = employee.EmployeeId,
                                };
                            }
                            document.AccountDocument = accountMasterDoc;
                        }
                    }
                    _assetItems.Add(new Tuple<long, PortableAsset>(CurrentIndent.IndentId, item));
                }
            }
            else if (sType == StuffType.OrderConsumption)
            {
                if (fromUnconsuption)
                {
                    var entity = UnConsumptionViewModel.CurrentEntity;
                    entity.Name = CurrentIndent.StuffName;
                    entity.KalaUid = CurrentIndent.KalaUid;
                    entity.KalaNo = CurrentIndent.kalaNo;
                    entity.CurState = MAssetCurState.AtOperation;
                    entity.Num = 1;
                    entity.ObjectState = ObjectState.Added;
                    entity.InsertDate = GlobalClass._Today;
                    entity.ModeifiedDate = GlobalClass._Today;
                    entity.ISCompietion = CompietionState.NotReported;
                    entity.ISConfirmed = true;
                    entity.Label = null;
                    foreach (var l in UnConsumptionViewModel.Labels)
                    {
                        var item = entity.ToInCommidity();
                        item.AssetId = l;
                        item.Locations.Add(new Location
                        {
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.Executive,
                            AccountDocumentType = AccountDocumentType.ExecutiveToReached
                        });

                        if (document != null)
                        {
                            item.Locations.Add(new Location
                            {
                                StoreId = storeBill.StoreId.Value,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.StoreDeActive,
                                AccountDocumentType = AccountDocumentType.ReachedToStock
                            });

                            item.Locations.Add(new Location
                            {
                                StoreId = storeBill.StoreId.Value,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                OrganizId = CurrentIndent.OrganizId ?? 0,
                                PersonId = CurrentIndent.NationalId,
                                StrategyId = CurrentIndent.StrategyId ?? 0,
                                Status = LocationStatus.Active,
                                AccountDocumentType = AccountDocumentType.StockToUnits
                            });

                            item.Documetns.Add(document);
                        }
                        else
                        {
                            item.Locations.Add(new Location
                            {
                                StoreId = CurrentIndent.StoreId ?? storeBill.StoreId.Value,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.StoreActive,
                                AccountDocumentType = AccountDocumentType.ReachedToStock,
                                StoreAddressId = CurrentIndent.StoreAddressId ?? 0
                            });
                        }
                        item.StoreBill = storeBill;
                        _assetItems.Add(new Tuple<long, PortableAsset>(CurrentIndent.IndentId,item));
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
                        item.Name = CurrentIndent.StuffName;
                        item.KalaUid = CurrentIndent.KalaUid;
                        item.KalaNo = CurrentIndent.kalaNo;
                        item.CurState = MAssetCurState.AtOperation;
                        item.Num = 1;
                        item.ObjectState = ObjectState.Added;
                        item.InsertDate = GlobalClass._Today;
                        item.ModeifiedDate = GlobalClass._Today;
                        item.ISCompietion = CompietionState.NotReported;
                        item.ISConfirmed = true;
                        item.Label = null;
                        item.AssetId = i;
                        item.Locations.Add(new Location
                        {
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.Executive,
                            AccountDocumentType = AccountDocumentType.ExecutiveToReached
                        });

                        if (document != null)
                        {
                            item.Locations.Add(new Location
                            {
                                StoreId = storeBill.StoreId.Value,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.StoreDeActive,
                                AccountDocumentType = AccountDocumentType.ReachedToStock
                            });

                            item.Locations.Add(new Location
                            {
                                StoreId = storeBill.StoreId.Value,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                OrganizId = CurrentIndent.OrganizId ?? 0,
                                PersonId = CurrentIndent.NationalId,
                                StrategyId = CurrentIndent.StrategyId ?? 0,
                                Status = LocationStatus.Active,
                                AccountDocumentType = AccountDocumentType.StockToUnits
                            });

                            item.Documetns.Add(document);
                        }
                        else
                        {
                            item.Locations.Add(new Location
                            {
                                StoreId = CurrentIndent.StoreId ?? storeBill.StoreId.Value,
                                InsertDate = GlobalClass._Today,
                                ObjectState = ObjectState.Added,
                                Status = LocationStatus.StoreActive,
                                AccountDocumentType = AccountDocumentType.ReachedToStock,
                                StoreAddressId = CurrentIndent.StoreAddressId ?? 0
                            });
                        }
                        item.StoreBill = storeBill;
                        _assetItems.Add(new Tuple<long, PortableAsset>(CurrentIndent.IndentId, item));
                    }
                }
            }
            else if (sType == StuffType.Consumable)
            {
                var entity = CommodityViewModel.CurrentEntity;
                entity.Name = CurrentIndent.StuffName;
                entity.KalaUid = CurrentIndent.KalaUid;
                entity.KalaNo = CurrentIndent.kalaNo;
                entity.ObjectState = ObjectState.Added;
                entity.ModeifiedDate = GlobalClass._Today;
                entity.InsertDate = GlobalClass._Today;
                entity.Quality = "A";
                if (document != null)
                {
                    entity.PlaceOfUses.Add(new PlaceOfUse
                   {
                       ObjectState = ObjectState.Added,
                       Document = document,
                       Num = tempNum,
                       OrganizId = CurrentIndent.OrganizId??0,
                       StrategtyId = CurrentIndent.StrategyId??0,
                       UnitId = CommodityViewModel.UnitId,
                       InsertDate=GlobalClass._Today,
                       PersonId=Person.NationalId,
                   });
                }
                entity.StoreBill = storeBill;
                _assetItems.Add(new Tuple<long, PortableAsset>(CurrentIndent.IndentId, entity));
            }
            else if (sType == StuffType.Installable)
            {
                var entity = InstallableViewModel.CurrentEntity;
                for (int i = InstallableViewModel.Labels.Min(); i <= InstallableViewModel.Labels.Max(); i++)
                {
                    var item = new Installable();
                    item.AssetId = i;
                    item.Name = CurrentIndent.StuffName;
                    item.Description = entity.Description;
                    item.UnitId = entity.UnitId;
                    item.KalaUid = CurrentIndent.KalaUid;
                    item.KalaNo = CurrentIndent.kalaNo;
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
                    item.ISConfirmed = true;
                    item.StoreBill = storeBill;
                    item.Locations.Add(new Location
                    {
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        Status = LocationStatus.Executive,
                        AccountDocumentType = AccountDocumentType.ExecutiveToReached
                    });

                    if (document != null)
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreDeActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock
                        });

                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            OrganizId = CurrentIndent.OrganizId ?? 0,
                            PersonId = CurrentIndent.NationalId,
                            StrategyId = CurrentIndent.StrategyId ?? 0,
                            Status = LocationStatus.Active,
                            AccountDocumentType = AccountDocumentType.StockToUnits
                        });

                        item.Documetns.Add(document);
                    }
                    else
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = CurrentIndent.StoreId ?? storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = CurrentIndent.StoreAddressId ?? 0
                        });
                    }
                    _assetItems.Add(new Tuple<long, PortableAsset>(CurrentIndent.IndentId, item));
                }
            }
            else if (sType == StuffType.Belonging)
            {
                var entity = BelongingViewModel.CurrentEntity;
                for (int i = BelongingViewModel.Labels.Min(); i <= BelongingViewModel.Labels.Max(); i++)
                {
                    var item = new Belonging();
                    item.AssetId = i;
                    item.Name = CurrentIndent.StuffName;
                    item.Description = entity.Description;
                    item.UnitId = entity.UnitId;
                    item.KalaUid = CurrentIndent.KalaUid;
                    item.KalaNo = CurrentIndent.kalaNo;
                    item.CurState = MAssetCurState.AtOperation;
                    item.Num = 1;
                    item.Quality = "A";
                    item.Cost = entity.Cost;
                    item.ParentMAsset = entity.ParentMAsset;
                    item.Desc3 = entity.Desc3;
                    item.Label = i;
                    _bLabels.Add(i);
                    item.Desc2 = entity.Desc2;
                    item.Desc1 = entity.Desc1;
                    item.Desc4 = entity.Desc4;
                    item.Desc5 = entity.Desc5;
                    item.ObjectState = ObjectState.Added;
                    item.InsertDate = GlobalClass._Today;
                    item.ModeifiedDate = GlobalClass._Today;
                    item.ISCompietion = CompietionState.NotReported;
                    item.ISConfirmed = true;
                    item.StoreBill = storeBill;
                    item.Locations.Add(new Location
                    {
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        Status = LocationStatus.Executive,
                        AccountDocumentType = AccountDocumentType.ExecutiveToReached
                    });

                    if (document != null)
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreDeActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock
                        });

                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            OrganizId = CurrentIndent.OrganizId ?? 0,
                            PersonId = CurrentIndent.NationalId,
                            StrategyId = CurrentIndent.StrategyId ?? 0,
                            Status = LocationStatus.Active,
                            AccountDocumentType = AccountDocumentType.StockToUnits
                        });

                        item.Documetns.Add(document);
                    }
                    else
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = CurrentIndent.StoreId ?? storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = CurrentIndent.StoreAddressId ?? 0
                        });
                    }
                    _assetItems.Add(new Tuple<long, PortableAsset>(CurrentIndent.IndentId, item));
                }
            }

            var index = _collection.IndexOf(CurrentIndent);
            CurrentIndent.Remain -= tempNum;
            var tempIndent = CurrentIndent;
            _collection.RemoveAt(index);
            _collection.Insert(index, tempIndent);
            CurrentIndent = tempIndent;
            AssetCount = _assetItems.Count;
            this.Num = 0;
        }

        private string checkMaxBillNo()
        {
            string maxBillNo = StoreBillViewModel.StoreBillNo;
            int temp;
            if (_storeBills.Count > 0)
            {
                int maxVal = _storeBills.Select(x => int.TryParse(x.StoreBillNo, out temp) ? temp : 0).Max();
                maxBillNo = (maxVal + 1).ToString();
            }
            return maxBillNo;
        }

        private void setAccountDocDetails(UnConsumption assets, Employee emp, AccountDocumentMaster accountMaster)
        {
            if (assets != null || emp != null || accountMaster != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                var ma = assets;
                ma.Locations.ForEach(l =>
                {
                    string desc = "نامشخص";
                    string code = "0";

                    if (l.AccountDocumentType == AccountDocumentType.ExecutiveToReached)
                    {
                        if (ma.StoreBill.SellerId.HasValue)
                        {
                            var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                            desc = "خریداری**" + seller.Name;
                            code = seller.Coding;
                        }
                        currentAccountCodings.Add(
                            new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                            "Debtor", desc, code, ma.Cost, ma));
                        if (ma.Label.HasValue)
                        {
                            code = ma.Label.ToString();
                            desc = "برچسب**" + code;
                        }
                        else
                        {
                            desc = "نامشخص";
                            code = "0";
                        }
                        currentAccountCodings.Add(
                            new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ExecutiveSequenceLabel),
                            "Creditor", desc, code, ma.Cost, ma));
                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ReachedToStock)
                    {
                        if (ma.StoreBill.SellerId.HasValue)
                        {
                            var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                            desc = "خریداری**" + seller.Name;
                            code = seller.Coding;
                        }
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                           "Creditor", desc, code, ma.Cost, ma));
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

                    if (accountMaster.ObjectState == ObjectState.Added)
                    {
                        newDetails.AccountDocumentMaster = accountMaster;
                    }
                    else
                    {
                        newDetails.MasterId = accountMaster.ID;
                    }

                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
            }
        }

        private void setAccountDocDetailsForDoc(UnConsumption assets, Employee emp, AccountDocumentMaster accountMaster)
        {
            if (assets != null || emp != null || accountMaster != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                var ma = assets;
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

                        var organization = _employeeService.GetParentNode(1).FirstOrDefault(x => x.BuidldingDesignId == l.OrganizId);
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

                    if (accountMaster.ObjectState == ObjectState.Added)
                    {
                        newDetails.AccountDocumentMaster = accountMaster;
                    }
                    else
                    {
                        newDetails.MasterId = accountMaster.ID;
                    }

                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
            }
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

        private void EditMAsset(IList<object> parameter)
        {
            var objWindow = parameter[1] as Window;
            if (objWindow == null) return;
            var myTuple = parameter[0] as Tuple<long, PortableAsset>;

            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", objWindow);
            if (myTuple.Item2 is MovableAsset)
            {
                var mAsset = myTuple.Item2 as MovableAsset;
                decimal cost = mAsset.Cost;
                if (mAsset == null) return;
                this.SelectedAsset = mAsset;
                var cloneAsset = mAsset.Clone();
                var viewModel = new MovableAssetDetailsViewModel(_container, -1, cloneAsset, true, false);
                var window = _navigationService.ShowMAssetDetailsWindow(viewModel);
                int index = _assetItems.IndexOf(myTuple);
                var storeBill = mAsset.StoreBill;
                if (window.DialogResult == true)
                {
                    if (cloneAsset.AssetId == -1001)
                    {
                        this.deleteAsset(mAsset);
                    }
                    else
                    {
                        if (cloneAsset is UnConsumption)
                        {
                            if (cloneAsset.Cost < APPSettings.Default.MStuffPrice)
                            {
                                var newAsset = new MovableAsset();
                                if (cloneAsset is Belonging)
                                {
                                    var belong = viewModel.CurrentAsset as Belonging;
                                    belong.Label = null;
                                    newAsset = belong;
                                }
                                else
                                {
                                    var inComodity = ((UnConsumption)cloneAsset).ToInCommidity();
                                    mAsset.Locations.ForEach(l =>
                                    {
                                        inComodity.Locations.Add(l);
                                    });
                                    var inCommodityStoreBill = _storeBills.Where(x => x.StuffType == StuffType.OrderConsumption).LastOrDefault();
                                    if (inCommodityStoreBill == null)
                                    {
                                        inCommodityStoreBill = new StoreBill
                                        {
                                            AcqType = storeBill.AcqType,
                                            ArrivalDate = storeBill.ArrivalDate,
                                            ObjectState = ObjectState.Added,
                                            ModifiedDate = GlobalClass._Today,
                                            StoreBillNo = checkMaxBillNo(),
                                            StoreId = storeBill.StoreId,
                                            Desc1 = storeBill.Desc1,
                                            StuffType = StuffType.OrderConsumption,
                                            SellerId = storeBill.SellerId
                                        };
                                        _storeBills.Add(inCommodityStoreBill);
                                    }
                                    inComodity.StoreBill = inCommodityStoreBill;
                                    mAsset.Documetns.ForEach(d =>
                                    {
                                        inComodity.Documetns.Add(d);
                                    });
                                    newAsset = inComodity;
                                }

                                if (cloneAsset.Label.HasValue)
                                {
                                    this.queryGraterLabels(cloneAsset.Label.Value);
                                }
                                _assetItems.RemoveAt(index);
                                _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1,newAsset));
                            }
                            else
                            {
                                _assetItems.RemoveAt(index);
                                _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, viewModel.CurrentAsset));
                            }
                        }
                        else if (cloneAsset is InCommidity)
                        {
                            if (cloneAsset.Cost >= APPSettings.Default.MStuffPrice)
                            {
                                var unconsumption = ((InCommidity)cloneAsset).ToUnconsumpton();
                                if (_labels.Count > 0)
                                {
                                    int label = _labels.Max() + 1;
                                    unconsumption.Label = label;
                                    _labels.Add(label);
                                }
                                else
                                {
                                    var labels = _movableAssetService.Queryable().Where(x => x.Label.HasValue).Select(x => x.Label.Value).ToList();
                                    if (labels.Count() > 0)
                                    {
                                        int label = labels.Max() + 1;
                                        unconsumption.Label = label;
                                        _labels.Add(label);
                                    }
                                    else
                                    {
                                        unconsumption.Label = 1;
                                        _labels.Add(1);
                                    }
                                }

                                mAsset.Locations.ForEach(l =>
                                {
                                    unconsumption.Locations.Add(l);
                                });
                                var unconsumptionStoreBill = _storeBills.Where(x => x.StuffType == StuffType.UnConsumption).LastOrDefault();
                                if (unconsumptionStoreBill == null)
                                {
                                    var employee = _employeeService.Queryable().First();
                                    unconsumptionStoreBill = new StoreBill
                                    {
                                        AcqType = storeBill.AcqType,
                                        ArrivalDate = storeBill.ArrivalDate,
                                        ObjectState = ObjectState.Added,
                                        ModifiedDate = GlobalClass._Today,
                                        StoreBillNo = checkMaxBillNo(),
                                        StoreId = storeBill.StoreId,
                                        Desc1 = storeBill.Desc1,
                                        StuffType = StuffType.UnConsumption,
                                        SellerId = storeBill.SellerId
                                    };
                                    accountMaster = new AccountDocumentMaster
                                    {
                                        AccountDate = GlobalClass._Today,
                                        AccountCover = "1",
                                        ObjectState = ObjectState.Added,
                                        EmployeeId = employee.EmployeeId,
                                    };
                                    accountMaster.StoreBill = unconsumptionStoreBill;
                                    _relatedAccounts.Add(unconsumptionStoreBill, accountMaster);
                                    _storeBills.Add(unconsumptionStoreBill);
                                }
                                unconsumption.StoreBill = unconsumptionStoreBill;
                                mAsset.Documetns.ForEach(d =>
                                {
                                    unconsumption.Documetns.Add(d);
                                });
                                _assetItems.RemoveAt(index);
                                _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, unconsumption));
                            }
                            else
                            {
                                _assetItems.RemoveAt(index);
                                _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, viewModel.CurrentAsset));
                            }
                        }
                        else
                        {
                            _assetItems.RemoveAt(index);
                            _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, viewModel.CurrentAsset));
                        }
                    }
                }
            }
            else if(myTuple.Item2 is Commodity)
            {
                var commodity = myTuple.Item2 as Commodity;
                if (commodity == null)
                {
                    return;
                }
                this.SelectedAsset = commodity;
                var cloneAsset = commodity.Clone();
                var viewModel = new CommodityDetailsViewModel(_container, -1, cloneAsset, true, false);
                var window = _navigationService.ShowCommodityDetailsWindow(viewModel);
                if (window.DialogResult == true)
                {
                    int index = _assetItems.IndexOf(myTuple);
                    if (cloneAsset.AssetId == -1001)
                    {
                        this.deleteAsset(commodity);
                    }
                    else
                    {
                        _assetItems.RemoveAt(index);
                        _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, viewModel.CurrentAsset));
                    }
                }
            }
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", objWindow);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void deleteAsset(object parameter)
        {
            var myTuple = parameter as Tuple<long, PortableAsset>;
            if (myTuple == null) return;
            var mAsset = myTuple.Item2 as PortableAsset;
            if(mAsset is UnConsumption)
            {
                var unconsuption = myTuple.Item2 as UnConsumption;
                if (unconsuption.Label.HasValue)
                {
                    this.queryGraterLabels(unconsuption.Label.Value);
                }
            }
            else if(mAsset is Belonging)
            {
                var belonging = myTuple.Item2 as Belonging;
                if (belonging.Label.HasValue)
                {
                    this.queryGraterBelongingLabels(belonging.Label.Value);
                }
            }
            else if(mAsset is Installable)
            {
                var installable = myTuple.Item2 as Installable;
                if (installable.Label.HasValue)
                {
                    this.queryGraterInstallableLabels(installable.Label.Value);
                }
            }

            var indent = _collection.First(i => i.IndentId == myTuple.Item1);
            bool isCurrentIndent = false;
            if (indent == CurrentIndent)
            {
                isCurrentIndent = true;
            }
            int index = _collection.IndexOf(indent);
            indent.Remain += mAsset.Num;
            _collection.RemoveAt(index);
            _collection.Insert(index, indent);
            if (isCurrentIndent)
            {
                CurrentIndent = indent;
            }
            _assetItems.Remove(myTuple);
        }

        private void queryGraterLabels(int label)
        {
            int currentLabel = label;
            int bigestLabel = label;
            var _movableAssetCollection = _assetItems.Select(ma => ma.Item2).OfType<UnConsumption>().ToList();
            var mAssetsCurrent = _movableAssetCollection.OfType<UnConsumption>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _assetItems.Select(v=>v.Item2).OfType<UnConsumption>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        var myTuple = _assetItems.Where(mi=>mi.Item2.GetType()==typeof(UnConsumption))
                            .SingleOrDefault(mi => (mi.Item2 as UnConsumption).Label==item.Label);
                        int index = _assetItems.IndexOf(myTuple);
                        item.Label = i;
                        _assetItems.RemoveAt(index);
                        _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1,item));
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
            var _movableAssetCollection = _assetItems.Select(ma => ma.Item2).OfType<Belonging>().ToList();
            var mAssetsCurrent = _movableAssetCollection.OfType<Belonging>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _assetItems.Select(v => v.Item2).OfType<Belonging>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        var myTuple = _assetItems.Where(mi => mi.Item2.GetType() == typeof(Belonging))
                            .SingleOrDefault(mi => (mi.Item2 as Belonging).Label == item.Label);
                        int index = _assetItems.IndexOf(myTuple);
                        item.Label = i;
                        _assetItems.RemoveAt(index);
                        _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, item));
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
            var _movableAssetCollection = _assetItems.Select(ma => ma.Item2).OfType<Installable>().ToList();
            var mAssetsCurrent = _movableAssetCollection.OfType<Installable>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _assetItems.Select(v => v.Item2).OfType<Installable>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        var myTuple = _assetItems.Where(mi => mi.Item2.GetType() == typeof(Installable))
                            .SingleOrDefault(mi => (mi.Item2 as Installable).Label == item.Label);
                        int index = _assetItems.IndexOf(myTuple);
                        item.Label = i;
                        _assetItems.RemoveAt(index);
                        _assetItems.Insert(index, new Tuple<long, PortableAsset>(myTuple.Item1, item));
                    }
                }
                _iLabels.Remove(bigestLabel);
            }
            else
            {
                _iLabels.Remove(currentLabel);
            }
        }
        
        private void confirmIndents()
        {
            if (_assetItems.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                var emp = _employeeService.Queryable().SingleOrDefault();
                Order currentOrder = null;
                var orderList = new List<Order>();
                _assetItems.ForEach(tp =>
                {
                    var indentModel = _collection.Single(ind => ind.IndentId == tp.Item1);
                    var subOrder = _orderService.GetSubOrderBySupplierIndent(tp.Item1);
                    var subOrders = _orderService.GetSubOrders(subOrder.OrderDetailsId.Value);
                    var currentOrderDetails = _orderService.GetOrderDetails(subOrder.OrderDetailsId.Value);
                    var order = _orderService.Query(o => o.OrderId == currentOrderDetails.OrderId)
                      .Include(o => o.OrderDetails).Include(p => p.Person).Select().Single();

                    if (currentOrder == null)
                    {
                        currentOrder = order;
                    }
                    else if (currentOrder.OrderId != order.OrderId)
                    {
                        currentOrder = order;
                    }

                    var spIndent = subOrder.SupplierIndents.First(sp => sp.ID == tp.Item1);
                    spIndent.Remain = indentModel.Remain;
                    bool hasBiilRezerf = false;

                    if (tp.Item2 is Commodity)
                    {
                        var com = tp.Item2 as Commodity;
                        com.IndentId = spIndent.ID;
                        if (!_isDocumentIssue)
                        {
                            if (currentOrder.OrderType != OrderType.Store)
                            {
                                com.CommodityAssetReserveHistories.Add(new CommodityAssetReserveHistory
                                {
                                    ObjectState = ObjectState.Added,
                                    Status = MAssetReserveStatus.Reserved,
                                    Description = "رزرو این مال در انبار با صدور قبض انبار خرید",
                                });
                            }
                           
                            hasBiilRezerf = _orderService.Queryable().Where(x => x.OrderId == currentOrder.OrderId)
                           .SelectMany(ma => ma.Commodities).Any(ma => ma.CommodityAssetReserveHistories.Any(rs => rs.Status == MAssetReserveStatus.Reserved));
                        }
                        com.StoreBill.SupplierIndents.Add(spIndent);
                        currentOrder.Commodities.Add(com);
                    }
                    else
                    {
                        var ma = tp.Item2 as MovableAsset;
                        ma.IndentId = spIndent.ID;
                        if (ma is UnConsumption)
                        {
                            var unconsump = ma as UnConsumption;
                            this.setAccountDocDetails(unconsump, emp, accountMaster);
                            if (ma.Documetns.Any())
                            {
                                this.setAccountDocDetailsForDoc(unconsump, emp, accountMasterDoc);
                            }
                        }
                        if (!_isDocumentIssue)
                        {
                            if (currentOrder.OrderType != OrderType.Store)
                            {
                                ma.MovableAssetReserveHistories.Add(new MovableAssetReserveHistory
                                {
                                    ObjectState = ObjectState.Added,
                                    Status = MAssetReserveStatus.Reserved,
                                    Description = "رزرو این مال در انبار با صدور قبض انبار خرید",
                                });
                            }
                            hasBiilRezerf = _orderService.Queryable().Where(x => x.OrderId == currentOrder.OrderId)
                           .SelectMany(mao => mao.MovableAssets)
                           .Any(mao => mao.MovableAssetReserveHistories.Any(rs => rs.Status == MAssetReserveStatus.Reserved));
                        }
                        ma.StoreBill.SupplierIndents.Add(spIndent);
                        currentOrder.MovableAssets.Add(ma);
                    }

                    if (spIndent.Remain <= 0)
                    {
                        spIndent.State = SupplierIndentState.Delivery;
                    }

                    spIndent.ObjectState = ObjectState.Modified;
                    if (subOrder.UnitId == tp.Item2.UnitId)
                    {
                        subOrder.Remain -= tp.Item2.Num;
                    }
                    else
                    {
                        var selectedUnit = Units.Find(u => u.UnitId == tp.Item2.UnitId);
                        var realUnit = Units.Find(u => u.UnitId == subOrder.UnitId);
                        double subOrderVal = _unitHelper.CalculateUnitNum(selectedUnit, tp.Item2.Num);
                        subOrder.Remain -= _unitHelper.ReverseCalculateUnitNum(realUnit, subOrderVal);
                    }

                    if (subOrder.Remain<=0
                       && subOrder.State != SubOrderState.Deliviry)
                    {
                        subOrder.State = SubOrderState.Deliviry;
                    }

                    subOrder.ObjectState = ObjectState.Modified;

                    if (currentOrderDetails.SubOrders.All(so => so.State == SubOrderState.Deliviry)
                        && currentOrderDetails.State == OrderDetailsState.SubOrder && !hasBiilRezerf)
                    {
                        if (currentOrder.OrderType == OrderType.Store)
                        {
                            currentOrderDetails.State = OrderDetailsState.Deliviry;
                        }
                        else
                        {
                            if (_isDocumentIssue)
                            {
                                currentOrderDetails.State = OrderDetailsState.Deliviry;
                            }
                        }

                        if (currentOrderDetails.State == OrderDetailsState.Deliviry && !hasBiilRezerf)
                        {
                            var history = new OrderUserHistory
                            {
                                UserDecision = true,
                                Description = "تحویل درخواست.توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                           "در تاریخ:" + " " + GlobalClass._Today.PersianDateString(),
                                ObjectState = ObjectState.Added,
                                UserId = UserLog.UniqueInstance.LogedUser.UserId,
                            };
                            currentOrderDetails.OrderUserHistories.Add(history);
                            currentOrderDetails.ObjectState = ObjectState.Modified;
                        }
                    }
                    
                    if (currentOrder.OrderDetails.All(od => od.State == OrderDetailsState.Deliviry)
                    && currentOrder.Status != OrderStatus.Deliviry && !hasBiilRezerf)
                    {
                        currentOrder.Status = OrderStatus.Deliviry;
                        currentOrder.DueDate = GlobalClass._Today;
                        currentOrder.ObjectState = ObjectState.Modified;
                    }

                    if (!orderList.Contains(currentOrder))
                    {
                        orderList.Add(currentOrder);
                    }
                });

                try
                {
                    _orderService.InsertGraphRange(orderList);
                    _unitOfWork.SaveChanges();
                    UserLog.UniqueInstance.AddLog(new EventLog
                    {
                        EntryDate = GlobalClass._Today,
                        Key = UserLog.UniqueInstance.LogedUser.FullName,
                        Message = "ثبت مال رسیده از خرید به تعداد "+_assetItems.Count,
                        ObjectState = ObjectState.Added,
                        Type = EventType.Update,
                        UserId = UserLog.UniqueInstance.LogedUser.UserId
                    });
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    _assetItems.Clear();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region commands

        public ICommand AddListCommand { get; private set; }
        public ICommand StoreBillCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand StoreDraftCommand { get; private set; }
        public ICommand EditCommand { get; private set;}
        public ICommand DeleteCommand { get; private set; }
        public ICommand ShowListCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initializCommands()
        {
            AddListCommand = new MvvmCommand(
                (parameter) => { this.addAssetToList(); },
                (parameter) => { return true; }
                );

            StoreDraftCommand = new MvvmCommand(
                (parameter) => { },
                (parameter) => { return true; }
                );

            EditCommand = new MvvmCommand(
                (parameter) => { this.EditMAsset(parameter as IList<object>); },
                (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                 (parameter) => { this.deleteAsset(parameter); },
                 (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.confirmIndents(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IEmployeeService _employeeService;
        private readonly ISellerService _sellerService;
        private readonly IPersonService _personService;
        private readonly IUnitService _unitService;
        private readonly IStoreService _storeService;
        private readonly IStoreBillService _storeBillService;
        private readonly IStuffService _stuffService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly HashSet<StoreBill> _storeBills;
        private readonly HashSet<Document> _documents;
        private readonly Dictionary<StoreBill, AccountDocumentMaster> _relatedAccounts;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private readonly Boolean _isDocumentIssue;
        private readonly Users _currentSupplier;
        private readonly ObservableCollection<SupplierIndentModel> _collection;
        private readonly ObservableCollection<Tuple<Int64, PortableAsset>> _assetItems;
        private double _num;
        private readonly HashSet<int> _labels;
        private readonly HashSet<int> _bLabels;
        private readonly HashSet<int> _iLabels;
        AccountDocumentMaster accountMaster = null;
        AccountDocumentMaster accountMasterDoc = null;
        private readonly UnitHelper _unitHelper;

        #endregion
    }
}
