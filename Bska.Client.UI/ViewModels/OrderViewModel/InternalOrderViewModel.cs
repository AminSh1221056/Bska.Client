
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.API;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.Domain.Entity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Threading;

    public sealed class InternalOrderViewModel : BaseViewModel
    {
        #region ctor

        public InternalOrderViewModel(IUnityContainer container,OrderType orderTyp)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._personService = _container.Resolve<IPersonService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository",_unitOfWork.Repository<Order>()));
            _collection = new ObservableCollection<OrderDetails>();
            this.InternalOrderDetails = new InternalOrderDetailsViewModel(new OrderDetails()) {Num=0,ImportantDegree=1,EstitmatePrice=0,OfferQuality="A"};
            _firstGeneration = new ObservableCollection<StuffTreeViewModel>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._storefiristGeneration = new ObservableCollection<StoreTreeViewModel>();
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._stuffList = new ObservableCollection<Stuff>();
            this._recursiveCallHelper = new RecursiveCallHelper();
            _personPermit = new List<RequestPermit>();
            this._availableStuffs = new HashSet<StuffType>();
            this._orderType = orderTyp;
            this.initializObj();
            this.initializCommand();
        }

        #endregion

        #region proeperties

        public Window Window
        {
            get { return GetValue(() => Window); }
            set
            {
                SetValue(() => Window, value);
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

        public ObservableCollection<StuffTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public Stuff SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
                if(value!=null)
                this.initOnStuff(value.StuffType);
            }
        }
        public EmployeeDesignTreeViewModel StrategySelected
        {
            get { return GetValue(() => StrategySelected); }
            set
            {
                SetValue(() => StrategySelected, value);
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
        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> StrategyCollection
        {
            get { return _strategyCollection; }
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
        public ObservableCollection<Stuff> StuffList
        {
            get { return _stuffList; }
        }
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }
        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
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

        public Boolean StuffIsBelonging
        {
            get { return GetValue(() => StuffIsBelonging); }
            set
            {
                SetValue(() => StuffIsBelonging, value);
            }
        }

        public UnConsumption BelongingParent
        {
            get { return GetValue(() => BelongingParent); }
            set
            {
                SetValue(() => BelongingParent, value);
            }
        }

        public Boolean FreeBelonging
        {
            get { return GetValue(() => FreeBelonging); }
            set
            {
                SetValue(() => FreeBelonging, value);
            }
        }

        public Boolean IsStoreRequest
        {
            get { return GetValue(() => IsStoreRequest); }
            set
            {
                SetValue(() => IsStoreRequest, value);
            }
        }

        public string NewStuffName
        {
            get { return GetValue(() => NewStuffName); }
            set
            {
                SetValue(() => NewStuffName, value);
            }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
            if (_orderType == OrderType.InternalRequest)
            {
                if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager)
                {
                    Persons = _personService.Queryable().Where(x => x.PersonId!=1).Select(p => new PersonModel
                    {
                        FullName = p.FirstName + " " + p.LastName,
                        NationalId = p.NationalId,
                        PersonId = p.PersonId
                    }).ToList();
                }
                else
                {
                    Persons = _personService.Queryable()
                       .Where(x => x.PersonId == UserLog.UniqueInstance.LogedUser.PersonId).Select(x => new PersonModel
                       {
                           FullName = x.FirstName + " " + x.LastName,
                           NationalId = x.NationalId,
                           PersonId = x.PersonId
                       }).ToList();
                }

                SelectedPerson = Persons.FirstOrDefault();
            }
            else if(_orderType==OrderType.Store)
            {
                if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager ||
               UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest)
                {
                    Stores = _storeService.Queryable().Where(s=>s.StoreType!=StoreType.Retiring).ToList();
                }
                else if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
                {
                    var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                        .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                    Stores = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId) && x.StoreType != StoreType.Retiring).ToList();
                }

                IsStoreRequest = true;
            }

            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                _availableStuffs.Add(StuffType.Belonging);
                _availableStuffs.Add(StuffType.Consumable);
                _availableStuffs.Add(StuffType.OrderConsumption);
                _availableStuffs.Add(StuffType.UnConsumption);
                _availableStuffs.Add(StuffType.Installable);
            }
            else
            {
                if (UserLog.UniqueInstance.LogedUser.UserAttribute.CanRequestBelonging)
                {
                    _availableStuffs.Add(StuffType.Belonging);
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.CanRequestConsum)
                {
                    _availableStuffs.Add(StuffType.Consumable);
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.CanRequestInConsum)
                {
                    _availableStuffs.Add(StuffType.OrderConsumption);
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.CanRequestUnConsum)
                {
                    _availableStuffs.Add(StuffType.UnConsumption);
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.CanRequestInstallable)
                {
                    _availableStuffs.Add(StuffType.Installable);
                }
            }

            Units = _unitService.Queryable().ToList();
            await this.stuffGenerationAsync();
        }

        private Task stuffGenerationAsync()
        {
            _stuffList.Clear();
            _firstGeneration.Clear();
            var ts = new Task(() =>
            {
                _stuffService.Query(s=>_availableStuffs.Contains(s.StuffType)).Include(x => x.Parent)
                .Include(x=>x.OrganizationPefectStuffs)
                           .Select().AsEnumerable().Where(x => x.Parent == null || x.IsStuff == true).ForEach(s =>
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

        private void initOnStuff(StuffType sId)
        {
            if (SelectedStuff.StuffType==StuffType.Belonging)
            {
                StuffIsBelonging = true;
                FreeBelonging = false;
            }
            else
            {
                FreeBelonging = true;
                StuffIsBelonging = false;
                BelongingParent = null;
                if (SelectedStuff.Parent != null)
                {
                    int stuffId = this.SelectedStuff.Parent.StuffId;
                    if (stuffId >= 25101 && stuffId <= 25110)
                    {
                        this.InternalOrderDetails.Num = 1;
                        this.InternalOrderDetails.NumIsEnable = false;
                    }
                    else
                    {
                        this.InternalOrderDetails.Num = 0;
                        this.InternalOrderDetails.NumIsEnable = true;
                    }
                }
            }

            _subUnits.Clear();
            Units.Where(x=>x.Parent==null && (x.StuffId == StuffType.None || x.StuffId == sId)).ForEach(x =>
            {
                _subUnits.Add(new UnitTreeViewModel(x, _container));
            });

            InternalOrderDetails.UnitId =initCurrentUnit();
            InternalOrderDetails.EstitmatePrice = 0;
        }

        private int initCurrentUnit()
        {
            return 0;
        }

        private void initStuffAsync()
        {
            _stuffList.Clear();
            Task.Run(() =>
            {
                _stuffService.Query(s => _availableStuffs.Contains(s.StuffType))
                .Include(x => x.Parent).Include(x => x.OrganizationPefectStuffs)
                    .Select(x => x).Where(x => x.IsStuff == true).ToList().ForEach(x =>
                {
                    if (SelectedNode != null)
                    {
                        bool isTrue =_recursiveCallHelper.stuffParentRecovery(x.Parent,SelectedNode.StuffCurrent);
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
        
        private void GetPersonPermit()
        {
            if (SelectedPerson != null)
            {
                _personPermit.Clear();
                foreach (var k in _personService.GetPersonPermit(SelectedPerson.PersonId))
                {
                    _personPermit.Add(k);
                }
            }
        }

        private void GetParentNode()
        {
            StrategySelected = null;
            OrganizSelected = null;
            _organizCollection.Clear();
            _strategyCollection.Clear();
            var organizPermit = _personPermit.Select(x => x.OrganizId);

            _allOrganiz =_employeeService.GetParentNode(1).ToList();
            var orgItems = _allOrganiz.Where(x => x.ParentNode == null);

            foreach (var org in orgItems)
            {
                _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null, organizPermit));
            }

            _allStrategy = _employeeService.GetParentNode(2).ToList();
        }

        private void GetStoreParentNode()
        {
            if (SelectedStore == null) return;
            var items = _storeService.GetParentNode(SelectedStore.StoreId).Where(x => x.ParentNode == null).ToList();

            _storefiristGeneration.Clear();
            foreach (var store in items) _storefiristGeneration.Add(new StoreTreeViewModel(store, null, true));
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

        private void addOrderDetails()
        {
            if (this.InternalOrderDetails.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            var orderDetails = this.InternalOrderDetails.CurrentEntity;
            if (this.SelectedStuff == null)
            {
                if (string.IsNullOrWhiteSpace(NewStuffName))
                {
                    _dialogService.ShowAlert("توجه", "هیچ مالی انتخاب نشده است");
                    return;
                }
                else
                {
                    orderDetails.StuffType = StuffType.None;
                    orderDetails.StuffName = NewStuffName+"(جدید)";
                    orderDetails.KalaUid =-1;
                    orderDetails.kalaNo ="-1";
                }
            }
            else
            {
                orderDetails.StuffType = SelectedStuff.StuffType;
                orderDetails.StuffName = SelectedStuff.Name;
                orderDetails.KalaUid = SelectedStuff.StuffId;
                orderDetails.kalaNo = SelectedStuff.KalaNo;
            }
           
            if (_orderType == OrderType.InternalRequest)
            {
                if (SelectedPerson == null || OrganizSelected == null || StrategySelected == null)
                {
                    _dialogService.ShowAlert("توجه", "انتخاب پرسنل،منطقه سازمانی و منطقه استراتژیکی الزامی است");
                    return;
                }
                orderDetails.StrategyId = StrategySelected.BuildingDesignCurrent.BuidldingDesignId;
                orderDetails.OrganizId = OrganizSelected.BuildingDesignCurrent.BuidldingDesignId;
            }
            else if (_orderType == OrderType.Store)
            {
                if (SelectedStore==null)
                {
                    _dialogService.ShowAlert("توجه", "انتخاب انبار الزامی است");
                    return;
                }
                orderDetails.StoreId = SelectedStore.StoreId;
                orderDetails.StoreDesignId = StoreDesignSelected != null ? StoreDesignSelected.StoreDesignCurrent.StoreDesignId : default(int?);
            }
            else
            {
                return;
            }

            if (SelectedStuff?.StuffType==StuffType.Belonging)
            {
                if (_orderType == OrderType.Store)
                {
                    if (!FreeBelonging)
                    {
                        if (BelongingParent == null)
                        {
                            _dialogService.ShowAlert("توجه", "مال اصلی انتخاب نشده است");
                            return;
                        }
                        orderDetails.BelongingParentLable = BelongingParent.AssetId;
                    }
                    else
                    {
                        orderDetails.BelongingParentLable = null;
                    }
                }
                else
                {
                    if (BelongingParent == null)
                    {
                        _dialogService.ShowAlert("توجه", "مال اصلی انتخاب نشده است");
                        return;
                    }
                    bool isOkParent = checkBelongingLocation();
                    if (!isOkParent)
                    {
                        _dialogService.ShowAlert("نوجه", "مکان مال اصلی با مکان انتخاب شده مغایرت دارد");
                        return;
                    }
                    orderDetails.BelongingParentLable = BelongingParent.AssetId;
                }
            }
            else if (SelectedStuff?.StuffType == StuffType.UnConsumption)
            {
                if (SelectedNode != null)
                {
                    var lastParent = _recursiveCallHelper.GetStuffLastTreeParent(SelectedNode);
                    if (lastParent.StuffCurrent.StuffType==StuffType.OrderConsumption)
                    {
                        orderDetails.StuffType = StuffType.OrderConsumption;
                    }
                }
            }

            orderDetails.ObjectState = ObjectState.Added;
            _collection.Add(orderDetails);
            this.InternalOrderDetails = new InternalOrderDetailsViewModel(new OrderDetails()) { Num = 0, ImportantDegree = 1,OfferQuality="A", EstitmatePrice = 0,UnitId=orderDetails.UnitId };
        }

        private bool checkBelongingLocation()
        {
            var pLoc = BelongingParent.Locations.Single(x => x.Status == LocationStatus.Active);
            if(pLoc.StrategyId!=StrategySelected.BuildingDesignCurrent.BuidldingDesignId
                || pLoc.OrganizId != OrganizSelected.BuildingDesignCurrent.BuidldingDesignId)
            {
                return false;
            }
            return true;
        }
        private void removeOrderDetails(object parameter)
        {
            var orderDetails = parameter as OrderDetails;
            if (orderDetails == null) return;
            _collection.Remove(orderDetails);
        }

        private void showOrderDetailsWindow(object parameter)
        {
            var orderDetails = parameter as OrderDetails;
            if (orderDetails == null) return;
            this.Selected = orderDetails;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new OrderDetailsManageViewModel(_container,true);
            viewModel.CurrentOrderDetails =(OrderDetails) orderDetails.Clone();
            viewModel.AllOrderDetails = new List<OrderDetails> { viewModel.CurrentOrderDetails};
            viewModel.Units = this.Units;
            var win= _navigationService.ShowOrderDetailsWindow(viewModel);
            if (win.DialogResult == true)
            {
                orderDetails.Num = viewModel.CurrentOrderDetails.Num;
                orderDetails.UnitId = viewModel.CurrentOrderDetails.UnitId;
                orderDetails.UsingLocation = viewModel.CurrentOrderDetails.UsingLocation;
                orderDetails.EstimatePrice = viewModel.CurrentOrderDetails.EstimatePrice;
                orderDetails.OfferQuality = viewModel.CurrentOrderDetails.OfferQuality;
                orderDetails.OfferSpecification = viewModel.CurrentOrderDetails.OfferSpecification;
                orderDetails.ImportantDegree = viewModel.CurrentOrderDetails.ImportantDegree;
                int index = _collection.IndexOf(orderDetails);
                _collection.RemoveAt(index);
                _collection.Insert(index, orderDetails);
                this.Selected = orderDetails;
            }

            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void AddOrder()
        {
            if (_collection.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", "هیچ درخواستی ثبت نشده است");
                return;
            }
             Boolean confrim = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
             if (confrim)
             {
                 var order = new Order
                 {
                     ModifiedDate = GlobalClass._Today,
                     OrderDate = GlobalClass._Today,
                     ObjectState = ObjectState.Added,
                     OrderType = _orderType,
                     Status = OrderStatus.None,
                 };

                 if (_orderType == OrderType.InternalRequest)
                 {
                     var buildingRole = _personService.GetSpecificRole(PermissionsType.GeneralManager);
                     var stuffHonestRole = _personService.GetSpecificRole(PermissionsType.StuffHonest);

                     if (stuffHonestRole == null)
                     {
                         _dialogService.ShowError("خطا", "هیچ گونه امین اموالی یافت نشد");
                         return;
                     }

                     order.PersonId = SelectedPerson.PersonId;
                     _collection.ForEach(od =>
                     {
                         od.OrderUserHistories.Add(new OrderUserHistory
                         {
                             Description = "ثبت درخواست توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                             "در تاریخ:" + " " + GlobalClass._Today.PersianDateString() + ".به تعداد " + od.Num,
                             ObjectState = ObjectState.Added,
                             UserDecision = true,
                             UserId = UserLog.UniqueInstance.LogedUser.UserId,
                             IsCurrent=false
                         });

                         var design= _employeeService.GetDesignById(od.OrganizId.Value, 1);
                         var designIds = _recursiveCallHelper.GetHirecharyDesignNodeIds(design).Split(',');
                         //ago go for count this operation ,check stuff if is perfect for organization
                         int count = designIds.Count();
                         Queue<Roles> roles = new Queue<Roles>();
                         var lstPerfects = new HashSet<int>();
                         var stuff = _stuffList.Single(x => x.KalaNo == od.kalaNo);
                         _recursiveCallHelper.getStuffPerfecs(stuff, lstPerfects);

                         for(int i = 1; i <= count; i++)
                         {
                             int temp = 0;
                             string ds = designIds[count - i];
                             if(int.TryParse(ds,out temp))
                             {
                                 var rle = GetOrganizRole(temp);
                                 if (rle != null)
                                 {
                                     roles.Enqueue(rle);
                                 }
                             }
                         }

                         if (lstPerfects.Any())
                         {
                             lstPerfects.ForEach(op =>
                             {
                                 var rle = GetOrganizRole(op);
                                 if (rle != null)
                                 {
                                     if (!roles.Contains(rle))
                                     {
                                         roles.Enqueue(rle);
                                     }
                                 }
                             });
                         }

                         bool managerisCurrent = true;
                         bool stuffHonestIsCurrent = true;
                         if (roles.Count > 0)
                         {
                             managerisCurrent = false;
                             stuffHonestIsCurrent = false;
                             od.State = OrderDetailsState.OrganizManagerConfirm;
                             bool isCurrent = true;
                             while (roles.Count > 0)
                             {
                                 var role = roles.Dequeue();
                                 if (od.OrderUserHistories.Any(s => s.IsCurrent))
                                 {
                                     isCurrent = false;
                                 }
                                 od.OrderUserHistories.Add(new OrderUserHistory
                                 {
                                     ObjectState = ObjectState.Added,
                                     UserId = role != null ? role.UserId.Value : 0,
                                     Identity = role.OrganizId.ToString(),
                                     IsCurrent = isCurrent
                                 });
                             }
                         }
                         else
                         {
                             if (buildingRole != null)
                             {
                                 stuffHonestIsCurrent = false;
                                 od.State = OrderDetailsState.ManagerConfirm;
                             }
                             else
                             {
                                 managerisCurrent = false;
                                 od.State = OrderDetailsState.StuffHonest;
                             }
                         }

                         if (buildingRole != null)
                         {
                             od.OrderUserHistories.Add(new OrderUserHistory
                             {
                                 ObjectState = ObjectState.Added,
                                 UserId = buildingRole.UserId.Value,
                                 Identity = "OrganManager",
                                 UserDecision = false,
                                 IsCurrent = managerisCurrent
                             });
                         }

                         od.OrderUserHistories.Add(new OrderUserHistory
                         {
                             ObjectState = ObjectState.Added,
                             UserId = stuffHonestRole.UserId.Value,
                             Identity = "StuffHonest",
                             UserDecision = false,
                             IsCurrent = stuffHonestIsCurrent
                         });
                         
                         order.OrderDetails.Add(od);
                     });

                     if (order.OrderDetails.Any(od => od.State == OrderDetailsState.OrganizManagerConfirm))
                     {
                         order.Status = OrderStatus.OrganizManagerConfirm;
                     }
                     else if (order.OrderDetails.Any(od => od.State == OrderDetailsState.ManagerConfirm))
                     {
                         order.Status = OrderStatus.ManagerConfirm;
                     }
                     else
                     {
                         order.Status = OrderStatus.StuffHonest;
                     }
                 }
                 else if (_orderType == OrderType.Store)
                 {
                     order.PersonId = UserLog.UniqueInstance.LogedUser.PersonId;
                     _collection.ForEach(od =>
                     {
                         od.OrderUserHistories.Add(new OrderUserHistory
                         {
                             Description = "ثبت درخواست برای انبار توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                                "در تاریخ:" + " " + GlobalClass._Today.PersianDateString() + ".به تعداد "+od.Num,
                             ObjectState = ObjectState.Added,
                             UserDecision = true,
                             UserId = UserLog.UniqueInstance.LogedUser.UserId
                         });
                         order.OrderDetails.Add(od);
                     });

                    order.Status = OrderStatus.OrganizManagerConfirm;
                    order.OrderDetails.ForEach(od =>
                    {
                        od.State = OrderDetailsState.OrganizManagerConfirm;
                    });
                }

                 _orderService.InsertOrUpdateGraph(order);

                 try
                 {
                     Mouse.SetCursor(Cursors.Wait);
                     _unitOfWork.SaveChanges();
                     _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                     _collection.Clear();
                     Mouse.SetCursor(Cursors.Arrow);
                 }
                 catch (DbUpdateException ex)
                 {
                     _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
                 }
                 catch (Exception)
                 {
                     throw;
                 }
             }
        }

        private Roles GetOrganizRole(int organizId)
        {
            var item = _personService.GetBuildingUserRoles(organizId);
            return item;
        }

        private void showParentAssetForBelongingWindow()
        {
            ParentAssetForBelongingViewModel viewModel;
            Mouse.SetCursor(Cursors.Wait);
            if (_orderType == OrderType.InternalRequest)
            {
                if (SelectedPerson == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ پرسنلی انتخاب نشده است");
                    return;
                }
                viewModel = new ParentAssetForBelongingViewModel(_container, SelectedPerson, null,null);
            }
            else
            {
                if (SelectedStore == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ انباری انتخاب نشده است");
                    return;
                }
                viewModel = new ParentAssetForBelongingViewModel(_container,null, SelectedStore,null);
            }
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var window= _navigationService.ShowParentAssetForBelongingWindow(viewModel);
            if (window.DialogResult == true)
            {
                BelongingParent = viewModel.Selected;
            }
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
        }

        private void showUserHistory(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderUserHistoryViewModel(_container);
            viewModel.CurrentOrder = od;
            this.Selected = od;
            viewModel.OrderUserHistories = od.OrderUserHistories.ToList();
            _navigationService.ShowOrderUserHistoryWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand AddListCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ParentAssetForBelongingCommand { get; private set; }
        public ICommand OrderDetailsHistoryCommand { get; private set; }
        private void initializCommand()
        {
            AddListCommand = new MvvmCommand(
                (parameter) => { this.addOrderDetails(); },
                (parameter) => { return true; }
                );

            RemoveCommand = new MvvmCommand(
                 (parameter) => { this.removeOrderDetails(parameter); },
                (parameter) => { return true; }
                );

            OrderDetailsCommand = new MvvmCommand(
                (parameter) => { this.showOrderDetailsWindow(parameter); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.AddOrder(); },
                (parameter) => { return true; }
                );

            ParentAssetForBelongingCommand = new MvvmCommand(
                (parameter) => { this.showParentAssetForBelongingWindow(); },
                (parameter) => { return true; }
                );

            OrderDetailsHistoryCommand = new MvvmCommand(
              (parameter) => { this.showUserHistory(parameter); },
              (parameter) => { return true; }
              );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IOrderService _orderService; 
        private readonly IPersonService _personService;
        private readonly IStuffService _stuffService;
        private readonly IUnitService _unitService;
        private readonly IEmployeeService _employeeService;
        private readonly IStoreService _storeService;
        private readonly ObservableCollection<OrderDetails> _collection;
        private readonly ObservableCollection<StuffTreeViewModel> _firstGeneration;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private readonly ObservableCollection<StoreTreeViewModel> _storefiristGeneration;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private readonly ObservableCollection<Stuff> _stuffList;
        private readonly RecursiveCallHelper _recursiveCallHelper;
        private List<RequestPermit> _personPermit;
        private List<EmployeeDesign> _allOrganiz;
        private List<EmployeeDesign> _allStrategy;
        private OrderType _orderType;
        private HashSet<StuffType> _availableStuffs;

        #endregion
    }
}
