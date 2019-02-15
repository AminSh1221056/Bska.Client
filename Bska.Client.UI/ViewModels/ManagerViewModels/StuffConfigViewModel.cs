
namespace Bska.Client.UI.ViewModels.ManagerViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.TreeViewModels;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public sealed class StuffConfigViewModel : BaseViewModel
    {
        #region ctor

        public StuffConfigViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._stuffService = _container.Resolve<IStuffService>(new ParameterOverride("repository", _unitOfWork.Repository<Stuff>()));
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._firstGeneration = new ObservableCollection<KalaManageTreeViewModel>();
            this._dropTreeFirstGeneration = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._organizationPerfecStuffs = new ObservableCollection<OrganizationPerfectStuffModel>();
            this.initializObj();
            this.initializCommands();
        }
        #endregion

        #region properties
        public ObservableCollection<EmployeeDesignTreeViewModel> DropTreeFirstGeneration
        {
            get { return _dropTreeFirstGeneration; }
        }

        public ObservableCollection<OrganizationPerfectStuffModel> OrganizationStuffs
        {
            get { return _organizationPerfecStuffs; }
        }

        public OrganizationPerfectStuffModel SelectedOrganizationStuff
        {
            get { return GetValue(() => SelectedOrganizationStuff); }
            set
            {
                SetValue(() => SelectedOrganizationStuff, value);
            }
        }

        public ObservableCollection<KalaManageTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public KalaManageTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
                if (value != null)
                {
                    this.initStuffAsync();
                }
            }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
            await Task.Run(() =>
            {
                _stuffService.Queryable().Include(st => st.OrganizationPefectStuffs)
               .ToList().Where(x => x.Parent == null).ForEach(x =>
               {
                   DispatchService.Invoke(() =>
                   {
                       _firstGeneration.Add(new KalaManageTreeViewModel(x, false));
                   });
               });
            });

            this.getOrganizDesignAsync();
        }

        private void initStuffAsync()
        {
            if (SelectedNode != null)
            {
                _organizationPerfecStuffs.Clear();
                SelectedNode.StuffCurrent.OrganizationPefectStuffs.ForEach(orp =>
                {
                    _organizationPerfecStuffs.Add(new OrganizationPerfectStuffModel
                    {
                        Description = this.GetHirecharyNode(orp.EmployeeDesign),
                        KalaNo = orp.KalaNo,
                        OrganId = orp.BuidldingDesignId,
                    });
                });
            }
        }
        private async void getOrganizDesignAsync()
        {
            _dropTreeFirstGeneration.Clear();
            await Task.Run(() =>
            {
                _employeeService.GetParentNode(1).Where(nd => nd.ParentNode == null).ToList().ForEach(nd =>
                {
                    _rootNode = new EmployeeDesignTreeViewModel(nd, null);
                    DispatchService.Invoke(() =>
                    {
                        _dropTreeFirstGeneration.Add(_rootNode);
                    });
                });
            });
        }

        private void HandlePreviewDrop(object inObject)
        {
            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            IDataObject ido = inObject as IDataObject;
            if (null == ido) return;
            var stuffTreeItem = ido.GetData(typeof(KalaManageTreeViewModel));
            var dropingObj = ido.GetData(typeof(EmployeeDesignTreeViewModel)) as EmployeeDesignTreeViewModel;
            if (stuffTreeItem != null)
            {
                var item = stuffTreeItem as KalaManageTreeViewModel;
                var stuff = item.StuffCurrent;
                var organization = dropingObj.BuildingDesignCurrent as OrganizationDesign;
                if (stuff.OrganizationPefectStuffs.Any(ors => ors.BuidldingDesignId == organization.BuidldingDesignId))
                {
                    _dialogService.ShowAlert("توجه", "این قسمت سازمانی قبلا برای این کد کالا تعریف شده است");
                    return;
                }

                bool confirm = _dialogService.AskConfirmation("پرسش", " مدیر این قسمت سازمانی باید اموال درخواست برای این گروه از اموال را تایید کند.آیا مطمئن به انجام این عملیات می باشید");
                if (confirm)
                {
                    var newStuffOrganization = new OrganizationPefectStuff
                    {
                        Stuff = stuff,
                        ObjectState = ObjectState.Added
                    };
                    organization.OrganizationPerfectStuffs.Add(newStuffOrganization);
                    employee.EmployeeDesign.Add(organization);
                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _employeeService.InsertOrUpdateGraph(employee);
                        _unitOfWork.SaveChanges();
                        _organizationPerfecStuffs.Add(new OrganizationPerfectStuffModel
                        {
                            KalaNo = stuff.KalaNo,
                            OrganId = organization.BuidldingDesignId,
                            Description = this.GetHirecharyNode(organization)
                        });

                        if (!SelectedNode.IsPerfect)
                        {
                            SelectedNode.IsPerfect = true;
                        }

                        Mouse.SetCursor(Cursors.Arrow);
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
            }
        }

        private void deleteRelatedOrganization(object parameter)
        {
            if (SelectedNode == null)
            {
                return;
            }
            var orgRelated = parameter as OrganizationPerfectStuffModel;
            if (orgRelated != null)
            {
                var employee = _employeeService.Queryable().SingleOrDefault();
                if (employee == null)
                {
                    _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                    return;
                }

                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    var design = _employeeService.GetDesignById(orgRelated.OrganId, 1) as OrganizationDesign;
                    var rs = design.OrganizationPerfectStuffs.Single(x => x.KalaNo == orgRelated.KalaNo);
                    rs.ObjectState = ObjectState.Deleted;
                    design.OrganizationPerfectStuffs.Remove(rs);
                    employee.EmployeeDesign.Add(design);
                    try
                    {
                        _employeeService.InsertOrUpdateGraph(employee);
                        _unitOfWork.SaveChanges();
                        int index = _organizationPerfecStuffs.IndexOf(orgRelated);
                        _organizationPerfecStuffs.RemoveAt(index);
                        if (!SelectedNode.StuffCurrent.OrganizationPefectStuffs.Any())
                        {
                            SelectedNode.IsPerfect = false;
                        }
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

        #endregion

        #region commands
        
        public ICommand PreviewDropCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        
        private void initializCommands()
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
                (parameter) => { this.deleteRelatedOrganization(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IEmployeeService _employeeService;
        private readonly IStuffService _stuffService;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<KalaManageTreeViewModel> _firstGeneration;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _dropTreeFirstGeneration;
        private readonly ObservableCollection<OrganizationPerfectStuffModel> _organizationPerfecStuffs;
        EmployeeDesignTreeViewModel _rootNode;

        #endregion

    }
}
