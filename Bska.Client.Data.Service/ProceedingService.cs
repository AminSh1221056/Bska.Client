
namespace Bska.Client.Data.Service
{
    using Bska.Client.Common;
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Service.Pattern;
    using Bska.Client.Repository;
    using System.Collections.Generic;
    using System;

    public interface IProceedingService : IService<Proceeding>
    {
        IEnumerable<AssetProceeding> getAssetProceedingsByAssetId(long assetId);
        AssetProceeding getAssetProceeding(long assetId,int procId);
        Int32 GetRecivedProceedingsCount(ProceedingState procState);
        IEnumerable<Proceeding> GetProceedingByExportDetails(Int32 exDetailsId);
        IEnumerable<MovableAsset> GetAssetsByExportDetails(Int32 exDetailsId);
    }

    public class ProceedingService : Service<Proceeding>,IProceedingService
    {
        private readonly IRepositoryAsync<Proceeding> _repository;
        public ProceedingService(IRepositoryAsync<Proceeding> repository)
            :base(repository)
        {
            this._repository = repository;
        }

        public AssetProceeding getAssetProceeding(long assetId,int procId)
        {
            return _repository.getAssetProceeding(assetId, procId);
        }

        public IEnumerable<AssetProceeding> getAssetProceedingsByAssetId(long assetId)
        {
            return _repository.getAssetProceedingsByAssetId(assetId);
        }

        public IEnumerable<MovableAsset> GetAssetsByExportDetails(int exDetailsId)
        {
            return _repository.GetAssetsByExportDetails(exDetailsId);
        }

        public IEnumerable<Proceeding> GetProceedingByExportDetails(int exDetailsId)
        {
            return _repository.GetProceedingByExportDetails(exDetailsId);
        }

        public int GetRecivedProceedingsCount(ProceedingState procState)
        {
            return _repository.GetRecivedProceedingsCount(procState);
        }
    }
}
