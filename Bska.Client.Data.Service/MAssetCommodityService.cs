
namespace Bska.Client.Data.Service
{
    using API.Repositories;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Client.Service.Pattern;
    using Client.Repository;
    using Repository.Model;
    using System.Collections.Generic;
    using System;
    using Common;
    using Domain.Entity.OrderEntity;
    using Bska.Client.Domain.Entity.AssetEntity.CommodityAsset;

    public interface IMAssetCommodityService: IService<Commodity>
    {
        IEnumerable<Commodity> GetCommodities(DateTime? fromDate, DateTime? toDate);
        AnalizModel GetInternalDocAnaliz(int kalaUid, DateTime fromDate, DateTime toDate);
        IEnumerable<StoreBillIssueModel> GetStoreDocumentsIssue(int kalaUid, DateTime fromDate,
           DateTime toDate, int storeId, DocumentType docType);
        IEnumerable<Order> GetOrders(Int64 commodityId);
        IEnumerable<Document> GetDocuments(Int64 commodityId);
        IEnumerable<MovableAssetModel> GetCommodityByDocId(Int64 documentId);
        IEnumerable<MovableAssetModel> GetCommodityByBillId(Int32 billId);
        IEnumerable<MovableAssetModel> GetCommodityByBillIdToDocument(Int32 billId);
        IEnumerable<Commodity> GetCommodityToDeliviryOrders(int kalaUid, int storeId,DateTime expirationDate);
        IEnumerable<MovableAssetModel> GetCommodityForOutOfStore(DateTime? fromDate, DateTime? toDate);
        IEnumerable<PlaceOfUse> GetPlaceOfUse(Int64 coId);
        IEnumerable<CommodityAssetReserveHistory> GetReserveHistories(Int64 coId);
        IEnumerable<Document> GetAllDocumentsToCommodity();
    }

    public class MAssetCommodityService : Service<Commodity>, IMAssetCommodityService
    {
        private readonly IRepositoryAsync<Commodity> _repository;

        public MAssetCommodityService(IRepositoryAsync<Commodity> repository)
            :base(repository)
        {
            this._repository = repository;
        }

        public IEnumerable<Document> GetAllDocumentsToCommodity()
        {
            return _repository.GetAllDocumentsToCommodity();
        }

        public IEnumerable<Commodity> GetCommodities(DateTime? fromDate, DateTime? toDate)
        {
            return _repository.GetCommodities(fromDate, toDate);
        }

        public IEnumerable<MovableAssetModel> GetCommodityByBillId(int billId)
        {
            return _repository.GetCommodityByBillId(billId);
        }

        public IEnumerable<MovableAssetModel> GetCommodityByBillIdToDocument(int billId)
        {
            return _repository.GetCommodityByBillIdToDocument(billId);
        }

        public IEnumerable<MovableAssetModel> GetCommodityByDocId(long documentId)
        {
            return _repository.GetCommodityByDocId(documentId);
        }

        public IEnumerable<MovableAssetModel> GetCommodityForOutOfStore(DateTime? fromDate, DateTime? toDate)
        {
            return _repository.GetCommodityForOutOfStore(fromDate, toDate);
        }

        public IEnumerable<Commodity> GetCommodityToDeliviryOrders(int kalaUid, int storeId, DateTime expirationDate)
        {
            return _repository.GetCommodityToDeliviryOrders(kalaUid, storeId,expirationDate);
        }

        public IEnumerable<Document> GetDocuments(long commodityId)
        {
            return _repository.GetDocuments(commodityId);
        }

        public AnalizModel GetInternalDocAnaliz(int kalaUid, DateTime fromDate, DateTime toDate)
        {
            return _repository.GetInternalDocAnaliz(kalaUid, fromDate, toDate);
        }
        

        public IEnumerable<Order> GetOrders(Int64 commodityId)
        {
            return _repository.GetOrders(commodityId);
        }

        public IEnumerable<PlaceOfUse> GetPlaceOfUse(long coId)
        {
            return _repository.GetPlaceOfUse(coId);
        }

        public IEnumerable<CommodityAssetReserveHistory> GetReserveHistories(long coId)
        {
            return _repository.GetReserveHistories(coId);
        }

        public IEnumerable<StoreBillIssueModel> GetStoreDocumentsIssue(int kalaUid, DateTime fromDate, DateTime toDate, int storeId, DocumentType docType)
        {
            return _repository.GetStoreDocumentsIssue(kalaUid, fromDate, toDate, storeId, docType);
        }

    }
}
