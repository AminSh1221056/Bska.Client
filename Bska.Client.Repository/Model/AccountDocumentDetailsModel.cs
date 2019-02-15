using System;

namespace Bska.Client.Repository.Model
{
    public class AccountDocumentDetailsModel
    {
        public Int64 ID { get; set; }
        public string AccountNo { get; set; }
        public string Description { get; set; }
        public Decimal Creditor { get; set; }
        public String TotalAccount { get; set; }
        public Boolean IsCurent { get; set; }
        public decimal Debtor { get; set; }
        public Int32? MasterId { get; set; }
        public Int64? AssetId { get; set; }
        public string GroupColumnName { get; set; }
        public string AssetName { get; set; }
        public int? AssetLabel { get; set; }
        public DateTime AccountDate { get; set; }
    }
}
