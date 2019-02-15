
namespace Bska.Client.Domain.Entity.AssetEntity.Meters
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class MobileMeter : Meter
    {
        [MaxLength(25)]
        public String EconomicNo { get; set; }
    }
}
