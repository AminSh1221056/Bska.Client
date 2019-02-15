
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.UserControlls;
    using Microsoft.Practices.Unity;
    using System;
    using System.Windows.Input;
    using System.Linq;
    using System.Windows;
    using System.Security.Principal;
    using Domain.Entity;
    using Client.API.Infrastructure;
    using Domain.Entity.StoredProcedures;

    public sealed class LoginWindowViewModel : BaseViewModel
    {
        #region ctor

        public LoginWindowViewModel(IUnityContainer container)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this.initializeCommands();
        }

        #endregion

        #region properties

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
            set { SetValue(() => Password, value); }
        }

        #endregion

        #region methods

        private void ExecuteSubmit(object commandParameter)
        {
            Mouse.SetCursor(Cursors.Wait);
            var accessControlSystem = commandParameter as SmartLoginOverlay;
            if (accessControlSystem != null)
            {
                String passwordHash = GlobalClass.DecryptStringAES(APPSettings.Default.ConfigPass, "66Ak679Du4V3qo92");

                if (this.UserName.Equals(APPSettings.Default.ConfigUserName, StringComparison.InvariantCultureIgnoreCase) && passwordHash == this.Password)
                {
                    _navigationService.ShowConfigWindow();
                    return;
                }
                else
                {
                    if (this.validateUser(this.UserName, GlobalClass.GetMd5Hash(this.Password)) == true)
                    {
                        accessControlSystem.Unlock();
                        LoginWindow parentWindow = Window.GetWindow(accessControlSystem) as LoginWindow;
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                        IIdentity identity = new GenericIdentity(this.UserName);
                        IPrincipal principal = new GenericPrincipal(identity, new[] { UserLog.UniqueInstance.LogedUser.PermissionType.ToString() });
                        currentDomain.SetThreadPrincipal(principal);
                        parentWindow.DialogResult = true;
                    }
                    else
                    {
                        accessControlSystem.ShowWrongCredentialsMessage();
                    }
                }
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        //comment on deploy
        public void excecuteFackeUser()
        {
            if (this.validateUser("Manager", GlobalClass.GetMd5Hash("Manager")) == true)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                IIdentity identity = new GenericIdentity("Manager");
                IPrincipal principal = new GenericPrincipal(identity, new[] { UserLog.UniqueInstance.LogedUser.PermissionType.ToString() });
                currentDomain.SetThreadPrincipal(principal);
            }
        }

        private bool validateUser(string username, string passwordHash)
        {
            var user = _personService.LoginToBeska(username, passwordHash);
            if (user != null)
            {
                UserLog.UniqueInstance.LogedUser = user;
                UserLog.UniqueInstance.IBskaStoredProc = _container.Resolve<IBskaStoredProcedures>();
                UserLog.UniqueInstance.AddLog(new EventLog
                {
                    EntryDate = GlobalClass._Today,
                    Type = EventType.LogIn,
                    ObjectState = ObjectState.Added,
                    UserId=user.UserId
                });
                UserLog.UniqueInstance.LogedEmployee = _employeeService.Queryable().FirstOrDefault();
            }
            return user != null;
        }

        #endregion

        #region commands

        public ICommand SubmitCommand { get; private set; }
        private void initializeCommands()
        {
            this.SubmitCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.ExecuteSubmit(parameter);
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
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;

        #endregion
    }
}
