
namespace Bska.Client.Repository
{
    using API.Repositories;
    using Domain.Entity.AssetEntity;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using Model;
    using Common;

    public static class StoreBillRepository
    {
        public static int getRelatedAccountMasterId(this IRepository<StoreBill> repository,int billId)
        {
            var acdm = repository.GetRepository<AccountDocumentMaster>().Queryable();
            return (from ac in acdm
                    where ac.StoreBill.StoreBillId==billId
                    select ac.ID).FirstOrDefault();
        }

        public static IEnumerable<StoreBillEditModel> GetRecivedEditsByState(this IRepository<StoreBill> repository, GlobalRequestStatus state)
        {
            var bill = repository.GetRepository<StoreBillEdit>().Queryable();
            return bill.Where(s => s.State == state).Select(s => new StoreBillEditModel
            {
                AcqType=s.StoreBill.AcqType,
                StoreBillNo=s.StoreBill.StoreBillNo,
                State=s.State,
                Description=s.Description,
                InsertDate=s.InsertDate,
                StoreBillId=s.StoreBillId.Value,
                Id=s.Id
            }).AsEnumerable();
        }

        public static int CountRecivedEditsByState(this IRepository<StoreBill> repository, GlobalRequestStatus state)
        {
            var bill = repository.GetRepository<StoreBillEdit>().Queryable();
            return bill.Where(s => s.State == state).Count();
        }

        public static int CountAssetReserveHistories(this IRepository<StoreBill> repository, MAssetReserveStatus status)
        {
            var bill = repository.GetRepository<StoreBill>().Queryable();

            return bill.SelectMany(b=>b.Commodities).Where(co => co.CommodityAssetReserveHistories
                    .Any(rs => rs.Status == status)).Count()
                    + bill.SelectMany(b => b.MAssets).Where(co => co.MovableAssetReserveHistories
                      .Any(rs => rs.Status == status)).Count();
        }

        public static IEnumerable<StoreBillIssueModel> GetIssueBillByStuff(this IRepository<StoreBill> repository, int kalaUid, StuffType sType, DateTime fromDate,
            DateTime toDate, int storeId, bool filterbyStype, bool forCommodity)
        {
            var bill = repository.GetRepository<StoreBill>().Queryable();
            if (forCommodity)
            {
                if (storeId == 0)
                {
                    var items = bill.Where(x => x.Commodities.Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate))
                        .Select(b => new StoreBillIssueModel
                        {
                            Date = b.ArrivalDate,
                            Num = b.Commodities.Count(x => x.KalaUid == kalaUid),
                            SBNo = b.StoreBillNo,
                            Price = b.Commodities.Where(x => x.KalaUid == kalaUid).Sum(x => x.Cost),
                            KalaUid = kalaUid
                        });
                    return items.AsEnumerable();
                }
                var items1 = bill.Where(x => x.Commodities.Any(ma => ma.KalaUid == kalaUid) && x.StoreId == storeId && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate))
                       .Select(b => new StoreBillIssueModel
                       {
                           Date = b.ArrivalDate,
                           Num = b.Commodities.Count(x => x.KalaUid == kalaUid),
                           SBNo = b.StoreBillNo,
                           Price = b.Commodities.Where(x => x.KalaUid == kalaUid).Sum(x => x.Cost),
                           KalaUid = kalaUid
                       });
                return items1.AsEnumerable();
            }
            else
            {
                IQueryable<StoreBill> filterBill = null;
                if (filterbyStype)
                {
                    switch (sType)
                    {
                        case StuffType.Belonging:
                            filterBill = bill.Where(x => x.MAssets.OfType<Belonging>()
                            .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                            break;
                        case StuffType.Installable:
                            filterBill = bill.Where(x => x.MAssets.OfType<Installable>()
                             .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                            break;
                        case StuffType.OrderConsumption:
                            filterBill = bill.Where(x => x.MAssets.OfType<InCommidity>()
                              .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                            break;
                        default:
                            filterBill = bill.Where(x => x.MAssets.OfType<UnConsumption>()
                                .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                            break;
                    }
                }
                else
                {
                    filterBill = bill.Where(x => x.MAssets.Any(ma => ma.KalaUid == kalaUid) 
                    && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                }

                if (storeId == 0)
                {
                    var items = filterBill
                        .Select(b => new StoreBillIssueModel
                        {
                            Date = b.ArrivalDate,
                            Num = b.MAssets.Count(x => x.KalaUid == kalaUid),
                            SBNo = b.StoreBillNo,
                            Price = b.MAssets.Where(x => x.KalaUid == kalaUid).Sum(x => x.Cost),
                            KalaUid = kalaUid
                        });
                    return items.AsEnumerable();
                }
                var items1 = filterBill.Where(x => x.StoreId == storeId)
                       .Select(b => new StoreBillIssueModel
                       {
                           Date = b.ArrivalDate,
                           Num = b.MAssets.Count(x => x.KalaUid == kalaUid),
                           SBNo = b.StoreBillNo,
                           Price = b.MAssets.Where(x => x.KalaUid == kalaUid).Sum(x => x.Cost),
                           KalaUid = kalaUid
                       });
                return items1.AsEnumerable();
            }
        }

        public static AnalizModel GetStoreBillAnalized(this IRepository<StoreBill> repository, int kalaUid, StuffType sType, DateTime fromDate,
            DateTime toDate,bool filterbyStype, bool forCommodity)
        {
            var bill = repository.GetRepository<StoreBill>().Queryable();
            if (forCommodity)
            {
                var items1 = bill.Where(x => x.Commodities.Any(ma => ma.KalaUid == kalaUid) &&
             (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate)).AsEnumerable();
                return new AnalizModel
                {
                    Description = "تعداد قبض انبار کشیده شده",
                    Num = items1.Count(),
                    UnitName = "عدد",
                    Identity = AnalizModelIdentity.StoreBill
                };
            }

            IQueryable<StoreBill> filterBill = null;
            if (filterbyStype)
            {
                switch (sType)
                {
                    case StuffType.Belonging:
                        filterBill = bill.Where(x => x.MAssets.OfType<Belonging>()
                        .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                        break;
                    case StuffType.Installable:
                        filterBill = bill.Where(x => x.MAssets.OfType<Installable>()
                         .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                        break;
                    case StuffType.OrderConsumption:
                        filterBill = bill.Where(x => x.MAssets.OfType<InCommidity>()
                          .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                        break;
                    default:
                        filterBill = bill.Where(x => x.MAssets.OfType<UnConsumption>()
                            .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
                        break;
                }
            }
            else
            {
                filterBill = bill.Where(x => x.MAssets
                            .Any(ma => ma.KalaUid == kalaUid) && (x.ArrivalDate > fromDate && x.ArrivalDate <= toDate));
            }

            return new AnalizModel
            {
                Description = "تعداد قبض انبار کشیده شده",
                Num = filterBill.Count(),
                UnitName = "عدد",
                Identity = AnalizModelIdentity.StoreBill
            };
        }

        public static IEnumerable<StoreBillModel> GetStoreBillsForInternalDraft(this IRepository<StoreBill> repository, int storeId, HashSet<MAssetReserveStatus> rState)
        {
            var bill = repository.GetRepository<StoreBill>().Queryable();
            return bill.Where(x => x.StoreId == storeId &&
            (x.MAssets.Any(ma=>ma.MovableAssetReserveHistories.Any(rs=>rState.Contains(rs.Status)))
            || x.Commodities.Any(ma => ma.CommodityAssetReserveHistories.Any(rs => rState.Contains(rs.Status)))))
                .Select(sb=>new StoreBillModel
            {
                AcqType=sb.AcqType,
                ArrivalDate=sb.ArrivalDate,
                SellerId=sb.SellerId,
                StoreBillId=sb.StoreBillId,
                StuffType=sb.StuffType,
                StoreBillNo=sb.StoreBillNo,
                StoreId=sb.StoreId
            }).AsEnumerable();
        }
        
    }
}
