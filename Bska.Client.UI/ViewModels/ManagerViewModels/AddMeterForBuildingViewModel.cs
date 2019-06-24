
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.MetersViewModels;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Windows.Data;
    using Bska.Client.UI.Helper;

    public sealed class AddMeterForBuildingViewModel : BaseListViewModel<Meter>
    {
        #region ctor

        public AddMeterForBuildingViewModel(IUnityContainer container)
            :base(new List<BaseDetailsViewModel<Meter>>())
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._buildingService = _container.Resolve<IBuildingService>(new ParameterOverride("repository", _unitOfWork.Repository<Building>()));
            this._employeeSrvice = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._metersCollection = new ObservableCollection<Meter>();
            this._firstGeneration = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this.MeterFilterView = new CollectionViewSource { Source = MeterCollection }.View;
            this.initialozObj();
            this.initializCommand();
        }

        #endregion

        #region properties
        
        public String MeterType
        {
            get { return GetValue(() => MeterType); }
            set
            {
                SetValue(() => MeterType, value);
                initializVM();
            }
        }

        public ObservableCollection<Meter> MeterCollection
        {
            get { return _metersCollection; }
        }

        public ICollectionView MeterFilterView { get; set; }

        public Meter SelectedMeter
        {
            get { return GetValue(() => SelectedMeter); }
            set
            {
                SetValue(() => SelectedMeter, value);
                this.setVM();
            }
        }

        public GasViewModel GasVM
        {
            get { return GetValue(() => GasVM); }
            private set
            {
                SetValue(() => GasVM, value);
            }
        }

        public WaterViewModel WaterVM
        {
            get { return GetValue(() => WaterVM); }
            private set
            {
                SetValue(() => WaterVM, value);
            }
        }

        public PowerViewModel PowerVM
        {
            get { return GetValue(() => PowerVM); }
            private set
            {
                SetValue(() => PowerVM, value);
            }
        }

        public TellViewModel TellVM
        {
            get { return GetValue(() => TellVM); }
            private set
            {
                SetValue(() => TellVM, value);
            }
        }

        public MobileViewModel MobileVM
        {
            get { return GetValue(() => MobileVM); }
            private set
            {
                SetValue(() => MobileVM, value);
            }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public EmployeeDesignTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
            }
        }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SearchMeters();
            }
        }

        public Boolean GasIsVisible
        {
            get { return GetValue(() => GasIsVisible); }
            set
            {
                SetValue(() => GasIsVisible, value);
            }
        }

        public Boolean MobileIsVisible
        {
            get { return GetValue(() => MobileIsVisible); }
            set
            {
                SetValue(() => MobileIsVisible, value);
            }
        }

        public Boolean PowerIsVisible
        {
            get { return GetValue(() => PowerIsVisible); }
            set
            {
                SetValue(() => PowerIsVisible, value);
            }
        }

        public Boolean TellIsVisible
        {
            get { return GetValue(() => TellIsVisible); }
            set
            {
                SetValue(() => TellIsVisible, value);
            }
        }

        public Boolean WaterIsVisible
        {
            get { return GetValue(() => WaterIsVisible); }
            set
            {
                SetValue(() => WaterIsVisible, value);
            }
        }
        #endregion

        #region methods

        public override void SelectedItemChanged()
        {
            throw new NotImplementedException();
        }

        private void setMetersForBuilding()
        {
            _metersCollection.Clear();
        }

        private void initialozObj()
        {
            GasIsVisible = MobileIsVisible = WaterIsVisible = TellIsVisible = PowerIsVisible = false;
            _metersCollection.Clear();
            _allMeters = _buildingService.GetMeters();
            _allMeters.ToList().ForEach(mr =>
            {
                _metersCollection.Add(mr);
            });
            this.MeterType = "A";
        }

        private void SearchMeters()
        {
            this.MeterFilterView.Filter = (obj) =>
            {
                var cus = (Meter)obj;
                return cus.Name.Contains(SearchCriteria) || cus.CaseNo.Contains(SearchCriteria);
            };
        }

        private void initializVM()
        {
            if (!string.IsNullOrWhiteSpace(MeterType))
            {
                _metersCollection.Clear();
                switch (MeterType)
                {
                    case "A":
                        PowerVM = new PowerViewModel(new PowerMeter()) { Name="",CaseNo="",PostalCode="",TariffType=1,EarlyInstallationDate=GlobalClass._Today};

                        _allMeters.OfType<PowerMeter>().ToList().ForEach(mr =>
                        {
                            _metersCollection.Add(mr);
                        });
                        PowerIsVisible = true;
                        GasIsVisible = false;
                        MobileIsVisible = false;
                        TellIsVisible = false;
                        WaterIsVisible = false;
                        break;
                    case "B":
                        WaterVM = new WaterViewModel(new WaterMeter()) { Name = "", CaseNo = "", TariffType = 1, PostalCode = ""};

                        _allMeters.OfType<WaterMeter>().ToList().ForEach(mr =>
                        {
                            _metersCollection.Add(mr);
                        });
                        PowerIsVisible = false;
                        GasIsVisible = false;
                        MobileIsVisible = false;
                        TellIsVisible = false;
                        WaterIsVisible = true;
                        break;
                    case "C":
                        GasVM = new GasViewModel(new GasMeter()) { Name = "", CaseNo = "", TariffType = 0, PostalCode = ""};

                        _allMeters.OfType<GasMeter>().ToList().ForEach(mr =>
                        {
                            _metersCollection.Add(mr);
                        });
                        PowerIsVisible = false;
                        GasIsVisible = true;
                        MobileIsVisible = false;
                        TellIsVisible = false;
                        WaterIsVisible = false;
                        break;
                    case "D":
                        TellVM = new TellViewModel(new TellMeter()) { Name = "", CaseNo = "", TariffType = 0, PostalCode = "", SubscriptionNo = "" };

                        _allMeters.OfType<TellMeter>().ToList().ForEach(mr =>
                        {
                            _metersCollection.Add(mr);
                        });
                        PowerIsVisible = false;
                        GasIsVisible = false;
                        MobileIsVisible = false;
                        TellIsVisible = true;
                        WaterIsVisible = false;
                        break;
                    case "E":
                        MobileVM = new MobileViewModel(new MobileMeter()) { Name = "", CaseNo = "", TariffType = 0, PostalCode = "", SubscriptionNo = "" };

                        _allMeters.OfType<MobileMeter>().ToList().ForEach(mr =>
                        {
                            _metersCollection.Add(mr);
                        });
                        PowerIsVisible = false;
                        GasIsVisible = false;
                        MobileIsVisible = true;
                        TellIsVisible = false;
                        WaterIsVisible = false;
                        break;
                }

                _firstGeneration.Clear();
                if (string.Equals("F", MeterType, StringComparison.CurrentCulture)
                    || string.Equals("E", MeterType, StringComparison.CurrentCulture))
                {
                    _employeeSrvice.GetParentNode(1).Where(nd => nd.ParentNode == null).ToList().ForEach(nd =>
                    {
                        _rootNode = new EmployeeDesignTreeViewModel(nd, null);
                        _firstGeneration.Add(_rootNode);
                    });
                }
               else if (string.Equals("G", MeterType, StringComparison.CurrentCulture))
                {
                    _employeeSrvice.GetParentNode(2).Where(nd => nd.ParentNode == null).ToList().ForEach(nd =>
                    {
                        _rootNode = new EmployeeDesignTreeViewModel(nd, null);
                        _firstGeneration.Add(_rootNode);
                    });
                }
                else
                {
                    List<int> permit=new List<int>();
                  
                    _employeeSrvice.GetParnetNodeToBuilding(2).ToList().ForEach(nd =>
                    {
                        if (nd.Building != null)
                        {
                            permit.Add(nd.BuidldingDesignId);
                        }
                    });
                    _employeeSrvice.GetParentNode(2).Where(nd => nd.ParentNode == null).ToList().ForEach(nd =>
                    {
                        _rootNode = new EmployeeDesignTreeViewModel(nd, null,permit);
                        _firstGeneration.Add(_rootNode);
                    });
                }
            }
        }
        
        private void setVM()
        {
            if (SelectedMeter == null) return;
            if (SelectedMeter is GasMeter)
            {
                GasVM = new GasViewModel(SelectedMeter as GasMeter);

                PowerIsVisible = false;
                GasIsVisible = true;
                MobileIsVisible = false;
                TellIsVisible = false;
                WaterIsVisible = false;
            }
            else if (SelectedMeter is PowerMeter)
            {
                PowerVM = new PowerViewModel(SelectedMeter as PowerMeter);

                PowerIsVisible = true;
                GasIsVisible = false;
                MobileIsVisible = false;
                TellIsVisible = false;
                WaterIsVisible = false;
            }
            else if (SelectedMeter is WaterMeter)
            {
                WaterVM = new WaterViewModel(SelectedMeter as WaterMeter);

                PowerIsVisible = false;
                GasIsVisible = false;
                MobileIsVisible = false;
                TellIsVisible = false;
                WaterIsVisible = true;
            }
            else if (SelectedMeter is TellMeter)
            {
                TellVM = new TellViewModel(SelectedMeter as TellMeter);

                PowerIsVisible = false;
                GasIsVisible = false;
                MobileIsVisible = false;
                TellIsVisible = true;
                WaterIsVisible = false;
            }
            else if (SelectedMeter is MobileMeter)
            {
                MobileVM = new MobileViewModel(SelectedMeter as MobileMeter);

                PowerIsVisible = false;
                GasIsVisible = false;
                MobileIsVisible = true;
                TellIsVisible = false;
                WaterIsVisible = false;
            }
        }

        private void saveMeter()
        {
            if (SelectedNode == null)
            {
                _dialogService.ShowAlert("هشدار", "هیچ شاخه ای انتخاب نشده است");
                return;
            }

            var employee = _employeeSrvice.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            Meter newMeter;
            Boolean haveError = false;
            int type = 0;
            if (SelectedMeter == null)
            {
                if (string.Equals("F", MeterType, StringComparison.CurrentCulture)
                     || string.Equals("E", MeterType, StringComparison.CurrentCulture))
                {
                    type = 1;
                    _dialogService.ShowAlert("", "Not implements...");
                    return;
                }
                else
                {
                    type = 2;
                    
                    if (string.Equals(MeterType, "A", StringComparison.InvariantCulture))
                    {
                        if (PowerVM.HasErrors)
                        {
                            _dialogService.ShowAlert("", ErrorMessages.Default.InputInvalid);
                            return;
                        }
                        newMeter = this.PowerVM.CurrentEntity;
                        haveError = PowerVM.HasErrors;
                    }
                    else if (string.Equals(MeterType, "B", StringComparison.InvariantCulture))
                    {
                        if (WaterVM.HasErrors)
                        {
                            _dialogService.ShowAlert("", ErrorMessages.Default.InputInvalid);
                            return;
                        }
                        newMeter = this.WaterVM.CurrentEntity;
                        haveError = WaterVM.HasErrors;
                    }
                    else if (string.Equals(MeterType, "C", StringComparison.InvariantCulture))
                    {
                        if (GasVM.HasErrors)
                        {
                            _dialogService.ShowAlert("", ErrorMessages.Default.InputInvalid);
                            return;
                        }
                        newMeter = this.GasVM.CurrentEntity;
                        haveError = GasVM.HasErrors;
                    }
                    else if (string.Equals(MeterType, "D", StringComparison.InvariantCulture))
                    {
                        if (TellVM.HasErrors)
                        {
                            _dialogService.ShowAlert("", ErrorMessages.Default.InputInvalid);
                            return;
                        }
                        newMeter = this.TellVM.CurrentEntity;
                        haveError = this.TellVM.HasErrors;
                    }
                    else if (string.Equals(MeterType, "E", StringComparison.InvariantCulture))
                    {
                        if (MobileVM.HasErrors)
                        {
                            _dialogService.ShowAlert("", ErrorMessages.Default.InputInvalid);
                            return;
                        }
                        newMeter = this.MobileVM.CurrentEntity;
                        haveError = this.MobileVM.HasErrors;
                    }
                    else
                    {
                        _dialogService.ShowAlert("توجه", "نوع کنتور انتخابی نامعتبر است");
                        return;
                    }
                }
                newMeter.ObjectState = ObjectState.Added;
                newMeter.InsertDate = GlobalClass._Today;
                newMeter.ModeifiedDate = GlobalClass._Today;
            }
            else
            {
                newMeter = SelectedMeter;
                newMeter.ObjectState = ObjectState.Modified;
            }

            if (type == 2)
            {
                ((StrategyDesign)SelectedNode.BuildingDesignCurrent).Building.Meters.Add(newMeter);
            }
            
            employee.EmployeeDesign.Add(SelectedNode.BuildingDesignCurrent);

            Boolean confirm = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                _employeeSrvice.InsertOrUpdateGraph(employee);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    if(!_metersCollection.Contains(newMeter))
                    _metersCollection.Add(newMeter);
                    SelectedMeter = null;
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

        private void DeleteMeter()
        {
            if (SelectedMeter == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کنتوری انتخاب نشده است");
                return;
            }
             Boolean confirm = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.AskConfrimation);
             if (confirm)
             {
                 SelectedMeter.ObjectState = ObjectState.Deleted;
                 try
                 {
                     Mouse.SetCursor(Cursors.Wait);
                     _unitOfWork.SaveChanges();
                     Mouse.SetCursor(Cursors.Arrow);
                     _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                     _metersCollection.Remove(SelectedMeter);
                     SelectedMeter = null;
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
            viewModel.showGlobalSinglePageHelp("110006");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initializCommand()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveMeter(); },
                (parameter) => { return true; });

            DeleteCommand = new MvvmCommand(
                (parameter) => { this.DeleteMeter(); },
                (parameter) => { return true; }
                );

            HelpCommand = new MvvmCommand((parameter) => { this.showHelp(); }, (parameter) => { return true; });
        }

        #endregion

        #region files

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IBuildingService _buildingService;
        private readonly IEmployeeService _employeeSrvice;
        private readonly ObservableCollection<Meter> _metersCollection;
        IEnumerable<Meter> _allMeters;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _firstGeneration;
        EmployeeDesignTreeViewModel _rootNode;

        #endregion
    }
}
