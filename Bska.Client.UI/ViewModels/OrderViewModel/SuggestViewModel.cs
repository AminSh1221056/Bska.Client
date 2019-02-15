
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    public sealed class SuggestViewModel : BaseViewModel
    {
        #region ctor

        public SuggestViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitService = _container.Resolve<IUnitService>();
            this._orderService = _container.Resolve<IOrderService>();
            this.initializCommand();
        }

        #endregion

        #region properties

        public Order CurrentOrder
        {
            get { return GetValue(() => CurrentOrder); }
            set
            {
                SetValue(() => CurrentOrder, value);
                this.initializObj();
            }
        }

        public List<OrderDetails> Collection
        {
            get { return GetValue(() => Collection); }
            set
            {
                SetValue(() => Collection, value);
            }
        }

        public OrderDetails Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
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

        private void initializObj()
        {
            this.Units = _unitService.Queryable().ToList();
            this.Collection = CurrentOrder.OrderDetails.ToList();
            this.Selected = Collection.FirstOrDefault();
            if(Selected!=null)
            this.SubOrders = _orderService.GetSubOrders(Selected.OrderDetialsId).ToList();
        }

        private void getSubOrders(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            this.Selected = od;
            this.SubOrders = _orderService.GetSubOrders(od.OrderDetialsId).ToList();
        }

        #endregion

        #region commands

        public ICommand SuggestCommand { get; private set; }

        private void initializCommand()
        {
            SuggestCommand = new MvvmCommand(
                (parameter) => { this.getSubOrders(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;

        #endregion
    }
}
