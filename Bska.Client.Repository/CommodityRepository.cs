
namespace Bska.Client.Repository
{
    using API.Repositories;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Common;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Domain.Entity.OrderEntity;
    using Domain.Entity.AssetEntity.CommodityAsset;

    public static class CommodityRepository
    {
        public static IEnumerable<Commodity> GetCommodities(this IRepository<Commodity> repository, DateTime? fromDate, DateTime? toDate)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
            if(fromDate.HasValue && toDate.HasValue)
            {
                return commodity.Where(co => co.InsertDate > fromDate && co.InsertDate <= toDate).Include(co => co.PlaceOfUses).AsEnumerable();
            }
            return commodity.Include(co => co.PlaceOfUses).AsEnumerable();
        }

        public static AnalizModel GetInternalDocAnaliz(this IRepository<Commodity> repository, int kalaUid, DateTime fromDate, DateTime toDate)
        {
            var docs = repository.GetRepository<Document>().Queryable();
            var issue = docs.Where(x => x.Commodities.Any(ma => ma.Commodity.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft &&
                (x.DocumentDate > fromDate && x.DocumentDate <= toDate)).AsEnumerable();
            return new AnalizModel
            {
                Description = "تعداد حواله انبار کشیده شده",
                Num = issue.Count(),
                UnitName = "عدد",
                Identity = AnalizModelIdentity.InternalDraft
            };
        }

        public static IEnumerable<StoreBillIssueModel> GetStoreDocumentsIssue(this IRepositoryAsync<Commodity> repository, int kalaUid, DateTime fromDate,
          DateTime toDate, int storeId, DocumentType docType)
        {
            var docs = repository.GetRepository<Document>().Queryable();

            if (storeId == 0)
            {
                var items = docs.Where(x => x.Commodities.Any(ma => ma.Commodity.KalaUid == kalaUid) && x.DocumentType == docType &&
                (x.DocumentDate > fromDate && x.DocumentDate <= toDate))
                    .Select(b => new StoreBillIssueModel
                    {
                        Date = b.DocumentDate,
                        Num = b.Commodities.Count(x => x.Commodity.KalaUid == kalaUid),
                        SBNo = b.Desc1,
                        Price = b.Commodities.Where(x => x.Commodity.KalaUid == kalaUid).Sum(x => x.Commodity.Cost),
                        KalaUid = kalaUid,
                        Transfree = b.Transferee
                    });
                return items.AsEnumerable();
            }
            var items1 = docs.Where(x => x.Commodities.Any(ma => ma.Commodity.KalaUid == kalaUid) && x.DocumentType == docType &&
                (x.DocumentDate > fromDate && x.DocumentDate <= toDate) && x.StoreId == storeId)
                    .Select(b => new StoreBillIssueModel
                    {
                        Date = b.DocumentDate,
                        Num = b.Commodities.Count(x => x.Commodity.KalaUid == kalaUid),
                        SBNo = b.Desc1,
                        Price = b.Commodities.Where(x => x.Commodity.KalaUid == kalaUid).Sum(x => x.Commodity.Cost),
                        KalaUid = kalaUid,
                        Transfree = b.Transferee
                    });
            return items1.AsEnumerable();
        }

        public static IEnumerable<Order> GetOrders(this IRepositoryAsync<Commodity> repository, Int64 commodityId)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
            return commodity.Where(co => co.AssetId == commodityId).SelectMany(co => co.Orders).AsEnumerable();
        }

        public static IEnumerable<PlaceOfUse> GetPlaceOfUse(this IRepositoryAsync<Commodity> repository, long coId)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
            return commodity.Where(co => co.AssetId == coId).SelectMany(co => co.PlaceOfUses).AsEnumerable();
        }
        public static IEnumerable<MovableAssetModel> GetCommodityByBillId(this IRepositoryAsync<Commodity> repository, int billId)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
           return  commodity.Where(co => co.StoreBillId == billId).Select(co => new MovableAssetModel
            {
                AcqType = StateOwnership.Purchase,
                AssetId = co.AssetId,
                InsertDate = co.InsertDate,
                CurState = MAssetCurState.AtOperation,
                IsCompietion = CompietionState.NotReported,
                IsConfirmed = false,
                IsInStore = true,
                kalaUid = co.KalaUid,
                KalaNo=co.KalaNo,
                Label = null,
                MAssetType = "مصرفی",
                Name = co.Name,
                Num = co.Num,
                UnitId = co.UnitId
            }).AsEnumerable();
        }

        public static IEnumerable<MovableAssetModel> GetCommodityByBillIdToDocument(this IRepositoryAsync<Commodity> repository, int billId)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
            return commodity.Where(co => co.StoreBillId == billId).Select(co => new MovableAssetModel
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
            }).AsEnumerable();
        }

        public static IEnumerable<MovableAssetModel> GetCommodityByDocId(this IRepositoryAsync<Commodity> repository, long documentId)
        {
            var placeOfUse = repository.GetRepository<PlaceOfUse>().Queryable();
            var items = from co in placeOfUse
                        where co.DocumentId == documentId
                        select new MovableAssetModel
                        {
                            AcqType = StateOwnership.Purchase,
                            AssetId = co.Commodity.AssetId,
                            InsertDate = co.Document.DocumentDate,
                            CurState = MAssetCurState.AtOperation,
                            IsCompietion = CompietionState.NotReported,
                            IsConfirmed = false,
                            IsInStore = false,
                            kalaUid = co.Commodity.KalaUid,
                            Label = null,
                            MAssetType = "مصرفی",
                            Name = co.Commodity.Name,
                            Num = co.Num,
                            UnitId = co.UnitId
                        };
            return items.AsEnumerable();
        }

        public static IEnumerable<Document> GetDocuments(this IRepositoryAsync<Commodity> repository, long commodityId)
        {
            var placeOfUse = repository.GetRepository<PlaceOfUse>().Queryable();
            return (from pu in placeOfUse
                    where pu.CommodityId == commodityId
                    select pu.Document).Distinct().AsEnumerable();
        }
        
        public static IEnumerable<Commodity> GetCommodityToDeliviryOrders(this IRepositoryAsync<Commodity> repository, int kalaUid, int storeId,DateTime expirationDate)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
            return commodity.Where(co =>co.KalaUid == kalaUid && co.StoreBill.StoreId == storeId &&
            co.CommodityAssetReserveHistories.All(s=>s.Status==MAssetReserveStatus.UnReserved) &&
            (co.ExpirationDate>=expirationDate || co.ExpirationDate==null))
                        .Include(co => co.PlaceOfUses).AsEnumerable();
        }

        public static IEnumerable<MovableAssetModel> GetCommodityForOutOfStore(this IRepositoryAsync<Commodity> repository, DateTime? fromDate,
          DateTime? toDate)
        {
            var placeOfUse = repository.GetRepository<PlaceOfUse>().Queryable();
            if(fromDate.HasValue && toDate.HasValue)
            {
               return placeOfUse.Where(pl => pl.InsertDate > fromDate && pl.InsertDate <= toDate)
                    .Select(pl => new MovableAssetModel
                    {
                        AssetId = pl.CommodityId.Value,
                        Cost = pl.Commodity.Cost,
                        InsertDate = pl.InsertDate,
                        IsInStore = false,
                        KalaNo = pl.Commodity.KalaNo,
                        kalaUid = pl.Commodity.KalaUid,
                        MAssetType = "مصرفی",
                        Name = pl.Commodity.Name,
                        Num = pl.Num,
                        UnitId = pl.UnitId,
                        OrganizId = pl.OrganizId,
                        StrategyId = pl.StrategtyId,
                        PersonId = pl.PersonId,
                        AcqType = pl.Commodity.StoreBill.AcqType
                    });
            }
            else
            {
                return placeOfUse.Select(pl => new MovableAssetModel
                     {
                         AssetId = pl.CommodityId.Value,
                         Cost = pl.Commodity.Cost,
                         InsertDate = pl.InsertDate,
                         IsInStore = false,
                         KalaNo = pl.Commodity.KalaNo,
                         kalaUid = pl.Commodity.KalaUid,
                         MAssetType = "مصرفی",
                         Name = pl.Commodity.Name,
                         Num = pl.Num,
                         UnitId = pl.UnitId,
                         OrganizId = pl.OrganizId,
                         StrategyId = pl.StrategtyId,
                         PersonId = pl.PersonId,
                         AcqType = pl.Commodity.StoreBill.AcqType
                     });
            }
        }

        public static IEnumerable<CommodityAssetReserveHistory> GetReserveHistories(this IRepositoryAsync<Commodity> repository, Int64 coId)
        {
            var commodity = repository.GetRepository<Commodity>().Queryable();
            return commodity.Where(co => co.AssetId == coId).SelectMany(co => co.CommodityAssetReserveHistories).AsEnumerable();
        }

        public static IEnumerable<Document> GetAllDocumentsToCommodity(this IRepositoryAsync<Commodity> repository)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            return (from d in doc
                    where d.MovableAsset.Any()
                    select d).Include(d => d.Commodities).Include(d => d.Store).AsEnumerable();
        }
    }
}
