
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    public sealed class DisplacementOrderViewModel : BaseViewModel
    {
        #region ctor

        public DisplacementOrderViewModel(IUnityContainer container,OrderType orderType=OrderType.Displacement)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this._buildingService = _container.Resolve<IBuildingService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._allOrganiz = new List<EmployeeDesign>();
            this._personPermit = new List<RequestPermit>();
            _collection = new ObservableCollection<MovableAsset>();
            this._selectedAssets = new HashSet<MovableAsset>();
            this._recursiveCallHelper = new RecursiveCallHelper();
            this._orderType = orderType;
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
                if (value != null)
                {
                    _collection.Clear();
                    this.GetPersonPermit();
                    this.GetParentNode();
                }
            }
        }

        public ObservableCollection<MovableAsset> DisCollection
        {
            get { return _collection; }
        }

        public MovableAsset DisSelected
        {
            get { return GetValue(() => DisSelected); }
            set
            {
                SetValue(() => DisSelected, value);
            }
        }
        
        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public EmployeeDesignTreeViewModel OrganizSelected
        {
            get { return GetValue(() => OrganizSelected); }
            set
            {
                SetValue(() => OrganizSelected, value);
                this.getMAssets();
            }
        }

        public String Description
        {
            get { return GetValue(() => Description); }
            set
            {
                SetValue(() => Description, value);
            }
        }

        public Int32 CurrentNum
        {
            get { return GetValue(() => CurrentNum); }
            set
            {
                SetValue(() => CurrentNum, value);
            }
        }

        public Boolean IsProceedingOrder
        {
            get { return GetValue(() => IsProceedingOrder); }
            set
            {
                SetValue(() => IsProceedingOrder, value);
            }
        }

        public ProceedingsType ProcType
        {
            get { return GetValue(() => ProcType); }
            set
            {
                SetValue(() => ProcType, value);
                initProceedings();
            }
        }

        #endregion

        #region methods

        private void initializObj()
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
            if (_orderType == OrderType.Procceding)
            {
                IsProceedingOrder = true;
                ProcType = ProceedingsType.None;
            }
        }
        private void GetPersonPermit()
        {
            _personPermit.Clear();
            foreach (var k in _personService.GetPersonPermit(SelectedPerson.PersonId))
            {
                _personPermit.Add(k);
            }
        }

        private void GetParentNode()
        {
            Mouse.SetCursor(Cursors.Wait);
            _organizCollection.Clear();
            _allOrganiz = _employeeService.GetParentNode(1).ToList();
            var orgItems = _allOrganiz.Where(x => x.ParentNode == null);
            if (SelectedPerson == null)
            {
                foreach (var org in orgItems)
                {
                    _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null));
                }
            }
            else
            {
                GetPersonNodes();
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void GetPersonNodes()
        {
            _organizCollection.Clear();

            var organizPermit = _personPermit.Select(x => x.OrganizId);
            var orgItems = _allOrganiz.Where(x => x.ParentNode == null);

            foreach (var org in orgItems)
            {
                _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null, organizPermit));
            }
        }

        private void initProceedings()
        {
            _collection.Clear();
            _selectedAssets.Clear();
            GetParentNode();
        }

        private void getMAssets()
        {
            if (OrganizSelected == null) return;
            if (SelectedPerson == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ پرسنلی انتخاب نشده است");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            _collection.Clear();
            _selectedAssets.Clear();
            this.CurrentNum = 0;
            List<MovableAsset> assets = new List<MovableAsset>();

            //get confirmed Assets
            _movableAssetService.GetDisplacemetnAssets(SelectedPerson.NationalId, OrganizSelected.BuildingDesignCurrent.BuidldingDesignId).ForEach(ma =>
            {
                if (ma.AssetProceedings.Count > 0)
                {
                    if (ma.AssetProceedings.All(ap => ap.State!=AssetProceedingState.InProgress))
                    {
                        assets.Add(ma);
                    }
                }
                else
                {
                    assets.Add(ma);
                }
            });

            if (_orderType == OrderType.Procceding)
            {
                if (ProcType==ProceedingsType.None)
                {
                    _dialogService.ShowAlert("توجه", "نوع صورت جلسه انتخاب نشده است");
                    this.GetParentNode();
                    return;
                }
                
                if (ProcType==ProceedingsType.AssetRetiring ||
                    ProcType==ProceedingsType.Fire || ProcType == ProceedingsType.Flood||
                    ProcType ==ProceedingsType.Earthquake || ProcType == ProceedingsType.Theft
                    || ProcType == ProceedingsType.Accident)
                {
                    assets.OfType<UnConsumption>().ForEach(ma =>
                    {
                        _collection.Add(ma);
                    });
                }
                else
                {
                    assets.OfType<UnConsumption>().ForEach(ma =>
                    {
                        if (ma.ISCompietion == CompietionState.Reported)
                            _collection.Add(ma);
                    });
                }
            }
            else
            {
                assets
               .ToList().ForEach(m =>
               {
                   _collection.Add(m);
               });
            }
           
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
                if (mAsset is UnConsumption)
                {
                    bool hasBelonging = _movableAssetService.ContainBelongings(mAsset.AssetId);
                    if (hasBelonging)
                    {
                        bool continu = _dialogService.AskConfirmation("توجه", "این مال دارای اموال متعلقه می باشد.تمام اموال متعلقه نیز شامل درخواست می شود آیا می خواهید ادامه دهید");
                        if (!continu)
                        {
                            ch.IsChecked = false;
                            return;
                        }

                        var belongs = _movableAssetService.GetBelongingsToLocation(mAsset.AssetId);
                        Boolean hasMovedReqBelong = false;
                        string bLabel = "";

                        foreach (var b in belongs)
                        {
                            if (b.Locations.Any(x => x.Status == LocationStatus.MovedRequest))
                            {
                                hasMovedReqBelong = true;
                                bLabel = b.Label.ToString();
                                break;
                            }
                            else
                            {
                                var unconsumption = mAsset as UnConsumption;
                                if (!unconsumption.Belongings.Contains(b))
                                {
                                    unconsumption.Belongings.Add(b);
                                }
                            }

                            if (_collection.Contains(b))
                            {
                                _collection.Remove(b);
                            }

                            if (_selectedAssets.Contains(b))
                            {
                                _selectedAssets.Remove(b);
                            }
                        }

                        if (hasMovedReqBelong)
                        {
                            _dialogService.ShowAlert("توجه", "مال متعلقه با شماره برچسب " + bLabel + " " + "دارای یک درخواست درجریان می باشد.لذا تا مشخص نشدن وضعیت نهایی این مال برای مال اصلی نمیتوان درخواست داد");
                            ((UnConsumption)mAsset).Belongings.ForEach(b =>
                            {
                                if (!_collection.Contains(b))
                                {
                                    _collection.Add(b);
                                }
                            });
                            ch.IsChecked = false;
                            return;
                        }
                    }
                }
                _selectedAssets.Add(mAsset);
            }
            else
            {
                if (_selectedAssets.Contains(mAsset))
                {
                    if (mAsset is UnConsumption)
                    {
                        var unconsum = mAsset as UnConsumption;
                        if (unconsum.Belongings.Any())
                        {
                            unconsum.Belongings.ForEach(b =>
                            {
                                if (!_collection.Contains(b))
                                {
                                    _collection.Add(b);
                                }
                            });
                        }
                    }
                   _selectedAssets.Remove(mAsset);
                }
            }
            CurrentNum = _selectedAssets.Count;
        }

        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAsset;
            if (mAsset == null) return;
            this.DisSelected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void AddOrder()
        {
            if (_selectedAssets.Count <= 0)
            {
                _dialogService.ShowAlert("انتخاب اموال", "هیچ مالی انتخاب نشده است");
                return;
            }

            Roles stuffHonestRole = _personService.GetSpecificRole(PermissionsType.StuffHonest);
            if (stuffHonestRole == null)
            {
                _dialogService.ShowError("خطا", "هیچ گونه امین اموالی یافت نشد");
                return;
            }

            Boolean confrim = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confrim)
            {
                var order = new Order
                {
                    Description=this.Description,
                    ModifiedDate=GlobalClass._Today,
                    OrderDate=GlobalClass._Today,
                    ObjectState=ObjectState.Added,
                    OrderType=_orderType,
                    Status=OrderStatus.None,
                    PersonId=SelectedPerson.PersonId,
                };

                if (_orderType == OrderType.Procceding)
                {
                    order.OrderProcType = ProcType;
                }

                var orderDetails = new OrderDetails
                {
                    ObjectState = ObjectState.Added,
                    StuffType = StuffType.None,
                    StuffName = _orderType.GetDescription(),
                    OrganizId = OrganizSelected.BuildingDesignCurrent.BuidldingDesignId,
                    KalaUid = 0,
                    kalaNo="0",
                };

                orderDetails.OrderUserHistories.Add(new OrderUserHistory
                {
                    Description = "ثبت درخواست توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                    "در تاریخ:" + " " + GlobalClass._Today.PersianDateString() + ".",
                    ObjectState = ObjectState.Added,
                    UserDecision = true,
                    UserId = UserLog.UniqueInstance.LogedUser.UserId,
                    IsCurrent = false,
                });

                Roles buildingRole = _personService.GetSpecificRole(PermissionsType.GeneralManager);

                var design = _employeeService.GetDesignById(orderDetails.OrganizId.Value, 1);
                var designIds = _recursiveCallHelper.GetHirecharyDesignNodeIds(design).Split(',');
                int count = designIds.Count();
                Queue<Roles> roles = new Queue<Roles>();
                for (int i = 1; i <= count; i++)
                {
                    int temp = 0;
                    string ds = designIds[count - i];
                    if (int.TryParse(ds, out temp))
                    {
                        var rle = GetOrganizRole(temp);
                        if (rle != null)
                        {
                            roles.Enqueue(rle);
                        }
                    }
                }

                bool managerisCurrent = true;
                bool stuffHonestIsCurrent = true;

                if (roles.Count > 0)
                {
                     managerisCurrent = false;
                     stuffHonestIsCurrent = false;

                    orderDetails.State = OrderDetailsState.OrganizManagerConfirm;
                    order.Status = OrderStatus.OrganizManagerConfirm;
                    bool isCurrent = true;
                    while (roles.Count > 0)
                    {
                        var role = roles.Dequeue();
                        if (orderDetails.OrderUserHistories.Any(s => s.IsCurrent))
                        {
                            isCurrent = false;
                        }
                        orderDetails.OrderUserHistories.Add(new OrderUserHistory
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
                    if (order.OrderType == OrderType.InternalTransfer)
                    {
                        if (APPSettings.Default.ModirConfirmForDisplacementPersonOrder)
                        {
                            if (buildingRole != null)
                            {
                                stuffHonestIsCurrent = false;
                                order.Status = OrderStatus.ManagerConfirm;
                                orderDetails.State = OrderDetailsState.ManagerConfirm;
                            }
                            else
                            {
                                managerisCurrent = false;
                                order.Status = OrderStatus.StuffHonest;
                                orderDetails.State = OrderDetailsState.StuffHonest;
                            }
                        }
                        else
                        {
                            managerisCurrent = false;
                            order.Status = OrderStatus.StuffHonest;
                            orderDetails.State = OrderDetailsState.StuffHonest;
                        }
                    }
                    else
                    {
                        if (buildingRole != null)
                        {
                            stuffHonestIsCurrent = false;
                            order.Status = OrderStatus.ManagerConfirm;
                            orderDetails.State = OrderDetailsState.ManagerConfirm;
                        }
                        else
                        {
                            managerisCurrent = false;
                            order.Status = OrderStatus.StuffHonest;
                            orderDetails.State = OrderDetailsState.StuffHonest;
                        }
                    }
                }

                if (buildingRole != null)
                {
                    orderDetails.OrderUserHistories.Add(new OrderUserHistory
                    {
                        ObjectState = ObjectState.Added,
                        UserId = buildingRole.UserId.Value,
                        Identity = "OrganManager",
                        UserDecision = false,
                        IsCurrent= managerisCurrent
                    });
                }

                orderDetails.OrderUserHistories.Add(new OrderUserHistory
                {
                    ObjectState = ObjectState.Added,
                    UserId = stuffHonestRole.UserId.Value,
                    Identity = "StuffHonest",
                    UserDecision = false,
                    IsCurrent=stuffHonestIsCurrent
                });
              
                order.OrderDetails.Add(orderDetails);

                foreach (var item in _selectedAssets)
                {
                    var loc = item.Locations.SingleOrDefault(x => x.Status == LocationStatus.Active);
                    loc.ObjectState = ObjectState.Modified;
                    loc.Status = LocationStatus.MovedRequest;
                    loc.MovedRequestDate = GlobalClass._Today;
                    item.Locations.Add(loc);
                    item.ObjectState = ObjectState.Modified;

                    var unconsum = item as UnConsumption;
                    if (unconsum != null)
                    {
                        if (unconsum.Belongings.Any())
                        {
                            foreach (var b in unconsum.Belongings)
                            {
                                var bloc = b.Locations.SingleOrDefault(x => x.Status == LocationStatus.Active);
                                bloc.ObjectState = ObjectState.Modified;
                                bloc.Status = LocationStatus.MovedRequest;
                                bloc.MovedRequestDate = GlobalClass._Today;
                            }
                        }
                    }
                    order.MovableAssets.Add(item);
                }
                
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _orderService.InsertOrUpdateGraph(order);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    _selectedAssets.ForEach(sm =>
                    {
                        _collection.Remove(sm);
                    });

                    _selectedAssets.Clear();
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

        private Roles GetOrganizRole(int organizId)
        {
            var item = _personService.GetBuildingUserRoles(organizId);
            return item;
        }

        #endregion

        #region commands

        public ICommand SelectCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        private void initializCommand()
        {
            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectMAssets(parameter); },
                (parameter) => { return true; }
                );

            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.AddOrder(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;
        private readonly IBuildingService _buildingService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IOrderService _orderService;
        private readonly ObservableCollection<MovableAsset> _collection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly RecursiveCallHelper _recursiveCallHelper;
        private List<EmployeeDesign> _allOrganiz;
        private List<RequestPermit> _personPermit;
        private HashSet<MovableAsset> _selectedAssets;
        private OrderType _orderType;

        #endregion
    }
}
