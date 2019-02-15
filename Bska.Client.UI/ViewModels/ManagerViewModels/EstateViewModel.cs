
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Windows.Input;
    using System.Xml.Linq;
    using System.Linq;
    using Bska.Client.UI.Helper;
    using System.ComponentModel;
    using System.Windows.Data;

    public sealed class EstateViewModel : BaseListViewModel<Estate>
    {
        #region ctor

        public EstateViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Estate>>())
        {
            this._container = container;
            _unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            _employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this.EstateFilterView = new CollectionViewSource { Source = Collection }.View;
            this.initializObj();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public EstateDetailsViewModel EstateDetailsVM
        {
            get { return _estateDetailsVM; }
            set
            {
                _estateDetailsVM = value;
                if (value != null)
                {
                    this.EstateDetailsVM.State = value.State;
                    this.EstateDetailsVM.RegistryOffice = value.RegistryOffice;
                    this.EstateDetailsVM.SectionRecords = value.SectionRecords;
                    this.EstateDetailsVM.AreaRecords = value.AreaRecords;
                }
                OnPropertyChanged("EstateDetailsVM");
            }
        }

        public ICollectionView EstateFilterView { get; set; }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SearchCollection();
            }
        }

        #endregion

        #region methods

        public override void SelectedItemChanged()
        {
            if (Selected != null)
            {
                this.EstateDetailsVM = (EstateDetailsViewModel)Selected;
                UserLog.UniqueInstance.LastView("estate", Selected.CurrentEntity.ImAssetId.ToString());
            }
        }

        private void SearchCollection()
        {
            this.EstateFilterView.Filter = (obj) =>
            {
                var cus = (EstateDetailsViewModel)obj;
                return cus.Name.Contains(SearchCriteria);
            };
        }

        private void initializObj()
        {
            Collection.Clear();
            this.EstateDetailsVM = new EstateDetailsViewModel(new Estate()) { Text = "", Address = "", State = "", Area = 0, Latitude = 0,
                Longitude = 0, RegistryOffice = "", Type = EstateType.FarmLand };


            _employeeService.Query().Include(x => x.Estates)
                .Select().SelectMany(x => x.Estates).ToList().ForEach(e =>
                {
                    Collection.Add(new EstateDetailsViewModel(e));
                });

            //init for last view
            var lastStore = UserLog.UniqueInstance.LastView("estate", null);
            int temp = 0;
            int.TryParse(lastStore, out temp);
            if (temp > 0)
            {
                var item = Collection.FirstOrDefault(s => s.CurrentEntity.ImAssetId == temp);
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

        private void SaveEstate()
        {
            if (this.EstateDetailsVM.HasErrors)
            {
                _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                return;
            }
            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }
            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (Selected == null)
                {
                    EstateDetailsVM.CurrentEntity.ObjectState = ObjectState.Added;
                    EstateDetailsVM.CurrentEntity.InsertDate = GlobalClass._Today;
                    EstateDetailsVM.CurrentEntity.ModeifiedDate = GlobalClass._Today;
                    EstateDetailsVM.CurrentEntity.Name = employee.Name + "-" + "املاک";
                }
                else
                {
                    EstateDetailsVM.CurrentEntity.ObjectState = ObjectState.Modified;
                    EstateDetailsVM.CurrentEntity.ModeifiedDate = GlobalClass._Today;
                }
                employee.Estates.Add(EstateDetailsVM.CurrentEntity);
                try
                {
                    _employeeService.Update(employee);
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    if (Selected == null)
                    {
                        Collection.Add(new EstateDetailsViewModel(EstateDetailsVM.CurrentEntity));
                    }
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

        private void DeleteEstate()
        {
            if (Selected == null || this.HasErrors)
            {
                _dialogService.ShowAlert("انتخاب ساختمان", "هیچ ملکی انتخاب نشده است");
                return;
            }
            var employee = _employeeService.Query().Include(x => x.Estates).Select().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            bool IsDeleteing = _dialogService.AskConfirmation("حذف ملک", ErrorMessages.Default.ConfirmDelete);
            if (IsDeleteing)
            {
                this.EstateDetailsVM.CurrentEntity.ObjectState = ObjectState.Deleted;
                employee.Estates.Remove(this.EstateDetailsVM.CurrentEntity);
                _employeeService.Update(employee);
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.Collection.Remove(this.Selected);
                    this.initializObj();
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

        private void showHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110007");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initalizCommand()
        {
            SaveCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SaveEstate();
                },
                (parameter) => { return true; }
                );

            NewCommand = new MvvmCommand(
                (paramter) =>
                {
                    this.initializObj();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            DeleteCommand = new MvvmCommand(
                (paramter) =>
                {
                    this.DeleteEstate();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            HelpCommand = new MvvmCommand((parameter) => { this.showHelp(); }, (parameter) => { return true; });
        }

        #endregion

        #region fields

        private EstateDetailsViewModel _estateDetailsVM;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;

        #endregion
    }

    public sealed class EstateDetailsViewModel : BaseDetailsViewModel<Estate>
    {
        #region ctor

        public EstateDetailsViewModel(Estate currentEntity)
            : base(currentEntity)
        {
            this._seedDataApi = new SeedDataHelper();
            this.GetProvinces();
        }

        #endregion

        #region properties

        public Int32 EstateId
        {
            get { return CurrentEntity.ImAssetId; }
        }

        public String Name
        {
            get { return CurrentEntity.Name; }
        }

        [Required(ErrorMessage = "استان الزامی است")]
        public String State
        {
            get { return CurrentEntity.State; }
            set
            {
                CurrentEntity.State = value;
                ValidateProperty(value);
                OnPropertyChanged("State");
                this.GetOrginalOffices(value);
            }
        }

        [Required(ErrorMessage = "واحد ثبتی الزامی است")]
        public String RegistryOffice
        {
            get { return CurrentEntity.RegistryOffice; }
            set
            {
                CurrentEntity.RegistryOffice = value;
                ValidateProperty(value);
                OnPropertyChanged("RegistryOffice");
            }
        }

        public String SectionRecords
        {
            get { return CurrentEntity.SectionRecords; }
            set
            {
                CurrentEntity.SectionRecords = value;
                OnPropertyChanged("SectionRecords");
            }
        }

        public String AreaRecords
        {
            get { return CurrentEntity.AreaRecords; }
            set
            {
                CurrentEntity.AreaRecords = value;
                OnPropertyChanged("AreaRecords");
            }
        }

        public String OriginalPlaque
        {
            get { return CurrentEntity.OriginalPlaque; }
            set
            {
                CurrentEntity.OriginalPlaque = value;
                OnPropertyChanged("OriginalPlaque");
            }
        }

        public String MinorPlaque
        {
            get { return CurrentEntity.MinorPlaque; }
            set
            {
                CurrentEntity.MinorPlaque = value;
                OnPropertyChanged("MinorPlaque");
            }
        }

        [Required(ErrorMessage = "نوع ملک الزامی است")]
        public EstateType Type
        {
            get { return CurrentEntity.Type; }
            set
            {
                CurrentEntity.Type = value;
                ValidateProperty(value);
                OnPropertyChanged("Type");
            }
        }

        public String BookNo
        {
            get { return CurrentEntity.BookNo; }
            set
            {
                CurrentEntity.BookNo = value;
                OnPropertyChanged("BookNo");
            }
        }

        public String PageNumber
        {
            get { return CurrentEntity.PageNumber; }
            set
            {
                CurrentEntity.PageNumber = value;
                OnPropertyChanged("PageNumber");
            }
        }

        [Required(ErrorMessage = "متن الزامی است")]
        public String Text
        {
            get { return CurrentEntity.Text; }
            set
            {
                CurrentEntity.Text = value;
                ValidateProperty(value);
                OnPropertyChanged("Text");
            }
        }

        [Required(ErrorMessage = "آدرس الزامی است")]
        public String Address
        {
            get { return CurrentEntity.Address; }
            set
            {
                CurrentEntity.Address = value;
                ValidateProperty(value);
                OnPropertyChanged("Address");
            }
        }

        public String PostalCode
        {
            get { return CurrentEntity.PostalCode; }
            set
            {
                CurrentEntity.PostalCode = value;
                OnPropertyChanged("PostalCode");
            }
        }

        [Required(ErrorMessage = "مساحت الزامی است")]
        [PositiveNumberAttribute(ErrorMessage = "مقدار وارد شده نامعتبر است")]
        public Double Area
        {
            get { return CurrentEntity.Area; }
            set
            {
                CurrentEntity.Area = value;
                ValidateProperty(value);
                OnPropertyChanged("Area");
            }
        }

        [Required(ErrorMessage = "طول جغرافیایی الزامی است")]
        [PositiveNumberAttribute(ErrorMessage = "مقدار وارد شده نامعتبر است")]
        public float Longitude
        {
            get { return CurrentEntity.Longitude; }
            set
            {
                CurrentEntity.Longitude = value;
                ValidateProperty(value);
                OnPropertyChanged("Longitude");
            }
        }

        [Required(ErrorMessage = "عرض جغرافیایی الزامی است")]
        [PositiveNumberAttribute(ErrorMessage = "مقدار وارد شده نامعتبر است")]
        public float Latitude
        {
            get { return CurrentEntity.Latitude; }
            set
            {
                CurrentEntity.Latitude = value;
                ValidateProperty(value);
                OnPropertyChanged("Latitude");
            }
        }

        public List<Province> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                OnPropertyChanged("Provinces");
            }
        }

        public List<EstateOrgan> OriginalOffices
        {
            get { return _orginalOffice; }
            set
            {
                _orginalOffice = value;
                OnPropertyChanged("OriginalOffices");
            }
        }

        #endregion

        #region methods

        private void GetProvinces()
        {
            try
            {
                Provinces = _seedDataApi.GetProvinces();
            }
            catch (Exception ex) { throw ex; }
        }

        private void GetOrginalOffices(string provinceId)
        {
            if (string.IsNullOrEmpty(provinceId))
            {
                return;
            }
            try
            {
                OriginalOffices = _seedDataApi.GetEstateOrganbyProvinceId(provinceId);
            }
            catch (Exception ex) { throw ex; }
        }

        #endregion

        #region commands
        #endregion

        #region fields

        private List<Province> _provinces;
        private List<EstateOrgan> _orginalOffice;
        private readonly SeedDataHelper _seedDataApi;

        #endregion
    }
}
