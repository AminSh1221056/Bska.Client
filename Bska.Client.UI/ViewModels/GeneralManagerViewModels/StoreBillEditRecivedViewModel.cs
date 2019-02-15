
namespace Bska.Client.UI.ViewModels.GeneralManagerViewModels
{
    using Bska.Client.Common;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.UI.API;
    using System.Collections.Generic;
    using Bska.Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;

    public sealed class StoreBillEditRecivedViewModel : BaseViewModel
    {
        #region ctor
        public StoreBillEditRecivedViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._storeBillService = _container.Resolve<IStoreBillService>(new ParameterOverride("repository", _unitOfWork.Repository<StoreBill>()));
            this._storeBillEditCollection = new ObservableCollection<StoreBillEditModel>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this.StoreBillEditView = new CollectionViewSource { Source = StoreBillEditCollection }.View;
            this.initializObj();
            this.initalizCommands();
        }
        #endregion

        #region properties

        public Window Window
        {
            get; set;
        }

        public ObservableCollection<StoreBillEditModel> StoreBillEditCollection
        {
            get { return _storeBillEditCollection; }
        }

        public ICollectionView StoreBillEditView { get; set; }

        public StoreBillEditModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SbEditFilters();
            }
        }

        public GlobalRequestStatus CurrentEditStatus
        {
            get { return GetValue(() => CurrentEditStatus); }
            set
            {
                SetValue(() => CurrentEditStatus, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            CurrentEditStatus = GlobalRequestStatus.Pending;
            this.searchOrder();
        }

        private void searchOrder()
        {
            _storeBillEditCollection.Clear();
            _storeBillService.GetRecivedEditsByState(CurrentEditStatus).ToList()
                .ForEach(sbe =>
                {
                    _storeBillEditCollection.Add(sbe);
                });
        }
        private void SbEditFilters()
        {
            this.StoreBillEditView.Filter = ((obj) =>
            {
                StoreBillEditModel items = obj as StoreBillEditModel;
                if (items != null)
                    return items.Description.StartsWith(SearchCriteria) || items.StoreBillNo.StartsWith(SearchCriteria);
                return false;
            });
        }

        private void showStoreBillDetails(object parameter)
        {
            var storeBillModel = parameter as StoreBillEditModel;
            if (storeBillModel == null) return;
            this.Selected = storeBillModel;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var storeBill = _storeBillService.Find(storeBillModel.StoreBillId);
            var viewModel = new StoreBillDetailsViewModel(_container, storeBill,false);
            _navigationService.ShowStoreBillDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);

        }

        private void SearchMovableAssetByStoreBill(Object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);

            var storeBillModel = parameter as StoreBillEditModel;
            if (storeBillModel == null) return;
            Selected = storeBillModel;
            List<MovableAssetModel> mAssets = null;
            var storeBill = _storeBillService.Find(storeBillModel.StoreBillId);
            if (storeBill.StuffType == StuffType.Consumable)
            {
                mAssets = _commodityService.GetCommodityByBillId(storeBill.StoreBillId).ToList();
            }
            else
            {
                mAssets = _movableAssetService.GetMovableAssetByStoreBill(storeBill.StoreBillId).ToList();
            }

            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new MAssetListViewModel(_container, 1002, storeBill.StoreBillId, storeBill.StoreBillNo, 0);
            viewModel.AssetList = mAssets;
            var window = _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void editOrder(object parameter, GlobalRequestStatus newState)
        {
            var editModel = parameter as StoreBillEditModel;
            if (editModel != null)
            {
                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    var storeBill = _storeBillService.Query(s => s.StoreBillId == editModel.StoreBillId)
                   .Include(s => s.StoreBillEdits).Select().Single();
                    var sbEditModel = storeBill.StoreBillEdits.Single(s => s.Id == editModel.Id);
                    sbEditModel.ObjectState = ObjectState.Modified;
                    sbEditModel.State = newState;
                    storeBill.StoreBillEdits.Add(sbEditModel);
                    _storeBillService.InsertOrUpdateGraph(storeBill);
                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        editModel.State = newState;
                        int index = _storeBillEditCollection.IndexOf(editModel);
                        _storeBillEditCollection.RemoveAt(index);
                        _storeBillEditCollection.Insert(index, editModel);
                        this.Selected = editModel;
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
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
        }
        #endregion

        #region commands

        public ICommand DetailsCommand { get; private set; }
        public ICommand BillDetailsCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        private void initalizCommands()
        {
            DetailsCommand = new MvvmCommand(
              (parameter) =>
              {
                  this.SearchMovableAssetByStoreBill(parameter);
              },
              (parameter) =>
              {
                  return true;
              }
              );

            BillDetailsCommand = new MvvmCommand(
               (parameter) => { this.showStoreBillDetails(parameter); },
               (parameter) => { return true; }
               );

            SearchCommand = new MvvmCommand(
               (parameter) => { this.searchOrder(); },
               (parameter) => { return true; }
               );

            RejectCommand= new MvvmCommand(
               (parameter) => { this.editOrder(parameter, GlobalRequestStatus.Rejected); },
               (parameter) => { return true; }
               );

            ConfirmCommand = new MvvmCommand(
             (parameter) => { this.editOrder(parameter, GlobalRequestStatus.Confirmed); },
             (parameter) => { return true; }
             );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStoreBillService _storeBillService;
        private readonly ObservableCollection<StoreBillEditModel> _storeBillEditCollection;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;

        #endregion

    }
}
