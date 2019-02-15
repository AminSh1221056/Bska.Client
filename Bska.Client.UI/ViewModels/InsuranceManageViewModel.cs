
namespace Bska.Client.UI.ViewModels
{
    using Common;
    using Domain.Entity;
    using Microsoft.Practices.Unity;
    using System.Collections.Generic;
    using System;
    using Client.API.UnitOfWork;
    using System.ComponentModel.DataAnnotations;
    using System.Windows.Input;
    using Services;
    using Data.Service;
    using Domain.Entity.AssetEntity;
    using System.Threading;
    using System.Linq;
    using API;
    using Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;
    using Helper;
    using System.Windows;

    public sealed class InsuranceManageListViewModel : BaseListViewModel<Insurance>
    {
        #region ctor

        public InsuranceManageListViewModel(IUnityContainer container, UnConsumption currentAsset, Boolean isRealAsset)
            :base(new List<BaseDetailsViewModel<Insurance>>())
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository",_unitOfWork.Repository<MovableAsset>()));
            _isRealAsset = isRealAsset;
            this.initailzObj(currentAsset);
            this.initializCommands();
        }

        #endregion

        #region properties
        
        public Boolean IsEditableAsset
        {
            get { return GetValue(() => IsEditableAsset); }
            set
            {
                SetValue(() => IsEditableAsset, value);
            }
        }
        public UnConsumption CurrentMovableAsset
        {
            get { return GetValue(() => CurrentMovableAsset); }
            set
            {
                SetValue(() => CurrentMovableAsset, value);
            }
        }
        #endregion

        #region methods

        public override void SelectedItemChanged()
        {
            //throw new NotImplementedException();
        }

        private void initailzObj(UnConsumption currentAsset)
        {
            if (_isRealAsset)
            {
                CurrentMovableAsset = _movableAssetService.Queryable()
                    .Where(ma => ma.AssetId == currentAsset.AssetId).OfType<UnConsumption>().SingleOrDefault();
            }
            else
            {
                CurrentMovableAsset = currentAsset;
            }

            if (Thread.CurrentPrincipal.IsInRole("Manager") || Thread.CurrentPrincipal.IsInRole("StuffHonest"))
            {
                IsEditableAsset = true;
                this.initNewItem();
            }
            this.getInsurance();
        }

        private void getInsurance()
        {
            Collection.Clear();
            if (_isRealAsset)
            {
                _movableAssetService.Queryable().OfType<UnConsumption>().Where(x=>x.AssetId==CurrentMovableAsset.AssetId)
                    .SelectMany(x=>x.Insurances).ForEach(tc =>
                {
                    Collection.Add(new InsuranceManageDetailsViewModel(tc));
                });
            }
            else
            {
                CurrentMovableAsset.Insurances.ForEach(tc =>
                {
                    Collection.Add(new InsuranceManageDetailsViewModel(tc));
                });
            }
        }

        private void saveInsurance(object parameter)
        {
            if (this.Selected.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (_isRealAsset)
            {
                Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    if (Selected.CurrentEntity.ObjectState != ObjectState.Added)
                    {
                        Selected.CurrentEntity.ObjectState = ObjectState.Modified;
                    }
                    CurrentMovableAsset.Insurances.Add(Selected.CurrentEntity);
                    _movableAssetService.InsertOrUpdateGraph(CurrentMovableAsset);
                    try
                    {
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        if (!Collection.Contains(Selected))
                        {
                            Collection.Add(Selected);
                        }
                        this.initNewItem();
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
            else
            {
                var win = parameter as Window;
                if (win != null)
                {
                    CurrentMovableAsset.Insurances.Add(Selected.CurrentEntity);
                    if (!Collection.Contains(Selected))
                    {
                        Collection.Add(Selected);
                    }
                    this.initNewItem();
                    Boolean confirm = _dialogService.AskConfirmation("پرسش", "تغییرات با موفقیت انجام شد.آیا می خواهید هزینه جدید ثبت کنید");
                    if (!confirm)
                    {
                        win.DialogResult = true;
                    }
                }
            }

        }

        private void DeleteInsurance()
        {

        }

        private void initNewItem()
        {
            this.Selected = new InsuranceManageDetailsViewModel(new Insurance() { ObjectState=ObjectState.Added}) { InsuranceCompany = "", InsuranceNo = "", Missionary = 0, Type = InsuranceType.QualityGuarantee, ValidityDate = GlobalClass._Today };
        }

        #endregion

        #region commands

        public ICommand NewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set;}
        public ICommand ReportCommand { get; private set; }

        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveInsurance(parameter); },
                (parameter) => { return true; }
                );

            NewCommand = new MvvmCommand(
                (parameter) => { this.initNewItem(); },
               (parameter) => { return IsEditableAsset; }
          ).AddListener<AssetCostViewModel>(this, x => x.IsEditableAsset);
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly Boolean _isRealAsset;

        #endregion

    }

    public sealed class InsuranceManageDetailsViewModel : BaseDetailsViewModel<Insurance>
    {
        #region ctor

        public InsuranceManageDetailsViewModel(Insurance currentEntity)
            : base(currentEntity)
        {

        }

        #endregion

        #region properties

        public Int32 InsuranceId
        {
            get { return CurrentEntity.InsuranceId; }
        }

        [Required(ErrorMessage = "نوع بیمه الزامی است")]
        public InsuranceType Type
        {
            get { return CurrentEntity.Type; }
            set
            {
                CurrentEntity.Type = value;
                ValidateProperty(value);
                OnPropertyChanged("Type");
            }
        }

        [Required(ErrorMessage = "نام سازمان بیمه الزامی است")]
        public string InsuranceCompany
        {
            get { return CurrentEntity.InsuranceCompany; }
            set
            {
                CurrentEntity.InsuranceCompany = value;
                ValidateProperty(value);
                OnPropertyChanged("InsuranceCompany");
            }
        }

        [Required(ErrorMessage ="شماره بیمه الزامی است")]
        public string InsuranceNo
        {
            get { return CurrentEntity.InsuranceNo; }
            set
            {
                CurrentEntity.InsuranceNo = value;
                ValidateProperty(value);
                OnPropertyChanged("InsuranceNo");
            }
        }

        [Required(ErrorMessage = "تاریخ بیمه نامه الزامی است")]
        public DateTime ValidityDate
        {
            get { return CurrentEntity.ValidityDate; }
            set
            {
                CurrentEntity.ValidityDate = value;
                ValidateProperty(value);
                OnPropertyChanged("ValidityDate");
            }
        }

        [Required(ErrorMessage = "مبلغ بیمه نامه الزامی است")]
        public decimal Missionary
        {
            get { return CurrentEntity.Missionary; }
            set
            {
                CurrentEntity.Missionary = value;
                ValidateProperty(value);
                OnPropertyChanged("Missionary");
            }
        }

        public string NoDamage
        {
            get { return CurrentEntity.NoDamage; }
            set
            {
                CurrentEntity.NoDamage = value;
                OnPropertyChanged("NoDamage");
            }
        }

        public Byte[] InsurancePolicyImage
        {
            get { return CurrentEntity.InsurancePolicyImage; }
            set
            {
                CurrentEntity.InsurancePolicyImage = value;
                OnPropertyChanged("InsurancePolicyImage");
            }
        }
        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region fields
        #endregion
    }
}
