
namespace Bska.Client.Domain.Entity.AssetEntity.Meters
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PowerMeter : Meter
    {
        public Int32 FamiliesNum { get; set; }

        [MaxLength(25)]
        public String IdentificationNo { get; set; }
        public DateTime EarlyInstallationDate { get; set; }
        public Int32 Phase { get; set; }
        public Double Amper { get; set; }

        [MaxLength(2)]
        public String Statistic { get; set; }
        public Double Factor { get; set; }
    }
}
