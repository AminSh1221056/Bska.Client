
namespace Bska.Client.UI.ViewModels.AccessoriesViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.ComponentModel.DataAnnotations;
    public sealed class HandmadeCarpetViewModel : BaseDetailsViewModel<MovableAsset>
    {
        #region ctor

        public HandmadeCarpetViewModel(MovableAsset currentEntity)
            : base(currentEntity)
        {

        }
        #endregion

        #region properties

        [Required(ErrorMessage = "مساحت الزامی است")]
        public String Area
        {
            get { return CurrentEntity.Uid1; }
            set
            {
                CurrentEntity.Uid1 = value;
                ValidateProperty(value);
                OnPropertyChanged("Area");
            }
        }

        public String Length
        {
            get { return CurrentEntity.Uid2; }
            set
            {
                CurrentEntity.Uid2 = value;
                OnPropertyChanged("Length");
            }
        }

        public String Width
        {
            get { return CurrentEntity.Uid3; }
            set
            {
                CurrentEntity.Uid3 = value;
                OnPropertyChanged("Width");
            }
        }

        [Required(ErrorMessage = "تعداد رج الزامی است")]
        public String RowCount
        {
            get { return CurrentEntity.Uid4; }
            set
            {
                CurrentEntity.Uid4 = value;
                ValidateProperty(value);
                OnPropertyChanged("RowCount");
            }
        }

        [Required(ErrorMessage = "رنگ الزامی است")]
        public String Color
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                ValidateProperty(value);
                OnPropertyChanged("Color");
            }
        }

        public String Plan
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Plan");
            }
        }

        public String PlaceTissue
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                OnPropertyChanged("PlaceTissue");
            }
        }

        [Required(ErrorMessage = "شکل هندسی الزامی است")]
        public String Diagram
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                ValidateProperty(value);
                OnPropertyChanged("Diagram");
            }
        }

        #endregion
    }
}
