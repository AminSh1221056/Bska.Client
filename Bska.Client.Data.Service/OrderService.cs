
namespace Bska.Client.Data.Service
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.Repository;
    using Bska.Client.Service.Pattern;
    using System;
    using System.Collections.Generic;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Common;
    using Domain.Entity;

    public interface IOrderService : IService<Order>
    {
        IEnumerable<OrderModel> GetOrders(int personId, DateTime minDate);
        IEnumerable<OrderModel> GetRecivedOrders(Boolean IsManager, int userId, Boolean forStore, OrderStatus oStatus);
        IEnumerable<OrderModel> GetRecivedOrganizManageOrders(int userId);
        IEnumerable<OrderUserHistory> GetUserHistories(Int64 orderDetailsId);
        IEnumerable<OrderDetails> GetRecivedOrderDetails(Int64 orderId, Int32 userId, Boolean isManager=false);
        IEnumerable<OrderDetails> GetHonestRecivedOrderDetails(Int64 orderId);
        IEnumerable<OrderModel> GetHonestRecivedOrders(int userId, bool isManager);
        IEnumerable<SubOrder> GetSubOrders(Int64 orderDetailsId);
        IEnumerable<SubOrderModel> GetBuySubOrders();
        IEnumerable<SubOrderModel> GetSupplierIndentWithouReturnRequest(int supplierId);
        IEnumerable<SubOrderModel> GetSubOrdersByIdentity(String identity, SubOrderState subState,
            SubOrderType subType,bool allIndent,DateTime fromDate,DateTime toDate);

        IEnumerable<SubOrderModel> GetRecivedIndentToSupplier(String identity);
        IEnumerable<OrderModel> GetPersonOrdersInfo(OrderType orderType, DateTime fromDate, DateTime ToDate, Int32 personId, OrderStatus status);
        IEnumerable<OrderDetailsModel> GetMyOrdersForManage(bool fromManager,OrderType[] ordertypes,int personId);
        IEnumerable<OrderDetailsModel> GetManagedOrdersByMy(OrderType[] orderTypes, Int32 userId);
        Int32 GetRecivedOrdersCount(int UserId,params OrderDetailsState[] odState);
        Int32 GetRecivedOrdersForStuffHonest(int userId);
        Int32 GetRecivedStoreIndentsCount(string[] storeId);
        Int32 GetStoreOrdersForManager();
        Int32 GetRecivedMunituinIndentsCount(string identity);
        OrderDetails GetOrderDetails(Int64 orderDetailsId);
        IEnumerable<MovableAsset> GetOrderdAssetsForProceeding(OrderType OType, bool checkCompietion,ProceedingsType procType);
        IEnumerable<SupplierIndent> GetSupplierIndents(long subOrderId);
        IEnumerable<SubOrderModel> GetSupplierIndentsForSupplier(int supplierId, SupplierIndentState state, bool allIndent = true);
        IEnumerable<SubOrderModel> GetSupplierCurrentIndent(int supplierId);
        Person GetRelatedOrderPerson(Int64 orderId);
        IEnumerable<SubOrderModel> GetRelatedSubOrders(string identity, Int64 orderDetailsId);
        IEnumerable<OrderSumModel> GetOrderDetailsByKalaUid(int kalaUid,StuffType stype, DateTime fromDate, DateTime ToDate,bool filterByStuff);
        SubOrder GetSubOrderBySupplierIndent(long spIndentId);
        IEnumerable<SupplierIndentModel> GetSupplierIndentModel(int supplierid,HashSet<SupplierIndentState> spStates);
        Tuple<IEnumerable<OrderDetails>, OrderDetails> GetRelatedOrderDetailsByIndent(Int64 spIndentId);
        UnConsumption GetParentBelongingAsstBySupllierIndent(long spIndent);
        SupplierIndent GetSupplierIndent(Int64 spId);
        IEnumerable<SubOrderModel> GetSupplierIndentByRequest(Int32 requestId);
        IEnumerable<SubOrderModel> GetTrenderSubOrders(bool isManager, String identity, HashSet<SubOrderState> subState, DateTime fromDate, DateTime toDate);

        IEnumerable<SubOrderModel> GetTrenderSubOrdersForSupplier(bool isManager, int supplierId, DateTime fromDate, DateTime toDate);
        SubOrder GetSubOrder(long subOrderId);
        Boolean IsTrenderOffersConfirmed(long subOrderId);
        SupplierTrenderOffer GetSubOrderTrenderOffers(int trenderOfferId);
        IEnumerable<SupplierTrenderOffer> GetTrenderOffersBySubOrder(long subOrderId);

    }

    public sealed class OrderService : Service<Order>,IOrderService
    {
        private readonly IRepositoryAsync<Order> _repository;

        public OrderService(IRepositoryAsync<Order> repository)
            :base(repository)
        {
            this._repository = repository;
        }

        public IEnumerable<OrderModel> GetOrders(int personId, DateTime minDate)
        {
            return _repository.GetOrders(personId, minDate);
        }

        public IEnumerable<OrderModel> GetRecivedOrders(bool IsManager, int userId, Boolean forStore, OrderStatus oStatus)
        {
            return _repository.GetRecivedOrders(IsManager, userId,forStore,oStatus);
        }

        public IEnumerable<OrderUserHistory> GetUserHistories(long orderDetailsId)
        {
            return _repository.GetUserHistories(orderDetailsId);
        }

        public IEnumerable<OrderDetails> GetRecivedOrderDetails(long orderId, int userId, Boolean isManager=false)
        {
            return _repository.GetRecivedOrderDetails(orderId, userId,isManager);
        }

        public IEnumerable<OrderDetails> GetHonestRecivedOrderDetails(long orderId)
        {
            return _repository.GetHonestRecivedOrderDetails(orderId);
        }

        public IEnumerable<OrderModel> GetHonestRecivedOrders(int userId,bool isManager)
        {
            return _repository.GetHonestRecivedOrders(userId,isManager);
        }

        public IEnumerable<SubOrder> GetSubOrders(long orderDetailsId)
        {
            return _repository.GetSubOrders(orderDetailsId);
        }

        public IEnumerable<SubOrderModel> GetBuySubOrders()
        {
            return _repository.GetBuySubOrders();
        }

        public IEnumerable<SubOrderModel> GetSubOrdersByIdentity(string identity, SubOrderState subState, SubOrderType subType,
            bool allIndent, DateTime fromDate, DateTime toDate)
        {
            return _repository.GetSubOrdersByIdentity(identity, subState, 
                subType,allIndent,fromDate,toDate);
        }

        public IEnumerable<SubOrderModel> GetRecivedIndentToSupplier(string identity)
        {
            return _repository.GetRecivedIndentToSupplier(identity);
        }

        public IEnumerable<OrderModel> GetPersonOrdersInfo(OrderType orderType, DateTime fromDate, DateTime ToDate, int personId, OrderStatus status)
        {
            return _repository.GetPersonOrdersInfo(orderType, fromDate, ToDate, personId, status);
        }

        public int GetRecivedOrdersCount(int UserId, params OrderDetailsState[] odState)
        {
            return _repository.GetRecivedOrdersCount(UserId,odState);
        }

        public IEnumerable<OrderDetailsModel> GetMyOrdersForManage(bool fromManager, OrderType[] ordertypes, int personId)
        {
            return _repository.GetMyOrdersForManage(fromManager, ordertypes, personId);
        }

        public IEnumerable<OrderDetailsModel> GetManagedOrdersByMy(OrderType[] orderTypes, int userId)
        {
            return _repository.GetManagedOrdersByMy(orderTypes, userId);
        }

        public OrderDetails GetOrderDetails(long orderDetailsId)
        {
            return _repository.GetOrderDetails(orderDetailsId);
        }

        public IEnumerable<MovableAsset> GetOrderdAssetsForProceeding(OrderType OType, bool checkCompietion,ProceedingsType procType)
        {
            return _repository.GetOrderdAssetsForProceeding(OType, checkCompietion,procType);
        }

        public int GetRecivedStoreIndentsCount(string[] storeId)
        {
            return _repository.GetRecivedStoreIndentsCount(storeId);
        }

        public int GetRecivedMunituinIndentsCount(string identity)
        {
            return _repository.GetRecivedMunituinIndentsCount(identity);
        }

        public int GetStoreOrdersForManager()
        {
            return _repository.GetStoreOrdersForManager();
        }

        public int GetRecivedOrdersForStuffHonest(int userId)
        {
            return _repository.GetRecivedOrdersForStuffHonest(userId);
        }
        
        public IEnumerable<SupplierIndent> GetSupplierIndents(long subOrderId)
        {
            return _repository.GetSupplierIndents(subOrderId);
        }

        public IEnumerable<SubOrderModel> GetSupplierIndentsForSupplier(int supplierId, SupplierIndentState state, bool allIndent = true)
        {
            return _repository.GetSupplierIndentsForSupplier(supplierId, state, allIndent);
        }

        public Person GetRelatedOrderPerson(long orderId)
        {
            return _repository.GetRelatedOrderPerson(orderId);
        }

        public IEnumerable<SubOrderModel> GetSupplierCurrentIndent(int supplierId)
        {
            return _repository.GetSupplierCurrentIndent(supplierId);
        }

        public IEnumerable<SubOrderModel> GetRelatedSubOrders(string identity, long orderDetailsId)
        {
            return _repository.GetRelatedSubOrders(identity, orderDetailsId);
        }

        public IEnumerable<OrderSumModel> GetOrderDetailsByKalaUid(int kalaUid, StuffType stype, DateTime fromDate, DateTime ToDate, bool filterByStuff)
        {
            return _repository.GetOrderDetailsByKalaUid(stype,kalaUid, fromDate, ToDate,filterByStuff);
        }
        
        public IEnumerable<SupplierIndentModel> GetSupplierIndentModel(int supplierid, HashSet<SupplierIndentState> spStates)
        {
            return _repository.GetSupplierIndentModel(supplierid, spStates);
        }

        public Tuple<IEnumerable<OrderDetails>, OrderDetails> GetRelatedOrderDetailsByIndent(long spIndentId)
        {
            return _repository.GetRelatedOrderDetailsByIndent(spIndentId);
        }

        public UnConsumption GetParentBelongingAsstBySupllierIndent(long spIndent)
        {
            return _repository.GetParentBelongingAsstBySupllierIndent(spIndent);
        }

        public SubOrder GetSubOrderBySupplierIndent(long spIndentId)
        {
            return _repository.GetSubOrderBySupplierIndent(spIndentId);
        }

        public SupplierIndent GetSupplierIndent(long spId)
        {
            return _repository.GetSupplierIndent(spId);
        }

        public IEnumerable<SubOrderModel> GetSupplierIndentWithouReturnRequest(int supplierId)
        {
            return _repository.GetSupplierIndentWithouReturnRequest(supplierId);
        }

        public IEnumerable<SubOrderModel> GetSupplierIndentByRequest(int requestId)
        {
            return _repository.GetSupplierIndentByRequest(requestId);
        }

        public IEnumerable<OrderModel> GetRecivedOrganizManageOrders(int userId)
        {
            return _repository.GetRecivedOrganizManageOrders(userId);
        }

        public IEnumerable<SubOrderModel> GetTrenderSubOrders(bool isManager,string identity, HashSet<SubOrderState> subState, DateTime fromDate, DateTime toDate)
        {
            return _repository.GetTrenderSubOrders(isManager, identity, subState, fromDate, toDate);
        }

        public IEnumerable<SubOrderModel> GetTrenderSubOrdersForSupplier(bool isManager, int supplierId, DateTime fromDate, DateTime toDate)
        {
            return _repository.GetTrenderSubOrdersForSupplier(isManager, supplierId, fromDate, toDate);
        }

        public SubOrder GetSubOrder(long subOrderId)
        {
            return _repository.GetSubOrder(subOrderId);
        }

        public bool IsTrenderOffersConfirmed(long subOrderId)
        {
            return _repository.IsTrenderOffersConfirmed(subOrderId);
        }

        public SupplierTrenderOffer GetSubOrderTrenderOffers(int trenderOfferId)
        {
            return _repository.GetSubOrderTrenderOffers(trenderOfferId);
        }

        public IEnumerable<SupplierTrenderOffer> GetTrenderOffersBySubOrder(long subOrderId)
        {
            return _repository.GetTrenderOffersBySubOrder(subOrderId);
        }
    }
}
