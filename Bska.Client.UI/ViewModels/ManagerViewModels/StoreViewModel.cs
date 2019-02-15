
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows;
    using Bska.Client.UI.Helper;

    public sealed class StoreListViewModel : BaseListViewModel<Store>
    {

        #region ctor

        public StoreListViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Store>>())
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this._storeService = _container.Resolve<IStoreService>(new ParameterOverride("repository", _unitOfWork.Repository<Store>()));
            this._firstGeneration = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this.StoreFilteredView = new CollectionViewSource { Source = Collection }.View;
            this.initalizCommands();
            this.initalizObj();
        }

        #endregion

        #region properties

        public ICollectionView StoreFilteredView { get; set; }
        public StoreDetailsViewModel StoreDetailsVm
        {
            get { return _storeDetailsVm; }
            set
            {
                _storeDetailsVm = value;
                OnPropertyChanged("StoreDetailsVm");
            }
        }
        public ObservableCollection<EmployeeDesignTreeViewModel> DropTreeFirstGeneration
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

        #endregion

        #region methods
        private void initalizObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.SelectedNode = null;

            this.StoreDetailsVm = new StoreDetailsViewModel(new Store()) { Name = "", StoreType = StoreType.Mixed };
            Collection.Clear();
            var stores = _storeService.Queryable().ToList();
            if (stores.Count > 0)
            {
                foreach (var store in stores)
                {
                    Collection.Add(new StoreDetailsViewModel(store));
                }

                //init for last view
                var lastStore = UserLog.UniqueInstance.LastView("store",null);
                int temp = 0;
                int.TryParse(lastStore, out temp);
                if (temp > 0)
                {
                    var item = Collection.FirstOrDefault(s => s.CurrentEntity.StoreId == temp);
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

            _firstGeneration.Clear();
            _employeeService.GetParentNode(2).Where(nd => nd.ParentNode == null).ToList().ForEach(nd =>
            {
                _rootNode = new EmployeeDesignTreeViewModel(nd, null);
                _firstGeneration.Add(_rootNode);
            });

            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void SaveStore()
        {
            if (StoreDetailsVm.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }
            
            if (Selected == null)
            {
                StoreDetailsVm.CurrentEntity.CreateDate = GlobalClass._Today;
                StoreDetailsVm.CurrentEntity.ObjectState = ObjectState.Added;
                StoreDetailsVm.CurrentEntity.StoreDesign.Add(new StoreDesign
                {
                    Name=StoreDetailsVm.Name,
                    ObjectState=ObjectState.Added,
                });
            }
            else
            {
                StoreDetailsVm.CurrentEntity.ObjectState = ObjectState.Modified;
            }
            
           _storeService.InsertOrUpdateGraph(StoreDetailsVm.CurrentEntity);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                Mouse.SetCursor(Cursors.Arrow);
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);

                if (Selected == null)
                {
                    Collection.Add(new StoreDetailsViewModel(StoreDetailsVm.CurrentEntity));
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
        
        private void NewStore()
        {
            this.Selected = null;
            this.StoreDetailsVm = new StoreDetailsViewModel(new Store()) { Name = "", StoreType =StoreType.Mixed };
        }

        private void DeleteStore()
        {
            if (Selected == null)
            {
                _dialogService.ShowAlert("انتخاب انبار", "هیچ انباری انتخاب نشده است");
                return;
            }

            if (Selected.CurrentEntity.StoreId == 1 || Selected.CurrentEntity.StoreId == 2)
            {
                _dialogService.ShowAlert("توجه", "انبارهای پیش فرض در سیستم قابلیت حذف ندارند");
                return;
            }
            
            Boolean canDelete = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.ConfirmDelete);
            if (canDelete)
            {
                Selected.CurrentEntity.ObjectState = ObjectState.Deleted;
                _storeService.Delete(Selected.CurrentEntity);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    Collection.Remove(Selected);
                    this.NewStore();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public override void SelectedItemChanged()
        {
            this.StoreDetailsVm = (StoreDetailsViewModel)Selected;
            UserLog.UniqueInstance.LastView("store", Selected.CurrentEntity.StoreId.ToString());
            this.PerformSearch();
        }

        private void HandlePreviewDrop(object inObject)
        {
            IDataObject ido = inObject as IDataObject;
            if (null == ido) return;
            var listBoxItem = ido.GetData(typeof(StoreDetailsViewModel));
            var dropingObj= ido.GetData(typeof(EmployeeDesignTreeViewModel)) as EmployeeDesignTreeViewModel;
            if (listBoxItem!= null)
            {
                SelectedNode = dropingObj;
                if (SelectedNode == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ شاخه ای انتخاب نشده است");
                    return;
                }
                SelectedNode.IsSelected = true;
                var item = listBoxItem as StoreDetailsViewModel;

                var employee = _employeeService.Queryable().SingleOrDefault();
                if (employee == null)
                {
                    _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                    return;
                }
                item.CurrentEntity.ObjectState = ObjectState.Modified;
            ((StrategyDesign)SelectedNode.BuildingDesignCurrent).Stores.Add(item.CurrentEntity);
                employee.EmployeeDesign.Add(SelectedNode.BuildingDesignCurrent);
                _employeeService.InsertOrUpdateGraph(employee);

                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
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
        #region Search Logic

        void PerformSearch()
        {
            try
            {
                if (_matchingBuildingEnumerator == null || !_matchingBuildingEnumerator.MoveNext())
                    this.VerifyMatchingPeopleEnumerator();

                var buidingDesign = _matchingBuildingEnumerator.Current;

                if (buidingDesign == null)
                    return;

                if (buidingDesign.Parent != null)
                    buidingDesign.Parent.IsExpanded = true;

                buidingDesign.IsSelected = true;
            }
            catch (NullReferenceException) { }
            catch (Exception) { throw; }
           
        }

        void VerifyMatchingPeopleEnumerator()
        {
            foreach (var k in _firstGeneration)
            {
                var matches = this.FindMatches(Selected.CurrentEntity.StrategyId ?? 0, k);
                if (matches.Count() > 0)
                {
                    _matchingBuildingEnumerator = matches.GetEnumerator();
                    break;
                }
            }

            if (!_matchingBuildingEnumerator.MoveNext())
            {
                _dialogService.ShowInfo("دوباره سعی کنید", "هیچ بخش استراتژیکی برای این انبار ثبت نشده است");
            }
        }

        IEnumerable<EmployeeDesignTreeViewModel> FindMatches(int id, EmployeeDesignTreeViewModel buildingdesign)
        {
            if (buildingdesign.BuildingDesignCurrent.BuidldingDesignId == id)
                yield return buildingdesign;

            foreach (EmployeeDesignTreeViewModel child in buildingdesign.Children)
                foreach (EmployeeDesignTreeViewModel match in this.FindMatches(id, child))
                    yield return match;
        }

        private void showHelp()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110005");
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand PreviewDropCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        private void initalizCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SaveStore();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.DeleteStore();
                },
                (parameter) =>
                {
                    return true;
                }
                );

            NewCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.NewStore();
                },
                (parameter) =>
                {
                    return true;
                }
                );

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
        private readonly IStoreService _storeService;
        private StoreDetailsViewModel _storeDetailsVm;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _firstGeneration;
        EmployeeDesignTreeViewModel _rootNode;
        IEnumerator<EmployeeDesignTreeViewModel> _matchingBuildingEnumerator;

        #endregion
    }

    public sealed class StoreDetailsViewModel : BaseDetailsViewModel<Store>
    {

        #region ctor

        public StoreDetailsViewModel(Store currentEntity)
            : base(currentEntity)
        {
        }

        #endregion

        #region properties

        public Int32 StoreId
        {
            get { return CurrentEntity.StoreId; }
        }

        [Required(ErrorMessage = "نام انبار الزامی می باشد")]
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

        [Required(ErrorMessage = "نوع انبار الزامی است")]
        public StoreType StoreType
        {
            get { return CurrentEntity.StoreType; }
            set
            {
                CurrentEntity.StoreType = value;
                ValidateProperty(value);
                OnPropertyChanged("StoreType");
            }
        }

        public String Description
        {
            get { return CurrentEntity.Description; }
            set
            {
                CurrentEntity.Description = value;
                OnPropertyChanged("Description");
            }
        }

        #endregion

    }
}
