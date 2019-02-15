
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class AccountDocumentModel
    {
        public string Id { get; set; }
        public DateTime InsertDate { get; set; }
        public AccountDocumentType AccountDocumentType { get; set; }
        public LocationStatus Status { get; set; }
        public Int64 AssetId { get; set; }
        public Int32 OrganizId { get; set; }
        public Int32 StrategyId { get; set; }
        public Int32 StoreId { get; set; }
        public Int32 StoreAddressId { get; set; }
        public String AssetName { get; set; }
        public Int32? Label { get; set; }
        public Decimal Cost { get; set; }
    }
}
