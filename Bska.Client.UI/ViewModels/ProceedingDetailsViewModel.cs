
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.CustomAttributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    public class ProceedingDetailsViewModel : BaseDetailsViewModel<Proceeding>
    {
        #region ctor

        public ProceedingDetailsViewModel(Proceeding currentEntity)
            :base(currentEntity)
        {
        }

        #endregion

        #region properties

        public Int32 Id
        {
            get { return CurrentEntity.ProceedingId; }
        }

        [Required(ErrorMessage = "این فیلد الزامی است")]
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

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public String Desc2
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                ValidateProperty(value);
                OnPropertyChanged("Desc2");
            }
        }

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public String Desc3
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                ValidateProperty(value);
                OnPropertyChanged("Desc3");
            }
        }

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public String Desc4
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                ValidateProperty(value);
                OnPropertyChanged("Desc4");
            }
        }

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public string Desc5
        {
            get { return CurrentEntity.Desc5; }
            set
            {
                CurrentEntity.Desc5 = value;
                ValidateProperty(value);
                OnPropertyChanged("Desc5");
            }
        }

        public string Desc6
        {
            get { return CurrentEntity.Desc6; }
            set
            {
                CurrentEntity.Desc6 = value;
                OnPropertyChanged("Desc6");
            }
        }
        
        public double CalculateDay
        {
            get { return GetValue(() => CalculateDay); }
            set
            {
                SetValue(() => CalculateDay,value);
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

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set
            {
                _selectedStore = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedStore");
            }
        }

        #endregion

        #region methods
        #endregion

        #region fields

        Store _selectedStore;

        #endregion
    }

    public sealed class ProceedingTrustTansferDetailsViewModel : ProceedingDetailsViewModel
    {
        #region ctor
        public ProceedingTrustTansferDetailsViewModel(Proceeding currentEntity)
            :base(currentEntity)
        {

        }

        #endregion

        #region properties


        [Required(ErrorMessage = "این فیلد الزامی است")]
        public PersianDate Date1
        {
            get { return _pDate1; }
            set
            {
                _pDate1 = value;
                ValidateProperty(value);
                OnPropertyChanged("Date1");
                this.calculateTrustDay();
            }
        }

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public PersianDate Date2
        {
            get { return _pDate2; }
            set
            {
               _pDate2 = value;
                ValidateProperty(value);
                OnPropertyChanged("Date2");
                this.calculateTrustDay();
            }
        }

        #endregion

        #region Methods

        private void calculateTrustDay()
        {
            if (_pDate2 > _pDate1)
            {
                CalculateDay = (_pDate2 - _pDate1).TotalDays;
                this.CurrentEntity.Desc4 = _pDate2.ToString();
                this.CurrentEntity.Desc3 = _pDate1.ToString();
            }
        }

        #endregion

        #region fields

        private PersianDate _pDate1;
        private PersianDate _pDate2;

        #endregion

    }
}
