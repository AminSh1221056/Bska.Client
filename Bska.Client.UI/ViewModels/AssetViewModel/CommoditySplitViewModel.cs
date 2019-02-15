
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using Data.Service;
    using Domain.Entity;
    using Domain.Entity.AssetEntity;
    using Microsoft.Practices.Unity;
    using Repository.Model;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    public sealed class CommoditySplitViewModel : BaseViewModel
    {
        #region ctor

        public CommoditySplitViewModel(IUnityContainer container,Commodity currentAsset)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._unitService = _container.Resolve<IUnitService>();
            this.CurrentAsset = currentAsset;
            this.initializObj();
        }
        #endregion

        #region properties

        public Commodity CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            set
            {
                SetValue(() => CurrentAsset, value);
            }
        }
        public List<PlaceOfUseModel> PlaceOfUses
        {
            get { return GetValue(() => PlaceOfUses); }
            set
            {
                SetValue(() => PlaceOfUses, value);
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
            Units = _unitService.Queryable().ToList();
            var organiz = _employeeService.GetParentNode(1);
            var strategy = _employeeService.GetParentNode(2);
            var items = new List<PlaceOfUseModel>();
            _commodityService.Queryable().Where(v => v.AssetId == CurrentAsset.AssetId)
                .SelectMany(co => co.PlaceOfUses).Include(pu=>pu.Document).ToList().ForEach(pu =>
            {
                items.Add(new PlaceOfUseModel
                {
                    InsertDate = pu.Document.DocumentDate,
                    DocumentId=pu.Document.Desc1,
                    Num = pu.Num,
                    UnitId = pu.UnitId,
                    OrganizName = GetHirecharyNode(organiz.FirstOrDefault(o => o.BuidldingDesignId == pu.OrganizId)),
                    StrategyName = GetHirecharyNode(strategy.FirstOrDefault(o => o.BuidldingDesignId == pu.StrategtyId)),
                });
            });
            this.PlaceOfUses = items;
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

        #endregion

        #region commands
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IUnitService _unitService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IEmployeeService _employeeService;

        #endregion
    }
}
