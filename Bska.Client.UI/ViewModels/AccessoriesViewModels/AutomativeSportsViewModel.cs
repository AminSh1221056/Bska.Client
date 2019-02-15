
namespace Bska.Client.UI.ViewModels.AccessoriesViewModels
{

    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
   
    public sealed class AutomativeSportsViewModel:BaseDetailsViewModel<MovableAsset>
    {
        #region ctor

        public AutomativeSportsViewModel(MovableAsset currentEntity)
            :base(currentEntity)
        {
           
        }

        #endregion

        #region properties

        public String Power
        {
            get { return CurrentEntity.Uid1; }
            set
            {
                CurrentEntity.Uid1 = value;
                OnPropertyChanged("Power");
            }
        }

        public String MotorNo
        {
            get { return CurrentEntity.Uid2; }
            set
            {
                CurrentEntity.Uid2 = value;
                OnPropertyChanged("MotorNo");
            }
        }

        public String BodyNo
        {
            get { return CurrentEntity.Uid3; }
            set
            {
                CurrentEntity.Uid3 = value;
                OnPropertyChanged("BodyNo");
            }
        }

        public String Model
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1= value;
                OnPropertyChanged("Model");
            }
        }

        public String Country
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                OnPropertyChanged("Country");
            }
        }

        public String Company
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Company");
            }
        }
        #endregion

    }
}
