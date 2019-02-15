
namespace Bska.Client.Repository.Model
{
    using System;
    public class CustomerModel
    {
        public Int32 OrganId { get; set; }
        public String Name { get; set; }
        public String NationalCode { get; set; }
        public Int32 BudgetNo { get; set; }
        public String Tell { get; set; }
        public String Email { get; set; }
        public String Province { get; set; }
        public String Zone { get; set; }
        public String City { get; set; }
        public String TownShip { get; set; }
        public String ParentName { get; set; }
        public String Fax { get; set; }
        public String WebAddress { get; set; }
        public String AddressLine { get; set; }
    }
}
