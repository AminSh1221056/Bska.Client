
namespace Bska.Client.Data.Service
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Repository;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Service.Pattern;
    using System;
    using System.Collections.Generic;
    using Bska.Client.Repository.Model;
    public interface IMovableAssetService : IService<MovableAsset>
    {
        Document GetDocument(string docNo);
        Document GetDocumentToRelatedAccount(int docId);
        StoreBill GetStoreBill(Int64 assetId);
        IEnumerable<MovableAssetModel> GetOuterMovableAssetByLocation(DateTime? fromDate, DateTime? toDate);
        IEnumerable<MovableAssetModel> GetStoreMovableAssetByLocation(DateTime? fromDate, DateTime? toDate,int storeId);
        IEnumerable<MovableAssetModel> GetStoreMovableAssetInRetiringStores(DateTime? fromDate, DateTime? toDate, int storeId);
        IEnumerable<MovableAssetModel> GetMovableAssetByStoreBill(int billId);
        IEnumerable<MovableAssetModel> GetMAssetsByStoreBillToDocument(int billId);
        IEnumerable<MovableAssetModel> GetMovableAssetByDocument(Int64 docId);
        IEnumerable<Document> GetDocuments(bool includeAssets = true, bool toRetiringStore = false);
        Document GetInitialBalanceDocumentByLocation(Location location);
        IEnumerable<Document> GetAllDocuments();
        IEnumerable<Document> GetDocumentsByType(DocumentType docType,bool isRetiringStore=false);
        IEnumerable<MovableAsset> GetDisplacemetnAssets(string personId,int organizId);
        Location GetLocation(Int64 assetId, Boolean isInStore);
        Location GetLastOneBeforeLocation(long assetId);
        Location GetLastLocation(Int64 assetId);
        Location GetMovedRequestLocation(Int64 assetId);
        IEnumerable<UnConsumption> GetUnConsuptionAssetToLabelInStore(bool checkCompietion);
        IEnumerable<UnConsumption> GetUnConsuptionAssetToLabelInRetiringStore(bool ForProceeding,bool isCompietion=true);
        IEnumerable<UnConsumption> GetUnConsuptionAssetToAccident(bool ForProceeding);
        IEnumerable<UnConsumption> GetTrustAssetToReturntFromTrust();
        IEnumerable<MovableAssetModel> GetPersonAssets(string personId,Boolean isActive=false);
        IEnumerable<AccountDocumentModel> GetLocationsForAccountDocuments(DateTime fromDate,DateTime toDate);
        IEnumerable<AccountDocumentModel> GetAccountDocumentsForAssetByLabel(int Label);
        IEnumerable<AccountDocumentModel> GetAccountDocumentsForAssetById(Int64 assetId);
        IEnumerable<AccountDocumentModel> GetAccountDocumentForDisaster();
        IEnumerable<AccountDocumentModel> GetAccountDocumentForTrust(Int64[] assetIds);
        IEnumerable<AccountDocumentModel> GetAccountDocumentForRefundTrust(Int64[] assetIds);
        IEnumerable<AccountDocumentModel> GetAccountDocumentForEscrow(Int64[] assetIds);
        IEnumerable<AccountDocumentModel> GetAccountDocumentsForSpecificLoc(int loc, Boolean inStore);
        IEnumerable<MovableAsset> getMovableAssetToRefundTrust(bool inStore = false);
        IEnumerable<AccountDocumentModel> getAccountDocumentsByAcqType(StateOwnership acqTyp);
        IEnumerable<AccountDocumentModel> getAccountDocumentForLabelContiunity(Int32 fromLabel, Int32 ToLabel);
        IEnumerable<AccountDocumentModel> GetAccountDocumentByAssetName(string assetName);
        IEnumerable<AccountDocumentModel> GetAccountDocumentByCurState(MAssetCurState locstatus);
        Boolean ContainBelongings(Int64 assetId);
        UnConsumption GetBelongingParnet(Int64 BelongingId);
        IEnumerable<Belonging> GetBelongingsToLocation(Int64 assetid);
        IEnumerable<MovableAssetModel> GetStoreMovableAssetWithoutProcceding(int storeId=0,bool checkCompietion=false);
        IEnumerable<MovableAssetModel> GetStoreMovableAssetWithoutProccedingByKalaUid(int storeId, int kalaUid);
        IEnumerable<AssetProceeding> GetAssetProceedingsToMAssets(int procId);
        IEnumerable<MovableAsset> GetStoreMovableAssetForProcceding(bool checkCompietion);
        IEnumerable<AssetTaxCost> GetTaxCosts(Int64 assetId);
        IEnumerable<MovableAsset> GetMAssetsByExportDetails(Int32 exdId,bool toRelated);
        AccountDocumentMaster GetLastAccountDocuemntForAsset(Int64 assetId);
        int GetRelatedAccountDocumentByDoc(Int64 docId);
        AccountDocumentMaster GetRelatedRetiringBillAccount(Int64 docId);
        IEnumerable<StoreBillIssueModel> GetStoreDocumentsIssue(int kalaUid, StuffType stype, DateTime fromDate,
           DateTime toDate, int storeId, DocumentType docType, bool filterbyStype);
        AnalizModel GetCurrentStoreByKalaUid(int kalaUid,StuffType sType,bool filterbyStype);
        AnalizModel GetStoreInternalDocAnaliz(int kalaUid,StuffType stype, DateTime fromDate,
           DateTime toDate, bool filterbyStype);

        IEnumerable<MovableAssetReserveHistory> GetReserveHistories(Int64 assetId);
        IEnumerable<Document> GetAllDocumentsToMovableAssets();
    }

    public class MovableAssetService : Service<MovableAsset>,IMovableAssetService
    {
        private readonly IRepositoryAsync<MovableAsset> _repository;

        public MovableAssetService(IRepositoryAsync<MovableAsset> repository):
            base(repository)
        {
            this._repository = repository;
        }

        public Document GetDocument(string docNo)
        {
            return _repository.GetDocument(docNo);
        }

        public IEnumerable<MovableAssetModel> GetOuterMovableAssetByLocation(DateTime? fromDate, DateTime? toDate)
        {
            return _repository.GetOuterMovableAssetByLocation(fromDate, toDate);
        }

        public IEnumerable<MovableAssetModel> GetStoreMovableAssetByLocation(DateTime? fromDate, DateTime? toDate,int storeId)
        {
            return _repository.GetStoreMovableAssetByLocation(fromDate, toDate, storeId,LocationStatus.StoreActive);
        }

        public IEnumerable<MovableAssetModel> GetStoreMovableAssetInRetiringStores(DateTime? fromDate, DateTime? toDate, int storeId)
        {
            return _repository.GetStoreMovableAssetByLocation(fromDate, toDate, storeId, LocationStatus.Retiring);
        }

        public IEnumerable<MovableAssetModel> GetMovableAssetByStoreBill(int billId)
        {
            return _repository.GetMovableAssetByStoreBill(billId);
        }

        public IEnumerable<MovableAssetModel> GetMovableAssetByDocument(long docId)
        {
            return _repository.GetMovableAssetByDocument(docId);
        }

        public IEnumerable<Document> GetDocuments(bool includeAssets = true, bool toRetiringStore = false)
        {
            return _repository.GetDocuments(includeAssets, toRetiringStore);
        }

        public IEnumerable<Document> GetAllDocuments()
        {
            return _repository.GetAllDocuments();
        }

        public IEnumerable<MovableAsset> GetDisplacemetnAssets(string personId,int organizId)
        {
            return _repository.GetDisplacemetnAssets(personId,organizId);
        }

        public Location GetLocation(long assetId, bool isInStore)
        {
            return _repository.GetLocation(assetId, isInStore);
        }

        public Location GetMovedRequestLocation(long assetId)
        {
            return _repository.GetMovedRequestLocation(assetId);
        }

        public IEnumerable<UnConsumption> GetUnConsuptionAssetToLabelInStore(bool checkCompietion)
        {
            return _repository.GetUnConsuptionAssetToLabelInStore(checkCompietion);
        }

        public IEnumerable<UnConsumption> GetUnConsuptionAssetToLabelInRetiringStore(bool ForProceeding,bool isCompietion= true)
        {
            return _repository.GetUnConsuptionAssetToLabelInRetiringStore(ForProceeding,isCompietion);
        }

        public IEnumerable<UnConsumption> GetUnConsuptionAssetToAccident(bool ForProceeding)
        {
            return _repository.GetUnConsuptionAssetToAccident(ForProceeding);
        }

        public IEnumerable<MovableAssetModel> GetPersonAssets(string personId,Boolean isActive=false)
        {
            return _repository.GetPersonAssets(personId,isActive);
        }
        
        public IEnumerable<AccountDocumentModel> GetLocationsForAccountDocuments(DateTime fromDate, DateTime toDate)
        {
            return _repository.GetLocationsForAccountDocuments(fromDate,toDate);
        }

        public Location GetLastLocation(long assetId)
        {
            return _repository.GetLastLocation(assetId);
        }

        public StoreBill GetStoreBill(long assetId)
        {
            return _repository.GetBillStores(assetId);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentsForAssetByLabel(int Label)
        {
            return _repository.GetAccountDocumentsForAssetByLabel(Label);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentsForSpecificLoc(int loc, bool inStore)
        {
            return _repository.GetAccountDocumentsForSpecificLoc(loc, inStore);
        }
        
        public IEnumerable<AccountDocumentModel> GetAccountDocumentForDisaster()
        {
            return _repository.GetAccountDocumentForDisaster();
        }

        public IEnumerable<Document> GetDocumentsByType(DocumentType docType,bool isRetiringStore=false)
        {
            return _repository.GetDocumentsByType(docType,isRetiringStore);
        }

        public IEnumerable<UnConsumption> GetTrustAssetToReturntFromTrust()
        {
            return _repository.GetTrustAssetToReturntFromTrust();
        }

        public Location GetLastOneBeforeLocation(long assetId)
        {
            return _repository.GetLastOneBeforeLocation(assetId);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentsForAssetById(long assetId)
        {
            return _repository.GetAccountDocumentsForAssetById(assetId);
        }

        public IEnumerable<MovableAsset> getMovableAssetToRefundTrust(bool inStore = false)
        {
            return _repository.getMovableAssetToRefundTrust(inStore);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentForTrust(Int64[] assetIds)
        {
            return _repository.GetAccountDocumentForTrust(assetIds);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentForRefundTrust(long[] assetIds)
        {
            return _repository.GetAccountDocumentForRefundTrust(assetIds);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentForEscrow(Int64[] assetIds)
        {
            return _repository.GetAccountDocumentForEscrow(assetIds);
        }

        public IEnumerable<AccountDocumentModel> getAccountDocumentsByAcqType(StateOwnership acqTyp)
        {
            return _repository.GetAccountDocumentsByAcqType(acqTyp);
        }

        public IEnumerable<AccountDocumentModel> getAccountDocumentForLabelContiunity(int fromLabel, int ToLabel)
        {
            return _repository.GetAccountDocumentForLabelContiunity(fromLabel, ToLabel);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentByAssetName(string assetName)
        {
            return _repository.GetAccountDocumentByAssetName(assetName);
        }

        public IEnumerable<AccountDocumentModel> GetAccountDocumentByCurState(MAssetCurState locstatus)
        {
            return _repository.GetAccountDocumentByCurState(locstatus);
        }

        public bool ContainBelongings(long assetId)
        {
            return _repository.ContainBelongings(assetId);
        }

        public UnConsumption GetBelongingParnet(long BelongingId)
        {
            return _repository.GetBelongingParnet(BelongingId);
        }

        public IEnumerable<Belonging> GetBelongingsToLocation(long assetid)
        {
            return _repository.GetBelongingsToLocation(assetid);
        }

        public IEnumerable<MovableAssetModel> GetStoreMovableAssetWithoutProcceding(int storeId = 0, bool checkCompietion = false)
        {
            return _repository.GetStoreMovableAssetWithoutProcceding(storeId, checkCompietion);
        }

        public IEnumerable<AssetProceeding> GetAssetProceedingsToMAssets(int procId)
        {
            return _repository.GetAssetProceedingsToMAssets(procId);
        }

        public IEnumerable<MovableAsset> GetStoreMovableAssetForProcceding(bool checkCompietion)
        {
            return _repository.GetStoreMovableAssetForProcceding(checkCompietion);
        }

        public Document GetInitialBalanceDocumentByLocation(Location location)
        {
            return _repository.GetInitialBalanceDocumentByLocation(location);
        }

        public IEnumerable<AssetTaxCost> GetTaxCosts(long assetId)
        {
            return _repository.GetTaxCosts(assetId);
        }

        public IEnumerable<MovableAsset> GetMAssetsByExportDetails(int exdId, bool toRelated)
        {
            return _repository.GetMAssetsByExportDetails(exdId,toRelated);
        }
        
        public AccountDocumentMaster GetLastAccountDocuemntForAsset(long assetId)
        {
            return _repository.GetLastAccountDocuemntForAsset(assetId);
        }

        public Document GetDocumentToRelatedAccount(int docId)
        {
            return _repository.GetDocumentToRelatedAccount(docId);
        }

        public int GetRelatedAccountDocumentByDoc(Int64 docId)
        {
            return _repository.GetRelatedAccountDocumentByDoc(docId);
        }

        public AccountDocumentMaster GetRelatedRetiringBillAccount(long docId)
        {
            return _repository.GetRelatedRetiringBillAccount(docId);
        }

        public AnalizModel GetCurrentStoreByKalaUid(int kalaUid, StuffType sType, bool filterbyStype)
        {
            return _repository.GetCurrentStoreByKalaUid(kalaUid,sType,filterbyStype);
        }

        public AnalizModel GetStoreInternalDocAnaliz(int kalaUid, StuffType stype, DateTime fromDate, DateTime toDate
            , bool filterbyStype)
        {
            return _repository.GetStoreInternalDocAnaliz(kalaUid,stype, fromDate, toDate,filterbyStype);
        }

        public IEnumerable<StoreBillIssueModel> GetStoreDocumentsIssue(int kalaUid, StuffType stype, DateTime fromDate
            , DateTime toDate, int storeId, DocumentType docType, bool filterbyStype)
        {
            return _repository.GetStoreDocumentsIssue(kalaUid, stype, fromDate, toDate, storeId, docType,filterbyStype);
        }

        public IEnumerable<MovableAssetModel> GetStoreMovableAssetWithoutProccedingByKalaUid(int storeId, int kalaUid)
        {
            return _repository.GetStoreMovableAssetWithoutProccedingByKalaUid(storeId, kalaUid);
        }

        public IEnumerable<MovableAssetModel> GetMAssetsByStoreBillToDocument(int billId)
        {
            return _repository.GetMAssetsByStoreBillToDocument(billId);
        }

        public IEnumerable<MovableAssetReserveHistory> GetReserveHistories(long assetId)
        {
            return _repository.GetReserveHistories(assetId);
        }

        public IEnumerable<Document> GetAllDocumentsToMovableAssets()
        {
            return _repository.GetAllDocumentsToMovableAssets();
        }
    }
}
