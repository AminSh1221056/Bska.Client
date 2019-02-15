
namespace Bska.Client.UI.ViewModels
{
    using Data.Service;
    using Domain.Entity;
    using Microsoft.Practices.Unity;
    using Services;
    using UI.Helper;
    using System.Windows.Input;
    using API;
    using System;
    using Client.API.Infrastructure;
    using Client.API.UnitOfWork;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Common;
    using System.Threading;

    public sealed class UserConfigViewModel : BaseViewModel
    {
        #region ctor

        public UserConfigViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._personService = _container.Resolve<IPersonService>(new ParameterOverride("repository",_unitOfWork.Repository<Person>()));
            this.initializObj();
            this.initializCommand();
        }

        #endregion;

        #region properties
        
        public string PasswordHinit
        {
            get { return GetValue(() => PasswordHinit); }
            set
            {
                SetValue(() => PasswordHinit, value);
            }
        }

        public string UserName
        {
            get { return GetValue(() => UserName); }
            set
            {
                SetValue(() => UserName, value);
            }
        }

        public string Password
        {
            get { return GetValue(() => Password); }
            set
            {
                SetValue(() => Password, value);
            }
        }

        public Boolean CanCnangePassword
        {
            get { return GetValue(() => CanCnangePassword); }
            set
            {
                SetValue(() => CanCnangePassword, value);
            }
        }

        public Boolean OrderNotify
        {
            get { return GetValue(() => OrderNotify); }
            set
            {
                SetValue(() => OrderNotify, value);
            }
        }

        public Boolean IsClosedMenu
        {
            get { return GetValue(() => IsClosedMenu); }
            set
            {
                SetValue(() => IsClosedMenu, value);
            }
        }
        #endregion;

        #region methods

        private void initializObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            if (!Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                CanCnangePassword = UserLog.UniqueInstance.LogedUser.UserAttribute.CanChangePassword;
            }
            else
            {
                CanCnangePassword = true;
            }
            OrderNotify = APPSettings.Default.OrderNotify;
            IsClosedMenu = APPSettings.Default.IsClosedMenu;
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void setUserCredential()
        {
            if(string.IsNullOrWhiteSpace(this.UserName)|| string.IsNullOrWhiteSpace(this.Password) || string.IsNullOrWhiteSpace(this.PasswordHinit))
            {
                _dialogService.ShowAlert("خطا", "لطفا ورودی های خود را کنترل کنید");
                return;
            }

            if (!string.Equals(this.Password, this.PasswordHinit, StringComparison.InvariantCultureIgnoreCase))
            {
                _dialogService.ShowAlert("خطا", "رمز عبور با تکرار رمز عبور یکی نیست");
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);

                var person = _personService.Query(x=>x.PersonId==UserLog.UniqueInstance.LogedUser.PersonId)
                    .Include(x=>x.Users).Select().Single();
                var user = person.Users.Single(x=>x.UserId==UserLog.UniqueInstance.LogedUser.UserId);
                user.ObjectState = ObjectState.Modified;
                user.Password = GlobalClass.GetMd5Hash(this.Password);
                user.UserName = this.UserName;
                person.Users.Add(user);
                _personService.InsertOrUpdateGraph(person);

                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }

                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void changeSettings()
        {
            Mouse.SetCursor(Cursors.Wait);
            APPSettings.Default.OrderNotify = this.OrderNotify;
            APPSettings.Default.IsClosedMenu = this.IsClosedMenu;
            APPSettings.Default.Save();
            APPSettings.Default.Reload();
            _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void changeTheme(object obj)
        {
            int themId = Convert.ToInt32(obj);
            if (themId > 0)
            {
                Boolean confirm = _dialogService.AskConfirmation("پرسش", "برنامه بسته و باید مجددا اجرا شود آیا میخواهید ادامه دهید");
                if (confirm)
                {
                    APPSettings.Default.CurrentTheme = themId;
                    APPSettings.Default.Save();
                    APPSettings.Default.Reload();
                    _dialogService.ShowInfo("توجه", "تغییرات با موفقیت ذخیره شد برنامه بسته و آنرا دوباره اجرا کنید");
                    App.Current.Shutdown();
                }
            }
        }
        #endregion

        #region commands

        public ICommand CredentialCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }
        public ICommand ThemCommand { get; private set; }
        private void initializCommand()
        {
            CredentialCommand = new MvvmCommand(
                (parameter) => { this.setUserCredential(); },
                (parameter) => { return this.CanCnangePassword; }
                ).AddListener<UserConfigViewModel>(this, x => x.CanCnangePassword);

            SettingCommand = new MvvmCommand(
                (parameter) => { this.changeSettings(); },
                (parameter) => { return true; }
                );

            ThemCommand = new MvvmCommand(
                (parameter) => { this.changeTheme(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IPersonService _personService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        #endregion
    }
}
