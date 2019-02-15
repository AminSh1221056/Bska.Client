
namespace Bska.Client.UI.ViewModels
{
    using Data.Service;
    using Domain.Entity;
    using Helper;
    using Microsoft.Practices.Unity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public sealed class EventLogViewModel : BaseViewModel
    {
        #region ctor

        public EventLogViewModel(IUnityContainer container)
        {
            this._container = container;
            this._personService = _container.Resolve<IPersonService>();
            this.initalizObj();
        }
        #endregion

        #region properties

        public List<Users> UserList
        {
            get { return GetValue(() => UserList); }
            set
            {
                SetValue(() => UserList, value);
            }
        }

        public Users CurrentUser
        {
            get { return GetValue(() => CurrentUser); }
            set
            {
                SetValue(() => CurrentUser, value);
                this.getEvents();
            }
        }

        public List<EventLog> Events
        {
            get { return GetValue(() => Events); }
            set
            {
                SetValue(() => Events, value);
            }
        }
        
        #endregion

        #region methods

        private void initalizObj()
        {
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                UserList = _personService.GetUsers(true).ToList();
                CurrentUser = UserList.First(u => u.UserName == UserLog.UniqueInstance.LogedUser.UserName);
            }
            else
            {
                UserList =new List<Users> { _personService.GetUser(UserLog.UniqueInstance.LogedUser.UserId) };
                CurrentUser = UserList.First();
            }
        }

        private void getEvents()
        {
            if (CurrentUser != null)
            {
                Events = _personService.GetEvents(CurrentUser.UserId).ToList();
            }
        }
        #endregion

        #region commands
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IPersonService _personService;

        #endregion
    }
}
