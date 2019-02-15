
namespace Bska.Client.Data.Service
{
    using System;
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Service.Pattern;
    using Repository;
    using System.Collections.Generic;
    using Repository.Model;
    using Bska.Client.Common;

    public interface IStoreBillService : IService<StoreBill>
    {
        int getRelatedAccountMasterId(int billId);
        IEnumerable<StoreBillIssueModel> GetIssueBillByStuff(int kalaUid, StuffType sType, DateTime fromDate, DateTime toDate,int storeId, bool filterbyStype, bool forCommodity);
        AnalizModel GetStoreBillAnalized(int kalaUid,StuffType sType, DateTime fromDate,DateTime toDate,bool filterbyStype, bool forCommodity=false);
        IEnumerable<StoreBillModel> GetStoreBillsForInternalDraft(int storeId, HashSet<MAssetReserveStatus> rState);
        IEnumerable<StoreBillEditModel> GetRecivedEditsByState(GlobalRequestStatus state);
        int CountRecivedEditsByState(GlobalRequestStatus state);
        int CountAssetReserveHistories(MAssetReserveStatus status);
    }
    public class StoreBillService : Service<StoreBill>,IStoreBillService
    {
         private readonly IRepositoryAsync<StoreBill> _repository;

         public StoreBillService(IRepositoryAsync<StoreBill> repository)
            :base(repository)
        {
            this._repository = repository;
        }

        public int CountAssetReserveHistories(MAssetReserveStatus status)
        {
            return _repository.CountAssetReserveHistories(status);
        }

        public int CountRecivedEditsByState(GlobalRequestStatus state)
        {
            return _repository.CountRecivedEditsByState(state);
        }

        public IEnumerable<StoreBillIssueModel> GetIssueBillByStuff(int kalaUid, StuffType sType, DateTime fromDate, DateTime toDate,int storeId, bool filterbyStype, bool forCommodity)
        {
            return _repository.GetIssueBillByStuff(kalaUid,sType, fromDate, toDate,storeId,filterbyStype,forCommodity);
        }

        public IEnumerable<StoreBillEditModel> GetRecivedEditsByState(GlobalRequestStatus state)
        {
            return _repository.GetRecivedEditsByState(state);
        }

        public int getRelatedAccountMasterId(int billId)
        {
            return _repository.getRelatedAccountMasterId(billId);
        }

        public AnalizModel GetStoreBillAnalized(int kalaUid, StuffType sType, DateTime fromDate, DateTime toDate,bool filterbyStype, bool forCommodity = false)
        {
            return _repository.GetStoreBillAnalized(kalaUid,sType, fromDate, toDate,forCommodity,filterbyStype);
        }

        public IEnumerable<StoreBillModel> GetStoreBillsForInternalDraft( int storeId, HashSet<MAssetReserveStatus> rState)
        {
            return _repository.GetStoreBillsForInternalDraft(storeId,rState);
        }
    }
}
