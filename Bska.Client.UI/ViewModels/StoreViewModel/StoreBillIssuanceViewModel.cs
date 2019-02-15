
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.UI.Services;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.ViewModels.AssetViewModel;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.API;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Common;
    using Bska.Client.API.Infrastructure;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using System.Data.Entity;
    using System.ComponentModel;
    using Repository.Model;

    public sealed class StoreBillIssuanceViewModel : BaseViewModel
    {
        #region ctor

        public StoreBillIssuanceViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._storeService = _container.Resolve<IStoreService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._personService = _container.Resolve<IPersonService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._stuffService = _container.Resolve<IStuffService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._collection = new ObservableCollection<PortableAsset>();
            this._firstGeneration = new ObservableCollection<StuffTreeViewModel>();
            this._stuffList = new ObservableCollection<Stuff>();
            this._storeBills = new HashSet<StoreBill>();
            this._documents = new HashSet<Document>();
            this._labels = new HashSet<int>();
            this._bLabels = new HashSet<int>();
            this._iLabels = new HashSet<int>();
            this._storefiristGeneration = new ObservableCollection<StoreTreeViewModel>();
            this.UnConsumptionVM = new UnConsumptionViewModel(new UnConsumption());
            this.BelongingVM = new BelongingViewModel(new Belonging());
            this.InstallableVM = new InstallableViewModel(new Installable());
            this.IncommodityVM = new InCommodityViewModel(new InCommidity());
            this.CommodityVM = new CommodityViewModel(new Commodity(),null);
            this._relatedAccounts = new Dictionary<StoreBill, AccountDocumentMaster>();
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
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

        public ObservableCollection<StuffTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public UnConsumption BelongingParent
        {
            get { return GetValue(() => BelongingParent); }
            set
            {
                SetValue(() => BelongingParent, value);
            }
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

        public ObservableCollection<Stuff> Stuffs
        {
            get { return _stuffList; }
        }

        public Stuff SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
                this.initViewModels();
            }
        }

        public ObservableCollection<PortableAsset> Collection
        {
            get { return _collection; }
        }

        public PortableAsset SelectedAsset
        {
            get { return GetValue(() => SelectedAsset); }
            set
            {
                SetValue(() => SelectedAsset, value);
            }
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

        public Boolean ToLabel
        {
            get { return GetValue(() => ToLabel); }
            set
            {
                SetValue(() => ToLabel, value);
            }
        }
        
        public StoreBillViewModel StoreBillVM
        {
            get;
            private set;
        }

        public UnConsumptionViewModel UnConsumptionVM
        {
            get;
            private set;
        }

        public BelongingViewModel BelongingVM
        {
            get;
            private set;
        }

        public InstallableViewModel InstallableVM
        {
            get;
            private set;
        }
        public InCommodityViewModel IncommodityVM
        {
            get;
            private set;
        }

        public CommodityViewModel CommodityVM
        {
            get;
            private set;
        }

        public StoreTreeViewModel StoreDesignSelected
        {
            get { return GetValue(() => StoreDesignSelected); }
            set
            {
                SetValue(() => StoreDesignSelected, value);
            }
        }

        public ObservableCollection<StoreTreeViewModel> StoreFiristGeneration
        {
            get { return _storefiristGeneration; }
        }

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        #endregion

        #region methods

        private void CreateListerner<T>(T ChildviewModel) where T : INotifyPropertyChanged
        {
            ChangeListener.Create(ChildviewModel).PropertyChanged += InitialMAssetViewModel_PropertyChanged;
        }

        private void InitialMAssetViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedStore":
                    this.GetStoreParentNode();
                    break;
            }
        }

        private async void initializObj()
        {
            _firstGeneration.Clear();
            _stuffService.Queryable().Where(x => x.Parent == null).ToList().ForEach(x =>
            {
                _rootNode = new StuffTreeViewModel(x, _container);
                _firstGeneration.Add(_rootNode);
            });
            List<Store> stores = null;
            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager ||
              UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest)
            {
                stores = _storeService.Queryable().Where(x => x.StoreType != StoreType.Retiring).ToList();
            }
            else if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
            {
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                    .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                stores = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId) && x.StoreType != StoreType.Retiring).ToList();
            }
            this.ToLabel = true;
            this.StoreBillVM = new StoreBillViewModel(new StoreBill()) { Stores = stores, ArrivalDate = PersianDate.Today,IsPurchaseEnabled=true,AcqTyp=default(StateOwnership),Desc1=null};
            this.CreateListerner<StoreBillViewModel>(StoreBillVM);
            this.StoreBillVM.SelectedStore = stores.FirstOrDefault();
            this.initBillNoAsync();
            await this.stuffGenerationAsync();
        }

        private void GetStoreParentNode()
        {
            if (StoreBillVM == null) return;
            if (StoreBillVM.SelectedStore != null)
            {
                _storefiristGeneration.Clear();
                var items = _storeService.GetParentNode(StoreBillVM.SelectedStore.StoreId).Where(x => x.ParentNode == null).ToList();
                foreach (var store in items)
                {
                    _storefiristGeneration.Add(new StoreTreeViewModel(store, null, true));
                }
                var first = _storefiristGeneration.FirstOrDefault();
                if (first != null)
                {
                    first.IsSelected = true;
                }
            }
        }

        private Task stuffGenerationAsync()
        {
            _stuffList.Clear();
            _firstGeneration.Clear();
            var ts = new Task(() =>
            {
                _stuffService.Query(x => (x.Parent == null || x.IsStuff == true)).Include(x => x.Parent)
                           .Select().AsEnumerable().ForEach(s =>
                           {
                               DispatchService.Invoke
                               (() =>
                               {
                                   if (s.IsStuff) _stuffList.Add(s);
                                   else
                                   {
                                       if (s.Parent == null) _firstGeneration.Add(new StuffTreeViewModel(s, _container));
                                   }
                               });
                           });
            });
            ts.Start();
            return ts;
        }

        private void initViewModels()
        {
            if (SelectedStuff != null)
            {
                this.Num = 1;
                if (SelectedStuff.StuffType==StuffType.Belonging)
                {
                    BelongingVM.Cost = 0;
                    BelongingVM.UnitId = 0;
                    BelongingVM.Quality = "";
                    BelongingVM.Units = _unitService.Queryable()
                        .Where(u=>u.StuffId==StuffType.None || u.StuffId==StuffType.Belonging).ToList();
                }
                else if (SelectedStuff.StuffType == StuffType.Installable)
                {
                    InstallableVM.Cost = 0;
                    InstallableVM.UnitId = 0;
                    InstallableVM.Quality = "";
                    InstallableVM.Units = _unitService.Queryable()
                        .Where(u => u.StuffId == StuffType.None || u.StuffId == StuffType.Installable).ToList();
                }
                else if (SelectedStuff.StuffType == StuffType.OrderConsumption)
                {
                    IncommodityVM.Cost = 0;
                    IncommodityVM.UnitId = 0;
                    IncommodityVM.Quality = "";
                    IncommodityVM.Num = 1;
                    IncommodityVM.Units = _unitService.Queryable()
                        .Where(u => u.StuffId == StuffType.None || u.StuffId == StuffType.OrderConsumption).ToList();
                }
                else if (SelectedStuff.StuffType == StuffType.Consumable)
                {
                    _subUnits.Clear();
                    CommodityVM.Units = _unitService.Queryable()
                        .Where(u => u.StuffId == StuffType.None || u.StuffId == StuffType.Consumable).ToList();
                    CommodityVM.Cost = 0;
                    CommodityVM.UnitId = 0;
                    CommodityVM.Units.Where(u => u.Parent == null).ForEach(u =>
                    {
                        _subUnits.Add(new UnitTreeViewModel(u, _container));
                    });
                }
                else
                {
                    UnConsumptionVM.Cost = 0;
                    UnConsumptionVM.UnitId = 0;
                    UnConsumptionVM.Quality = "";
                    UnConsumptionVM.Units = _unitService.Queryable()
                        .Where(u => u.StuffId == StuffType.None || u.StuffId == StuffType.UnConsumption).ToList();
                }
            }
        }

        private void initBillNoAsync()
        {
            Task.Run(() =>
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

                this.StoreBillVM.StoreBillNo = billNo;
            });
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
            if (parent.StuffId == SelectedNode.StuffId)
            {
                currentParent = parent;
            }
            else if (parent.Parent != null)
            {
                currentParent = this.GetStuffLastTreeParent(parent.Parent);
            }
            return currentParent;
        }

        private void generateLabelAsync()
        {
            if (Num <= 0) return;
            if (SelectedStuff == null) return;

            Task.Run(() =>
            {
                if (SelectedStuff.StuffType == StuffType.Belonging)
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

                    if (BelongingVM == null) return;
                    DispatchService.Invoke(() =>
                    {
                        this.BelongingVM.Labels = labels;
                    });
                }
                else if (SelectedStuff.StuffType == StuffType.Installable)
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

                    if (InstallableVM == null) return;
                    DispatchService.Invoke(() =>
                    {
                        this.InstallableVM.Labels = labels;
                    });
                }
                else if (SelectedStuff.StuffType == StuffType.UnConsumption)
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

                    if (UnConsumptionVM == null) return;
                    DispatchService.Invoke(() =>
                    {
                        this.UnConsumptionVM.Labels = labels;
                    });
                }
            });
        }

        private void addToList()
        {
            bool inputInvalid = true;
            if (SelectedStuff == null || this.HasErrors || this.StoreBillVM.HasErrors)
            {
                inputInvalid = false;
            }

            if (SelectedStuff.StuffType==StuffType.Belonging)
            {
                if (BelongingVM.HasErrors)
                {
                    inputInvalid = false;
                }
            }
            else if (SelectedStuff.StuffType == StuffType.Installable)
            {
                if (InstallableVM.HasErrors)
                {
                    inputInvalid = false;
                }
            }
            else if (SelectedStuff.StuffType == StuffType.OrderConsumption)
            {
                if (IncommodityVM.HasErrors)
                {
                    inputInvalid = false;
                }
            }
            else if (SelectedStuff.StuffType == StuffType.Consumable)
            {
                if (CommodityVM.HasErrors)
                {
                    inputInvalid = false;
                }
            }
            else if(SelectedStuff.StuffType==StuffType.UnConsumption)
            {
                if (UnConsumptionVM.HasErrors)
                {
                    inputInvalid = false;
                }

                SelectedStuff.StuffType = StuffType.UnConsumption;
                if (UnConsumptionVM.Cost < APPSettings.Default.MStuffPrice)
                {
                    SelectedStuff.StuffType = StuffType.OrderConsumption;
                }

                if (SelectedNode != null)
                {
                    var lastParent = this.GetStuffLastTreeParent(SelectedNode);
                    if (lastParent.StuffCurrent.StuffType == StuffType.OrderConsumption)
                    {
                        SelectedStuff.StuffType = StuffType.OrderConsumption;
                    }
                }
            }
            else
            {
                _dialogService.ShowAlert("", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (!inputInvalid)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            if ((StoreBillVM.ArrivalDate.Year < PersianDate.Today.Year))
            {
                _dialogService.ShowAlert("توجه", "تاریخ قبض انبار معتبر نیست");
                return;
            }

            if ((StoreBillVM.AcqTyp==default(StateOwnership)))
            {
                _dialogService.ShowAlert("توجه", "نوع قبض انبار معتبر نیست");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            StoreBillVM.CurrentEntity.StuffType = SelectedStuff.StuffType;

        AccountDocumentMaster accountMaster = null;
        StoreBill storeBill = _storeBills
               .Where(x => x.ArrivalDate.Year == StoreBillVM.ArrivalDate.ToDateTime().Year
               && x.StoreBillNo==StoreBillVM.StoreBillNo
               && x.StuffType == SelectedStuff.StuffType && x.AcqType==StoreBillVM.AcqTyp)
               .SingleOrDefault();

            if (storeBill == null)
            {
                storeBill = new StoreBill
                {
                    AcqType = StoreBillVM.AcqTyp,
                    ArrivalDate = StoreBillVM.ArrivalDate.ToDateTime(),
                    ObjectState = ObjectState.Added,
                    ModifiedDate = GlobalClass._Today,
                    StoreBillNo = checkMaxBillNo(),
                    StoreId = StoreBillVM.SelectedStore.StoreId,
                    Desc1 = StoreBillVM.Desc1,
                    Desc2 = StoreBillVM.Desc2,
                    Desc3 = StoreBillVM.Desc3,
                    StuffType = SelectedStuff.StuffType,
                };
                _storeBills.Add(storeBill);
            }
            else
            {
                if (storeBill.Desc1 != StoreBillVM.Desc1
                    || storeBill.Desc2 != StoreBillVM.Desc2 || storeBill.Desc3 != StoreBillVM.Desc3)
                {
                    bool isOk = _dialogService.AskConfirmation("توجه", "مشخصات این قبض انبار با مشخصات قبض انبار در لیست تفاوت دارد.مشخصات قبض انبار موجود در لیست ثبت خواهد شد.آیا میخواهید ادامه دهید");
                    if (!isOk) return;
                }

                accountMaster = _relatedAccounts[storeBill];
            }

            if (storeBill.AcqType == StateOwnership.Trust)
            {
                if (SelectedStuff.StuffType == StuffType.Belonging)
                {
                    if (BelongingVM.Parent == null)
                    {
                        _dialogService.ShowAlert("نوجه", "مال متعلقه امانی نمیتواند بدون مال اصلی ثبت شود");
                        return;
                    }
                }
                else if (SelectedStuff.StuffType == StuffType.Consumable)
                {
                    _dialogService.ShowAlert("نوجه", "اموال مصرفی قابلیت ثبت با قبض انبار امانی را ندارند");
                    return;
                }

                PersianDate maxDate;
                if (!string.IsNullOrWhiteSpace(StoreBillVM.Desc2))
                {
                    if (!PersianDate.TryParse(StoreBillVM.Desc2, out maxDate))
                    {
                        _dialogService.ShowAlert("توجه", "تاریخ برگست از امانی اشتباه است");
                        return;
                    }

                    if (maxDate >StoreBillVM._trustMaxReturnDate)
                    {
                        _dialogService.ShowAlert("توجه", "نهایت تاریخ برگشت از امانی اسفند سال بعد می باشد");
                        return;
                    }
                }
            }
            if (SelectedStuff.StuffType == StuffType.UnConsumption)
            {
                var entity = UnConsumptionVM.CurrentEntity;
                entity.Name =SelectedStuff.Name;
                entity.KalaUid = SelectedStuff.StuffId;
                entity.KalaNo = SelectedStuff.KalaNo;
                entity.CurState = MAssetCurState.AtOperation;
                entity.Num = 1;
                entity.ObjectState = ObjectState.Added;
                entity.InsertDate = GlobalClass._Today;
                entity.ModeifiedDate = GlobalClass._Today;
                entity.ISCompietion = CompietionState.NotReported;
                entity.ISConfirmed = true;
                for (int i = UnConsumptionVM.Labels.Min(); i <= UnConsumptionVM.Labels.Max(); i++)
                {
                    var item = new UnConsumption(entity);
                    item.AssetId = i;
                    if (storeBill.AcqType == StateOwnership.Trust)
                    {
                        item.Label = null;
                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.EscrowToTrust,
                            StoreAddressId = 0
                        });
                    }
                    else
                    {
                        if (ToLabel)
                        {
                            item.Label = i;
                            _labels.Add(i);
                        }

                        item.Locations.Add(new Location
                        {
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.Executive,
                            AccountDocumentType = AccountDocumentType.ExecutiveToReached
                        });

                        item.Locations.Add(new Location
                        {
                            StoreId =storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = StoreDesignSelected!=null?StoreDesignSelected.StoreDesignCurrent.StoreDesignId:0
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
                        _relatedAccounts.Add(storeBill, accountMaster);
                        accountMaster.StoreBill = storeBill;
                    }
                    _collection.Add(item);
                }
            }
            else if (SelectedStuff.StuffType == StuffType.OrderConsumption)
            {
                var entity = UnConsumptionVM.CurrentEntity;
                entity.Name = SelectedStuff.Name;
                entity.KalaUid = SelectedStuff.StuffId;
                entity.KalaNo = SelectedStuff.KalaNo;
                entity.CurState = MAssetCurState.AtOperation;
                entity.Num = 1;
                entity.ObjectState = ObjectState.Added;
                entity.InsertDate = GlobalClass._Today;
                entity.ModeifiedDate = GlobalClass._Today;
                entity.ISCompietion = CompietionState.NotReported;
                entity.ISConfirmed = true;
                entity.Label = null;
                foreach (var l in UnConsumptionVM.Labels)
                {
                    var item = entity.ToInCommidity();
                    item.AssetId = l;
                    if (storeBill.AcqType == StateOwnership.Trust)
                    {
                        item.Label = null;
                        item.Locations.Add(new Location
                        {
                            StoreId =storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.EscrowToTrust,
                            StoreAddressId =0
                        });
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

                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = StoreDesignSelected != null ? StoreDesignSelected.StoreDesignCurrent.StoreDesignId : 0
                        });
                    }
                    item.StoreBill = storeBill;
                    _collection.Add(item);
                }
            }
            else if (SelectedStuff.StuffType == StuffType.Belonging)
            {
                Location lastLocation = null;
                if (BelongingVM.Parent != null)
                    lastLocation = _movableAssetService.GetLastLocation(BelongingVM.Parent.AssetId);

                var entity = BelongingVM.CurrentEntity;
                
                for (int i = BelongingVM.Labels.Min(); i <= BelongingVM.Labels.Max(); i++)
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
                    item.Quality = entity.Quality;
                    item.Cost = entity.Cost;
                    item.Desc3 = entity.Desc3;
                    item.Desc2 = entity.Desc2;
                    item.Desc1 = entity.Desc1;
                    item.Desc4 = entity.Desc4;
                    item.Desc5 = entity.Desc5;

                    if (ToLabel)
                    {
                        item.Label = i;
                        _bLabels.Add(i);
                    }

                    if (storeBill.AcqType == StateOwnership.Trust)
                    {
                        item.Label = null;
                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.EscrowToTrust,
                            StoreAddressId = 0
                        });
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

                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = StoreDesignSelected != null ? StoreDesignSelected.StoreDesignCurrent.StoreDesignId : 0
                        });
                    }
                    item.ObjectState = ObjectState.Added;
                    item.InsertDate = GlobalClass._Today;
                    item.ModeifiedDate = GlobalClass._Today;
                    item.ISCompietion = CompietionState.NotReported;
                    item.ISConfirmed = true;
                    item.StoreBill = storeBill;
                    _collection.Add(item);
                }
            }
            else if (SelectedStuff.StuffType == StuffType.Installable)
            {
                var entity = InstallableVM.CurrentEntity;
                for (int i = InstallableVM.Labels.Min(); i <= InstallableVM.Labels.Max(); i++)
                {
                    var item = new Installable();
                    item.AssetId = i;
                    item.Name = SelectedStuff.Name;
                    item.Description = entity.Description;
                    item.UnitId = entity.UnitId;
                    item.KalaUid = SelectedStuff.StuffId;
                    item.KalaNo = SelectedStuff.KalaNo;
                    item.CurState = MAssetCurState.AtOperation;
                    item.Num = 1;
                    item.Quality = entity.Quality;
                    item.Cost = entity.Cost;
                    item.Desc3 = entity.Desc3;
                    item.Desc2 = entity.Desc2;
                    item.Desc1 = entity.Desc1;
                    item.Desc4 = entity.Desc4;
                    item.Desc5 = entity.Desc5;

                    if (ToLabel)
                    {
                        item.Label = i;
                        _iLabels.Add(i);
                    }

                    if (storeBill.AcqType == StateOwnership.Trust)
                    {
                        item.Label = null;
                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.EscrowToTrust,
                            StoreAddressId = 0
                        });
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

                        item.Locations.Add(new Location
                        {
                            StoreId = storeBill.StoreId.Value,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreActive,
                            AccountDocumentType = AccountDocumentType.ReachedToStock,
                            StoreAddressId = StoreDesignSelected != null ? StoreDesignSelected.StoreDesignCurrent.StoreDesignId : 0
                        });
                    }
                    item.ObjectState = ObjectState.Added;
                    item.InsertDate = GlobalClass._Today;
                    item.ModeifiedDate = GlobalClass._Today;
                    item.ISCompietion = CompietionState.NotReported;
                    item.ISConfirmed = true;
                    item.StoreBill = storeBill;
                    _collection.Add(item);
                }
            }
            else if (SelectedStuff.StuffType == StuffType.Consumable)
            {
                var entity = CommodityVM.CurrentEntity;
                entity.Name = SelectedStuff.Name;
                entity.KalaUid = SelectedStuff.StuffId;
                entity.KalaNo = SelectedStuff.KalaNo;
                entity.ObjectState = ObjectState.Added;
                entity.ModeifiedDate = GlobalClass._Today;
                entity.InsertDate = GlobalClass._Today;
                entity.Quality = "A";
                entity.StoreBill = storeBill;
                _collection.Add(entity);
            }
            this.initViewModels();
        }

        private void saveStoreBills()
        {
            if (_collection.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", "هیچ مالی در لیست موجود نیست");
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                var order = new Order
                {
                    Description = "این درخواست به طور مستقیم برای ثبت اموال رسیده به انبار  به غیر از خرید ثبت شده است",
                    ModifiedDate = GlobalClass._Today,
                    ObjectState = ObjectState.Added,
                    OrderDate = GlobalClass._Today,
                    Status = OrderStatus.Deliviry,
                    OrderType = OrderType.Store,
                    PersonId = UserLog.UniqueInstance.LogedUser.PersonId,
                    DueDate = GlobalClass._Today,
                };

                _collection.ForEach(ma =>
                {
                    var newdetails = new OrderDetails
                    {
                        StoreId =StoreBillVM.SelectedStore.StoreId,
                        Description = "درخواست مستقیم برای اموال رسیده به انبار غیر از خرید",
                        IsReject = false,
                        ObjectState = ObjectState.Added,
                        ImportantDegree = 1,
                        Num = ma.Num,
                        EstimatePrice = ma.Cost,
                        UnitId = ma.UnitId,
                        StuffType = SelectedStuff.StuffType,
                        StuffName = ma.Name,
                        State = OrderDetailsState.Deliviry,
                        KalaUid=SelectedStuff.StuffId,
                        kalaNo=SelectedStuff.KalaNo,
                    };
                    
                    newdetails.SubOrders.Add(new SubOrder
                    {
                        Num = ma.Num,
                        Remain = 0,
                        ObjectState = ObjectState.Added,
                        State = SubOrderState.Deliviry,
                        Type = SubOrderType.StoreBillDirect,
                        UnitId = ma.UnitId,
                    });

                    newdetails.OrderUserHistories.Add(new OrderUserHistory
                    {
                        UserDecision = true,
                        Description = "تحویل درخواست توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                          "در تاریخ:" + " " + GlobalClass._Today.PersianDateString(),
                        ObjectState = ObjectState.Added,
                        UserId = UserLog.UniqueInstance.LogedUser.UserId,
                    });

                    order.OrderDetails.Add(newdetails);
                    if(ma is Commodity)
                    {
                       ((Commodity)ma).Orders.Add(order);
                    }
                    else
                    {
                        ((MovableAsset)ma).Orders.Add(order);
                    }
                });

                var emp = _employeeService.Queryable().First();

                _collection.OfType<UnConsumption>().ForEach(item =>
                {
                    var account = _relatedAccounts[item.StoreBill];
                    this.setAccountDocDetails(item, emp, account);
                });

                _movableAssetService.InsertGraphRange(_collection.OfType<MovableAsset>());
                _commodityService.InsertGraphRange(_collection.OfType<Commodity>());

                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    UserLog.UniqueInstance.AddLog(new EventLog
                    {
                        EntryDate = GlobalClass._Today,
                        Key = UserLog.UniqueInstance.LogedUser.FullName,
                        Message = "ثبت مال با صدور قبض انبار مستقیم به تعداد " + _collection.Count.ToString(),
                        ObjectState = ObjectState.Added,
                        Type = EventType.Update,
                        UserId = UserLog.UniqueInstance.LogedUser.UserId
                    });
                    _collection.Clear();
                    _storeBills.Clear();
                    this.initBillNoAsync();
                    Mouse.SetCursor(Cursors.Arrow);
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
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
                    if (l.AccountDocumentType == AccountDocumentType.EscrowToTrust)
                    {
                        
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.Escrow),
                           "Creditor", "طرف حساب امانی", "", ma.Cost, ma));

                        var organ = StoreBillVM.Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                        if (organ != null)
                        {
                            desc = organ.Name;
                            code = organ.BudgetNo.ToString();
                        }
                        currentAccountCodings.Add(new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(
                            accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedTrustOrganizationSender),
                            "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ExecutiveToReached)
                    {
                        if (ma.Label.HasValue)
                        {
                            code = ma.Label.ToString();
                            desc = "برچسب"+"**"+code;
                        }
                        currentAccountCodings.Add(
                            new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ExecutiveSequenceLabel),
                            "Creditor", desc, code, ma.Cost, ma));

                        if (ma.StoreBill.AcqType == StateOwnership.Owned)
                        {
                            desc ="تملیکی**"+ ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                                new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetOther),
                                "Debtor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived)
                        {
                            var organ = StoreBillVM.Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                            if (organ != null)
                            {
                                desc ="انتقالی**"+ organ.Name;
                                code = organ.BudgetNo.ToString();
                            }

                            currentAccountCodings.Add(
                               new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetTransfer),
                               "Debtor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.Donation)
                        {
                            desc ="اهدایی**"+ ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                                new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetDenotion),
                                "Debtor", desc, code, ma.Cost, ma));
                        }

                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ReachedToStock)
                    {
                        if (ma.StoreBill.AcqType == StateOwnership.Owned)
                        {
                            desc ="تملیکی**"+ ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                               new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetOther),
                               "Creditor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived)
                        {
                            var organ = StoreBillVM.Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                            if (organ != null)
                            {
                                desc ="انتقالی**"+ organ.Name;
                                code = organ.BudgetNo.ToString();
                            }

                            currentAccountCodings.Add(
                              new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetTransfer),
                              "Creditor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.Donation)
                        {
                            desc ="اهدایی**"+ ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                              new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetDenotion),
                              "Creditor", desc, code, ma.Cost, ma));
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
                    acNo = parentcode.AccountCode + "-" + adc.Item1.Parent.AccountCode + "-" + adc.Item1.AccountCode + "-" +GlobalClass.CheckAccountCode(adc.Item4);
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
                        AccountDocumentMaster=accountMaster
                    };
                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
            }
        }

        private void EditMAsset(object parameter)
        {
            if (parameter is Commodity)
            {
                var commodity = parameter as Commodity;
                if (commodity == null) return;
                Mouse.SetCursor(Cursors.Wait);
                StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
                this.SelectedAsset = commodity;
                var cloneAsset = commodity.Clone();
                var viewModel = new CommodityDetailsViewModel(_container, -1, cloneAsset, true, false);
                var window = _navigationService.ShowCommodityDetailsWindow(viewModel);
                if (window.DialogResult == true)
                {
                    int index = _collection.IndexOf(commodity);
                    if (cloneAsset.AssetId == -1001)
                    {
                        this.deleteMAssets(commodity);
                    }
                    else
                    {
                        _collection.RemoveAt(index);
                        _collection.Insert(index, viewModel.CurrentAsset);
                    }
                }
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
                Mouse.SetCursor(Cursors.Arrow);
            }
            else
            {
                var mAsset = parameter as MovableAsset;
                if (mAsset == null) return;

                Mouse.SetCursor(Cursors.Wait);
                StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
                this.SelectedAsset = mAsset;
                var cloneAsset = mAsset.Clone();
                var viewModel = new MovableAssetDetailsViewModel(_container, -1, cloneAsset, true, false);
                var window = _navigationService.ShowMAssetDetailsWindow(viewModel);
                int index = _collection.IndexOf(mAsset);
                var storeBill = mAsset.StoreBill;
                if (window.DialogResult == true)
                {
                    if (cloneAsset.AssetId == -1001)
                    {
                        this.deleteMAssets(mAsset);
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
                                            StuffType = StuffType.OrderConsumption
                                        };
                                        _storeBills.Add(inCommodityStoreBill);
                                    }
                                    inComodity.StoreBill = inCommodityStoreBill;
                                    newAsset = inComodity;
                                }

                                if (cloneAsset.Label.HasValue)
                                {
                                    this.queryGraterLabels(cloneAsset.Label.Value);
                                    this.generateLabelAsync();
                                }
                                _collection.RemoveAt(index);
                                _collection.Insert(index, newAsset);
                            }
                            else
                            {
                                _collection.RemoveAt(index);
                                _collection.Insert(index, viewModel.CurrentAsset);
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
                                var unconsumptionStoreBill = _storeBills.Where(x => x.StuffType == StuffType.UnConsumption)
                                    .LastOrDefault();
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
                                        StuffType = StuffType.UnConsumption
                                    };
                                    var accountMaster = new AccountDocumentMaster
                                    {
                                        AccountDate = GlobalClass._Today,
                                        AccountCover = "1",
                                        ObjectState = ObjectState.Added,
                                        EmployeeId = employee.EmployeeId,
                                    };
                                    accountMaster.StoreBill = unconsumptionStoreBill;
                                    _relatedAccounts.Add(storeBill, accountMaster);
                                    _storeBills.Add(unconsumptionStoreBill);
                                }
                                unconsumption.StoreBill = unconsumptionStoreBill;
                                _collection.RemoveAt(index);
                                _collection.Insert(index, unconsumption);
                                this.generateLabelAsync();
                            }
                            else
                            {
                                _collection.RemoveAt(index);
                                _collection.Insert(index, viewModel.CurrentAsset);
                            }
                        }
                        else
                        {
                            _collection.RemoveAt(index);
                            _collection.Insert(index, viewModel.CurrentAsset);
                        }
                    }
                }
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void deleteMAssets(object parameter)
        {
            if (!(parameter is Commodity))
            {
                var mAsset = parameter as MovableAsset;
                if (mAsset == null) return;
                
                if (mAsset.Label.HasValue)
                {
                    if (mAsset is UnConsumption)
                    {
                        if (_collection.OfType<UnConsumption>().Where(x => x.Label.HasValue).Count() > 0)
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
                        if (_collection.OfType<Installable>().Where(x => x.Label.HasValue).Count() > 0)
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
                        if (_collection.OfType<Belonging>().Where(x => x.Label.HasValue).Count() > 0)
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

                _collection.Remove(mAsset);
            }
            else
            {
                var commodity = parameter as Commodity;
                if (commodity == null) return;
                _collection.Remove(commodity);
            }
        }

        private void queryGraterLabels(int label)
        {
            int currentLabel = label;
            int bigestLabel = _collection.OfType<UnConsumption>().Where(x => x.Label.HasValue).Max(x => x.Label.Value);
            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _collection.OfType<UnConsumption>().SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        int index = _collection.IndexOf(item);
                        item.Label = i;
                        _collection.RemoveAt(index);
                        _collection.Insert(index, item);
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
            var mAssetsCurrent = _collection.OfType<Belonging>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _collection.OfType<Belonging>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        int index = _collection.IndexOf(item);
                        item.Label = i;
                        _collection.RemoveAt(index);
                        _collection.Insert(index, item);
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
            var mAssetsCurrent = _collection.OfType<Installable>().Where(x => x.Label.HasValue);
            if (mAssetsCurrent.Count() > 0)
            {
                bigestLabel = mAssetsCurrent.Max(x => x.Label.Value);
            }

            if (bigestLabel > currentLabel)
            {
                for (int i = currentLabel; i < bigestLabel; i++)
                {
                    var item = _collection.OfType<Installable>()
                        .SingleOrDefault(ma => ma.Label == i + 1);
                    if (item != null)
                    {
                        int index = _collection.IndexOf(item);
                        item.Label = i;
                        _collection.RemoveAt(index);
                        _collection.Insert(index, item);
                    }
                }
                _iLabels.Remove(bigestLabel);
            }
            else
            {
                _iLabels.Remove(currentLabel);
            }
        }

        private string checkMaxBillNo()
        {
            string maxBillNo = StoreBillVM.StoreBillNo;
            int temp;
            if (_storeBills.Count > 0)
            {
                int maxVal = _storeBills.Select(x => int.TryParse(x.StoreBillNo, out temp) ? temp : 0).Max();
                maxBillNo = (maxVal + 1).ToString();
            }
            return maxBillNo;
        }

        private void showParentAssetForBelongingWindow()
        {
            ParentAssetForBelongingViewModel viewModel;
            Mouse.SetCursor(Cursors.Wait);
            if (StoreBillVM.SelectedStore == null || StoreBillVM.AcqTyp==default(StateOwnership))
            {
                _dialogService.ShowAlert("توجه", "ورودی های قبض انبار معتبر نیست");
                return;
            }
            bool isForTrust = false;
            List<UnConsumption> availableAssets = null;
            if (StoreBillVM.AcqTyp == StateOwnership.Trust)
            {
                isForTrust = true;
                availableAssets = _collection.OfType<UnConsumption>().Where(ma => ma.StoreBill.AcqType == StateOwnership.Trust).ToList();
            }
            else
            {
                availableAssets = _collection.OfType<UnConsumption>().Where(ma => ma.StoreBill.AcqType != StateOwnership.Trust).ToList();
            }
            _movableAssetService.Queryable().SelectMany(x => x.Locations).Where(l => l.Status == LocationStatus.StoreActive && l.StoreId == StoreBillVM.SelectedStore.StoreId)
                       .Select(l => l.MovableAsset).Include(ma => ma.AssetProceedings).OfType<UnConsumption>().ForEach(ma =>
                       {
                           if (ma.AssetProceedings.Count > 0)
                           {
                               if (ma.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                                   availableAssets.Add(ma);
                           }
                           else
                               availableAssets.Add(ma);
                       });

            viewModel = new ParentAssetForBelongingViewModel(_container, null,StoreBillVM.SelectedStore, availableAssets, isForTrust);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var window = _navigationService.ShowParentAssetForBelongingWindow(viewModel);
            if (window.DialogResult == true)
            {
                BelongingParent= BelongingVM.Parent = viewModel.Selected;
            }
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void showStoreUseableAsset()
        {
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewmodel = new StoreAssetDetailsViewModel(_container, null, new AnalizModel());
            _navigationService.ShowStoreAssetDetailsWindow(viewmodel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand AddListCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ParentAssetForBelongingCommand { get; private set; }
        public ICommand StoreDetailsCommand { get; private set; }
        private void initializCommands()
        {
            EditCommand = new MvvmCommand(
                (parameter) => { this.EditMAsset(parameter); },
                (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) => { this.deleteMAssets(parameter); },
                (parameter) => { return true; }
                );

            AddListCommand = new MvvmCommand(
               (parameter) => { this.addToList(); },
               (parameter) => { return true; }
               );

            SaveCommand = new MvvmCommand(
               (parameter) => { this.saveStoreBills(); },
               (parameter) => { return true; }
               );

            ParentAssetForBelongingCommand = new MvvmCommand(
                (parameter) => { this.showParentAssetForBelongingWindow(); },
                (parameter) => { return true; }
                );

            StoreDetailsCommand = new MvvmCommand(
          (parameter) => { this.showStoreUseableAsset(); },
          (parameter) => { return true; });
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitService _unitService;
        private readonly IStoreService _storeService;
        private readonly IStuffService _stuffService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IStoreBillService _storeBillService;
        private readonly IPersonService _personService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<PortableAsset> _collection;
        private readonly ObservableCollection<StuffTreeViewModel> _firstGeneration;
        private readonly ObservableCollection<Stuff> _stuffList;
        private readonly HashSet<StoreBill> _storeBills;
        private readonly HashSet<Document> _documents;
        private readonly ObservableCollection<StoreTreeViewModel> _storefiristGeneration;
        private double _num;
        private readonly HashSet<int> _labels;
        private readonly HashSet<int> _bLabels;
        private readonly HashSet<int> _iLabels;
        private StuffTreeViewModel _rootNode;
        private readonly Dictionary<StoreBill, AccountDocumentMaster> _relatedAccounts;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;

        #endregion
    }
}
