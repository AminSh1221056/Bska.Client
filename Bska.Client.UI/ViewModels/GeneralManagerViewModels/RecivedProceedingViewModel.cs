
namespace Bska.Client.UI.ViewModels.GeneralManagerViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.UI.Helper;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using Domain.Entity.OrderEntity;

    public sealed class RecivedProceedingViewModel : BaseViewModel
    { 
        #region ctor

        public RecivedProceedingViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._proceedingService = _container.Resolve<IProceedingService>(new ParameterOverride("repository", _unitOfWork.Repository<Proceeding>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._movableAssets = new ObservableCollection<MovableAssetModel>();
            this.initalizObj();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;
            set;
        }

        public List<Proceeding> ProceddingsItems
        {
            get { return GetValue(() => ProceddingsItems); }
            set
            {
                SetValue(() => ProceddingsItems, value);
            }
        }

        public Proceeding Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public ProceedingState ProceedingState
        {
            get { return GetValue(() => ProceedingState); }
            set
            {
                SetValue(() => ProceedingState, value);
                this.getProceedings();
            }
        }

        public ObservableCollection<MovableAssetModel> MovableAssetCollection
        {
            get { return _movableAssets; }
        }

        public MovableAssetModel SelectedAsset
        {
            get { return GetValue(() => SelectedAsset); }
            set
            {
                SetValue(() => SelectedAsset, value);
            }
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            ProceedingState = ProceedingState.ManagerConfirming;
        }

        private void getProceedings()
        {
            Mouse.SetCursor(Cursors.Wait);
            _movableAssets.Clear();
            ProceddingsItems = _proceedingService.Query(x => x.State == ProceedingState).Include(p=>p.AssetProceedings).Select().ToList();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void getProceedingDetails(object parameter)
        {
            Proceeding proc = parameter as Proceeding;
            if (proc == null) return;
            Mouse.SetCursor(Cursors.Wait);
            Selected = proc;
            _movableAssets.Clear();
            Int64[] assetIds = proc.AssetProceedings.Select(x => x.AssetId).ToArray();
            _movableAssetService.Queryable().Where(x => assetIds.Contains(x.AssetId)).Include(ma => ma.AssetProceedings).AsNoTracking().ToList().ForEach(ma =>
            {
                bool isEnabled = true;
                bool isSelected = false;
                if (ma.AssetProceedings.Where(x => x.ProceedingId != proc.ProceedingId).Any(ap => ap.State == AssetProceedingState.InProgress))
                {
                    isEnabled = false;
                }

                if (ma.AssetProceedings.Single(ap => ap.ProceedingId == proc.ProceedingId).State == AssetProceedingState.InProgress)
                {
                    isSelected = true;
                }

                _movableAssets.Add(new MovableAssetModel
                {
                    AssetId = ma.AssetId,
                    CurState = ma.CurState,
                    InsertDate = ma.InsertDate,
                    UnitId = ma.UnitId,
                    Num = ma.Num,
                    Name = ma.Name,
                    MAssetType = ma.ToString("t", null),
                    Label =ma.Label,
                    kalaUid = ma.KalaUid,
                    IsCompietion = ma.ISCompietion,
                    IsRowEnabled=isEnabled,
                    IsSelected=isSelected
                });
            });
            Units = _unitService.Query(u => u.StuffId == StuffType.UnConsumption || u.StuffId == StuffType.None).Select().ToList();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void mAssetProceeding(object parameter)
        {
            var ch = parameter as CheckBox;
            var mAssetModel = ch.Tag as MovableAssetModel;
            if (mAssetModel == null) return;

            if (Selected == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ صورت جلسه ای انتخاب نشده است");
                return;
            }

            if (Selected.State != ProceedingState.ManagerConfirming)
            {
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            var mAsset = _movableAssetService.Query(m => m.AssetId == mAssetModel.AssetId)
                .Include(m => m.AssetProceedings).Select().Single();
            var ap = mAsset.AssetProceedings.Single(x => x.ProceedingId == Selected.ProceedingId);

            if (ch.IsChecked == true)
            {
                bool confirm = _dialogService.AskConfirmation("هشدار", "آیا مطمئن به اضافه کردن این مال به صورت جلسه می باشید");
                if (!confirm)
                {
                    ch.IsChecked = false;
                    return;
                }

                ap.State = AssetProceedingState.InProgress;
                ap.ObjectState = ObjectState.Modified;
                mAssetModel.IsSelected = true;
            }
            else
            {
                bool confirm = _dialogService.AskConfirmation("هشدار", "آیا مطمئن به حذف این مال از صورت جلسه می باشید");
                if (!confirm)
                {
                    ch.IsChecked = true;
                    return;
                }

                ap.State = AssetProceedingState.IsRejected;
                ap.ObjectState = ObjectState.Modified;
                mAssetModel.IsSelected = false;
            }

            _movableAssetService.InsertOrUpdateGraph(mAsset);

            try
            {
                _unitOfWork.SaveChanges();
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

        private void rejectProceeding(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            this.Selected = proc;
            bool confirm = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.AskConfrimation);
            if (!confirm)
            {
                return;
            }

            proc.AssetProceedings.ForEach(ap =>
            {
                ap.State = AssetProceedingState.IsRejected;
                ap.ObjectState = ObjectState.Modified;
            });

            proc.State = ProceedingState.Rejected;
            proc.ObjectState = ObjectState.Modified;
            proc.Description += "*" + "رد صورت جلسه توسط مدیریت سازمان مذکور به نام " + UserLog.UniqueInstance.LogedUser.FullName
                + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();
           
            _proceedingService.InsertOrUpdateGraph(proc);
            try
            {
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                this.getProceedings();
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

        private void confrimProceeding(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            this.Selected = proc;
            bool confirm = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.AskConfrimation);
            if (!confirm)
            {
                return;
            }
            BskaUIHelper myHelper = new BskaUIHelper();
            MAssetCurState curState = myHelper.getAssetLicencingStateByProc(proc.Type);
            proc.State = ProceedingState.Confirmed;
            proc.ObjectState = ObjectState.Modified;
            proc.Description += "*" + "تایید صورت جلسه توسط مدیریت سازمان مذکور به نام " + UserLog.UniqueInstance.LogedUser.FullName
                + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();
            proc.AssetProceedings.ForEach(ap =>
            {
                var asset = _movableAssetService.Find(ap.AssetId);
                asset.ObjectState = ObjectState.Modified;
                asset.CurState = curState;
            });
            _proceedingService.InsertOrUpdateGraph(proc);

            try
            {
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                this.getProceedings();
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

        private void showProceedingDetails(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            Selected = proc;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new ProceedingInformationViewModel(_container, proc.ProceedingId);
            _navigationService.ShowProceedingDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showAssetDetailsWindow(object parameter)
        {
            var mAsset = parameter as MovableAssetModel;
            if (mAsset == null) return;
            SelectedAsset = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands
        public ICommand ProceedingDetailsCommand { get; private set; }
        public ICommand ProceedingMAssetCommand { get; private set; }
        public ICommand AssetDetailsCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        private void initalizCommand()
        {
            ProceedingDetailsCommand = new MvvmCommand(
                  (parameter) => { this.showProceedingDetails(parameter); },
                  (parameter) => { return true; });

            ProceedingMAssetCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.getProceedingDetails(parameter);
                },
                (parameter) => { return true; }
                );

            AssetDetailsCommand = new MvvmCommand(
                (parameter) => { this.showAssetDetailsWindow(parameter); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.mAssetProceeding(parameter);
                },
                (parameter) => { return true; }
                );

            RejectCommand = new MvvmCommand(
                (parameter) => { this.rejectProceeding(parameter); },
                (parameter) =>
                {
                    return true;
                }
                );

            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confrimProceeding(parameter); },
                (parameter) =>
                {
                    return true;
                }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IProceedingService _proceedingService;
        private readonly IUnitService _unitService;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<MovableAssetModel> _movableAssets;

        #endregion
    }
}
