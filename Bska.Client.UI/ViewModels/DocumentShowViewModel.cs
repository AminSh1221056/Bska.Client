
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.UI.ViewModels.StuffHonestViewModel;
    using Microsoft.Practices.Unity;
    using System;
    public sealed class DocumentShowViewModel : BaseViewModel
    {
        #region ctor

        public DocumentShowViewModel(IUnityContainer container, bool isStoreBill)
        {
            this._container = container;
            this._isStoreBill = isStoreBill;
            this.initalizObj();
        }

        #endregion

        #region properties

        public String DocType
        {
            get;
            set;
        }

        public Boolean StoreBillShow
        {
            get { return GetValue(() => StoreBillShow); }
            set
            {
                SetValue(() => StoreBillShow, value);
            }
        }

        public Boolean DocumentShow
        {
            get { return GetValue(() => DocumentShow); }
            set
            {
                SetValue(() => DocumentShow, value);
            }
        }

        public DocumentManageViewModel DocumentViewModel
        {
            get;
            private set;
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            if (_isStoreBill)
            {
                DocumentShow = false;
                StoreBillShow = true;
                this.DocumentViewModel = new DocumentManageViewModel(_container, 1);
            }
            else
            {
                this.DocumentViewModel = new DocumentManageViewModel(_container, 2);
                DocumentShow = true;
                StoreBillShow = false;
            }
        }

        #endregion

        #region commands

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private Boolean _isStoreBill;

        #endregion
    }
}
