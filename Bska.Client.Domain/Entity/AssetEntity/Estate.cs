
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.Common;
    using System;

    public class Estate : ImmovableAsset
    {
        public String State { get; set; }
        public String RegistryOffice { get; set; }
        public String SectionRecords { get; set; }
        public String AreaRecords { get; set; }
        public String OriginalPlaque { get; set; }
        public String MinorPlaque { get; set; }
        public EstateType Type { get; set; }
        public String BookNo { get; set; }
        public String PageNumber { get; set; }
        public String Text { get; set; }
        public String Address { get; set; }
        public String PostalCode { get; set; }
        public Double Area { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
