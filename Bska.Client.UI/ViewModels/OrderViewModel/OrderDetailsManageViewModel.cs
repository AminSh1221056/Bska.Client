
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using API;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public sealed class OrderDetailsManageViewModel : BaseViewModel
    {
        #region ctor

        public OrderDetailsManageViewModel(IUnityContainer container,bool isEnableEdit=false)
        {
            this._container = container;
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._orderService = _container.Resolve<IOrderService>();
            this._personService = _container.Resolve<IPersonService>();
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._isEditable = isEnableEdit;
            this.initializCommand();
        }

        #endregion

        #region properties

        public OrderDetails CurrentOrderDetails
        {
            get { return GetValue(() => CurrentOrderDetails); }
            set
            {
                SetValue(() => CurrentOrderDetails, value);
                this.initializObj();
            }
        }

        public List<OrderDetails> AllOrderDetails
        {
            get { return GetValue(() => AllOrderDetails); }
            set
            {
                SetValue(() => AllOrderDetails, value);
            }
        }

        public InternalOrderDetailsViewModel InternalOrderDetails
        {
            get
            {
                return GetValue(() => InternalOrderDetails);
            }
            private set
            {
                SetValue(() => InternalOrderDetails, value);
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
        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        public String PersonName
        {
            get { return GetValue(() => PersonName); }
            set
            {
                SetValue(() => PersonName, value);
            }
        }

        public String OrganizPath
        {
            get { return GetValue(() => OrganizPath); }
            set
            {
                SetValue(() => OrganizPath, value);
            }
        }

        public String StraegyPath
        {
            get { return GetValue(() => StraegyPath); }
            set
            {
                SetValue(() => StraegyPath, value);
            }
        }

        public string PersonRequestDesc
        {
            get { return GetValue(() => PersonRequestDesc); }
            set
            {
                SetValue(() => PersonRequestDesc, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            if (CurrentOrderDetails == null) return;

            InternalOrderDetails = new InternalOrderDetailsViewModel(CurrentOrderDetails,_isEditable);
            if (CurrentOrderDetails.OrganizId.HasValue)
            {
                var organiz = _employeeService.GetParentNode(1).Where(x => x.BuidldingDesignId
                        == CurrentOrderDetails.OrganizId).SingleOrDefault(); ;

                if (organiz != null)
                {
                    OrganizPath ="منطقه سازمانی : "+ GetHirecharyNode(organiz);
                }

                var strategy = _employeeService.GetParentNode(2).Where(x => x.BuidldingDesignId
                    == CurrentOrderDetails.StrategyId).SingleOrDefault();
                if (strategy != null)
                {
                    StraegyPath = "منطقه استراتژیکی : " + GetHirecharyNode(strategy);
                }

                if (CurrentOrderDetails.Order != null)
                {
                    var person = _personService.Find(CurrentOrderDetails.Order.PersonId);
                    PersonName = $"نام پرسنل: {person.FirstName} {person.LastName}";
                    var permit = _personService.GetPersonPermit(person.PersonId).Where(p => p.OrganizId == organiz.BuidldingDesignId && p.StrategyId == strategy.BuidldingDesignId).FirstOrDefault();
                    if (permit != null)
                    {
                        PersonRequestDesc = $"موقعیت: {permit.RequestPermitId}";
                    }
                }
            }
            else if (CurrentOrderDetails.StoreId.HasValue)
            {
                var store = _storeService.Find(CurrentOrderDetails.StoreId);
                PersonName = "نام انبار" + " : " + store.Name;

                var storeDesign = _storeService.GetParentNode(store.StoreId).Where(x => x.StoreDesignId == CurrentOrderDetails.StoreDesignId).SingleOrDefault();
                if (storeDesign != null)
                {
                    OrganizPath = "آدرس انبار : " + GetStoreHirecharyNode(storeDesign);
                }
            }

            this.GetUnits(CurrentOrderDetails.StuffType);
        }

        private void GetUnits(Common.StuffType sId)
        {
            _subUnits.Clear();
            if(Units!=null)
            Units.Where(x => x.Parent == null && (x.StuffId == Common.StuffType.None || x.StuffId == sId)).ForEach(x =>
            {
                _subUnits.Add(new UnitTreeViewModel(x, _container));
            });
        }

        private String GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private String GetStoreHirecharyNode(StoreDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetStoreHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private void confirmOrder(object parameter)
        {
            var window = parameter as Window;
            if (window != null)
            {
                window.DialogResult = true;
            }
        }

        #endregion

        #region commands

        public ICommand ConfirmCommand { get; private set; }

        private void initializCommand()
        {
            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confirmOrder(parameter); },
                (parameter) => { return _isEditable; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IEmployeeService _employeeService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly IStoreService _storeService;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private readonly bool _isEditable = false;

        #endregion
    }
}
