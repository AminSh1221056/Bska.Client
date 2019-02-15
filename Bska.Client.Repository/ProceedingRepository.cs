
namespace Bska.Client.Repository
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public static class ProceedingRepository
    {
        public static IEnumerable<AssetProceeding> getAssetProceedingsByAssetId(this IRepository<Proceeding> repository, long assetId)
        {
            var assetProc = repository.GetRepository<AssetProceeding>().Queryable();
            return assetProc.Where(x => x.AssetId == assetId).Include(x => x.Proceeding).AsEnumerable();
        }

        public static int GetRecivedProceedingsCount(this IRepository<Proceeding> repository, ProceedingState procState)
        {
            var proc = repository.GetRepository<Proceeding>().Queryable();
            return proc.Count(p => p.State == procState);
        }

        public static IEnumerable<Proceeding> GetProceedingByExportDetails(this IRepository<Proceeding> repository,Int32 exDetailsId)
        {
            var exp = repository.GetRepository<ExportDetailsProceeding>().Queryable();
            return exp.Where(x => x.ExportID == exDetailsId).Select(x => x.Proceeding).Include(x=>x.AssetProceedings).AsEnumerable();
        }

        public static IEnumerable<MovableAsset> GetAssetsByExportDetails(this IRepository<Proceeding> repository, Int32 exDetailsId)
        {
            var exp = repository.GetRepository<ExportDetailsProceeding>().Queryable();
            return exp.Where(x => x.ExportID == exDetailsId).SelectMany
                (x => x.Proceeding.AssetProceedings).Select(x => x.MAsset).AsEnumerable();
        }

        public static AssetProceeding getAssetProceeding(this IRepository<Proceeding> repository, long assetId,int procId)
        {
            var assetProc = repository.GetRepository<AssetProceeding>().Queryable();
            return (from asp in assetProc
                    where asp.AssetId == assetId && asp.ProceedingId == procId
                    select asp).SingleOrDefault();
        }
    }
}
