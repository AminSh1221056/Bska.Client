
namespace Bska.Client.UI.ViewModels
{
    using API;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.Helper;
    using Client.API.Infrastructure;
    using Client.API.UnitOfWork;
    using CustomAttributes;
    using Data.Service;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    public sealed class AssetCostViewModel : BaseListViewModel<AssetTaxCost>
    {
        #region ctor

        public AssetCostViewModel(IUnityContainer container,MovableAsset currentAsset,Boolean isRealAsset)
            :base(new List<BaseDetailsViewModel<AssetTaxCost>>())
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
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

        public MovableAsset CurrentMovableAsset
        {
            get { return GetValue(() => CurrentMovableAsset); }
            set
            {
                SetValue(() => CurrentMovableAsset, value);
            }
        }
        
        public decimal RealCost
        {
            get { return GetValue(() => RealCost); }
            set
            {
                SetValue(() => RealCost, value);
            }
        }

        #endregion

        #region methods

        private void initailzObj(MovableAsset currentAsset)
        {
            if (_isRealAsset)
            {
                CurrentMovableAsset = _movableAssetService.Queryable().Where(ma => ma.AssetId == currentAsset.AssetId).SingleOrDefault();
                
            }
            else
            {
                CurrentMovableAsset = currentAsset;
            }
            getTaxCosts();
            this.initNewItem();
            this.RealCost= this.calculateAccuraccy();
            if (Thread.CurrentPrincipal.IsInRole("Manager") || Thread.CurrentPrincipal.IsInRole("StuffHonest"))
            {
                IsEditableAsset = true;
            }
        }

        public override void SelectedItemChanged()
        {
            //throw new NotImplementedException();
        }

        private void getTaxCosts()
        {
            Collection.Clear();
            if (_isRealAsset)
            {
                _movableAssetService.GetTaxCosts(CurrentMovableAsset.AssetId).ForEach(tc =>
                {
                    Collection.Add(new AssetCostDetailsViewModel(tc));
                });
            }
            else
            {
                CurrentMovableAsset.AssetTaxCost.ForEach(tc =>
                {
                    Collection.Add(new AssetCostDetailsViewModel(tc));
                });
            }
        }

        private void saveTaxCost(object parameter)
        {
            if (this.Selected.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (calculateAccuraccy()<=0)
            {
                _dialogService.ShowError("خطا", "مقدار واقعی محاسبه شده با توجه به مقدار وارد شده صحیح نیست");
                return;
            }

            Selected.CurrentEntity.ModifiedDate = GlobalClass._Today;
            if (_isRealAsset)
            {
                Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    if (Selected.CurrentEntity.ObjectState != ObjectState.Added)
                    {
                        Selected.CurrentEntity.ObjectState = ObjectState.Modified;
                    }

                    CurrentMovableAsset.AssetTaxCost.Add(Selected.CurrentEntity);
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
                        this.RealCost = this.calculateAccuraccy();
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
                    CurrentMovableAsset.AssetTaxCost.Add(Selected.CurrentEntity);
                    if (!Collection.Contains(Selected))
                    {
                        Collection.Add(Selected);
                    }
                    this.initNewItem();
                    this.RealCost = this.calculateAccuraccy();
                    Boolean confirm = _dialogService.AskConfirmation("پرسش", "تغییرات با موفقیت انجام شد.آیا می خواهید هزینه جدید ثبت کنید");
                    if (!confirm)
                    {
                        win.DialogResult = true;
                    }
                }
            }
        }

        private decimal calculateAccuraccy()
        {
            decimal allTaxCost = this.Collection.Sum(s => s.CurrentEntity.Cost)+ Selected.CurrentEntity.Cost;
            return CurrentMovableAsset.Cost + allTaxCost;
        }

        private void initNewItem()
        {
            this.Selected = new AssetCostDetailsViewModel(new AssetTaxCost() { ObjectState = ObjectState.Added }) { Cost = 0, TaxCostType = TaxCostType.None };
        }
        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveTaxCost(parameter); },
                (parameter) => { return IsEditableAsset; }
           ).AddListener<AssetCostViewModel>(this,x=>x.IsEditableAsset);

            NewCommand=new MvvmCommand(
                 (parameter) => { this.initNewItem(); },
                (parameter) => { return IsEditableAsset; }
           ).AddListener<AssetCostViewModel>(this, x => x.IsEditableAsset);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly Boolean _isRealAsset;

        #endregion
    }

    public sealed class AssetCostDetailsViewModel : BaseDetailsViewModel<AssetTaxCost>
    {
        #region ctor
        public AssetCostDetailsViewModel(AssetTaxCost currentEntity)
            :base(currentEntity)
        {

        }
        #endregion

        #region properties

        public Int32 Id
        {
            get { return CurrentEntity.Id; }
        }

        [Required(ErrorMessage="نوع هزینه الزامی است")]
        public TaxCostType TaxCostType
        {
            get { return CurrentEntity.TaxCostType; }
            set
            {
                CurrentEntity.TaxCostType = value;
                OnPropertyChanged("TaxCostType");
            }
        }

        public string Description
        {
            get { return CurrentEntity.Description; }
            set
            {
                CurrentEntity.Description = value;
                ValidateProperty(Cost);
                OnPropertyChanged("Description");
            }
        }

        [Required(ErrorMessage = "مقدار هزینه الزامی است")]
        [PositiveNumber(ErrorMessage ="مقدار وارد شده صحیح نیست")]
        public decimal Cost
        {
            get { return CurrentEntity.Cost; }
            set
            {
                CurrentEntity.Cost = value;
                ValidateProperty(Cost);
                OnPropertyChanged("Cost");
            }
        }
        
        public DateTime ModifiedDate
        {
            get { return CurrentEntity.ModifiedDate; }
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
