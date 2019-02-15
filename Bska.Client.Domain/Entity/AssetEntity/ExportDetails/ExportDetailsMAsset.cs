
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ExportDetailsMAsset : Entity
    {
        [Key, Column(Order = 1)]
        public Int64 AssetId { get; set; }
        [Key, Column(Order = 0)]
        public Int32 ExportID { get; set; }
        public virtual MovableAsset MAsset { get; set; }
        public virtual ExportDetails ExportDetail { get; set; }
    }
}
