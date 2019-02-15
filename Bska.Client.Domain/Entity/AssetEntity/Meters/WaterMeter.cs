
namespace Bska.Client.Domain.Entity.AssetEntity.Meters
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class WaterMeter : Meter
    {
        public Double WaterSplitDiagonal { get; set; }
        public Double WasteSplitDiagonal { get; set; }

        [MaxLength(50)]
        public String MeterStatus { get; set; }
    }
}
