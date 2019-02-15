
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class StoreBillModel
    {
        public Int32 StoreBillId { get; set; }
        public String StoreBillNo { get; set; }
        public DateTime ArrivalDate { get; set; }
        public PersianDate PersianArrivalDate { get; set; }
        public StateOwnership AcqType { get; set; }
        public Int32? StoreId { get; set; }
        public Int32? SellerId { get; set; }
        public StuffType StuffType { get; set; }
    }
}
