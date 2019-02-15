
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.Repository.Model;
    using System.Collections.ObjectModel;
    using Helper;
    using System.ComponentModel;
    using System.Windows.Data;
    using Domain.Entity.AssetEntity;

    public sealed class StoreAssetDetailsViewModel : BaseViewModel
    {
        #region ctor

        public StoreAssetDetailsViewModel(IUnityContainer container,List<Tuple<string,StuffType,int,string>> stuffs,AnalizModel amModel)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._personService = _container.Resolve<IPersonService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._orderService = _container.Resolve<IOrderService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._storefiristGeneration = new ObservableCollection<StoreTreeViewModel>();
            this._orderCollection = new ObservableCollection<OrderSumModel>();
            this.OrderDetailsFilteredView = new CollectionViewSource { Source = OrderCollection }.View;
            this.Stuffs = stuffs;
            this.initalizObj(amModel);
            this.initalizCommands();
        }

        #endregion

        #region properties

        public List<Tuple<string, StuffType,int,string>> Stuffs
        {
            get { return GetValue(() => Stuffs); }
            private set
            {
                SetValue(() => Stuffs, value);
            }
        }

        public Tuple<string, StuffType,int,string> SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
                if (value != null)
                {
                    this.initLists();
                }
            }
        }

        public List<Store> Stores
        {
            get { return GetValue(() => Stores); }
            set
            {
                SetValue(() => Stores, value);
            }
        }

        public Store SelectedStore
        {
            get { return GetValue(() => SelectedStore); }
            set
            {
                SetValue(() => SelectedStore, value);
                this.GetStoreParentNode();
            }
        }

        public Boolean GroupingView
        {
            get { return GetValue(() => GroupingView); }
            set
            {
                SetValue(() => GroupingView, value);
                searchStoreAssets();
            }
        }

        public List<MovableAssetModel> MAssetList
        {
            get { return GetValue(() => MAssetList); }
            set
            {
                SetValue(() => MAssetList, value);
            }
        }

        public List<StoreBillIssueModel> StoreBillList
        {
            get { return GetValue(() => StoreBillList); }
            set
            {
                SetValue(() => StoreBillList, value);
            }
        }

        public List<StoreBillIssueModel> DocumentList
        {
            get { return GetValue(() => DocumentList); }
            set
            {
                SetValue(() => DocumentList, value);
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
        public StoreTreeViewModel StoreDesignSelected
        {
            get { return GetValue(() => StoreDesignSelected); }
            set
            {
                SetValue(() => StoreDesignSelected, value);
            }
        }
        public ObservableCollection<StoreTreeViewModel> StoreFiristGeneration
        {
            get { return _storefiristGeneration; }
        }

        public Boolean CurrentStock
        {
            get { return GetValue(() => CurrentStock); }
            set
            {
                SetValue(() => CurrentStock, value);
            }
        }

        public Boolean IdleOrder
        {
            get { return GetValue(() => IdleOrder); }
            set
            {
                SetValue(() => IdleOrder, value);
            }
        }

        public Boolean DelviryOrder
        {
            get { return GetValue(() => DelviryOrder); }
            set
            {
                SetValue(() => DelviryOrder, value);
            }
        }

        public Boolean StoreDraftDoc
        {
            get { return GetValue(() => StoreDraftDoc); }
            set
            {
                SetValue(() => StoreDraftDoc, value);
            }
        }

        public Boolean StoreBillDoc
        {
            get { return GetValue(() => StoreBillDoc); }
            set
            {
                SetValue(() => StoreBillDoc, value);
            }
        }

        public PersianDate FromDate
        {
            get { return GetValue(() => FromDate); }
            set
            {
                SetValue(() => FromDate, value);
            }
        }

        public PersianDate ToDate
        {
            get { return GetValue(() => ToDate); }
            set
            {
                SetValue(() => ToDate, value);
            }
        }

        public ObservableCollection<OrderSumModel> OrderCollection
        {
            get { return _orderCollection; }
        }

        public ICollectionView OrderDetailsFilteredView { get; set; }
        
        public OrderStatus OrderState
        {
            get { return GetValue(() => OrderState); }
            set
            {
                SetValue(() => OrderState, value);
                this.filterOrdersByState();
            }
        }
        #endregion

        #region methods

        private void initalizObj(AnalizModel amModel)
        {
            List<Store> stores = null;
            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
            {
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                    .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                stores = _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId)).ToList();
            }
            else
            {
                stores = _storeService.Queryable()
                    .Where(s => s.StoreType != StoreType.Retiring).ToList();
            }
            stores.Insert(0, new Store {StoreId=0,Name="کل انبارها",ObjectState=Client.API.Infrastructure.ObjectState.Unchanged});
            Stores = stores;

            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.PersianDateTime();

            SelectedStore = Stores.FirstOrDefault();
            Units = _unitService.Queryable().ToList();
            this.setRbByIdentity(amModel);
            OrderState = amModel.OrderStatus;
        }

        private void setRbByIdentity(AnalizModel amModel)
        {
            switch (amModel.Identity)
            {
                case AnalizModelIdentity.Stock:
                    CurrentStock = true;
                    break;
                case AnalizModelIdentity.Order:
                    DelviryOrder = true;
                    break;
                case AnalizModelIdentity.InternalDraft:
                    StoreDraftDoc = true;
                    break;
                case AnalizModelIdentity.StoreBill:
                    StoreBillDoc = true;
                    break;
                default:
                    CurrentStock = true;
                    break;
            }
        }

        private void initLists()
        {
            this.searchStoreAssets();
        }

        private void GetStoreParentNode()
        {
            if (SelectedStore == null) return;
            var items = _storeService.GetParentNode(SelectedStore.StoreId).Where(x => x.ParentNode == null).ToList();

            _storefiristGeneration.Clear();
            foreach (var store in items) _storefiristGeneration.Add(new StoreTreeViewModel(store, null, true));
        }

        private void searchStoreAssets()
        {
            if (SelectedStore == null || SelectedStuff == null)
            {
                return;
            }
            bool isCommodity = false;
            if (SelectedStuff.Item2 == StuffType.Consumable)
            {
                isCommodity = true;
            }
            Mouse.SetCursor(Cursors.Wait);

            DateTime date1 = FromDate.ToDateTime();
            DateTime dt = ToDate.ToDateTime().AddDays(1);

            if (CurrentStock)
            {
                if (isCommodity)
                {
                    var lstTemp = new List<MovableAssetModel>();
                    IEnumerable<Commodity> commodities = null;
                    if (SelectedStore.StoreId <= 0)
                    {
                        commodities = _commodityService.Query(co => co.KalaUid == SelectedStuff.Item3)
                       .Include(co => co.PlaceOfUses).Select().AsEnumerable();
                    }
                    else
                    {
                        commodities = _commodityService.Query(co => co.KalaUid == SelectedStuff.Item3 && co.StoreBill.StoreId == SelectedStore.StoreId)
                     .Include(co => co.PlaceOfUses).Select().AsEnumerable();
                    }
                    commodities.ForEach(co =>
                       {
                           if (co.PlaceOfUses.Count() > 0)
                           {
                               double memo = 0;
                               var coU = Units.First(v => v.UnitId == co.UnitId);
                               co.PlaceOfUses.ForEach(cop =>
                               {
                                   var copU = Units.First(u => u.UnitId == cop.UnitId);
                                   double coMemo = 0;
                                   if (coU.Equals(copU))
                                   {
                                       coMemo += cop.Num;
                                   }
                                   else
                                   {
                                       Unit isOrderChild = null;
                                       Unit IsPropertyChild = null;

                                       if (coU != null)
                                       {
                                           isOrderChild = mainparentRecovery(coU);
                                       }

                                       if (copU != null)
                                       {
                                           IsPropertyChild = mainparentRecovery(copU);
                                       }

                                       if (isOrderChild.Equals(IsPropertyChild))
                                       {
                                           Double orderval = CalculateUnitNum(copU, cop.Num);
                                           double propertyVal = ReverseCalculateUnitNum(coU, orderval);
                                           coMemo = propertyVal;
                                       }
                                   }
                                   memo += coMemo;
                               });

                               if (!(memo >= co.Num))
                               {
                                   co.Num -= memo;

                                   var m = new MovableAssetModel
                                   {
                                       AcqType = StateOwnership.Purchase,
                                       AssetId = co.AssetId,
                                       InsertDate = co.InsertDate,
                                       CurState = MAssetCurState.AtOperation,
                                       IsCompietion = CompietionState.NotReported,
                                       IsConfirmed = false,
                                       IsInStore = true,
                                       kalaUid = co.KalaUid,
                                       Label = null,
                                       MAssetType = "مصرفی",
                                       Name = co.Name,
                                       Num = co.Num,
                                       UnitId = co.UnitId
                                   };
                                   lstTemp.Add(m);
                               }
                           }
                           else
                           {
                               var m = new MovableAssetModel
                               {
                                   AcqType = StateOwnership.Purchase,
                                   AssetId = co.AssetId,
                                   InsertDate = co.InsertDate,
                                   CurState = MAssetCurState.AtOperation,
                                   IsCompietion = CompietionState.NotReported,
                                   IsConfirmed = false,
                                   IsInStore = true,
                                   kalaUid = co.KalaUid,
                                   Label = null,
                                   MAssetType = "مصرفی",
                                   Name = co.Name,
                                   Num = co.Num,
                                   UnitId = co.UnitId
                               };
                               lstTemp.Add(m);
                           }
                       });
                    MAssetList = lstTemp;
                }
                else
                {
                    MAssetList = _movableAssetService.GetStoreMovableAssetByLocation(null, null, SelectedStore.StoreId)
                        .Where(x => x.Name == SelectedStuff.Item1 && x.kalaUid == SelectedStuff.Item3).ToList();
                }

                if (GroupingView)
                {
                    MAssetList = (from c in MAssetList
                                  group c by new { c.UnitId, c.kalaUid, c.MAssetType } into g
                                  where g.Count() >= 1
                                  select new MovableAssetModel
                                  {
                                      AssetId = g.First().AssetId,
                                      Name = g.First().Name,
                                      Num = g.Sum(x => x.Num),
                                      UnitId = g.Key.UnitId,
                                      StoreDesignId = g.First().StoreDesignId,
                                      StoreId = SelectedStore != null ? SelectedStore.StoreId : 0,
                                      MAssetType = g.Key.MAssetType,
                                      InsertDate = g.First().InsertDate,
                                      Label = null
                                  }).ToList();
                }
            }
            else if (StoreBillDoc)
            {
                StoreBillList = _storeBillService.GetIssueBillByStuff(SelectedStuff.Item3,SelectedStuff.Item2,date1,dt,SelectedStore.StoreId,false,isCommodity).ToList();

                if (GroupingView)
                {
                    StoreBillList = (from b in StoreBillList
                                     group b by new { b.KalaUid } into g

                                     where g.Count()>=1
                                     select new StoreBillIssueModel
                                     {
                                         Date = new DateTime(1, 1, 1),
                                         KalaUid =g.Key.KalaUid,
                                         Num=g.Sum(v=>v.Num),
                                        Price=g.Sum(v=>v.Price),
                                        SBNo="**"
                                     }
                                   ).ToList();
                }
            }
            else if (StoreDraftDoc)
            {
                if (isCommodity)
                {
                    DocumentList =_commodityService.GetStoreDocumentsIssue(SelectedStuff.Item3, date1, dt, SelectedStore.StoreId, DocumentType.StoreInternalDraft).ToList();
                }
                else
                {
                    DocumentList = _movableAssetService.GetStoreDocumentsIssue(SelectedStuff.Item3,SelectedStuff.Item2, date1, 
                        dt, SelectedStore.StoreId, DocumentType.StoreInternalDraft,false).ToList();
                }
            
                if (GroupingView)
                {
                    DocumentList = (from b in DocumentList
                                    group b by new { b.KalaUid } into g
                                     where g.Count() >= 1
                                     select new StoreBillIssueModel
                                     {
                                         Date = new DateTime(1, 1, 1),
                                         KalaUid = g.Key.KalaUid,
                                         Num = g.Sum(v => v.Num),
                                         Price = g.Sum(v => v.Price),
                                         SBNo = "**",
                                         Transfree="**"
                                     }
                                   ).ToList();
                }
            }
            else if (DelviryOrder)
            {
                _orderCollection.Clear();
                var items = _orderService.GetOrderDetailsByKalaUid(SelectedStuff.Item3,SelectedStuff.Item2, date1, dt,false);

                if (OrderState != OrderStatus.None)
                {
                    items = items.Where(x => x.Status == OrderState);
                }

                if (GroupingView)
                {
                    items.ToList()
                        .GroupBy(g => new { g.KalaUid, g.UnitId}).Where(g => g.Count() > 0).Select(g => new OrderSumModel
                    {
                        KalaUid = g.Key.KalaUid,
                        Num = g.Sum(v => v.Num),
                        UnitId = g.Key.UnitId,
                        OrderType=default(OrderType),
                        Status=OrderStatus.None,
                        OrderDate=new DateTime(1,1,1)
                    }).ForEach(g =>
                    {
                        _orderCollection.Add(g);
                    });
                }
                else
                {
                    items.ToList().ForEach(od =>
                    {
                        _orderCollection.Add(od);
                    });

                }
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void filterOrdersByState()
        {
            if (OrderState != OrderStatus.None)
            {
                OrderDetailsFilteredView.Filter = ((obj) =>
                {
                    var od = obj as OrderSumModel;
                    return od.Status == OrderState;
                });
            }
        }

        private Unit mainparentRecovery(Unit parent)
        {
            Unit mparent = parent;

            if (parent.Parent != null)
            {
                mparent = this.mainparentRecovery(parent.Parent);
            }
            return mparent;
        }

        private Double CalculateUnitNum(Unit unit, Double val)
        {
            switch (unit.MathType)
            {
                case UnitMathType.Multiple:
                    val = (val * unit.MathNum.Value);
                    break;
                case UnitMathType.Divide:
                    val = (val / unit.MathNum.Value);
                    break;
            }
            if (unit.Parent != null)
            {
                val = CalculateUnitNum(unit.Parent, val);
            }
            return val;
        }

        private Double ReverseCalculateUnitNum(Unit unit, Double val)
        {
            switch (unit.MathType)
            {
                case UnitMathType.Divide:
                    val = (val * unit.MathNum.Value);
                    break;
                case UnitMathType.Multiple:
                    val = (val / unit.MathNum.Value);
                    break;
            }
            if (unit.Parent != null)
            {
                val = ReverseCalculateUnitNum(unit.Parent, val);
            }
            return val;
        }
        #endregion

        #region commands

        public ICommand SearchCommand { get;private set;}

        private void initalizCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.searchStoreAssets(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IStoreService _storeService;
        private readonly IStoreBillService _storeBillService;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly IUnitService _unitService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly ObservableCollection<StoreTreeViewModel> _storefiristGeneration;
        private readonly ObservableCollection<OrderSumModel> _orderCollection;

        #endregion
    }
}
