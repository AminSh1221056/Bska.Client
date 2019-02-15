
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;
    using System.Linq;
    public sealed class UserManageViewModel : BaseViewModel
    {
        #region ctor

        public UserManageViewModel(IUnityContainer container)
        {
            this._container = container;
            this._navigationService = container.Resolve<INavigationService>();
            this._dialogService = container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._personService = container.Resolve<IPersonService>(new ParameterOverride("repository", _unitOfWork.Repository<Person>()));
            this._allUser = new List<Users>();
            this.initalizObj();
            this.initalizCommnads();
        }

        #endregion

        #region properties

        public UserAttribute CurrentUser
        {
            get { return GetValue(() => CurrentUser); }
            set
            {
                SetValue(() => CurrentUser, value);
            }
        }
        public Users Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
                this.initControlls();
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

        #endregion

        #region methods
        private void initalizObj()
        {
            this.AllUser = _personService.GetUsers()
                .OrderBy(u=>u.UserName).ToList();
            this.Selected = AllUser.FirstOrDefault();
        }
        private void initControlls()
        {
            if (Selected == null) return;
            var attribute = Selected.UserAttribute;
            if (attribute != null)
            {
                _isNew = false;
                CurrentUser = attribute;
            }
            else
            {
                _isNew = true;
                CurrentUser = new UserAttribute();
            }
        }

        private void SaveAttribute()
        {
            if (Selected == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کاربری انتخاب نشده است");
                return;
            }
            var person = _personService.Find(Selected.PersonId);
            if (person == null)
            {
                _dialogService.ShowError("خطا", "هیچ پرسنلی یافت نشد");
                return;
            }

            if (_isNew) CurrentUser.ObjectState = ObjectState.Added;
            else CurrentUser.ObjectState = ObjectState.Modified;

            Selected.UserAttribute = CurrentUser;
            Selected.ObjectState = ObjectState.Modified;

            person.Users.Add(Selected);
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
        }
        private void showDocHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110010-3");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands
        public ICommand SaveCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        private void initalizCommnads()
        {
            SaveCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SaveAttribute();
                },
                (parameter) =>
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
        private List<Users> _allUser;
        private Boolean _isNew = true;

        #endregion
    }
}
