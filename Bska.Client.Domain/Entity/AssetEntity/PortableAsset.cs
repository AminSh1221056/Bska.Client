
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System;
    using System.Collections.Generic;

    public class PortableAsset : Asset
    {
        public Int64 AssetId { get; set; }
        public String Quality { get; set; }
        public double Num { get; set; }
        public Int32 UnitId { get; set; }
        public Decimal Cost { get; set; }
        public String Description { get; set; }
        public Int32 KalaUid { get; set; }
        public string KalaNo { get; set; }
        public long? IndentId { get; set; }
    }
}
