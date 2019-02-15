
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
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;
    using System.Linq;
    using System.ComponentModel;
    using System.Windows.Data;

    public sealed class UsersListViewModel : BaseListViewModel<Users>
    {
        #region ctor
        public UsersListViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Users>>())
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._personService = _container.Resolve<IPersonService>(new ParameterOverride("repository", _unitOfWork.Repository<Person>()));
            _persons = new ObservableCollection<PersonModel>();
            this.UsersFilterView = new CollectionViewSource { Source = Collection }.View;
            this.GetPersons();
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties

        public UserDetailsViewModel UsersDetailsVM
        {
            get { return GetValue(() => UsersDetailsVM); }
            set
            {
                SetValue(() => UsersDetailsVM, value);
            }
        }

        public Boolean EnableGrid
        {
            get { return GetValue(() => EnableGrid); }
            set
            {
                SetValue(() => EnableGrid, value);
            }
        }
        public ObservableCollection<PersonModel> Persons
        {
            get { return _persons; }
        }
        public ICollectionView UsersFilterView { get; set; }
        public PersonModel SelectedPerson
        {
            get { return GetValue(() => SelectedPerson); }
            set
            {
                SetValue(() => SelectedPerson, value);
                this.UsersDetailsVM.FullName = value != null ? value.FullName : "";
            }
        }

        public Dictionary<PermissionsType,string> AvailablePermissions
        {
            get { return GetValue(() => AvailablePermissions); }
            set
            {
                SetValue(() => AvailablePermissions, value);
            }
        }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SearchUsers();
            }
        }
        #endregion

        #region methods

        private void initalizObj()
        {
            this.Selected = null;
            UsersDetailsVM = new UserDetailsViewModel(new Users()) { Password = "", PasswordHinit = "", UserName = "", FullName = "", PermissionType = PermissionsType.StandardUser };

            Collection.Clear();

            var users = _personService.GetUsers();

            foreach (var user in users)
            {
                Collection.Add(new UserDetailsViewModel(user));
            }

            AvailablePermissions= new Dictionary<PermissionsType, string> { {PermissionsType.StuffHonest,"امین اموال" } ,{ PermissionsType.Accountant, "ذی حساب" }, {PermissionsType.GeneralManager,"مدیر اداره" }
            ,{PermissionsType.Guard,"نگهبان" },{PermissionsType.Manager,"مدیر سامانه" },{PermissionsType.MunitionLeader,"مدیر تدارکات" },{PermissionsType.StandardUser,"کاربر عادی" },{PermissionsType.StoreLeader,"مدیر انبار" }
            ,{PermissionsType.Supplier,"کارپرداز" }};
        }

        private void SearchUsers()
        {
            this.UsersFilterView.Filter = (obj) =>
            {
                var cus = (UserDetailsViewModel)obj;
                return cus.UserName.Contains(SearchCriteria);
            };
        }

        private void GetPersons()
        {
            _persons.Clear();
            var persons = _personService.Queryable().Where(x => x.PersonId != 1).Select(p => new PersonModel
            {
                FullName = p.FirstName + " " + p.LastName,
                NationalId = p.NationalId,
                PersonId = p.PersonId
            }).ToList();
            foreach (var person in persons)
            {
                Persons.Add(person);
            }
        }

        public override void SelectedItemChanged()
        {
            if (UsersDetailsVM != null && Selected != null)
            {
                var selecteUser = (UserDetailsViewModel)Selected;
                this.UsersDetailsVM = selecteUser;

                this.UsersDetailsVM.PasswordHinit = selecteUser.Password;
                SelectedPerson = _persons.Single(x => x.PersonId == Selected.CurrentEntity.PersonId);
            }
        }

        private void AddUser()
        {
            string password = GlobalClass.GetMd5Hash(UsersDetailsVM.Password);
            if (SelectedPerson == null)
            {
                _dialogService.ShowAlert("انتخاب کاربر", "لطفا یک پرسنل را انتخاب کنید");
                return;
            }

            UsersDetailsVM.CurrentEntity.FullName = SelectedPerson.FullName;
            if (this.UsersDetailsVM.HasErrors)
            {
                _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                return;
            }

            if (password != GlobalClass.GetMd5Hash(UsersDetailsVM.PasswordHinit))
            {
                _dialogService.ShowAlert("تایید رمز عبور", "رمز عبور با تایید آن برابر نیست");
                return;
            }

            var person = _personService.Find(SelectedPerson.PersonId);
            if (Selected == null)
            {
                if (Collection.Any(x => string.Equals(x.CurrentEntity.UserName, UsersDetailsVM.UserName, StringComparison.InvariantCulture)))
                {
                    _dialogService.ShowAlert("توجه", "نام کاربری وارد شده تکراری است");
                    return;
                }

                if (Collection.Any(x => x.CurrentEntity.PermissionType == PermissionsType.StuffHonest)
                  && UsersDetailsVM.CurrentEntity.PermissionType == PermissionsType.StuffHonest)
                {
                    _dialogService.ShowAlert("توجه", "شما قبلا یک امین اموال ثبت کرده اید.بسکا تنها می تواند یک امین اموال داشته باشد");
                    return;
                }

                if (Collection.Any(x => x.CurrentEntity.PermissionType == PermissionsType.Accountant)
                 && UsersDetailsVM.CurrentEntity.PermissionType == PermissionsType.Accountant)
                {
                    _dialogService.ShowAlert("توجه", "شما قبلا یک ذی حساب ثبت کرده اید.بسکا تنها می تواند یک ذی حساب داشته باشد");
                    return;
                }

                if (Collection.Any(x => x.CurrentEntity.PermissionType == PermissionsType.GeneralManager)
                  && UsersDetailsVM.CurrentEntity.PermissionType == PermissionsType.GeneralManager)
                {
                    _dialogService.ShowAlert("توجه", "شما قبلا یک مدیرکل ثبت کرده اید.بسکا تنها می تواند یک مدیر کل داشته باشد");
                    return;
                }

                var currentUsers = Collection.Select(x => x.CurrentEntity.UserName.ToUpperInvariant());
                UsersDetailsVM.CurrentEntity.ObjectState = ObjectState.Added;
                UsersDetailsVM.CurrentEntity.Password = password;
                var attribute = new UserAttribute { ObjectState = ObjectState.Added };
                UsersDetailsVM.CurrentEntity.UserAttribute = attribute;
                if (UsersDetailsVM.PermissionType == PermissionsType.GeneralManager
                    || UsersDetailsVM.PermissionType==PermissionsType.StuffHonest)
                {
                    UsersDetailsVM.CurrentEntity.Roles.Add(new Roles
                    {
                        ObjectState = ObjectState.Added,
                        OrganizId = null,
                        StoreId = null,
                        RoleType = UsersDetailsVM.PermissionType
                    });
                }
            }
            else
            {
                UsersDetailsVM.CurrentEntity.ObjectState = ObjectState.Modified;
            }

            person.Users.Add(UsersDetailsVM.CurrentEntity);
            _personService.InsertOrUpdateGraph(person);
            try
            {
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                if (Selected == null)
                {
                    Collection.Add(new UserDetailsViewModel(UsersDetailsVM.CurrentEntity));
                }
                this.NewUser();
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

        private void DeleteUser()
        {
            if (Selected == null)
            {
                _dialogService.ShowAlert("انتخاب کاربر", "هیچ کاربری انتخاب نشده است");
                return;
            }
            
            var person = _personService.Find(Selected.CurrentEntity.PersonId);
            if (person == null)
            {
                _dialogService.ShowAlert("انتخاب کاربر", "هیچ پرسنلی یافت نشد");
                return;
            }
            Boolean canDelete = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.ConfirmDelete);
            if (canDelete)
            {
                UsersDetailsVM.CurrentEntity.ObjectState = ObjectState.Deleted;
                if (UsersDetailsVM.CurrentEntity.UserAttribute != null)
                {
                    UsersDetailsVM.CurrentEntity.UserAttribute.ObjectState = ObjectState.Deleted;
                }
                person.Users.Remove(UsersDetailsVM.CurrentEntity);
                _personService.InsertOrUpdateGraph(person);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    Collection.Remove(UsersDetailsVM);
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

        private void RefreshPass(object user)
        {
            var userVm = user as UserDetailsViewModel;
            if (user == null) return;
            this.Selected = user as BaseDetailsViewModel<Users>;
           
            var person = _personService.Find(Selected.CurrentEntity.PersonId);
            if (person == null)
            {
                _dialogService.ShowAlert("انتخاب کاربر", "هیچ پرسنلی یافت نشد");
                return;
            }
            Boolean canRefresh = _dialogService.AskConfirmation("هشدار", "آیا مطمئن به بازیابی رمز عبور این کاربر به 123456 می باشید");
            if (canRefresh)
            {
                UsersDetailsVM.CurrentEntity.ObjectState = ObjectState.Modified;
                UsersDetailsVM.CurrentEntity.Password = GlobalClass.GetMd5Hash("123456");
                person.Users.Add(UsersDetailsVM.CurrentEntity);
                person.ObjectState = ObjectState.Modified;
                _unitOfWork.Repository<Person>().InsertOrUpdateGraph(person);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void CancelUser()
        {
            this.EnableGrid = false;
            UsersDetailsVM = new UserDetailsViewModel(new Users()) { Password = "", PasswordHinit = "", UserName = "", FullName = "", PermissionType = PermissionsType.StandardUser };
            this.SelectedPerson = null;
        }

        private void NewUser()
        {
            this.EnableGrid = true;
            this.Selected = null;
            UsersDetailsVM = new UserDetailsViewModel(new Users())
            {
                Password = "",
                PasswordHinit = "",
                UserName = "",
                FullName = "",
                PermissionType = PermissionsType.StandardUser,
                EnableControls = true
            };
        }

        private void showHelpDoc()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110010-1");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommnad { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand RefreshPassWordCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initalizCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.AddUser();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            CancelCommnad = new MvvmCommand(
                (parameter) =>
                {
                    this.CancelUser();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            NewCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.NewUser();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeleteUser();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            RefreshPassWordCommand = new MvvmCommand((parameter) =>
            {
                this.RefreshPass(parameter);
            },
                (parameter) =>
                {
                    return true;
                }
                );

            HelpCommand = new MvvmCommand(
                (parameter) => { this.showHelpDoc(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IPersonService _personService;
        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<PersonModel> _persons;

        #endregion
    }

    public sealed class UserDetailsViewModel : BaseDetailsViewModel<Users>
    {
        #region ctor

        public UserDetailsViewModel(Users currentEntity)
            : base(currentEntity)
        {
        }

        #endregion

        #region properties

        public Int32 UserId
        {
            get { return CurrentEntity.UserId; }
        }

        public String FullName
        {
            get { return CurrentEntity.FullName; }
            set
            {
                CurrentEntity.FullName = value;
                ValidateProperty(value);
                OnPropertyChanged("FullName");
            }
        }

        [Required(ErrorMessage = "نام کاربری الزامی می باشد")]
        public String UserName
        {
            get { return CurrentEntity.UserName; }
            set
            {
                CurrentEntity.UserName = value;
                ValidateProperty(value);
                OnPropertyChanged("UserName");
            }
        }

        [Required(ErrorMessage = "رمز عبور الزامی می باشد")]
        [MinLength(6, ErrorMessage = "کمترین تعداد کارکترها برای رمز عبور 6 می باشد")]
        [DataType(DataType.Password)]
        public String Password
        {
            get { return CurrentEntity.Password; }
            set
            {
                CurrentEntity.Password = value;
                ValidateProperty(value);
                OnPropertyChanged("Password");
            }
        }

        [Required(ErrorMessage = "تایید رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        public String PasswordHinit
        {
            get { return _passwordhinit; }
            set
            {
                _passwordhinit = value;
                ValidateProperty(value);
                OnPropertyChanged("PasswordHinit");
            }
        }

        [Required(ErrorMessage = "نقش کاربر الزامی می باشد")]
        public PermissionsType PermissionType
        {
            get { return CurrentEntity.PermissionType; }
            set
            {
                CurrentEntity.PermissionType = value;
                ValidateProperty(value);
                OnPropertyChanged("PermissionType");
            }
        }

        public Boolean EnableControls
        {
            get { return _enableControls; }
            set
            {
                _enableControls = value;
                OnPropertyChanged("EnableControls");
            }
        }

        #endregion

        #region fields

        private string _passwordhinit;
        private bool _enableControls = false;

        #endregion
    }
}
