
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using System;
    using Bska.Client.API.EF6;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AccountDocumentDetails : Entity
    {
        public Int64 ID { get; set; }
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public Decimal Creditor { get; set; }
        public decimal Debtor { get; set; }
        public Int32? MasterId { get; set; }
        public Int64? AssetId { get; set; }
        public virtual AccountDocumentMaster AccountDocumentMaster { get; set; }
        public virtual UnConsumption MAsset { get; set; }
    }
}
