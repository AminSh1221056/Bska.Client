
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Microsoft.Practices.Unity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using System;
    using System.Collections.Generic;

    public sealed class SubOrderDetailsViewModel : BaseViewModel
    {
        #region ctor

        public SubOrderDetailsViewModel()
        { }

        #endregion

        #region properties

        public OrderDetails CurrentOrderDetails
        {
            get { return GetValue(() => CurrentOrderDetails); }
            set
            {
                SetValue(() => CurrentOrderDetails,value);
            }
        }

        public List<SubOrder> SubOrders
        {
            get { return GetValue(() => SubOrders); }
            set
            {
                SetValue(() => SubOrders, value);
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
