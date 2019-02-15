
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.ComponentModel.DataAnnotations;
    public sealed class DocumentViewModel : BaseDetailsViewModel<Document>
    {
        #region ctor

        public DocumentViewModel(Document currentEntity)
            : base(currentEntity)
        {

        }

        #endregion

        #region properties

        public Int64 DocumentId
        {
            get
            {
                return CurrentEntity.DocumentId;
            }
        }

        [Required(ErrorMessage = "شماره الزامی است")]
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

        public String Desc4
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                OnPropertyChanged("Desc4");
            }
        }

        public PersianDate DocumentDate
        {
            get { return GetValue(() => DocumentDate); }
            set
            {
                SetValue(() => DocumentDate, value);
            }
        }

        #endregion
    }
}
