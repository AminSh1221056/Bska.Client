
namespace Bska.Client.Domain.Entity
{
    using System;
    using Bska.Client.API.EF6;
    using System.Collections.Generic;
    using AssetEntity;
    using Common;
    public class AccountDocumentCoding : Entity
    {
        public AccountDocumentCoding()
        {
            this.Childeren = new List<AccountDocumentCoding>();
        }

        public Int32 ID { get; set; }
        public String Name { get; set; }
        public string AccountCode { get; set; }
        public AccountingDescrtiption TotalAccountType { get; set; }
        public CertainAccountsType CertainAccountType { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual AccountDocumentCoding Parent { get; set; }
        public virtual ICollection<AccountDocumentCoding> Childeren { get; private set; }
        public virtual Employee Employee { get; set; }
    }
}
