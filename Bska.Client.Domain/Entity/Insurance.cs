
namespace Bska.Client.Domain.Entity
{
    using AssetEntity;
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    public class Insurance : Entity
    {
        public Int32 InsuranceId { get; set; }
        public InsuranceType Type { get; set; }
        public String InsuranceCompany { get; set; }
        public String InsuranceNo { get; set;}
        public DateTime ValidityDate { get; set; }
        public Decimal Missionary { get; set; }
        public String NoDamage { get; set; }
        public Byte[] InsurancePolicyImage { get; set; }
        public Int64? AssetId { get; set; }
        public virtual UnConsumption MAsset { get; set; }
    }
}
