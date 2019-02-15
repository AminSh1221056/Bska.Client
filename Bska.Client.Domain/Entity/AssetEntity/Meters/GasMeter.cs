
namespace Bska.Client.Domain.Entity.AssetEntity.Meters
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class GasMeter : Meter
    {
        [MaxLength(25)]
        public String CommonCode { get; set; }

        [MaxLength(25)]
        public String MeterSerialNo { get; set; }

        [MaxLength(1)]
        public String Group { get; set; }

        [MaxLength(25)]
        public String AddressCode { get; set; }
    }
}
