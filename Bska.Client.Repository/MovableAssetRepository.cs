
namespace Bska.Client.Repository
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Domain.Entity;
    using Domain.Entity.OrderEntity;

    public static class MovableAssetRepository
    {
        public static Document GetDocument(this IRepositoryAsync<MovableAsset> repository, string docNo)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            return (from d in doc
                    where d.Desc1 == docNo && d.Store.StoreType!=StoreType.Retiring
                    select d).FirstOrDefault();
        }

        public static Document GetDocumentToRelatedAccount(this IRepositoryAsync<MovableAsset> repository, int docId)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            return (from d in doc
                    where d.DocumentId==docId
                    select d).Include(x => x.AccountDocument).SingleOrDefault();
        }

        public static int GetRelatedAccountDocumentByDoc(this IRepositoryAsync<MovableAsset> repository, Int64 docId)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            return (from d in doc
                    where d.DocumentId == docId
                    select d.AccountDocument.ID).FirstOrDefault();
        }

        public static AccountDocumentMaster GetRelatedRetiringBillAccount(this IRepositoryAsync<MovableAsset> repository, long docId)
        {
            var account = repository.GetRepository<AccountDocumentMaster>().Queryable();
            return (from c in account
                    where c.Document.DocumentId == docId
                    select c).FirstOrDefault();
        }

        public static IEnumerable<MovableAssetModel> GetOuterMovableAssetByLocation(this IRepositoryAsync<MovableAsset> repository, DateTime? fromDate, DateTime? toDate)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (fromDate != null && toDate != null)
            {
                return location.Where(x => (x.Status == LocationStatus.Active || x.Status == LocationStatus.MovedRequest) &&
                (x.MovableAsset.InsertDate > fromDate && x.MovableAsset.InsertDate <= toDate))
                   .Select(o => new
                   {
                       o.AssetId.Value,
                       o.MovableAsset,
                       o.PersonId,
                       o.StrategyId,
                       o.OrganizId,
                       o.MovableAsset.StoreBill.AcqType
                   }).AsNoTracking().AsEnumerable().Select(x => new MovableAssetModel
                   {
                       AssetId = x.Value,
                       MAssetType = x.MovableAsset.ToString("t", null),
                       UnitId = x.MovableAsset.UnitId,
                       Name = x.MovableAsset.Name,
                       Num = x.MovableAsset.Num,
                       Label = x.MovableAsset.Label,
                       InsertDate = x.MovableAsset.InsertDate,
                       PersonId = x.PersonId,
                       StrategyId = x.StrategyId,
                       OrganizId = x.OrganizId,
                       IsCompietion = x.MovableAsset.ISCompietion,
                       kalaUid = x.MovableAsset.KalaUid,
                       KalaNo = x.MovableAsset.KalaNo,
                       AcqType = x.AcqType,
                       IsConfirmed = x.MovableAsset.ISConfirmed,
                       Cost = x.MovableAsset.Cost,
                       PersianInsertDate = x.MovableAsset.InsertDate.PersianDateTime()
                   });
            }

            return location.Where(x => (x.Status == LocationStatus.Active || x.Status == LocationStatus.MovedRequest)
            && x.AssetId.HasValue)
                   .Select(o => new
                   {
                       o.AssetId.Value,
                       o.MovableAsset,
                       o.PersonId,
                       o.StrategyId,
                       o.OrganizId,
                       o.MovableAsset.StoreBill.AcqType
                   }).AsNoTracking().ToList().Select(x => new MovableAssetModel
                   {
                       AssetId = x.Value,
                       MAssetType = x.MovableAsset.ToString("t", null),
                       UnitId = x.MovableAsset.UnitId,
                       Name = x.MovableAsset.Name,
                       Num = x.MovableAsset.Num,
                       Label = x.MovableAsset.Label,
                       InsertDate = x.MovableAsset.InsertDate,
                       PersonId = x.PersonId,
                       StrategyId = x.StrategyId,
                       OrganizId = x.OrganizId,
                       IsCompietion = x.MovableAsset.ISCompietion,
                       AcqType=x.AcqType,
                       kalaUid = x.MovableAsset.KalaUid,
                       KalaNo = x.MovableAsset.KalaNo,
                       IsConfirmed = x.MovableAsset.ISConfirmed,
                       Cost = x.MovableAsset.Cost,
                       PersianInsertDate = x.MovableAsset.InsertDate.PersianDateTime()
                   });
        }

        public static IEnumerable<MovableAssetModel> GetStoreMovableAssetByLocation(this IRepositoryAsync<MovableAsset> repository, DateTime? fromDate, DateTime? toDate,int storeId,LocationStatus storeStatus)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (storeId == 0)
            {
                if (fromDate != null && toDate != null)
                {
                    return location.Where(x => x.Status == storeStatus
                        && (x.MovableAsset.InsertDate > fromDate && x.MovableAsset.InsertDate <= toDate))
                   .Select(o => new
                   {
                       o.AssetId.Value,
                       o.MovableAsset,
                       o.StoreId,
                       o.StoreAddressId,
                       o.MovableAsset.StoreBill.AcqType,
                   }).AsNoTracking().AsEnumerable()
                    .Select(x => new MovableAssetModel
                    {
                        AssetId = x.Value,
                        MAssetType = x.MovableAsset.ToString("t", null),
                        UnitId = x.MovableAsset.UnitId,
                        Name = x.MovableAsset.Name,
                        kalaUid=x.MovableAsset.KalaUid,
                        KalaNo = x.MovableAsset.KalaNo,
                        Num = x.MovableAsset.Num,
                        Label = x.MovableAsset.Label,
                        InsertDate = x.MovableAsset.InsertDate,
                        StoreId = x.StoreId,
                        StoreDesignId = x.StoreAddressId,
                        IsCompietion = x.MovableAsset.ISCompietion,
                        Cost=x.MovableAsset.Cost,
                        AcqType = x.AcqType,
                        PersianInsertDate=x.MovableAsset.InsertDate.PersianDateTime()
                    });
                }

                return location.Where(x => x.Status == storeStatus)
                   .Select(o => new
                   {
                       o.AssetId.Value,
                       o.MovableAsset,
                       o.StoreId,
                       o.StoreAddressId,
                       o.MovableAsset.StoreBill.AcqType,
                   }).AsNoTracking().AsEnumerable()
                    .Select(x => new MovableAssetModel
                    {
                        AssetId = x.Value,
                        MAssetType = x.MovableAsset.ToString("t", null),
                        UnitId = x.MovableAsset.UnitId,
                        Name = x.MovableAsset.Name,
                        kalaUid = x.MovableAsset.KalaUid,
                        KalaNo = x.MovableAsset.KalaNo,
                        Num = x.MovableAsset.Num,
                        Label = x.MovableAsset.Label,
                        InsertDate = x.MovableAsset.InsertDate,
                        StoreId = x.StoreId,
                        StoreDesignId = x.StoreAddressId,
                        IsCompietion = x.MovableAsset.ISCompietion,
                        Cost = x.MovableAsset.Cost,
                        AcqType = x.AcqType,
                        PersianInsertDate = x.MovableAsset.InsertDate.PersianDateTime()
                    });
            }
            else
            {
                if (fromDate != null && toDate != null)
                {
                    return location.Where(x => x.Status == storeStatus
                        && (x.MovableAsset.InsertDate > fromDate && x.MovableAsset.InsertDate <= toDate) && x.StoreId == storeId)
                   .Select(o => new
                   {
                       o.AssetId.Value,
                       o.MovableAsset,
                       o.StoreId,
                       o.StoreAddressId,
                       o.MovableAsset.StoreBill.AcqType,
                   }).AsNoTracking().AsEnumerable()
                    .Select(x => new MovableAssetModel
                    {
                        AssetId = x.Value,
                        MAssetType = x.MovableAsset.ToString("t", null),
                        UnitId = x.MovableAsset.UnitId,
                        Name = x.MovableAsset.Name,
                        Num = x.MovableAsset.Num,
                        kalaUid = x.MovableAsset.KalaUid,
                        KalaNo = x.MovableAsset.KalaNo,
                        Label = x.MovableAsset.Label,
                        InsertDate = x.MovableAsset.InsertDate,
                        StoreId = x.StoreId,
                        StoreDesignId = x.StoreAddressId,
                        IsCompietion = x.MovableAsset.ISCompietion,
                        Cost = x.MovableAsset.Cost,
                        AcqType = x.AcqType,
                        PersianInsertDate = x.MovableAsset.InsertDate.PersianDateTime()
                    });
                }

                return location.Where(x => x.Status == storeStatus && x.StoreId == storeId)
                   .Select(o => new
                   {
                       o.AssetId.Value,
                       o.MovableAsset,
                       o.StoreId,
                       o.StoreAddressId,
                       o.MovableAsset.StoreBill.AcqType,
                   }).AsNoTracking().AsEnumerable()
                    .Select(x => new MovableAssetModel
                    {
                        AssetId = x.Value,
                        MAssetType = x.MovableAsset.ToString("t", null),
                        UnitId = x.MovableAsset.UnitId,
                        Name = x.MovableAsset.Name,
                        Num = x.MovableAsset.Num,
                        Label = x.MovableAsset.Label,
                        kalaUid = x.MovableAsset.KalaUid,
                        KalaNo = x.MovableAsset.KalaNo,
                        InsertDate = x.MovableAsset.InsertDate,
                        StoreId = x.StoreId,
                        StoreDesignId = x.StoreAddressId,
                        IsCompietion = x.MovableAsset.ISCompietion,
                        AcqType = x.AcqType,
                        PersianInsertDate = x.MovableAsset.InsertDate.PersianDateTime()
                    });
            }
        }

        public static IEnumerable<MovableAssetModel> GetMovableAssetByStoreBill(this IRepositoryAsync<MovableAsset> repository, int billId)
        {
            var storeBill = repository.GetRepository<StoreBill>().Queryable();

            var items = (from b in storeBill
                         from m in b.MAssets
                         where b.StoreBillId == billId
                         select new
                         {
                             m,
                         }).ToList().Select(x => new MovableAssetModel
                         {
                             AssetId = x.m.AssetId,
                             MAssetType = x.m.ToString("t", null),
                             UnitId = x.m.UnitId,
                             Name = x.m.Name,
                             Num = x.m.Num,
                             kalaUid=x.m.KalaUid,
                             KalaNo = x.m.KalaNo,
                             Label = x.m.Label,
                             InsertDate = x.m.InsertDate,
                         });

            return items.AsEnumerable();
        }

        public static IEnumerable<MovableAssetModel> GetMAssetsByStoreBillToDocument(this IRepositoryAsync<MovableAsset> repository, int billId)
        {
            var mAssets = repository.GetRepository<MovableAsset>().Queryable();
            return mAssets.Where(ma => ma.StoreBillId == billId)
            .AsEnumerable().Select(x=>new MovableAssetModel
            {
                AssetId = x.AssetId,
                MAssetType = x.ToString("t", null),
                UnitId = x.UnitId,
                Name = x.Name,
                Num = x.Num,
                kalaUid = x.KalaUid,
                KalaNo=x.KalaNo,
                Label = x.Label,
                InsertDate = x.InsertDate,
            });
        }
        
        public static IEnumerable<MovableAssetModel> GetMovableAssetByDocument(this IRepositoryAsync<MovableAsset> repository, Int64 docId)
        {
            var mAssets = repository.GetRepository<MovableAsset>().Queryable();
            var items = (from m in mAssets
                         from d in m.Documetns
                         where d.DocumentId == docId
                         select new
                         {
                             m
                         }).ToList().Select(x => new MovableAssetModel
                         {
                             AssetId = x.m.AssetId,
                             MAssetType = x.m.ToString("t", null),
                             UnitId = x.m.UnitId,
                             Name = x.m.Name,
                             Num = x.m.Num,
                             kalaUid=x.m.KalaUid,
                             KalaNo=x.m.KalaNo,
                             Label = x.m.Label,
                             InsertDate = x.m.InsertDate,
                         });

            return items;
        }

        public static IEnumerable<Document> GetDocuments(this IRepositoryAsync<MovableAsset> repository,bool includeAssets
            ,bool toRetiringStore)
        {
            var doc = repository.GetRepository<Document>().Queryable().Where(x=>x.DocumentType!=DocumentType.InitialBalance);
            if (!includeAssets)
            {
                if (toRetiringStore)
                {
                    return (from d in doc
                            where d.Store.StoreType == StoreType.Retiring && d.DocumentType != DocumentType.StoreBillRetiring
                            select d).Include(d => d.Store).AsEnumerable();
                }
                return (from d in doc
                        where d.Store.StoreType != StoreType.Retiring
                        select d).Include(d=>d.Store).AsEnumerable();
            }

            if (toRetiringStore)
            {
                return (from d in doc
                        where d.Store.StoreType == StoreType.Retiring && d.DocumentType != DocumentType.StoreBillRetiring
                        select d).Include(d => d.MovableAsset).Include(d => d.Store).AsEnumerable();
            }
            return (from d in doc
                    where d.Store.StoreType != StoreType.Retiring
                    select d).Include(d => d.MovableAsset).Include(d => d.Store).AsEnumerable();
        }

        public static IEnumerable<Document> GetAllDocumentsToMovableAssets(this IRepositoryAsync<MovableAsset> repository)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            return (from d in doc
                    where d.MovableAsset.Any()
                    select d).Include(d => d.MovableAsset).Include(d => d.Store).AsEnumerable();
        }

        public static IEnumerable<Document> GetAllDocuments(this IRepositoryAsync<MovableAsset> repository)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            return (from d in doc
                    select d).Include(d => d.Store).AsEnumerable();
        }

        public static IEnumerable<Document> GetDocumentsByType(this IRepositoryAsync<MovableAsset> repository, DocumentType docType,bool isRetiringStore)
        {
            var doc = repository.GetRepository<Document>().Queryable();
            if (isRetiringStore)
            {
                return (from d in doc
                        where d.DocumentType == docType && d.Store.StoreType == StoreType.Retiring
                        select d).Include(d => d.MovableAsset).AsEnumerable();
            }
            return (from d in doc
                    where d.DocumentType == docType && d.Store.StoreType != StoreType.Retiring
                    select d).Include(d => d.MovableAsset).AsEnumerable();
        }

        public static IEnumerable<MovableAsset> GetDisplacemetnAssets(this IRepositoryAsync<MovableAsset> repository, string personId,int orgranizId)
        {
            var location = repository.GetRepository<Location>().Queryable();
            return location.Where(x => x.PersonId == personId && (x.Status == LocationStatus.Active && x.OrganizId==orgranizId) && x.MovableAsset.ISConfirmed)
                .Select(m => m.MovableAsset).Include(m => m.Locations).Include(m=>m.AssetProceedings).AsEnumerable();
        }

        public static IEnumerable<Belonging> GetBelongingsToLocation(this IRepositoryAsync<MovableAsset> repository, Int64 assetid)
        {
            var mAssets = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return mAssets.Where(m => m.AssetId == assetid).SelectMany(ma => ma.Belongings).Include(ma => ma.Locations).AsEnumerable();
        }

        public static Location GetLocation(this IRepositoryAsync<MovableAsset> repository, Int64 assetId, Boolean isInStore)
        {
            var location = repository.GetRepository<Location>().Queryable();

            if (isInStore)
            {
                return (from l in location
                        where l.AssetId == assetId
                        && (l.Status == LocationStatus.StoreActive)
                        select l).SingleOrDefault();
            }

            return (from l in location
                    where l.AssetId == assetId
                    && (l.Status == LocationStatus.Active || l.Status == LocationStatus.MovedRequest)
                    select l).SingleOrDefault();
        }

        public static Location GetLastOneBeforeLocation(this IRepositoryAsync<MovableAsset> repository, long assetId)
        {
            var location = repository.GetRepository<Location>().Queryable();
            return location.Where(x => x.AssetId == assetId).OrderByDescending(x=>x.LocationId).Take(2).AsEnumerable().LastOrDefault();
        }

        public static Location GetLastLocation(this IRepositoryAsync<MovableAsset> repository, long assetId)
        {
            var location = repository.GetRepository<Location>().Queryable();
            return location.Where(x => x.AssetId == assetId).OrderByDescending(x => x.LocationId).FirstOrDefault();
        }

        public static Location GetMovedRequestLocation(this IRepositoryAsync<MovableAsset> repository, Int64 assetId)
        {
            var location = repository.GetRepository<Location>().Queryable();

            return (from l in location
                    where l.AssetId == assetId
                    && (l.Status == LocationStatus.MovedRequest)
                    select l).SingleOrDefault();
        }

        public static IEnumerable<UnConsumption> GetUnConsuptionAssetToLabelInStore(this IRepositoryAsync<MovableAsset> repository,
            bool checkCompietion)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (checkCompietion)
            {
                return location.Where(x => x.Status == LocationStatus.StoreActive && x.MovableAsset.Label.HasValue)
               .Select(m => m.MovableAsset).Where(m=>m.ISCompietion==CompietionState.Reported).AsNoTracking().OfType<UnConsumption>().AsEnumerable();
            }
            return location.Where(x =>x.Status ==LocationStatus.StoreActive && x.MovableAsset.Label.HasValue)
                .Select(m => m.MovableAsset).AsNoTracking().OfType<UnConsumption>().AsEnumerable();
        }

        public static IEnumerable<UnConsumption> GetUnConsuptionAssetToLabelInRetiringStore(this IRepositoryAsync<MovableAsset> repository, bool ForProceeding, bool isCompietion)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (ForProceeding)
            {
                List<UnConsumption> items = null;

                if (isCompietion)
                {
                    items = location.Where(x => x.Status == LocationStatus.Retiring)
                    .Select(m => m.MovableAsset).Where(m => m.ISCompietion == CompietionState.Reported && m.Label.HasValue).Include(m => m.AssetProceedings)
                    .OfType<UnConsumption>().AsNoTracking().ToList();
                }
                else
                {
                    items = location.Where(x => x.Status == LocationStatus.Retiring)
                   .Select(m => m.MovableAsset).Where(m => m.Label.HasValue).Include(m => m.AssetProceedings)
                   .OfType<UnConsumption>().AsNoTracking().ToList();
                }

                var availableItems = new List<UnConsumption>();
                items.ForEach(m =>
                {
                    if (m.AssetProceedings.Count > 0)
                    {
                        if (m.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                        {
                            availableItems.Add(m);
                        }
                    }
                    else
                    {
                        availableItems.Add(m);
                    }
                });

                return availableItems;
            }
            return location.Where(x => x.Status == LocationStatus.Retiring)
                .Select(m => m.MovableAsset).OfType<UnConsumption>().AsEnumerable();
        }

        public static IEnumerable<UnConsumption> GetUnConsuptionAssetToAccident(this IRepositoryAsync<MovableAsset> repository, bool ForProceeding)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (ForProceeding)
            {
                var items = location.Where(x => x.Status == LocationStatus.Accident)
                .Select(m => m.MovableAsset).Where(m => m.ISCompietion == CompietionState.Reported && m.Label.HasValue).Include(m => m.AssetProceedings)
                .OfType<UnConsumption>().AsNoTracking().ToList();
                var availableItems = new List<UnConsumption>();
                items.ForEach(m =>
                {
                    if (m.AssetProceedings.Count > 0)
                    {
                        if (m.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                        {
                            availableItems.Add(m);
                        }
                    }
                    else
                    {
                        availableItems.Add(m);
                    }
                });

                return availableItems;
            }
            return location.Where(x => x.Status == LocationStatus.Accident)
                .Select(m => m.MovableAsset).OfType<UnConsumption>().AsEnumerable();
        }

        public static IEnumerable<UnConsumption> GetTrustAssetToReturntFromTrust(this IRepositoryAsync<MovableAsset> repository)
        {
            var location = repository.GetRepository<Location>().Queryable();
            var items = location.Where(x => x.Status == LocationStatus.Trust)
                 .Select(m => m.MovableAsset).Include(m => m.AssetProceedings).Include(m=>m.AssetProceedings.Select(map=>map.Proceeding)).Include(ma=>ma.StoreBill)
                 .OfType<UnConsumption>().AsNoTracking().ToList();
            var availableItems = new List<UnConsumption>();
            items.ForEach(m =>
            {
                if (m.AssetProceedings.Count > 0)
                {
                    if (m.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                    {
                        availableItems.Add(m);
                    }
                }
                else
                {
                    availableItems.Add(m);
                }
            });

            return availableItems;
        }

        public static IEnumerable<MovableAssetModel> GetPersonAssets(this IRepositoryAsync<MovableAsset> repository, string personId,Boolean isActive)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (isActive)
            {
                return location.Where(x => x.PersonId == personId && (x.Status == LocationStatus.Active)).Select(o => new
                {
                    o.AssetId.Value,
                    o.MovableAsset,
                    o.PersonId,
                    o.StrategyId,
                    o.OrganizId,
                    o.Status
                }).ToList().Select(x => new MovableAssetModel
                {
                    AssetId = x.Value,
                    MAssetType = x.MovableAsset.ToString("t", null),
                    UnitId = x.MovableAsset.UnitId,
                    Name = x.MovableAsset.Name,
                    Num = x.MovableAsset.Num,
                    kalaUid=x.MovableAsset.KalaUid,
                    KalaNo=x.MovableAsset.KalaNo,
                    Label = x.MovableAsset.Label,
                    InsertDate = x.MovableAsset.InsertDate,
                    PersonId = x.PersonId,
                    StrategyId = x.StrategyId,
                    OrganizId = x.OrganizId,
                    Status = x.Status
                });
            }
            return location.Where(x => x.PersonId == personId && (x.Status == LocationStatus.Active || x.Status == LocationStatus.MovedRequest)).Select(o => new
                     {
                         o.AssetId.Value,
                         o.MovableAsset,
                         o.PersonId,
                         o.StrategyId,
                         o.OrganizId,
                         o.Status
                     }).ToList().Select(x => new MovableAssetModel
                     {
                         AssetId = x.Value,
                         MAssetType = x.MovableAsset.ToString("t", null),
                         UnitId = x.MovableAsset.UnitId,
                         Name = x.MovableAsset.Name,
                         Num = x.MovableAsset.Num,
                         kalaUid = x.MovableAsset.KalaUid,
                         KalaNo = x.MovableAsset.KalaNo,
                         Label = x.MovableAsset.Label,
                         InsertDate = x.MovableAsset.InsertDate,
                         PersonId = x.PersonId,
                         StrategyId = x.StrategyId,
                         OrganizId = x.OrganizId,
                         Status = x.Status
                     });
        }
        
        public static IEnumerable<AccountDocumentModel> GetLocationsForAccountDocuments(this IRepositoryAsync<MovableAsset> repository,DateTime fromDate,DateTime toDate)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return mAsset.SelectMany(x => x.Locations).Where(x=> x.AccountDocumentType != AccountDocumentType.None && (x.InsertDate>=fromDate && x.InsertDate<toDate)).Select(x => new AccountDocumentModel
            {
                AssetId = x.AssetId.Value,
                AssetName = x.MovableAsset.Name,
                InsertDate = x.InsertDate,
                Label = x.MovableAsset.Label,
                StoreAddressId = x.StoreAddressId,
                OrganizId = x.OrganizId,
                StoreId = x.StoreId,
                StrategyId = x.StrategyId,
                AccountDocumentType = x.AccountDocumentType,
                Status = x.Status,
                Cost = x.MovableAsset.Cost
            }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentsForAssetByLabel(this IRepositoryAsync<MovableAsset> repository, int Label)
        {
            var location = repository.GetRepository<Location>().Queryable();
            return location.Where(x => x.MovableAsset.Label == Label && x.AccountDocumentType != AccountDocumentType.None).Select(x => new AccountDocumentModel
            {
                AssetId = x.AssetId.Value,
                AssetName = x.MovableAsset.Name,
                InsertDate = x.InsertDate,
                Label = x.MovableAsset.Label,
                StoreAddressId = x.StoreAddressId,
                OrganizId = x.OrganizId,
                StoreId = x.StoreId,
                StrategyId = x.StrategyId,
                AccountDocumentType = x.AccountDocumentType,
                Status = x.Status,
                Cost = x.MovableAsset.Cost
            }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentsForAssetById(this IRepositoryAsync<MovableAsset> repository, Int64 assetId)
        {
            var location = repository.GetRepository<Location>().Queryable();
            return location.Where(x => x.MovableAsset.AssetId == assetId && x.AccountDocumentType != AccountDocumentType.None).Select(x => new AccountDocumentModel
            {
                AssetId = x.AssetId.Value,
                AssetName = x.MovableAsset.Name,
                InsertDate = x.InsertDate,
                Label = x.MovableAsset.Label,
                StoreAddressId = x.StoreAddressId,
                OrganizId = x.OrganizId,
                StoreId = x.StoreId,
                StrategyId = x.StrategyId,
                AccountDocumentType = x.AccountDocumentType,
                Status = x.Status,
                Cost = x.MovableAsset.Cost
            }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentsForSpecificLoc(this IRepositoryAsync<MovableAsset> repository, int loc, bool inStore)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            if (inStore)
            {
                return (from ma in mAsset
                        from l in ma.Locations
                        where l.StoreId == loc && l.AccountDocumentType!=AccountDocumentType.None
                        select l).Select(x => new AccountDocumentModel
                        {
                            AssetId=x.AssetId.Value,
                            AssetName=x.MovableAsset.Name,
                            InsertDate=x.InsertDate,
                            Label=x.MovableAsset.Label,
                            StoreAddressId = x.StoreAddressId,
                            OrganizId=x.OrganizId,
                            StoreId=x.StoreId,
                            StrategyId=x.StrategyId,
                            AccountDocumentType = x.AccountDocumentType,
                            Status = x.Status,
                            Cost = x.MovableAsset.Cost
                        }).AsEnumerable();
            }
            return (from ma in mAsset
                    from l in ma.Locations
                    where l.AccountDocumentType != AccountDocumentType.None
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost=x.MovableAsset.Cost
                    }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentForDisaster(this IRepositoryAsync<MovableAsset> repository)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status=x.Status,
                        Cost=x.MovableAsset.Cost,
                    }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentForTrust(this IRepositoryAsync<MovableAsset> repository,Int64[] assetIds)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    where assetIds.Contains(l.AssetId.Value) && (l.AccountDocumentType == AccountDocumentType.UnitsToTrust || l.AccountDocumentType==AccountDocumentType.StockToTrust)
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentForRefundTrust(this IRepositoryAsync<MovableAsset> repository,long[] assetIds)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    where assetIds.Contains(l.AssetId.Value) && (l.AccountDocumentType == AccountDocumentType.TrustToUnits || l.AccountDocumentType == AccountDocumentType.TrustToStock)
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentForEscrow(this IRepositoryAsync<MovableAsset> repository, Int64[] assetIds)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    where assetIds.Contains(l.AssetId.Value) && (l.AccountDocumentType == AccountDocumentType.EscrowToTrust || l.AccountDocumentType == AccountDocumentType.TrustToEscrow)
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentsByAcqType(this IRepositoryAsync<MovableAsset> repository,StateOwnership acqTyp)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();

            if (acqTyp == StateOwnership.Trust)
            {
                return (from ma in mAsset
                        from l in ma.Locations
                        where ma.StoreBill.AcqType == acqTyp && (l.Status == LocationStatus.Trust)
                        select l).Select(x => new AccountDocumentModel
                        {
                            AssetId = x.AssetId.Value,
                            AssetName = x.MovableAsset.Name,
                            InsertDate = x.InsertDate,
                            Label = x.MovableAsset.Label,
                            StoreAddressId = x.StoreAddressId,
                            OrganizId = x.OrganizId,
                            StoreId = x.StoreId,
                            StrategyId = x.StrategyId,
                            AccountDocumentType = x.AccountDocumentType,
                            Status = x.Status,
                            Cost = x.MovableAsset.Cost,
                        }).AsEnumerable();
            }

            return (from ma in mAsset
                    from l in ma.Locations
                    where ma.StoreBill.AcqType==acqTyp
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentForLabelContiunity(this IRepositoryAsync<MovableAsset> repository, int fromLabel, int ToLabel)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    where (ma.Label >= fromLabel && ma.Label <= ToLabel)
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).OrderBy(x=>x.Label).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentByAssetName(this IRepositoryAsync<MovableAsset> repository, string assetName)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    where ma.Name.Contains(assetName)
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).OrderBy(x => x.Label).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentModel> GetAccountDocumentByCurState(this IRepositoryAsync<MovableAsset> repository, MAssetCurState locStatus)
        {
            var mAsset = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return (from ma in mAsset
                    from l in ma.Locations
                    where l.MovableAsset.CurState==locStatus
                    select l).Select(x => new AccountDocumentModel
                    {
                        AssetId = x.AssetId.Value,
                        AssetName = x.MovableAsset.Name,
                        InsertDate = x.InsertDate,
                        Label = x.MovableAsset.Label,
                        StoreAddressId = x.StoreAddressId,
                        OrganizId = x.OrganizId,
                        StoreId = x.StoreId,
                        StrategyId = x.StrategyId,
                        AccountDocumentType = x.AccountDocumentType,
                        Status = x.Status,
                        Cost = x.MovableAsset.Cost,
                    }).OrderBy(x => x.Label).AsEnumerable();
        }

        public static StoreBill GetBillStores(this IRepositoryAsync<MovableAsset> repository, long assetId)
        {
            var Assets = repository.GetRepository<MovableAsset>().Queryable();
            var bill =(from ma in Assets
                       where ma.AssetId == assetId
                       select ma.StoreBill).SingleOrDefault();
            return bill;
        }

        public static IEnumerable<MovableAsset> getMovableAssetToRefundTrust(this IRepositoryAsync<MovableAsset> repository,bool inStore)
        {
            var location = repository.GetRepository<Location>().Queryable();
            var items = new List<UnConsumption>();
            if (inStore)
            {
                items = (from l in location
                         where (l.MovableAsset.StoreBill.AcqType == StateOwnership.Trust) && (l.Status == LocationStatus.StoreActive)
                         select l.MovableAsset).Include(x => x.StoreBill).Include(m => m.AssetProceedings).Distinct().AsNoTracking().OfType<UnConsumption>().ToList();
            }
            else
            {
                items = (from l in location
                         from o in l.MovableAsset.Orders
                         where (l.MovableAsset.StoreBill.AcqType == StateOwnership.Trust) && (l.Status == LocationStatus.MovedRequest)
                         && (o.OrderType == OrderType.Procceding && o.Status == OrderStatus.StuffHonest)
                         select l.MovableAsset).Include(x => x.StoreBill).Include(m => m.AssetProceedings).Distinct().AsNoTracking().OfType<UnConsumption>().ToList();
            }
            
            var availableItems = new List<UnConsumption>();
            items.ForEach(m =>
            {
                if (m.AssetProceedings.Count > 0)
                {
                    if (m.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                    {
                        availableItems.Add(m);
                    }
                }
                else
                {
                    availableItems.Add(m);
                }
            });

            return availableItems;
        }

        public static bool ContainBelongings(this IRepositoryAsync<MovableAsset> repository,long assetId)
        {
            var Assets = repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            return Assets.Where
                (x => x.AssetId == assetId).Any(x => x.Belongings.Count()>0);
        }

        public static UnConsumption GetBelongingParnet(this IRepositoryAsync<MovableAsset> repository,long BelongingId)
        {
            var Assets = repository.GetRepository<MovableAsset>().Queryable();
            return Assets.Where(x => x.AssetId == BelongingId)
                .OfType<Belonging>().Select(x => x.ParentMAsset).SingleOrDefault();
        }

        public static IEnumerable<MovableAssetModel> GetStoreMovableAssetWithoutProcceding(this IRepositoryAsync<MovableAsset> repository, int storeId, bool checkCompietion)
        {
            var location = repository.GetRepository<Location>().Queryable();
            List<MovableAsset> mAssets = new List<MovableAsset>();
            if (storeId <= 0)
            {
                if(checkCompietion)
                mAssets = location.Where(x => x.Status == LocationStatus.StoreActive &&
                    x.MovableAsset.Label.HasValue && x.MovableAsset.ISCompietion == CompietionState.Reported).Select(x => x.MovableAsset).Include(m => m.AssetProceedings).AsNoTracking().ToList();
                else 
                    mAssets = location.Where(x => x.Status == LocationStatus.StoreActive &&
                    x.MovableAsset.Label.HasValue).Select(x => x.MovableAsset).Include(m => m.AssetProceedings).AsNoTracking().ToList();
            }
            else
            {
                if(checkCompietion)
                    mAssets = location.Where(x => x.Status == LocationStatus.StoreActive &&
                   x.StoreId == storeId && x.MovableAsset.ISCompietion == CompietionState.Reported).Select(x => x.MovableAsset).Include(m => m.AssetProceedings).ToList();
                else mAssets= location.Where(x => x.Status == LocationStatus.StoreActive &&
                    x.StoreId == storeId).Select(x => x.MovableAsset).Include(m => m.AssetProceedings).ToList();
            }
            List<MovableAssetModel> availableAssets = new List<MovableAssetModel>();
            mAssets.ForEach(l =>
            {
                if (l.AssetProceedings.Count > 0)
                {
                    if (l.AssetProceedings.All(x=>x.State!=AssetProceedingState.InProgress))
                    {
                        availableAssets.Add(new MovableAssetModel
                        {
                            AssetId = l.AssetId,
                            MAssetType = l.ToString("t", null),
                            UnitId = l.UnitId,
                            Name = l.Name,
                            Num = l.Num,
                            kalaUid=l.KalaUid,
                            KalaNo=l.KalaNo,
                            Label = l.Label,
                            InsertDate = l.InsertDate,
                            IsCompietion = l.ISCompietion,
                            StoreId = storeId,
                        });
                    }
                }
                else
                {
                    availableAssets.Add(new MovableAssetModel
                    {
                        AssetId = l.AssetId,
                        MAssetType = l.ToString("t", null),
                        UnitId = l.UnitId,
                        Name = l.Name,
                        Num = l.Num,
                        kalaUid = l.KalaUid,
                        KalaNo = l.KalaNo,
                        Label = l.Label,
                        InsertDate = l.InsertDate,
                        IsCompietion =l.ISCompietion,
                        StoreId=storeId,
                    });
                }
            });
            return availableAssets;
        }

        public static IEnumerable<MovableAssetModel> GetStoreMovableAssetWithoutProccedingByKalaUid(this IRepositoryAsync<MovableAsset> repository,int storeId, int kalaUid)
        {
            var location = repository.GetRepository<Location>().Queryable();
            List<MovableAsset> mAssets = new List<MovableAsset>();
            List<MovableAssetModel> availableAssets = new List<MovableAssetModel>();
            mAssets = location.Where(x => x.Status == LocationStatus.StoreActive &&
                    x.StoreId == storeId && x.MovableAsset.KalaUid==kalaUid)
                    .Select(x => x.MovableAsset).Include(m=>m.MovableAssetReserveHistories)
                    .Include(m => m.AssetProceedings).ToList();

            mAssets.ForEach(l =>
            {
                if (l.AssetProceedings.Any())
                {
                    if (l.AssetProceedings.All(x => x.State != AssetProceedingState.InProgress))
                    {
                        availableAssets.Add(new MovableAssetModel
                        {
                            AssetId = l.AssetId,
                            MAssetType = l.ToString("t", null),
                            UnitId = l.UnitId,
                            Name = l.Name,
                            Num = l.Num,
                            kalaUid = l.KalaUid,
                            KalaNo = l.KalaNo,
                            Label = l.Label,
                            InsertDate = l.InsertDate,
                            IsCompietion = l.ISCompietion,
                            StoreId = storeId,
                        });
                    }
                }
                else if (l.MovableAssetReserveHistories.Any())
                {
                    if (!l.MovableAssetReserveHistories.All(s => s.Status == MAssetReserveStatus.UnReserved
                     || s.Status != MAssetReserveStatus.UnReservedRequested))
                    {
                        availableAssets.Add(new MovableAssetModel
                        {
                            AssetId = l.AssetId,
                            MAssetType = l.ToString("t", null),
                            UnitId = l.UnitId,
                            Name = l.Name,
                            Num = l.Num,
                            kalaUid = l.KalaUid,
                            KalaNo = l.KalaNo,
                            Label = l.Label,
                            InsertDate = l.InsertDate,
                            IsCompietion = l.ISCompietion,
                            StoreId = storeId,
                        });
                    }
                }
                else
                {
                    availableAssets.Add(new MovableAssetModel
                    {
                        AssetId = l.AssetId,
                        MAssetType = l.ToString("t", null),
                        UnitId = l.UnitId,
                        Name = l.Name,
                        Num = l.Num,
                        kalaUid = l.KalaUid,
                        KalaNo = l.KalaNo,
                        Label = l.Label,
                        InsertDate = l.InsertDate,
                        IsCompietion = l.ISCompietion,
                        StoreId = storeId,
                    });
                }
            });
            return availableAssets;
        }

        public static IEnumerable<MovableAsset> GetStoreMovableAssetForProcceding(this IRepositoryAsync<MovableAsset> repository,bool checkCompietion)
        {
            var location = repository.GetRepository<Location>().Queryable();
            if (checkCompietion)
            {
                return location.Where(x => x.Status == LocationStatus.StoreActive &&
                    x.MovableAsset.Label.HasValue && x.MovableAsset.ISCompietion == CompietionState.Reported && x.MovableAsset.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                    .Select(l => l.MovableAsset).AsNoTracking().AsEnumerable();
            }

           return location.Where(x => x.Status == LocationStatus.StoreActive &&
            x.MovableAsset.Label.HasValue && x.MovableAsset.AssetProceedings.All(ap=>ap.State!=AssetProceedingState.InProgress))
              .Select(l=>l.MovableAsset).AsNoTracking().AsEnumerable();
        }

        public static IEnumerable<AssetProceeding> GetAssetProceedingsToMAssets(this IRepositoryAsync<MovableAsset> repository, int procId)
        {
            var ap = repository.GetRepository<AssetProceeding>().Queryable();
            return ap.Where(p => p.ProceedingId == procId).Include(p => p.MAsset).Include(p=>p.MAsset.Locations).AsEnumerable();
        }

        public static Document GetInitialBalanceDocumentByLocation(this IRepositoryAsync<MovableAsset> repository,Location location)
        {
            var loc = repository.GetRepository<Location>().Queryable();
            if (location.Status == LocationStatus.Active)
            {
                return loc.Where(l => l.PersonId == location.PersonId && 
                    l.OrganizId == location.OrganizId && l.StrategyId == location.StrategyId)
                    .SelectMany(l => l.MovableAsset.Documetns).Where(d => d.DocumentType == DocumentType.InitialBalance).FirstOrDefault();
            }
            //...l.status==locationstatus.storeDeactive is not acceptable...multiple locations they can this status
            return loc.Where(l => l.StoreId == location.StoreId && l.Status==LocationStatus.StoreActive)
                    .SelectMany(l => l.MovableAsset.Documetns).Where(d => d.DocumentType == DocumentType.InitialBalance).FirstOrDefault();
        }

        public static IEnumerable<AssetTaxCost> GetTaxCosts(this IRepositoryAsync<MovableAsset> repository,Int64 assetId)
        {
            var taxCosts = repository.GetRepository<AssetTaxCost>().Queryable();
            return (from c in taxCosts
                    where c.AssetId == assetId
                    select c).AsEnumerable();
        }

        public static IEnumerable<MovableAsset> GetMAssetsByExportDetails(this IRepositoryAsync<MovableAsset> repository, int exdId, bool toRelated)
        {
            var exDetails = repository.GetRepository<ExportDetails>().Queryable();
            if (toRelated) return exDetails.Where(e => e.ID == exdId).SelectMany(ex => ex.ExportDetailsMAsset).Select(ex=>ex.MAsset).Include(x => x.Locations).Include(x => x.StoreBill).AsEnumerable();
            return exDetails.Where(e => e.ID == exdId).SelectMany(ex => ex.ExportDetailsMAsset).Select(ex=>ex.MAsset).AsEnumerable();
        }
        
        public static AccountDocumentMaster GetLastAccountDocuemntForAsset(this IRepositoryAsync<MovableAsset> repository,Int64 assetId)
        {
            var accountdetails= repository.GetRepository<AccountDocumentDetails>().Queryable();
            return accountdetails.Where(x => x.AssetId == assetId).Select(x => x.AccountDocumentMaster).Distinct().OrderByDescending(x => x.ID)
                .Include(x => x.AccountDocumentDetails).FirstOrDefault();
        }

        public static IEnumerable<StoreBillIssueModel> GetStoreDocumentsIssue(this IRepositoryAsync<MovableAsset> repository,int kalaUid, StuffType stype, DateTime fromDate,
            DateTime toDate, int storeId,DocumentType docType, bool filterbyStype)
        {
            var docs = repository.GetRepository<Document>().Queryable();

            IQueryable<Document> filterdoc = null;

            if (filterbyStype)
            {
                switch (stype)
                {
                    case StuffType.Belonging:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<Belonging>()
                        .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                    case StuffType.Installable:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<Installable>()
                         .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                    case StuffType.OrderConsumption:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<InCommidity>()
                          .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                    default:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<UnConsumption>()
                            .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                }
            }
            else
            {
                filterdoc = docs.Where(x => x.MovableAsset
                          .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
            }
            

            if (storeId == 0)
            {
                var items = filterdoc.Select(b => new StoreBillIssueModel
                    {
                        Date = b.DocumentDate,
                        Num = b.MovableAsset.Count(x => x.KalaUid == kalaUid),
                        SBNo = b.Desc1,
                        Price = b.MovableAsset.Where(x => x.KalaUid == kalaUid).Sum(x => x.Cost),
                        KalaUid = kalaUid,
                        Transfree=b.Transferee
                    });
                return items.AsEnumerable();
            }
            var items1 = filterdoc.Where(x => x.StoreId==storeId)
                    .Select(b => new StoreBillIssueModel
                    {
                        Date = b.DocumentDate,
                        Num = b.MovableAsset.Count(x => x.KalaUid == kalaUid),
                        SBNo = b.Desc1,
                        Price = b.MovableAsset.Where(x => x.KalaUid == kalaUid).Sum(x => x.Cost),
                        KalaUid = kalaUid,
                        Transfree = b.Transferee
                    });
            return items1.AsEnumerable();
        }

        public static AnalizModel GetCurrentStoreByKalaUid(this IRepositoryAsync<MovableAsset> repository, int kalaUid, StuffType sType, bool filterbyStype)
        {
            var locs = repository.GetRepository<Location>().Queryable();
            IEnumerable<MovableAsset> lo = null;
            if (filterbyStype)
            {
                if (sType == StuffType.OrderConsumption)
                {
                    lo = (from l in locs
                          where l.Status == LocationStatus.StoreActive && l.MovableAsset.KalaUid == kalaUid
                          select l.MovableAsset).OfType<InCommidity>().AsEnumerable();
                }
                else if (sType == StuffType.Installable)
                {
                    lo = (from l in locs
                          where l.Status == LocationStatus.StoreActive && l.MovableAsset.KalaUid == kalaUid
                          select l.MovableAsset).OfType<Installable>().AsEnumerable();
                }
                else if (sType == StuffType.Belonging)
                {
                    lo = (from l in locs
                          where l.Status == LocationStatus.StoreActive && l.MovableAsset.KalaUid == kalaUid
                          select l.MovableAsset).OfType<Belonging>().AsEnumerable();
                }
                else
                {
                    lo = (from l in locs
                          where l.Status == LocationStatus.StoreActive && l.MovableAsset.KalaUid == kalaUid
                          select l.MovableAsset).OfType<UnConsumption>().AsEnumerable();
                }
            }
            else
            {
                lo = (from l in locs
                      where l.Status == LocationStatus.StoreActive && l.MovableAsset.KalaUid == kalaUid
                      select l.MovableAsset).AsEnumerable();
            }

            return new AnalizModel
            {
                Description = "موجودی حال حاضر",
                Num = lo.Count(),
                UnitName="عدد",
                Identity=AnalizModelIdentity.Stock
            };
        }

        public static AnalizModel GetStoreInternalDocAnaliz(this IRepositoryAsync<MovableAsset> repository, int kalaUid, StuffType sType, DateTime fromDate,
            DateTime toDate, bool filterbyStype)
        {

            var docs = repository.GetRepository<Document>().Queryable();

            IQueryable<Document> filterdoc = null;
            if (filterbyStype)
            {
                switch (sType)
                {
                    case StuffType.Belonging:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<Belonging>()
                        .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                    case StuffType.Installable:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<Installable>()
                         .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                    case StuffType.OrderConsumption:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<InCommidity>()
                          .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                    default:
                        filterdoc = docs.Where(x => x.MovableAsset.OfType<UnConsumption>()
                            .Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
                        break;
                }
            }
            else
            {
                filterdoc = docs.Where(x => x.MovableAsset.Any(ma => ma.KalaUid == kalaUid) && x.DocumentType == DocumentType.StoreInternalDraft && (x.DocumentDate > fromDate && x.DocumentDate <= toDate));
            }
            
            
            return new AnalizModel
            {
                Description = "تعداد حواله انبار کشیده شده",
                Num = filterdoc.Count(),
                UnitName = "عدد",
                Identity = AnalizModelIdentity.InternalDraft
            };
        }

        public static IEnumerable<MovableAssetReserveHistory> GetReserveHistories(this IRepositoryAsync<MovableAsset> repository, Int64 assetId)
        {
            var mAssets = repository.GetRepository<MovableAsset>().Queryable();
            return mAssets.Where(ma => ma.AssetId == assetId).SelectMany(ma => ma.MovableAssetReserveHistories).AsEnumerable();
        }
    }
}
