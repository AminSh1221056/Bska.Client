
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using System.IO;
    using System.Text.RegularExpressions;

    public sealed class PersonListViewModel : BaseListViewModel<Person>
    {
        #region ctor

        public PersonListViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Person>>())
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._employeeService = container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._personService = container.Resolve<IPersonService>(new ParameterOverride("repository", _unitOfWork.Repository<Person>()));
            this.PersonFilterView = new CollectionViewSource { Source = Collection }.View;

            this.initalizObj();
            this.initalizCommands();
            this.GetAllPerson();
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

        public ICollectionView PersonFilterView { get; set; }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SearchPerson();
            }
        }

        public Boolean EnableControlls
        {
            get { return GetValue(() => EnableControlls); }
            set
            {
                SetValue(() => EnableControlls, value);
            }
        }

        public String FilePath
        {
            get { return GetValue(() => FilePath); }
            set
            {
                SetValue(() => FilePath, value);
                if (performHeaderAccuracy())
                {

                }
                else
                {
                    _dialogService.ShowAlert("خطا", "تعداد ستونهای فایل انتخاب شده نا معتبر است");
                }
            }
        }

        #endregion

        #region methods

        public override void SelectedItemChanged()
        {
            if (Selected != null)
            {
                this.PersonDetailsVM = (PersonDetailsViewModel)Selected;
                this.EnableControlls = true;
                UserLog.UniqueInstance.LastView("person", Selected.CurrentEntity.PersonId.ToString());
            }
        }

        private void initalizObj()
        {
            this.Selected = null;
            this.PersonDetailsVM = new PersonDetailsViewModel(new Person()) { FirstName = "", LastName = "", NationalId = "" };
        }

        private void GetAllPerson()
        {
            this.Collection.Clear();
            var persons = _employeeService.Query()
                .Include(x => x.Persons).Select().SelectMany(x => x.Persons)
                .Where(x=>x.PersonId!=1).ToList();

            foreach (var person in persons)
            {
                Collection.Add(new PersonDetailsViewModel(person));
            }
            //init for last view
            var lastStore = UserLog.UniqueInstance.LastView("person", null);
            int temp = 0;
            int.TryParse(lastStore, out temp);
            if (temp > 0)
            {
                var item = Collection.FirstOrDefault(s => s.CurrentEntity.PersonId == temp);
                if (item != null)
                {
                    this.Selected = item;
                }
            }
            else
            {
                Selected = Collection.FirstOrDefault();
            }
        }

        private void SearchPerson()
        {
            this.PersonFilterView.Filter = (obj) =>
            {
                var cus = (PersonDetailsViewModel)obj;
                return cus.NationalId.Contains(SearchCriteria) || string.Format("{0} {1}", cus.FirstName, cus.LastName).Contains(SearchCriteria);
            };
        }

        private void NewPerson()
        {
            this.initalizObj();
            this.EnableControlls = true;
        }

        private void SavePeson()
        {
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
                if (Collection.Any(x => x.CurrentEntity.NationalId == PersonDetailsVM.NationalId))
                {
                    _dialogService.ShowError("خطا", "کد ملی وارد شده تکراری است");
                    return;
                }

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
                _unitOfWork.SaveChanges();
                Mouse.SetCursor(Cursors.Arrow);
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);

                if (Selected == null)
                {
                    Collection.Add(new PersonDetailsViewModel(PersonDetailsVM.CurrentEntity));
                }
                this.initalizObj();
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

        private void DeletePerson()
        {
            if (Selected == null)
            {
                _dialogService.ShowAlert("انتخاب پرسنل", "هیچ پرسنلی انتخاب نشده است");
                return;
            }
            bool IsDeleteing = _dialogService.AskConfirmation("حذف پرسنل", ErrorMessages.Default.ConfirmDelete);
            if (IsDeleteing)
            {
                Selected.CurrentEntity.ObjectState = ObjectState.Deleted;
                Selected.CurrentEntity.Employee = null;
                _personService.Delete(Selected.CurrentEntity);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.Collection.Remove(this.Selected);
                    this.initalizObj();
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

        private bool performHeaderAccuracy()
        {
            if (string.IsNullOrWhiteSpace(this.FilePath)) return false;

            string text = File.ReadAllText(this.FilePath);

            Regex.Matches(text, "\"[A-Za-z ., ()'-/]+\"").Cast<Match>()
           .Select(m => m.Value)
           .ToList()
           .ForEach(z => text = text.Replace(z, z.Replace(",", "[__COMMA__]")));

            int headerCount = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(t => t.Split(',')).First().ToList().Count;
            return headerCount == 6;
        }

        private void showHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110008");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
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

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeletePerson();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            NewCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.NewPerson();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            CancelCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.EnableControlls = false;
                    this.Selected = null;
                },
                (parameter) =>
                {
                    return true;
                }
                );

            HelpCommand = new MvvmCommand((paremeter) => { this.showHelp(); }, (paremeter) => { return true; });
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private PersonDetailsViewModel _personDetailsVM;
        private readonly IPersonService _personService;

        #endregion
    }

    public sealed class PersonDetailsViewModel : BaseDetailsViewModel<Person>
    {
        #region ctor

        public PersonDetailsViewModel(Person currentEntity)
            : base(currentEntity)
        {
        }

        #endregion

        #region properties

        public Int32 PersonId
        {
            get { return CurrentEntity.PersonId; }
        }

        [Required(ErrorMessage = "نام پرسنل الزامی است")]
        public String FirstName
        {
            get { return CurrentEntity.FirstName; }
            set
            {
                CurrentEntity.FirstName = value;
                ValidateProperty(value);
                OnPropertyChanged("FirstName");
            }
        }

        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        public String LastName
        {
            get { return CurrentEntity.LastName; }
            set
            {
                CurrentEntity.LastName = value;
                ValidateProperty(value);
                OnPropertyChanged("LastName");
            }
        }

        [Required(ErrorMessage = "کد ملی الزامی است")]
        [NationalCode(ErrorMessage = "کد ملی وارد شده نامعتبر می باشد")]
        public String NationalId
        {
            get { return CurrentEntity.NationalId; }
            set
            {
                CurrentEntity.NationalId = value;
                ValidateProperty(value);
                OnPropertyChanged("NationalId");
            }
        }

        public String PersonCode
        {
            get { return CurrentEntity.PersonCode; }
            set
            {
                CurrentEntity.PersonCode = value;
                OnPropertyChanged("PersonCode");
            }
        }

        [Mobile(ErrorMessage = "شماره موبایل وارد شده نامعتبر می باشد")]
        public String Mobile
        {
            get { return CurrentEntity.Mobile; }
            set
            {
                CurrentEntity.Mobile = value;
                ValidateProperty(value);
                OnPropertyChanged("Mobile");
            }
        }

        [Required(ErrorMessage = "نوع قرارداد الزامی می باشد")]
        public ContractType Contract
        {
            get { return CurrentEntity.ContractcType; }
            set
            {
                CurrentEntity.ContractcType = value;
                OnPropertyChanged("Contract");
            }
        }

        public string FatherName
        {
            get { return CurrentEntity.FatherName; }
            set
            {
                CurrentEntity.FatherName = value;
                OnPropertyChanged("FatherName");
            }
        }

        public string Postalcode
        {
            get { return CurrentEntity.Postalcode; }
            set
            {
                CurrentEntity.Postalcode = value;
                OnPropertyChanged("Postalcode");
            }
        }

        public Boolean Married
        {
            get { return CurrentEntity.Married; }
            set
            {
                CurrentEntity.Married = value;
                OnPropertyChanged("Married");
            }
        }

        public String AddressLine
        {
            get { return CurrentEntity.AddressLine; }
            set
            {
                CurrentEntity.AddressLine = value;
                OnPropertyChanged("Married");
            }
        }

        public Byte[] Photo
        {
            get { return CurrentEntity.Photo; }
            set
            {
                CurrentEntity.Photo = value;
                OnPropertyChanged("Photo");
            }
        }

        public PersianDate? BirthDate
        {
            get { return CurrentEntity.BirthDate?.PersianDateTime(); }
            set
            {
                CurrentEntity.BirthDate = value?.ToDateTime();
                OnPropertyChanged("BirthDate");
            }
        }

        public PersianDate? EmployeeDate
        {
            get { return CurrentEntity.EmployeeDate?.PersianDateTime(); }
            set
            {
                CurrentEntity.EmployeeDate = value?.ToDateTime();
                OnPropertyChanged("EmployeeDate");
            }
        }

        #endregion

    }
}
