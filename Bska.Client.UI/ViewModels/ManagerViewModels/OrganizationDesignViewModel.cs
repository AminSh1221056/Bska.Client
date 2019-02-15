
namespace Bska.Client.UI.ViewModels
{
    using System;
    using Microsoft.Practices.Unity;
    using Bska.Client.UI.Services;
    using Bska.Client.Data.Service;
    using System.Collections.ObjectModel;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.UI.API;
    using System.Windows.Input;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.Domain.Entity;
    using System.Collections.Generic;
    using System.Linq;
    using Bska.Client.Repository.Model;
    public sealed class OrganizationDesignViewModel : BaseViewModel
    {
        #region ctor
        public OrganizationDesignViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._firstGeneration = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this.GetParentNode();
            this.initalizCommand();
            this.IsEditable = true;
        }
        #endregion

        #region properties
        
        public EmployeeDesignTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
            }
        }

        public String NodeName
        {
            get { return GetValue(() => NodeName); }
            set
            {
                SetValue(() => NodeName, value);
            }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public bool IsEditable
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
            _employeeService.GetParentNode(1).Where(nd=>nd.ParentNode==null).ToList().ForEach(nd =>
            {
                _rootNode = new EmployeeDesignTreeViewModel(nd, null);
                _firstGeneration.Add(_rootNode);
            });
            Mouse.SetCursor(Cursors.Arrow);
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
            var newNode = new OrganizationDesign { Name = "",Code="", ObjectState = ObjectState.Added, ParentNode = SelectedNode != null ? SelectedNode.BuildingDesignCurrent : null };

            SelectedNode.Children.Add(new EmployeeDesignTreeViewModel(newNode, SelectedNode) { IsSelected = true, IsEditing = true });
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
            if (treeItem.BuildingDesignCurrent.BuidldingDesignId == 1)
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
        private void showHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110004");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initalizCommand()
        {
            NewCommand = new MvvmCommand(
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
                (parameter) => { this.EditNode(); 
                },
                (parameter) => { return SelectedNode != null; }).AddListener<OrganizationDesignViewModel>(this, x => x.SelectedNode);

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeleteNode(parameter);
                },
                (parameter) => { return true; });

            HelpCommand = new MvvmCommand((parameter) => { this.showHelp(); },(parameter)=> { return true; });
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IEmployeeService _employeeService;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _firstGeneration;
        EmployeeDesignTreeViewModel _rootNode;

        #endregion
    }
}
