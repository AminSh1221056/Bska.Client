
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Windows;
    using System.Windows.Input;
    using System.Linq;
    using System.Windows.Controls;

    public sealed class RequestManagerViewModel : BaseViewModel
    {
        #region ctor

        public RequestManagerViewModel(IUnityContainer container)
        {

            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this.dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._personService = _container.Resolve<IPersonService>(new ParameterOverride("repository", _unitOfWork.Repository<Person>()));
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._personRequestPermit = new ObservableCollection<RequestPermit>();

            this.initalizObj();
            this.initalizCommand();
        }

        #endregion

        #region peroperties
        
        public ObservableCollection<PersonModel> Persons
        {
            get { return _persons; }
        }

        public PersonModel SelectedPerson
        {
            get { return GetValue(() => SelectedPerson); }
            set
            {
                SetValue(() => SelectedPerson, value);
                this.GetPersonPermit();
            }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> StrategyCollection
        {
            get { return _strategyCollection; }
        }

        public ObservableCollection<RequestPermit> PersonRequestPermit
        {
            get { return _personRequestPermit; }
            set
            {
                _personRequestPermit = value;
                OnPropertyChanged("PersonRequestPermit");
            }
        }

        public RequestPermit SelectedPermit
        {
            get { return GetValue(() => SelectedPermit); }
            set
            {
                SetValue(() => SelectedPermit, value);
            }
        }
        
        #endregion

        #region methods

        private void initalizObj()
        {
            Mouse.SetCursor(Cursors.Wait);

            Persons.Clear();

            var persons = _personService.Queryable().Where(x=>x.PersonId!=1).Select(p => new PersonModel
            {
                FullName=p.FirstName+" "+p.LastName,
                NationalId=p.NationalId,
                PersonId=p.PersonId,
            }).ToList();

            foreach (var person in persons)
            {
                _persons.Add(person);
            }
            _organizCollection.Clear();
            _strategyCollection.Clear();

            _allOrganiz = _employeeService.GetParentNode(1).ToList();
            var orgItems = _allOrganiz.Where(x => x.ParentNode == null);

            foreach (var org in orgItems)
            {
                _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null));
            }

            _allStrategy = _employeeService.GetParentNode(2).ToList();
            var stgyItems = _allStrategy.Where(x => x.ParentNode == null);
            foreach (var stgy in stgyItems)
            {
                _strategyCollection.Add(new EmployeeDesignTreeViewModel(stgy, null));
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void GetPersonPermit()
        {
            _personRequestPermit.Clear();

            if (SelectedPerson != null)
            {
                foreach (var permit in _personService.GetPersonPermit(SelectedPerson.PersonId).ToList())
                {
                    var org = _allOrganiz.SingleOrDefault(x => x.BuidldingDesignId == permit.OrganizId);
                    if (org != null) permit.OrganizName = GetHirecharyNode(org);

                    var str = _allStrategy.SingleOrDefault(x => x.BuidldingDesignId == permit.StrategyId);
                    if (str != null) permit.StragegyName = GetHirecharyNode(str);

                    _personRequestPermit.Add(permit);
                }
            }
        }

        private String GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;
            return _nodeName;
        }
        
        private void HandlePreviewDrop(object inObject)
        {
            if (SelectedPerson == null)
            {
                dialogService.ShowError("خطا", "هیچ پرسنلی انتخاب نشده است");
                return;
            }

            IDataObject ido = inObject as IDataObject;
            if (null == ido) return;
            var treeviewItem = ido.GetData(typeof(EmployeeDesignTreeViewModel));
            if (treeviewItem != null)
            {
                var Selecteditem = treeviewItem as EmployeeDesignTreeViewModel;
                RequestPermit item = null;

                if (SelectedPermit != null)
                {
                    if (Selecteditem.BuildingDesignCurrent is OrganizationDesign)
                    {
                        item = new RequestPermit
                        {
                            IsEnable = true,
                            OrganizId = Selecteditem.BuildingDesignCurrent.BuidldingDesignId,
                            OrganizName = GetNodeName(Selecteditem),
                            PersonId = SelectedPerson.PersonId,
                            StragegyName = SelectedPermit.StragegyName,
                            StrategyId = SelectedPermit.StrategyId,
                        };

                        if (_personRequestPermit.Any(x => x.OrganizId == item.OrganizId && x.StrategyId == item.StrategyId))
                        {
                            dialogService.ShowError("خطا", "شما قبلا این ارتباط را برای این پرسنل ثبت کرده اید");
                            return;
                        }

                        this.AddPermitRequest(item);
                    }
                    else if (Selecteditem.BuildingDesignCurrent is StrategyDesign)
                    {
                        item = new RequestPermit
                        {
                            IsEnable = true,
                            OrganizId = SelectedPermit.OrganizId,
                            OrganizName = SelectedPermit.OrganizName,
                            PersonId = SelectedPerson.PersonId,
                            StragegyName = GetNodeName(Selecteditem),
                            StrategyId = Selecteditem.BuildingDesignCurrent.BuidldingDesignId,
                        };

                        if (_personRequestPermit.Any(x => x.OrganizId == item.OrganizId && x.StrategyId == item.StrategyId))
                        {
                            dialogService.ShowError("خطا", "شما قبلا این ارتباط را برای این پرسنل ثبت کرده اید");
                            return;
                        }

                        this.AddPermitRequest(item);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (Selecteditem.BuildingDesignCurrent is OrganizationDesign)
                    {
                        item = new RequestPermit
                        {
                            IsEnable = false,
                            OrganizId = Selecteditem.BuildingDesignCurrent.BuidldingDesignId,
                            OrganizName = GetNodeName(Selecteditem),
                            PersonId = SelectedPerson.PersonId,
                            StragegyName = "",
                            StrategyId = -1,
                        };
                    }
                    else if (Selecteditem.BuildingDesignCurrent is StrategyDesign)
                    {
                        item = new RequestPermit
                        {
                            IsEnable = false,
                            OrganizId = -1,
                            OrganizName = "",
                            PersonId = SelectedPerson.PersonId,
                            StragegyName = GetNodeName(Selecteditem),
                            StrategyId = Selecteditem.BuildingDesignCurrent.BuidldingDesignId,
                        };
                    }

                    _personRequestPermit.Add(item);
                }
            }
        }

        private void AddPermitRequest(RequestPermit item)
        {
            bool canInsert = dialogService.AskConfirmation("توجه", "آیا مطمئن به ثبت اجازه در خواست برای" + " " + SelectedPerson.FullName + " " + "می باشید");
            if (canInsert)
            {
                var person = _personService.Query(p => p.PersonId == SelectedPerson.PersonId)
                    .Include(x => x.RequestPermit).Select().Single();
                var editPermit = person.RequestPermit.SingleOrDefault(x => x.RequestPermitId == SelectedPermit.RequestPermitId);

                if (editPermit != null)
                {
                    editPermit.ObjectState = ObjectState.Modified;
                    editPermit.OrganizId = item.OrganizId;
                    editPermit.IsEnable = item.IsEnable;
                    editPermit.StrategyId = item.StrategyId;
                }
                else
                {
                    editPermit = item;
                    editPermit = item;
                    editPermit.ObjectState = ObjectState.Added;
                }
                person.RequestPermit.Add(editPermit);
                _personService.InsertOrUpdateGraph(person);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);

                    int index = _personRequestPermit.IndexOf(SelectedPermit);
                    _personRequestPermit.RemoveAt(index);
                    _personRequestPermit.Insert(index, item);
                    Mouse.SetCursor(Cursors.Arrow);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.Message.Contains("See the inner exception for details"))
                    {
                        dialogService.ShowError("خطا", "شما قبلا این ارتباط را برای این پرسنل ثبت کرده اید");
                    }
                    else
                    {
                        dialogService.ShowError("خطا", ex.Message);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            SelectedPermit = null;
        }

        private String GetNodeName(EmployeeDesignTreeViewModel selectedItem)
        {
            String _nodeName = "";

            if (selectedItem.Parent != null)
            {
                _nodeName += GetNodeName(selectedItem.Parent);
                _nodeName += "***";
            }
            _nodeName += selectedItem.Name;
            return _nodeName;
        }

        private void DeltetePermit(RequestPermit deleteItem)
        {
            SelectedPermit = deleteItem;
            var person = _personService.Find(SelectedPerson.PersonId);
            if (person.RequestPermit.Contains(deleteItem))
            {
                Boolean canDelete = dialogService.AskConfirmation("هشدار", ErrorMessages.Default.ConfirmDelete);
                if (canDelete)
                {
                    deleteItem.ObjectState = ObjectState.Deleted;
                    person.RequestPermit.Remove(deleteItem);
                    _personService.InsertOrUpdateGraph(person);
                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        _personRequestPermit.Remove(deleteItem);
                        SelectedPermit = null;
                        Mouse.SetCursor(Cursors.Arrow);
                    }
                    catch (DbUpdateException ex)
                    {
                        dialogService.ShowError("خطا", ex.Message);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            else
            {
                _personRequestPermit.Remove(deleteItem);
            }
        }

        private void ConfirmPermit(RequestPermit permit)
        {
            var person = _personService.Find(SelectedPerson.PersonId);

            if (person.RequestPermit.Contains(permit))
            {
                permit.ObjectState = ObjectState.Modified;
                person.RequestPermit.Add(permit);
                _personService.InsertOrUpdateGraph(person);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    SelectedPermit = null;
                    Mouse.SetCursor(Cursors.Arrow);
                }
                catch (DbUpdateException ex)
                {
                    dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void confirmOrderDisable(object parameter)
        {
            if (SelectedPerson == null)
            {
                dialogService.ShowAlert("توجه", "هیچ پرسنلی انتخاب نشده است");
                return;
            }
            var ch = parameter as CheckBox;
            if (ch.IsChecked == true)
            {
                Boolean confirm = dialogService.AskConfirmation("هشدار", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    var person = _personService.Find(SelectedPerson.PersonId);
                    person.RequestPermit.ForEach(rp =>
                    {
                        rp.ObjectState = ObjectState.Modified;
                        rp.IsEnable = false;
                    });
                    _personService.InsertOrUpdateGraph(person);
                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        ch.IsChecked = false;
                        this.GetPersonPermit();
                        Mouse.SetCursor(Cursors.Arrow);
                    }
                    catch (DbUpdateException ex)
                    {
                        dialogService.ShowError("خطا", ex.Message);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        private void showHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110009");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand PreviewDropCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand PersonOrderDisableCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initalizCommand()
        {
            PreviewDropCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.HandlePreviewDrop(parameter);
                },
                (parameter) =>
                {
                    return true;
                }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeltetePermit(parameter as RequestPermit);
                },
                (parameter) =>
                {
                    return true;
                }
                );

            NewCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     this.SelectedPermit = null;
                 },
                (parameter) =>
                {
                    return true;
                }
                );

            ConfirmCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.ConfirmPermit(parameter as RequestPermit);
                },
                (parameter) =>
                {
                    return true;
                }
                );

            PersonOrderDisableCommand = new MvvmCommand(
                (parameter) => { this.confirmOrderDisable(parameter); },
                (parameter) =>
                {
                    return this.SelectedPerson!=null;
                }
                ).AddListener<RequestManagerViewModel>(this,x=>x.SelectedPerson);

            HelpCommand = new MvvmCommand((parameter)=> { this.showHelp(); },(parameter)=> { return true; });
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;
        private readonly ObservableCollection<PersonModel> _persons = new ObservableCollection<PersonModel>();
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private ObservableCollection<RequestPermit> _personRequestPermit;
        private List<EmployeeDesign> _allOrganiz = new List<EmployeeDesign>();
        private List<EmployeeDesign> _allStrategy = new List<EmployeeDesign>();

        #endregion
    }
}
