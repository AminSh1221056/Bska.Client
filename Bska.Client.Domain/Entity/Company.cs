
namespace Bska.Client.Domain.Entity
{
    using System;
    public class Company
    {
        public Int32 CompanyId { get; set; }
        public Int32? CountryId { get; set; }
        public Boolean IsCarCompany { get; set; }
        public String Name { get; set; }
    }
}
