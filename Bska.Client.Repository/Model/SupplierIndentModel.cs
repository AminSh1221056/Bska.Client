
namespace Bska.Client.Repository.Model
{
    using System;
    using Bska.Client.Common;
    public class SupplierIndentModel
    {
        public Int64 IndentId { get; set; }
        public string NationalId { get; set; }
        public int? OrganizId { get; set; }
        public int? StrategyId { get; set; }
        public int? StoreId { get; set; }
        public int? StoreAddressId { get; set; }
        public string PersonName { get; set; }
        public string StuffName { get; set; }
        public int KalaUid { get; set; }
        public string kalaNo { get; set; }
        public double Num { get; set; }
        public double Remain { get; set; }
        public Int32 UnitId { get; set; }
        public Int32? SellerId { get; set; }
        public StuffType StuffType { get; set; }
        public SupplierIndentState State { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
