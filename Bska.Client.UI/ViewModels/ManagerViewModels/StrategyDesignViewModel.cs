
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.UI.Services;
    using Bska.Client.Data.Service;
    using Bska.Client.UI.API;
    using Bska.Client.Common;
    using Microsoft.Practices.Unity;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Windows.Input;
    using System.Xml.Linq;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using Helper;
    using System.Windows.Data;
    using System.ComponentModel;

    public sealed class BuildingListViewModel : BaseListViewModel<Building>
    {
        #region ctor

        public BuildingListViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Building>>())
        {
            _container = container;
            _unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            _dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            _employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            _buildingService = _container.Resolve<IBuildingService>(new ParameterOverride("repository", _unitOfWork.Repository<Building>()));
            this._firstGeneration = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this.BuildingFilteredView = new CollectionViewSource { Source = Collection }.View;
            this.GetParentNode();
            this.InitalizCommands();
            IsEditable = true;
        }

        #endregion

        #region properties
        public ICollectionView BuildingFilteredView { get; set; }
        public BuildingDetailsViewModel BuildingDetailsViewModel
        {
            get { return _buildingDetailsVm; }
            set
            {
                _buildingDetailsVm = value;
                if (value != null)
                {
                    this.BuildingDetailsViewModel.SelectedProvince = value.SelectedProvince;
                    this.BuildingDetailsViewModel.SelectedTwonShip = value.SelectedTwonShip;
                    this.BuildingDetailsViewModel.SelectedZone = value.SelectedZone;
                    this.BuildingDetailsViewModel.SelectedCity = value.SelectedCity;
                }

                OnPropertyChanged("BuildingDetailsViewModel");
            }
        }

        public bool DetailsEnabled
        {
            get { return GetValue(() => DetailsEnabled); }
            set
            {
                SetValue(() => DetailsEnabled, value);
            }
        }

        public EmployeeDesignTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
            }
        }
        
        public ObservableCollection<EmployeeDesignTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }
        
        public Boolean IsEditable
        {
            get { return GetValue(() => IsEditable); }
            set
            {
                SetValue(() => IsEditable, value);
            }
        }

        #endregion

        #region methods

        private void GetParentNode()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.SelectedNode = null;
            _firstGeneration.Clear();
            this.Collection.Clear();
            var nodes = _employeeService.GetParnetNodeToBuilding(2);
            nodes.Where(nd => nd.ParentNode == null).ToList().OfType<StrategyDesign>().ForEach(nd =>
            {
                _rootNode = new EmployeeDesignTreeViewModel(nd, null);
                _firstGeneration.Add(_rootNode);
            });
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void initDetails()
        {
            if (Selected==null)
            {
                this.BuildingDetailsViewModel = new BuildingDetailsViewModel(new Building() {ObjectState=ObjectState.Added,CreateDate=GlobalClass._Today })
                {
                    Name = SelectedNode?.Name,
                    SelectedZone = "",
                    SelectedProvince = "",
                    SelectedTwonShip = "",
                    SelectedCity = "",
                    MainStreet = "",
                };
            }
        }
        
        public override void SelectedItemChanged()
        {
            //nothing
        }

        private void SaveBuilding()
        {
            if (this.BuildingDetailsViewModel.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (SelectedNode == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ شاخه ای انتخاب نشده است");
                return;
            }

            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }
            
            ((StrategyDesign)SelectedNode.BuildingDesignCurrent).Building = BuildingDetailsViewModel.CurrentEntity;
            employee.EmployeeDesign.Add(SelectedNode.BuildingDesignCurrent);
            _employeeService.InsertOrUpdateGraph(employee);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                Mouse.SetCursor(Cursors.Arrow);
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                this.initDetails();
                DetailsEnabled = false;
                this.GetParentNode();
            }
            catch (DbUpdateException ex)
            {
                _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DeleteBuilding()
        {
            if (this.HasErrors)
            {
                _dialogService.ShowAlert("انتخاب ساختمان", "لطفا ورودی های خود را کنترل کنید");
                return;
            }

            bool IsDeleteing = _dialogService.AskConfirmation("حذف ساختمان", ErrorMessages.Default.ConfirmDelete);
            if (IsDeleteing)
            {
                BuildingDetailsViewModel.CurrentEntity.ObjectState = ObjectState.Deleted;
                _buildingService.Delete(BuildingDetailsViewModel.CurrentEntity);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.initDetails();
                    DetailsEnabled = false;
                    this.GetParentNode();
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

        private void SaveNode(object parameter)
        {
            var treeItem = parameter as EmployeeDesignTreeViewModel;
            if (treeItem == null) return;
            this.SelectedNode = treeItem;

            var employee = _employeeService.Queryable().FirstOrDefault();
            if (employee == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ سازمانی یافت نشد");
                return;
            }
            var newNode = new StrategyDesign { Name ="",Code="", ObjectState = ObjectState.Added, ParentNode = SelectedNode != null ? SelectedNode.BuildingDesignCurrent : null };

            SelectedNode.Children.Add(new EmployeeDesignTreeViewModel(newNode, SelectedNode) { IsSelected = true,IsEditing=true});
            SelectedNode.IsExpanded = true;
        }

        private void EditNode()
        {
            if (SelectedNode == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedNode.Name))
            {
                _dialogService.ShowAlert("هشدار", "نام شاخه را وارد کنید");
                return;
            }
            
            if (SelectedNode.BuildingDesignCurrent.ObjectState != ObjectState.Added)
            {
                SelectedNode.BuildingDesignCurrent.ObjectState = ObjectState.Modified;
            }
            
            var employee = _employeeService.Queryable().FirstOrDefault();
            employee.EmployeeDesign.Add(SelectedNode.BuildingDesignCurrent);
            _employeeService.InsertOrUpdateGraph(employee);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                SelectedNode.IsSelected = false;
                SelectedNode.IsEditing = false;
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

        private void DeleteNode(object parameter)
        {
            var treeItem = parameter as EmployeeDesignTreeViewModel;
            if (treeItem == null) return;
            this.SelectedNode = treeItem;
            if(treeItem.BuildingDesignCurrent.BuidldingDesignId==1 
                || treeItem.BuildingDesignCurrent.BuidldingDesignId == 2)
            {
                _dialogService.ShowAlert("توجه", "شاخه های پیش فرض قابلیت حذف ندارند");
                return;
            }

            if (SelectedNode.BuildingDesignCurrent.ChildNode.Any())
            {
                _dialogService.ShowAlert("توجه", "بخش انتخابي داري زير مجموعه مي باشد.ابتدا بايد زير مجموعه حذف شود");
                return;
            }

            Boolean canDelete = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.ConfirmDelete);
            if (canDelete)
            {
                SelectedNode.BuildingDesignCurrent.ObjectState = ObjectState.Deleted;
                var employee = _employeeService.Queryable().FirstOrDefault();
                employee.EmployeeDesign.Remove(SelectedNode.BuildingDesignCurrent);
                _employeeService.InsertOrUpdateGraph(employee);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    var parent = SelectedNode.Parent;
                    if (parent == null)
                    {
                        _firstGeneration.Remove(SelectedNode);
                    }
                    else
                    {
                        parent.Children.Remove(SelectedNode);
                    }
                    this.SelectedNode = null;
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

        private void detailsConfig(object parameter)
        {
            var treeItem = parameter as EmployeeDesignTreeViewModel;
            if (treeItem == null) return;
            this.SelectedNode = treeItem;
            this.SelectedNode.IsSelected = true;
            if (((StrategyDesign)treeItem.BuildingDesignCurrent).Building != null)
            {
                var building = ((StrategyDesign)treeItem.BuildingDesignCurrent).Building;
                building.ObjectState = ObjectState.Modified;
                this.BuildingDetailsViewModel =new BuildingDetailsViewModel(((StrategyDesign)treeItem.BuildingDesignCurrent).Building);
            }
            else
            {
                initDetails();
            }
            this.DetailsEnabled = true;
        }

        private void showHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110003");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand NewTreeCommand { get; private set; }
        public ICommand DeleteTreeCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        
        private void InitalizCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SaveBuilding();
                },
                (parameter) =>
                {
                    return true;
                }
                );
            
            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeleteBuilding();
                },
                (parameter) =>
                {
                    return true;
                }
                );
            
            NewTreeCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SaveNode(parameter);
                },
                (parameter) =>
                {
                    return true;
                }
                );

            EditCommand = new MvvmCommand(
                (parameter) => {
                    this.EditNode();
                },
                (parameter) => { return SelectedNode != null; }).AddListener<BuildingListViewModel>(this, x => x.SelectedNode);

            DeleteTreeCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeleteNode(parameter);
                },
                (parameter) => { return true; });

            DetailsCommand = new MvvmCommand(
                (parameter) => {
                    this.detailsConfig(parameter);
                },
                (parameter) => { return true; }
                );

            HelpCommand = new MvvmCommand(
                (parameter) => { this.showHelp(); },
                (parameter) => { return true; });
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private readonly IBuildingService _buildingService;
        private BuildingDetailsViewModel _buildingDetailsVm;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _firstGeneration;
        EmployeeDesignTreeViewModel _rootNode;
      
        #endregion
    }

    public sealed class BuildingDetailsViewModel : BaseDetailsViewModel<Building>
    {
        #region ctor

        public BuildingDetailsViewModel(Building currentEntity)
            : base(currentEntity)
        {
            this.Provinces = new List<Province>();
            this.Zones = new List<Zone>();
            this.Cities = new List<City>();
            this.TwonShips = new List<TwonShip>();
            _helper = new SeedDataHelper();
            this.initalizObj();
        }

        #endregion

        #region properties

        [Required(ErrorMessage = "نام ساختمان الزامی است")]
        public String Name
        {
            get { return CurrentEntity.Name; }
            set
            {
                CurrentEntity.Name = value;
                ValidateProperty(value);
                OnPropertyChanged("Name");
            }
        }

        [Required(ErrorMessage = "نام استان الزامی است")]
        public String SelectedProvince
        {
            get { return CurrentEntity.Province; }
            set
            {
                CurrentEntity.Province = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedProvince");
              this.TwonShips=  _helper.GetTwonShips(value);
            }
        }

        [Required(ErrorMessage = "نام شهرستان الزامی است")]
        public String SelectedTwonShip
        {
            get { return CurrentEntity.TownShip; }
            set
            {
                CurrentEntity.TownShip = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedTwonShip");
              this.Zones=  _helper.GetZones(value);
            }
        }

        [Required(ErrorMessage = "نام بخش الزامی است")]
        public String SelectedZone
        {
            get { return CurrentEntity.Zone; }
            set
            {
                CurrentEntity.Zone = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedZone");
              this.Cities=  _helper.GetCities(value);
            }
        }

        [Required(ErrorMessage = "نام شهر الزامی است")]
        public String SelectedCity
        {
            get { return CurrentEntity.City; }
            set
            {
                CurrentEntity.City = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedCity");
            }
        }

        public String District
        {
            get { return CurrentEntity.District; }
            set
            {
                CurrentEntity.District = value;
                OnPropertyChanged("District");
            }
        }

        [Required(ErrorMessage = "خیابان اصلی الزامی است")]
        public String MainStreet
        {
            get { return CurrentEntity.MainStreet; }
            set
            {
                CurrentEntity.MainStreet = value;
                ValidateProperty(value);
                OnPropertyChanged("MainStreet");
            }
        }

        public String SecondaryStreet
        {
            get { return CurrentEntity.SecondaryStreet; }
            set
            {
                CurrentEntity.SecondaryStreet = value;
                OnPropertyChanged("SecondaryStreet");
            }
        }

        public String Alley
        {
            get { return CurrentEntity.Alley; }
            set
            {
                CurrentEntity.Alley = value;
                OnPropertyChanged("Alley");
            }
        }

        public String SecondaryAlley
        {
            get { return CurrentEntity.SecondaryStreet; }
            set
            {
                CurrentEntity.SecondaryAlley = value;
                OnPropertyChanged("SecondaryAlley");
            }
        }

        public String OldPlaque
        {
            get { return CurrentEntity.OldPlaque; }
            set
            {
                CurrentEntity.OldPlaque = value;
                OnPropertyChanged("OldPlaque");
            }
        }

        public String NewPlaque
        {
            get { return CurrentEntity.NewPlaque; }
            set
            {
                CurrentEntity.NewPlaque = value;
                OnPropertyChanged("NewPlaque");
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

        public List<Province> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                OnPropertyChanged("Provinces");
            }
        }

        public List<City> Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                OnPropertyChanged("Cities");
            }
        }

        public List<TwonShip> TwonShips
        {
            get { return _townShips; }
            set
            {
                _townShips = value;
                OnPropertyChanged("TwonShips");
            }
        }

        public List<Zone> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
          this.Provinces=  _helper.GetProvinces();
        }
        #endregion

        #region fields

        private List<Province> _provinces;
        private List<City> _cities;
        private List<TwonShip> _townShips;
        private List<Zone> _zones;
        private SeedDataHelper _helper;

        #endregion
    }
}
