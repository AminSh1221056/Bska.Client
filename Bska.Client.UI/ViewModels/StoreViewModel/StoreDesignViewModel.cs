
namespace Bska.Client.UI.ViewModels.StoreViewModel
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
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;
    using System.Linq;
    using Helper;
    public class StoreDesignViewModel : BaseViewModel
    {
        #region ctor

        public StoreDesignViewModel(IUnityContainer container)
        {
            this._container = container;
            this.dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this.navigationService = _container.Resolve<INavigationService>();
            this._storeService = _container.Resolve<IStoreService>(new ParameterOverride("repository", _unitOfWork.Repository<Store>()));
            this.initalizObj();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public List<Store> Stores
        {
            get { return GetValue(() => Stores); }
            set
            {
                SetValue(() => Stores, value);
            }
        }

        public StoreTreeViewModel StoreTreeVM
        {
            get { return GetValue(() => StoreTreeVM); }
            set
            {
                SetValue(() => StoreTreeVM, value);
            }
        }

        public Boolean IsEditable
        {
            get { return GetValue(() => IsEditable); }
            set
            {
                SetValue(() => IsEditable, value);
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

        public ObservableCollection<StoreTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public Store SelectedStore
        {
            get { return GetValue(() => SelectedStore); }
            set
            {
                SetValue(() => SelectedStore, value);
                this.GetParentNode();
            }
        }

        public StoreTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            Stores = _storeService.Queryable().ToList();
            this.IsEditable = true;
        }

        private void GetParentNode()
        {
            if (SelectedStore != null)
            {
                _firstGeneration.Clear();
                _storeService.Query(s=>s.StoreId==SelectedStore.StoreId).Include(s=>s.StoreDesign).Select().SelectMany(s=>s.StoreDesign).ToList().Where(x => x.ParentNode == null)
                    .ForEach(x =>
                    {
                        _rootNode = new StoreTreeViewModel(x, null, true);
                        _firstGeneration.Add(_rootNode);
                    });
            }
        }

        private void SaveNode(object parameter)
        {
            var treeItem = parameter as StoreTreeViewModel;
            if (treeItem == null) return;
            this.SelectedNode = treeItem;
            
            var newNode = new StoreDesign { Name = "", ObjectState = ObjectState.Added, ParentNode = SelectedNode != null ? SelectedNode.StoreDesignCurrent : null };

            SelectedNode.Children.Add(new StoreTreeViewModel(newNode,SelectedNode,false) { IsSelected = true, IsEditing = true });
            SelectedNode.IsExpanded = true;
        }

        private void DeleteNode(object parameter)
        {
            var treeItem = parameter as StoreTreeViewModel;
            if (treeItem == null) return;
            this.SelectedNode = treeItem;
            if (treeItem.StoreDesignCurrent.StoreDesignId == 1)
            {
                dialogService.ShowAlert("توجه", "شاخه های پیش فرض قابلیت حذف ندارند");
                return;
            }
            
            if (SelectedNode.StoreDesignCurrent.ChildNode.Any())
            {
                dialogService.ShowAlert("توجه", "بخش انتخابی داری زیر مجموعه می باشد.ابتدا باید زیر مجموعه حذف شود");
                return;
            }

            bool canDelete = dialogService.AskConfirmation("هشدار", ErrorMessages.Default.ConfirmDelete);
            if (canDelete)
            {
                SelectedNode.StoreDesignCurrent.ObjectState = ObjectState.Deleted;
                SelectedStore.StoreDesign.Remove(SelectedNode.StoreDesignCurrent);
                _storeService.InsertOrUpdateGraph(SelectedStore);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
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
                    dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void EditNode()
        {
            if (SelectedNode == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedNode.Name))
            {
                dialogService.ShowAlert("هشدار", "نام شاخه را وارد کنید");
                return;
            }

            if (SelectedNode.StoreDesignCurrent.ObjectState != ObjectState.Added)
            {
                SelectedNode.StoreDesignCurrent.ObjectState = ObjectState.Modified;
            }
                
            SelectedStore.StoreDesign.Add(SelectedNode.StoreDesignCurrent);
            _storeService.InsertOrUpdateGraph(SelectedStore);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                SelectedNode.IsEditing = false;
                SelectedNode.IsSelected = false;
                Mouse.SetCursor(Cursors.Arrow);
            }
            catch (DbUpdateException ex)
            {
                dialogService.ShowAlert("خطا", ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region commands
        
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand EditCommand { get; private set; }

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

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeleteNode(parameter);
                },
                (parameter) =>
                {
                    return true;
                }
                );

            EditCommand = new MvvmCommand(
                (parameter) => { this.EditNode(); },
                (parameter) => { return SelectedNode != null; }).AddListener<StoreDesignViewModel>(this, x => x.SelectedNode);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService navigationService;
        private readonly IStoreService _storeService;
        private readonly ObservableCollection<StoreTreeViewModel> _firstGeneration =
             new ObservableCollection<StoreTreeViewModel>();
        private StoreTreeViewModel _rootNode;

        #endregion
    }
}
