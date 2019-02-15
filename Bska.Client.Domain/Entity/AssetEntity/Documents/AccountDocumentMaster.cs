
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using System;
    using Bska.Client.API.EF6;
    using System.Collections.Generic;
    public class AccountDocumentMaster : Entity
    {
        public AccountDocumentMaster()
        {
            this.AccountDocumentDetails = new List<AccountDocumentDetails>();
        }

        public Int32 ID { get; set; }
        public DateTime AccountDate { get; set; }
        public string AccountCover { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual ICollection<AccountDocumentDetails> AccountDocumentDetails { get; private set; }
        public virtual StoreBill StoreBill { get; set; }
        public virtual Document Document { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
