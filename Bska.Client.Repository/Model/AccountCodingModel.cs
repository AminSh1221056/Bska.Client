
namespace Bska.Client.Repository.Model
{
    using Common;
    using System;
    using System.Collections.Generic;
    public class AccountCodingModel
    {
        public AccountCodingModel()
        {
            this.Childeren = new List<AccountCodingModel>();
        }

        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string AccountCode { get; set; }
        public AccountingDescrtiption TotalAccountType { get; set; }
        public CertainAccountsType CertainAccountType { get; set; }

        public AccountCodingModel Parent { get; set; }
        public ICollection<AccountCodingModel> Childeren { get; private set; }
    }
}
