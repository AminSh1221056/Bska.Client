
namespace Bska.Client.UI.ViewModels.MetersViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    
    public sealed class PowerViewModel : BaseDetailsViewModel<PowerMeter>
    {
        #region ctor

        public PowerViewModel(PowerMeter currentEntity)
            : base(currentEntity)
        {
        }

        #endregion

        #region properties

        public Int32 MeterId
        {
            get { return CurrentEntity.ImAssetId; }
        }

        [Required(ErrorMessage="نام مشترک الزامی است")]
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

        public String SubscriptionNo
        {
            get { return CurrentEntity.SubscriptionNo; }
            set
            {
                CurrentEntity.SubscriptionNo = value;
                OnPropertyChanged("SubscriptionNo");
            }
        }

        public String AddressLine
        {
            get { return CurrentEntity.AddressLine; }
            set
            {
                CurrentEntity.AddressLine = value;
                OnPropertyChanged("AddressLine");
            }
        }

        [Required(ErrorMessage="کد پستی الزامی است")]
        public String PostalCode
        {
            get { return CurrentEntity.PostalCode; }
            set
            {
                CurrentEntity.PostalCode = value;
                ValidateProperty(value);
                OnPropertyChanged("PostalCode");
            }
        }

        public String Plake
        {
            get { return CurrentEntity.Plake; }
            set
            {
                CurrentEntity.Plake = value;
                OnPropertyChanged("Plake");
            }
        }

        [Required(ErrorMessage="شماره پرونده الزامی است")]
        public String CaseNo
        {
            get { return CurrentEntity.CaseNo; }
            set
            {
                CurrentEntity.CaseNo = value;
                ValidateProperty(value);
                OnPropertyChanged("CaseNo");
            }
        }

        public String BodyNo
        {
            get { return CurrentEntity.BodyNo; }
            set
            {
                CurrentEntity.BodyNo = value;
                OnPropertyChanged("BodyNo");
            }
        }

        [Required(ErrorMessage="نوع تعرفه الزامی است")]
        public Int32 TariffType
        {
            get { return CurrentEntity.TariffType; }
            set
            {
                CurrentEntity.TariffType = value;
                OnPropertyChanged("TariffType");
            }
        }

        public Int32 FamiliesNum
        {
            get { return CurrentEntity.FamiliesNum; }
            set
            {
                CurrentEntity.FamiliesNum = value;
                OnPropertyChanged("FamiliesNum");
            }
        }

        public String IdentificationNo
        {
            get { return CurrentEntity.IdentificationNo; }
            set
            {
                CurrentEntity.IdentificationNo = value;
                OnPropertyChanged("IdentificationNo");
            }
        }

        public DateTime EarlyInstallationDate
        {
            get { return CurrentEntity.EarlyInstallationDate; }
            set
            {
                CurrentEntity.EarlyInstallationDate = value;
                OnPropertyChanged("EarlyInstallationDate");
            }
        }

        public Int32 Phase
        {
            get { return CurrentEntity.Phase; }
            set
            {
                CurrentEntity.Phase = value;
                OnPropertyChanged("Phase");
            }
        }

        public Double Amper
        {
            get { return CurrentEntity.Amper; }
            set
            {
                CurrentEntity.Amper = value;
                OnPropertyChanged("Amper");
            }
        }

        public String Statistic
        {
            get { return CurrentEntity.Statistic; }
            set
            {
                CurrentEntity.Statistic = value;
                OnPropertyChanged("Statistic");
            }
        }

        public Double Factor
        {
            get { return CurrentEntity.Factor; }
            set
            {
                CurrentEntity.Factor = value;
                OnPropertyChanged("Factor");
            }
        }

        #endregion

    }
}
