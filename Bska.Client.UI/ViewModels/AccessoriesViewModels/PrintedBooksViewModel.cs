
namespace Bska.Client.UI.ViewModels.AccessoriesViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;

    public sealed class PrintedBooksViewModel : BaseDetailsViewModel<MovableAsset>
    {
        #region ctor

        public PrintedBooksViewModel(MovableAsset currentEntity):
            base(currentEntity) { }

        #endregion

        #region properties

        public String BookName
        {
            get { return CurrentEntity.Uid1; }
            set
            {
                CurrentEntity.Uid1 = value;
                OnPropertyChanged("BookName");
            }
        }

        public String PageCount
        {
            get { return CurrentEntity.Uid2; }
            set
            {
                CurrentEntity.Uid2 = value;
                OnPropertyChanged("PageCount");
            }
        }

        public String CoverCount
        {
            get { return CurrentEntity.Uid3; }
            set
            {
                CurrentEntity.Uid3 = value;
                OnPropertyChanged("CoverCount");
            }
        }

        public String Language
        {
            get { return CurrentEntity.Uid4; }
            set
            {
                CurrentEntity.Uid4 = value;
                OnPropertyChanged("Language");
            }
        }

        public String PrintYear
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                OnPropertyChanged("PrintYear");
            }
        }

        public String Author
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Author");
            }
        }

        public String CoverType
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                OnPropertyChanged("CoverType");
            }
        }

        public String ISBN
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                OnPropertyChanged("ISBN");
            }
        }

        #endregion
    }
}
