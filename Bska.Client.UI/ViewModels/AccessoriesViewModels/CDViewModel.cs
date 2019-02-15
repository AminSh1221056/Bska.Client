
namespace Bska.Client.UI.ViewModels.AccessoriesViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    public sealed class CDViewModel : BaseDetailsViewModel<MovableAsset>
    {
        #region ctor

        public CDViewModel(MovableAsset currentEntity)
            : base(currentEntity)
        {

        }
        #endregion

        #region properties

        public String CDName
        {
            get { return CurrentEntity.Uid1; }
            set
            {
                CurrentEntity.Uid1 = value;
                OnPropertyChanged("CDName");
            }
        }

        public String Number
        {
            get { return CurrentEntity.Uid2; }
            set
            {
                CurrentEntity.Uid2 = value;
                OnPropertyChanged("Number");
            }
        }

        #endregion
    }
}
