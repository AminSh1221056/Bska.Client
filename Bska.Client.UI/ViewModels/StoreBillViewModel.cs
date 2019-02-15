
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Helper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public sealed class StoreBillViewModel : BaseDetailsViewModel<StoreBill>
    {
        #region ctor

        public StoreBillViewModel(StoreBill currentEntity)
            : base(currentEntity)
        {
            _trustMaxReturnDate = new PersianDate(PersianDate.Today.Year + 1, 12, 29);
            this._seedDataHelper = new SeedDataHelper();
        }

        #endregion

        #region properties

        public Int32 StoreBillId
        {
            get { return CurrentEntity.StoreBillId; }
        }

        [Required(ErrorMessage = "شماره قبض انبار الزامی است")]
        public String StoreBillNo
        {
            get { return CurrentEntity.StoreBillNo; }
            set
            {
                CurrentEntity.StoreBillNo = value;
                ValidateProperty(value);
                OnPropertyChanged("StoreBillNo");
            }
        }

        [Required(ErrorMessage = "تاریخ قبض انبار الزامی است")]
        public PersianDate ArrivalDate
        {
            get { return _arrivalDate; }
            set
            {
                _arrivalDate = value;
                ValidateProperty(value);
                OnPropertyChanged("ArrivalDate");
            }
        }

        [Required(ErrorMessage = "نوع قبض انبار الزامی است")]
        public StateOwnership AcqTyp
        {
            get { return CurrentEntity.AcqType; }
            set
            {
                CurrentEntity.AcqType = value;
                ValidateProperty(value);
                OnPropertyChanged("AcqTyp");
                this.initOnAcqTyp();
            }
        }
        
        [Required(ErrorMessage = "این فیلد الزامی می باشد")]
        public String Desc1
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                ValidateProperty(value);
                OnPropertyChanged("Desc1");
            }
        }

        public String Desc2
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Desc2");
            }
        }

        public String Desc3
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                OnPropertyChanged("Desc3");
            }
        }

        public List<Store> Stores
        {
            get { return GetValue(() => Stores); }
            set
            {
                SetValue(() => Stores, value);
            }
        }

        [Required(ErrorMessage = "انتخاب انبار الزامی است")]
        public Store SelectedStore
        {
            get { return CurrentEntity.Store; }
            set
            {
                CurrentEntity.Store = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedStore");
            }
        }

        public Boolean IsPurchaseEnabled
        {
            get { return GetValue(() => IsPurchaseEnabled); }
            set
            {
                SetValue(() => IsPurchaseEnabled, value);
            }
        }

        public List<OrganizationModel> Organizations
        {
            get { return GetValue(() => Organizations); }
            set
            {
                SetValue(() => Organizations, value);
            }
        }

        public List<Seller> Sellers
        {
            get { return GetValue(() => Sellers); }
            set
            {
                SetValue(() => Sellers, value);
            }
        }

        public Seller SelectedSeller
        {
            get { return GetValue(() => SelectedSeller); }
            set
            {
                SetValue(() => SelectedSeller, value);
            }
        }

        #endregion

        #region methods

        private void initOnAcqTyp()
        {
            if (AcqTyp != StateOwnership.Purchase)
            {
                if (AcqTyp == StateOwnership.Trust)
                {
                    this.Desc2 = _trustMaxReturnDate.ToString();
                }
                Organizations = _seedDataHelper.GetOrganizations();
            }
        }
        
        #endregion

        #region fields

        PersianDate _arrivalDate;
        internal PersianDate _trustMaxReturnDate;
        private readonly SeedDataHelper _seedDataHelper;

        #endregion
    }
}
