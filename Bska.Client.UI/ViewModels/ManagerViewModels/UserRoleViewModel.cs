
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;
    using System.Linq;
    public sealed class UserRoleViewModel : BaseViewModel
    {
        #region ctor

        public UserRoleViewModel(IUnityContainer container)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._personService = _container.Resolve<IPersonService>(new ParameterOverride("repository", _unitOfWork.Repository<Person>()));
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._storeService = _container.Resolve<IStoreService>(new ParameterOverride("repository", _unitOfWork.Repository<Store>()));
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._allUser = new List<Users>();
            this._allStore = new ObservableCollection<Store>();
            this._collection = new ObservableCollection<Roles>();
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties

        public Users CurrentUser
        {
            get { return GetValue(() => CurrentUser); }
            set
            {
                SetValue(() => CurrentUser, value);
                this.EnableControlls();
            }
        }
        
        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public Boolean StoreEnable
        {
            get { return GetValue(() => StoreEnable); }
            set
            {
                SetValue(() => StoreEnable, value);
            }
        }
        

        public String Storemsg
        {
            get { return GetValue(() => Storemsg); }
            set
            {
                SetValue(() => Storemsg, value);
            }
        }


        public List<Users> AllUser
        {
            get { return _allUser; }
            set
            {
                _allUser = value;
                OnPropertyChanged("AllUser");
            }
        }
        
        public ObservableCollection<Store> AllStore
        {
            get { return _allStore; }
        }

        public ObservableCollection<Roles> Collection
        {
            get { return _collection; }
        }
        
        public Store SelectedStore
        {
            get { return GetValue(() => SelectedStore); }
            set
            {
                SetValue(() => SelectedStore, value);
            }
        }

        public Roles SelectedRole
        {
            get { return GetValue(() => SelectedRole); }
            set
            {
                SetValue(() => SelectedRole, value);
                this.SelectedItemInit();
            }
        }

        #endregion

        #region methods
        private void initalizObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            _allStore.Clear();
            this.AllUser = _personService.GetUsers().ToList();

            foreach (var store in _storeService.Queryable().ToList())
            {
                AllStore.Add(store);
            }
            _organizCollection.Clear();
            foreach (var k in _employeeService.GetOrganizByRole().Where(x => x.ParentNode == null))
            {
                _organizCollection.Add(new EmployeeDesignTreeViewModel(k, null));
            }
            this.EnableControlls();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void SelectedItemInit()
        {
            if (SelectedRole != null)
            {
                if (SelectedRole.OrganizId == null)
                {
                    SelectedStore = null;
                }
                else if (SelectedRole.StoreId != null)
                {
                    SelectedStore = _allStore.SingleOrDefault(x => x.StoreId == SelectedRole.StoreId);
                }
                else if (SelectedRole.OrganizId != null && _organizCollection.Count > 0)
                {
                    this.PerformSearch();
                }
            }
        }

        private void EnableControlls()
        {
            if (CurrentUser != null)
            {
                _collection.Clear();
                var roles = CurrentUser.Roles;
                foreach (var k in roles) _collection.Add(k);

                switch (CurrentUser.PermissionType)
                {
                    case PermissionsType.GeneralManager:
                    case PermissionsType.StuffHonest:
                        this.StoreEnable = false;
                        Storemsg = "";
                        break;
                    case PermissionsType.StoreLeader:
                        this.StoreEnable = true;
                        Storemsg = "این کاربر می تواند مسئول انبار های زیر باشد";
                        break;
                    default:
                        this.StoreEnable = false;
                        Storemsg = "";
                        break;
                }
            }
        }
        
        private void AddOrganizRole(EmployeeDesignTreeViewModel organizSection)
        {
            if (CurrentUser == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کاربری انتخاب نشده است");
                return;
            }

            if (!organizSection.IsSelected)
            {
                return;
            }

            string nodeName = this.getAddressNode(organizSection);
            if (organizSection.HaveRole)
            {
                Boolean canDelete = _dialogService.AskConfirmation("هشدار", "برای این قسمت مسئول تعریف شده است.آیا می خواهید آنرا حذف کنید");
                if (canDelete)
                {
                    this.DeleteOrganizRole(organizSection);
                }
                return;
            }
            else
            {
                bool canInsert = _dialogService.AskConfirmation("ثبت مسئول بخش سازمانی", "آیا شما مایلید این کاربر را به عنوان مسئول قسمت سازمانی" + " " + nodeName + " " + "انتخاب کنید");
                if (canInsert)
                {
                    var user = _personService.GetUserToRole(CurrentUser.UserId);
                    var person = _personService.Find(CurrentUser.PersonId);
                    var newRole = new Roles
                    {
                        ObjectState = ObjectState.Added,
                        OrganizId = organizSection.BuildingDesignCurrent.BuidldingDesignId,
                        RoleType = CurrentUser.PermissionType,
                        UserId = CurrentUser.UserId
                    };
                    user.Roles.Add(newRole);
                    person.Users.Add(user);
                    _personService.InsertOrUpdateGraph(person);
                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        organizSection.HaveRole = true;
                        _collection.Add(newRole);
                        Mouse.SetCursor(Cursors.Arrow);
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.Message.Contains("See the inner exception for details"))
                        {
                            _dialogService.ShowError("خطا", "شما قبلا برای این قسمت سازمانی مدیر تعریف کرده اید");
                        }
                        else
                        {
                            _dialogService.ShowError("خطا", ex.Message);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        private void DeleteOrganizRole(EmployeeDesignTreeViewModel organizSection)
        {
            if (CurrentUser == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کاربری انتخاب نشده است");
                return;
            }
            var person = _personService.Find(CurrentUser.PersonId);
            var user = _personService.GetUserToRole(CurrentUser.UserId);
            var role = user.Roles.FirstOrDefault(r =>r.OrganizId == organizSection.BuildingDesignCurrent.BuidldingDesignId);
            if (role == null) return;
            role.ObjectState = ObjectState.Deleted;

            user.Roles.Remove(role);
            person.Users.Add(user);
            _personService.InsertOrUpdateGraph(person);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                organizSection.HaveRole = false;
                _collection.Remove(role);
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

        private void DeleteBuildingRole(int buildingid)
        {
            if (CurrentUser == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کاربری انتخاب نشده است");
                return;
            }
            var person = _personService.Find(CurrentUser.PersonId);
            var user = _personService.GetUserToRole(CurrentUser.UserId);
            var role = user.Roles.Where(x =>x.OrganizId == null && x.RoleType == CurrentUser.PermissionType).SingleOrDefault();

            if (role == null) return;
            role.ObjectState = ObjectState.Deleted;
            user.Roles.Remove(role);
            person.Users.Add(user);
            _personService.InsertOrUpdateGraph(person);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                _collection.Remove(role);

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

        private void DeleteStoreRole(int storeid)
        {
            if (CurrentUser == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کاربری انتخاب نشده است");
                return;
            }
            var person = _personService.Find(CurrentUser.PersonId);
            var user = _personService.GetUserToRole(CurrentUser.UserId);

            var role = user.Roles.Where(t => t.StoreId == storeid &&
              t.OrganizId == null && t.RoleType == CurrentUser.PermissionType).SingleOrDefault();

            if (role == null) return;
            role.ObjectState = ObjectState.Deleted;

            user.Roles.Remove(role);
            person.Users.Add(user);
            _personService.InsertOrUpdateGraph(person);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                _collection.Remove(role);
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

        private String getAddressNode(EmployeeDesignTreeViewModel organizSection)
        {
            string nodeName = "";
            if (organizSection.Parent != null)
            {
                nodeName += this.getAddressNode(organizSection.Parent);
                nodeName += ">>>";
            }
            nodeName += organizSection.Name;
            return nodeName;
        }

        private void AddRole(object selectedObj)
        {
            if (CurrentUser == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کاربری انتخاب نشده است");
                return;
            }
            var newRole = new Roles
            {
                ObjectState = ObjectState.Added,
                RoleType = CurrentUser.PermissionType,
                UserId = CurrentUser.UserId,
            };
            var user = _personService.GetUserToRole(CurrentUser.UserId);
            var person = _personService.Find(CurrentUser.PersonId);
            
            if (selectedObj is Store)
            {
                var store = selectedObj as Store;
                var HasRole = _storeService.HavingStoreRole(store.StoreId, user.PermissionType);
                if (HasRole)
                {
                    Boolean canDelete = _dialogService.AskConfirmation("خطا", "این وظیفه قبلا برای این انبار تعریف شده است . آیا می خواهید حذف کنید");
                    if (canDelete)
                    {
                        this.DeleteStoreRole(store.StoreId);
                    }
                    return;
                }
                newRole.StoreId = store.StoreId;
            }
            user.Roles.Add(newRole);
            person.Users.Add(user);
            _personService.InsertOrUpdateGraph(person);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                _collection.Add(newRole);
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

        private void showDocHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110010-2");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #region Search Logic

        void PerformSearch()
        {
            if (_matchingBuildingEnumerator == null || !_matchingBuildingEnumerator.MoveNext())
                this.VerifyMatchingPeopleEnumerator();

            var buidingDesign = _matchingBuildingEnumerator.Current;

            if (buidingDesign == null)
                return;

            if (buidingDesign.Parent != null)
                buidingDesign.Parent.IsExpanded = true;

            buidingDesign.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            try
            {
                foreach (var k in _organizCollection)
                {
                    var matches = this.FindMatches(SelectedRole.OrganizId.Value, k);
                    if (matches.Count() > 0)
                    {
                        _matchingBuildingEnumerator = matches.GetEnumerator();
                        break;
                    }
                }

                if (!_matchingBuildingEnumerator.MoveNext())
                {
                    _dialogService.ShowInfo("دوباره سعی کنید", "هیچ بخش سازمانی پیدا نشد.اگر ساختمان دیگری تعریف کردهاید ممکن است به آن ساختمان مربوط باشد");
                }
            }

            catch (NullReferenceException) { }
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

        #endregion

        #endregion

        #region commands
        public ICommand SaveOrganizCommand { get; private set; }
        public ICommand AddRoleCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initalizCommands()
        {
            SaveOrganizCommand = new MvvmCommand(
                (Parameter) =>
                {
                    this.AddOrganizRole(Parameter as EmployeeDesignTreeViewModel);
                },
                (Parameter) =>
                {
                    return true;
                }
                );

            AddRoleCommand = new MvvmCommand((Parameter) =>
            {
                this.AddRole(Parameter);
            },
                (Parameter) =>
                {
                    return true;
                }
                );

            HelpCommand = new MvvmCommand(
                (parameter) => { this.showDocHelp(); },
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
        private readonly IEmployeeService _employeeService;
        private readonly IStoreService _storeService;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private List<Users> _allUser;
        private readonly ObservableCollection<Store> _allStore;
        private readonly ObservableCollection<Roles> _collection;
        IEnumerator<EmployeeDesignTreeViewModel> _matchingBuildingEnumerator;

        #endregion
    }
}
