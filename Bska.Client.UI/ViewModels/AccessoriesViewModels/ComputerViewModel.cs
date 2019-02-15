
namespace Bska.Client.UI.ViewModels.AccessoriesViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    public sealed class ComputerViewModel : BaseDetailsViewModel<MovableAsset>
    {
        #region ctor

        public ComputerViewModel(MovableAsset currentEntity)
            : base(currentEntity)
        {

        }
        #endregion

        #region properties

        public String Size
        {
            get { return CurrentEntity.Uid1; }
            set
            {
                CurrentEntity.Uid1 = value;
                OnPropertyChanged("Size");
            }
        }

        public String Color
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                OnPropertyChanged("Color");
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

        public String Serial
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                OnPropertyChanged("Serial");
            }
        }

        #endregion
    }
}
