
namespace Bska.Client.UI.ViewModels
{
    using System;
    using Microsoft.Practices.Unity;
    using Bska.Client.UI.Services;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Linq;
    using System.Windows.Input;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using Bska.Client.UI.Helper;
    using System.Windows;
    using Bska.Client.UI.API;
    using Bska.Client.Common;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using System.Data.Entity.Infrastructure;
    using System.ComponentModel.DataAnnotations;
    using Bska.Client.UI.ViewModels.AssetViewModel;
    using System.Collections.Generic;
    using Bska.Client.Repository.Model;

    public sealed class StoreBillEditViewModel : BaseViewModel
    {
        #region ctor

        public StoreBillEditViewModel(IUnityContainer container,int storeBillId)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._storeBillService = _container.Resolve<IStoreBillService>(new ParameterOverride("repository", _unitOfWork.Repository<StoreBill>()));
            this._storeBillEditCollection = new ObservableCollection<StoreBillEdit>();
            this.StoreBillEditView = new CollectionViewSource { Source = StoreBillEditCollection }.View;
            this.initializObj(storeBillId);
            this.initializCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;set;
        }

        [Required(ErrorMessage = "توضیحات الزامی است")]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                ValidateProperty(value);
                OnPropertyChanged("Description");
            }
        }

        public ObservableCollection<StoreBillEdit> StoreBillEditCollection
        {
            get { return _storeBillEditCollection; }
        }

        public ICollectionView StoreBillEditView { get; set; }

        public StoreBillEdit Selected
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

        public Boolean ISBillReported
        {
            get { return GetValue(() => ISBillReported); }
            set
            {
                SetValue(() => ISBillReported, value);
            }
        }

        #endregion

        #region methods

        private void initializObj(int sbId)
        {
            this.Description = "";
            this._currentBill = _storeBillService.Query(sb => sb.StoreBillId == sbId)
                .Include(sb => sb.StoreBillEdits).Include(sb=>sb.MAssets)
                .Include(sb=>sb.Commodities).Select().Single();
            _storeBillEditCollection.Clear();
            _currentBill.StoreBillEdits.ForEach(sbe =>
            {
                _storeBillEditCollection.Add(sbe);
            });

            if (_currentBill.MAssets.Any(x => x.ISCompietion != CompietionState.NotReported))
            {
                ISBillReported = true;
            }
        }

        private void SbEditFilters()
        {
            this.StoreBillEditView.Filter = ((obj) =>
            {
                StoreBillEdit items = obj as StoreBillEdit;
                if (items != null)
                    return items.Description.StartsWith(SearchCriteria);
                return false;
            });
        }

        private void saveEdit()
        {
            if (this.HasErrors)
            {
                _dialogService.ShowAlert("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (ISBillReported)
            {
                _dialogService.ShowAlert("توجه", "این قبض انبار دارای مال فرستاده شده به دارایی می باشد و دیگر قابل ویرایش نیست");
                return;
            }

            if (this._currentBill.StoreBillEdits.Any(sbe => sbe.State == GlobalRequestStatus.Pending))
            {
                _dialogService.ShowAlert("توجه", "این قبض انبار دارای درحواست ویرایش تکمیل نشده است و نمیتوان برای آن درخواست ویرایش دیگر ثبت کرد");
                return;
            }

            var newEdit = new StoreBillEdit
            {
                Description=this.Description,
                InsertDate=GlobalClass._Today,
                ObjectState=ObjectState.Added,
                State= GlobalRequestStatus.Pending,
            };
            _currentBill.StoreBillEdits.Add(newEdit);
            _storeBillService.InsertOrUpdateGraph(_currentBill);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _storeBillEditCollection.Add(newEdit);
                this.Selected = newEdit;
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

        private void deleteSbEdit(object parameter)
        {
            var storeBillEdit = parameter as StoreBillEdit;
            if (storeBillEdit != null)
            {
                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    storeBillEdit.ObjectState = ObjectState.Deleted;
                    _currentBill.StoreBillEdits.Remove(storeBillEdit);
                    _storeBillService.InsertOrUpdateGraph(_currentBill);
                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        _storeBillEditCollection.Remove(storeBillEdit);
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
           
        private void editMAssets(IList<object> parameters)
        {
            var storeBillEdit = parameters[0] as StoreBillEdit;
            if (storeBillEdit == null) return;
            var window = parameters[1] as Window;
            if (window == null) return;
            this.Selected = storeBillEdit;

            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new StoreBillMAssetEditViewModel(_container,_currentBill);
            if (_currentBill.StuffType == StuffType.Consumable)
            {
                viewModel.AssetList = _currentBill.Commodities.Select(co => new MovableAssetModel
                {
                    AcqType = _currentBill.AcqType,
                    AssetId = co.AssetId,
                    Cost = co.Cost,
                    CurState = MAssetCurState.AtOperation,
                    ExpirationDate = co.ExpirationDate,
                    InsertDate = co.InsertDate,
                    Name = co.Name,
                    IsCompietion = CompietionState.NotReported,
                    IsConfirmed = true,
                    IsInStore = true,
                    IsRowEnabled = false,
                    IsSelected = true,
                    KalaNo = co.KalaNo,
                    kalaUid = co.KalaUid,
                    Label = null,
                    MAssetType = "مصرفی",
                    Num=co.Num,
                    UnitId=co.UnitId
                }).ToList();
            }
            else
            {
                viewModel.AssetList = _currentBill.MAssets.Select(co => new MovableAssetModel
                {
                    AcqType = _currentBill.AcqType,
                    AssetId = co.AssetId,
                    Cost = co.Cost,
                    CurState =co.CurState,
                    InsertDate = co.InsertDate,
                    Name = co.Name,
                    IsCompietion = co.ISCompietion,
                    IsConfirmed =co.ISConfirmed,
                    IsInStore = true,
                    IsRowEnabled = false,
                    IsSelected = true,
                    KalaNo = co.KalaNo,
                    kalaUid = co.KalaUid,
                    Label = co.Label,
                    MAssetType = co.ToString("t",null),
                    Num = co.Num,
                    UnitId = co.UnitId
                }).ToList();
            }
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            _navigationService.ShowStoreBillMAssetEditWindow(viewModel);
            this.confirmEdit(storeBillEdit);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void confirmEdit(StoreBillEdit editSb)
        {
            Boolean confirm = _dialogService.AskConfirmation("پرسش", "آیا ویرایش طبق این درخواست تکمیل شده است");
            if (confirm)
            {
                editSb.ObjectState = ObjectState.Modified;
                editSb.State = GlobalRequestStatus.Completed;
                _currentBill.StoreBillEdits.Add(editSb);
                _storeBillService.InsertOrUpdateGraph(_currentBill);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    int index= _storeBillEditCollection.IndexOf(editSb);
                    _storeBillEditCollection.RemoveAt(index);
                    _storeBillEditCollection.Insert(index, editSb);
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

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand EditCommand { get; private set; }

        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveEdit(); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
               (parameter) => { },
               (parameter) => { return true; }
               );

            DeleteCommand = new MvvmCommand(
                 (parameter) => { this.deleteSbEdit(parameter); },
                (parameter) => { return true; }
                );

            EditCommand = new MvvmCommand(
                  (parameter) => { this.editMAssets(parameter as IList<object>); },
                (parameter) => { return !ISBillReported; }
                ).AddListener<StoreBillEditViewModel>(this,s=>s.ISBillReported);
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStoreBillService _storeBillService;
        private readonly ObservableCollection<StoreBillEdit> _storeBillEditCollection;
        private  StoreBill _currentBill;
        private string _description;

        #endregion
    }
}
