
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public class Employee : Entity
    {
        public Employee()
        {
            this.Persons = new List<Person>();
            this.Estates = new List<Estate>();
            this.EmployeeDesign = new List<EmployeeDesign>();
            this.ExportDetails = new List<ExportDetails>();
            this.Sellers = new List<Seller>();
            this.AccountDocumentCodings = new List<AccountDocumentCoding>();
            this.AccountDocumentMasters = new List<AccountDocumentMaster>();
            this.ReturnIndentRequests = new List<ReturnIndentRequest>();
        }

        public Int32 EmployeeId { get; set; }
        public String Name { get; set; }
        public String ParentName { get; set; }
        public String Tell { get; set; }
        public String RegisterationNo { get; set; }
        public Int32 BudgetNo { get; set; }
        public String WebAddress { get; set; }
        public String Email { get; set; }
        public String Fax { get; set; }
        public String AddressLine { get; set; }
        public DateTime CreateDate { get; set; }
        public Byte[] Logo { get; set; }
        public string Province { get; set; }
        public string TwonShip { get; set; }
        public string Zone { get; set; }
        public string City { get; set; }
        public virtual ICollection<Person> Persons { get; private set; }
        public virtual ICollection<Estate> Estates { get; private set; }
        public virtual ICollection<EmployeeDesign> EmployeeDesign { get; private set; }
        public virtual ICollection<ExportDetails> ExportDetails { get; private set; }
        public virtual ICollection<Seller> Sellers { get; private set; }
        public virtual ICollection<AccountDocumentCoding> AccountDocumentCodings { get; private set; }
        public virtual ICollection<AccountDocumentMaster> AccountDocumentMasters { get; private set; }
        public virtual ICollection<ReturnIndentRequest> ReturnIndentRequests { get; private set; }
        public override string ToString()
        {
            return string.Format("{0}   {1}", this.ParentName, this.Name);
        }
    }
}
