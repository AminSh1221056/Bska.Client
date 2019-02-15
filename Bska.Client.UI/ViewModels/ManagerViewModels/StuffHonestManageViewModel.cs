
namespace Bska.Client.UI.ViewModels.ManagerViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Windows.Input;
    using System.Data.Entity;
    using Bska.Client.UI.Helper;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Newtonsoft.Json;
    using Bska.Client.Repository.Model;

    public sealed class StuffHonestManageViewModel : BaseListViewModel<Person>
    {
        #region ctor
        public StuffHonestManageViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Person>>())
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._employeeService = container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._personService = container.Resolve<IPersonService>(new ParameterOverride("repository", _unitOfWork.Repository<Person>()));
            this._client = UserLog.UniqueInstance.Client;
            this.initalizObj();
            this.initalizCommands();
        }

        #endregion

        #region properties
        public PersonDetailsViewModel PersonDetailsVM
        {
            get { return _personDetailsVM; }
            set
            {
                _personDetailsVM = value;
                OnPropertyChanged("PersonDetailsVM");
            }
        }

        [Required(ErrorMessage = "نام کاربری الزامی می باشد")]
        public String UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                ValidateProperty(value);
                OnPropertyChanged("UserName");
            }
        }

        [Required(ErrorMessage = "رمز عبور الزامی می باشد")]
        [MinLength(6, ErrorMessage = "کمترین تعداد کارکترها برای رمز عبور 6 می باشد")]
        [DataType(DataType.Password)]
        public String Password
        {
            get { return _password; }
            set
            {
                _password = value;
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

        public Boolean IsEditablePerson
        {
            get { return GetValue(()=>IsEditablePerson);  }
            set
            {
                SetValue(() => IsEditablePerson, value);
            }
        }
        #endregion

        #region methods
        private void initalizObj()
        {
            this.UserName = this.Password = this.PasswordHinit = "";

            var currentPerson = _personService.Queryable()
                .Where(p => p.Users.Any(u => u.PermissionType == PermissionsType.StuffHonest)).Include(p=>p.Users).FirstOrDefault();

            if (currentPerson != null)
            {
                IsEditablePerson = false;
                var honestUser = currentPerson.Users.First(u => u.PermissionType == PermissionsType.StuffHonest);
                _userName = honestUser.UserName;
                _password = honestUser.Password;
                _passwordhinit = honestUser.Password;
                this.PersonDetailsVM = new PersonDetailsViewModel(currentPerson);
                this.Selected = PersonDetailsVM;
            }
            else
            {
                IsEditablePerson = true;
                this.PersonDetailsVM = new PersonDetailsViewModel(new Person()) { FirstName = "", LastName = "", NationalId = "" };
            }
        }
        public override void SelectedItemChanged()
        {
            //throw new NotImplementedException();
        }

        private async void SavePeson()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            if (PersonDetailsVM.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            PersonDetailsVM.CurrentEntity.ModifiedDate = GlobalClass._Today;
            if (Selected == null)
            {
                if (this.HasErrors)
                {
                    _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                    return;
                }

                string password = GlobalClass.GetMd5Hash(_password);
                if (password != GlobalClass.GetMd5Hash(_passwordhinit))
                {
                    _dialogService.ShowAlert("تایید رمز عبور", "رمز عبور با تایید آن برابر نیست");
                    return;
                }

                var user = new Users {FullName=PersonDetailsVM.FirstName+" "+PersonDetailsVM.LastName,ObjectState=ObjectState.Added,
                UserName=_userName,PermissionType=PermissionsType.StuffHonest,
                Password=password};

                user.Roles.Add(new Roles
                {
                    ObjectState = ObjectState.Added,
                    OrganizId = null,
                    StoreId = null,
                    RoleType =PermissionsType.StuffHonest
                });

                PersonDetailsVM.CurrentEntity.Users.Add(user);
                PersonDetailsVM.CurrentEntity.ObjectState = ObjectState.Added;
                PersonDetailsVM.CurrentEntity.CreateDate = GlobalClass._Today;
            }
            else
            {
                PersonDetailsVM.CurrentEntity.ObjectState = ObjectState.Modified;
            }

            employee.Persons.Add(PersonDetailsVM.CurrentEntity);
            _employeeService.InsertOrUpdateGraph(employee);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                await this.UpdateStuffHonestAsync(new StuffHonestModel
                {
                    Address=PersonDetailsVM.AddressLine,
                    ContractType=PersonDetailsVM.Contract,
                    FirstName=PersonDetailsVM.FirstName,
                    LastName=PersonDetailsVM.LastName,
                    Mobile=PersonDetailsVM.Mobile,
                    NationalId=PersonDetailsVM.NationalId,
                    Tell=PersonDetailsVM.Mobile,
                });
                _unitOfWork.SaveChanges();
                UserLog.UniqueInstance.AddLog(new EventLog
                {
                    EntryDate = GlobalClass._Today,
                    Key = UserLog.UniqueInstance.LogedUser.FullName,
                    Message = "ثبت یا ویرایش امین اموال در سیستم مرکزی",
                    ObjectState = ObjectState.Added,
                    Type = EventType.Update,
                    UserId = UserLog.UniqueInstance.LogedUser.UserId
                });
                Mouse.SetCursor(Cursors.Arrow);
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                this.IsEditablePerson = false;
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

        private async Task<StuffHonestModel> UpdateStuffHonestAsync(StuffHonestModel model)
        {
            var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
            if (dtnDictionary.ContainsKey(UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()))
            {
                string identity = GlobalClass.DecryptStringAES(dtnDictionary[UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()], "66Ak679Du4V3qo92");

                HttpResponseMessage response = await _client.PutAsJsonAsync($"StuffHonest/{identity}", model);
                response.EnsureSuccessStatusCode();

                // Deserialize the updated product from the response body.
                model = await response.Content.ReadAsAsync<StuffHonestModel>();
                return model;
            }
            return null;
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }

        private void initalizCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SavePeson();
                },
                (parameter) =>
                {
                    return true;
                }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly IEmployeeService _employeeService;
        private PersonDetailsViewModel _personDetailsVM;
        private readonly IPersonService _personService;
        private string _passwordhinit;
        private string _userName;
        private string _password;
        private readonly HttpClient _client;

        #endregion
    }
}
