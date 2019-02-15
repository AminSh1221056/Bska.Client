
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Repository.Model;
    using Domain.Entity;
    using Helper;
    using System;
    using System.Collections.Generic;

    public sealed class ExternalOrderDetailsViewModel : BaseViewModel
    {
        #region ctor

        public ExternalOrderDetailsViewModel()
        {

        }
        #endregion

        #region properties

        public ExternalOrderModel CurrentExOrder
        {
            get { return GetValue(() => CurrentExOrder); }
            set
            {
                SetValue(() => CurrentExOrder, value);
            }
        }

        public List<ExternalOrderDetailsModel> ExOrderCollection
        {
            get { return GetValue(() => ExOrderCollection); }
            set
            {
                SetValue(() => ExOrderCollection, value);
            }
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }
        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region fields
        #endregion
    }
}
