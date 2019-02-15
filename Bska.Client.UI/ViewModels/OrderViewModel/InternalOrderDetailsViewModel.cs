
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.CustomAttributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    public sealed class InternalOrderDetailsViewModel : BaseDetailsViewModel<OrderDetails>
    {
         #region ctor

        public InternalOrderDetailsViewModel(OrderDetails currentEntity,bool IsEditable=true)
            :base(currentEntity)
        {
            this.IsEditable =IsEditable;
            this.NumIsEnable = true;
            EstimateAllPrice();
        }

        #endregion

        #region properties
        public Int64 Id
        {
            get { return CurrentEntity.OrderDetialsId; }
        }

        [Required(ErrorMessage = "تعداد الزامی می باشد")]
        [PositiveNumber(ErrorMessage = "تعداد نامعتبر می باشد")]
        public Double Num
        {
            get { return CurrentEntity.Num; }
            set
            {
                CurrentEntity.Num = value;
                ValidateProperty(value);
                OnPropertyChanged("Num");
                EstimateAllPrice();
            }
        }

        [Required(ErrorMessage = "واحد الزامی می باشد")]
        [PositiveIntNumber(ErrorMessage = "واحد نامعتبر می باشد")]
        public Int32 UnitId
        {
            get { return CurrentEntity.UnitId; }
            set
            {
                CurrentEntity.UnitId = value;
                ValidateProperty(value);
                OnPropertyChanged("UnitId");
            }
        }

        [Required(ErrorMessage = "قیمت تخمینی الزامی می باشد")]
        public Decimal EstitmatePrice
        {
            get { return CurrentEntity.EstimatePrice; }
            set
            {
                CurrentEntity.EstimatePrice = value;
                ValidateProperty(value);
                OnPropertyChanged("EstitmatePrice");
                EstimateAllPrice();
            }
        }

        public String UsingLocation
        {
            get { return CurrentEntity.UsingLocation; }
            set
            {
                CurrentEntity.UsingLocation = value;
                OnPropertyChanged("UsingLocation");
            }
        }

        public Decimal AllPrice
        {
            get { return GetValue(() => AllPrice); }
            set
            {
                SetValue(() => AllPrice, value);
            }
        }

        [Required(ErrorMessage = "درجه اهمیت الزامی می باشد")]
        public Int32 ImportantDegree
        {
            get { return CurrentEntity.ImportantDegree; }
            set
            {
                CurrentEntity.ImportantDegree = value;
                OnPropertyChanged("ImportantDegree");
            }
        }

        public String OfferQuality
        {
            get { return CurrentEntity.OfferQuality; }
            set
            {
                CurrentEntity.OfferQuality = value;
                OnPropertyChanged("OfferQuality");
            }
        }

        public String OfferSpecification
        {
            get { return CurrentEntity.OfferSpecification; }
            set
            {
                CurrentEntity.OfferSpecification = value;
                OnPropertyChanged("OfferSpecification");
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

        public Boolean IsEditable
        {
            get { return GetValue(() => IsEditable); }
            set
            {
                SetValue(() => IsEditable, value);
            }
        }

        public Boolean NumIsEnable
        {
            get { return GetValue(() => NumIsEnable); }
            set
            {
                SetValue(() => NumIsEnable, value);
            }
        }

        #endregion

        #region methods

        private void EstimateAllPrice()
        {
            if (Num > 0 && EstitmatePrice > 0)
            {
                AllPrice = (decimal)(Num * (double)EstitmatePrice);
            }
        }
        #endregion

        #region commands
        #endregion

        #region fields
        #endregion
    }
}
