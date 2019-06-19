
namespace Bska.Client.UI.ViewModels.AccountingViewModels
{
    using Client.API.UnitOfWork;
    using Data.Service;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using TreeViewModels;
    using System.Linq;
    using Domain.Entity;
    using Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;
    using API;

    public sealed class AccountCodingDesignViewModel : BaseViewModel
    {
        #region ctor

        public AccountCodingDesignViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._firstGeneration = new ObservableCollection<AccountCodingTreeViewModel>();
            this.initializObj();
            this.initalizCommand();
            IsEditable = false;
        }

        #endregion

        #region properties

        public AccountCodingTreeViewModel SelectedNode
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

        public ObservableCollection<AccountCodingTreeViewModel> FirstGeneration
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

        private void initializObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.SelectedNode = null;
            _firstGeneration.Clear();
            _employeeService.GetAccountCodings().Where(nd => nd.Parent == null).ToList().ForEach(nd =>
            {
                _firstGeneration.Add(new AccountCodingTreeViewModel(nd, null));
            });
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void EditNode()
        {
            if (SelectedNode == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedNode.Name) || string.IsNullOrWhiteSpace(SelectedNode.Code))
            {
                _dialogService.ShowAlert("توجه", "همه فیلدها الزامی است");
                return;
            }
            if (SelectedNode.AccountDocumentCodingCurrent.ObjectState != ObjectState.Added)
            {
                SelectedNode.AccountDocumentCodingCurrent.ObjectState = ObjectState.Modified;
            }

            var employee = _employeeService.Queryable().FirstOrDefault();
            employee.AccountDocumentCodings.Add(SelectedNode.AccountDocumentCodingCurrent);
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
        
        #endregion

        #region commands
        public ICommand EditCommand { get; private set; }

        private void initalizCommand()
        {
            EditCommand = new MvvmCommand(
                (parameter) => {
                    this.EditNode();
                },
                (parameter) => { return SelectedNode != null; }).AddListener<AccountCodingDesignViewModel>(this, x => x.SelectedNode);
            
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IEmployeeService _employeeService;
        private readonly ObservableCollection<AccountCodingTreeViewModel> _firstGeneration;

        #endregion

    }
}
