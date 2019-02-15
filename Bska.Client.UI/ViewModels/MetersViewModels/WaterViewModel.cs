
namespace Bska.Client.UI.ViewModels.MetersViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public sealed class WaterViewModel : BaseDetailsViewModel<WaterMeter>
    {
        #region ctor

        public WaterViewModel(WaterMeter currentEntity)
            : base(currentEntity)
        {
        }

        #endregion

        #region properties

        public Int32 MeterId
        {
            get { return CurrentEntity.ImAssetId; }
        }

        [Required(ErrorMessage = "نام مشترک الزامی است")]
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

        [Required(ErrorMessage = "کد پستی الزامی است")]
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

        [Required(ErrorMessage = "شماره پرونده الزامی است")]
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

        [Required(ErrorMessage = "نوع تعرفه الزامی است")]
        public Int32 TariffType
        {
            get { return CurrentEntity.TariffType; }
            set
            {
                CurrentEntity.TariffType = value;
                OnPropertyChanged("TariffType");
            }
        }

        public Double WaterSplitDiagonal
        {
            get { return CurrentEntity.WaterSplitDiagonal; }
            set
            {
                CurrentEntity.WaterSplitDiagonal = value;
                OnPropertyChanged("WaterSplitDiagonal");
            }
        }

        public Double WasteSplitDiagonal
        {
            get { return CurrentEntity.WasteSplitDiagonal; }
            set
            {
                CurrentEntity.WasteSplitDiagonal = value;
                OnPropertyChanged("WasteSplitDiagonal");
            }
        }

        public String MeterStatus
        {
            get { return CurrentEntity.MeterStatus; }
            set
            {
                CurrentEntity.MeterStatus = value;
                OnPropertyChanged("MeterStatus");
            }
        }

        #endregion

    }
}
