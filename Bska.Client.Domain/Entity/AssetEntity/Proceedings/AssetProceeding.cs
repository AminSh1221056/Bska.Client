
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AssetProceeding : Entity
    {
        public Decimal Price { get; set; }
        public String LicenseNumber { get; set; }
        public String AccidentDivanNo { get; set; }
        public Boolean IsOrganFault { get; set; }
        public String RecipetNo { get; set; }
        public AssetProceedingState State { get; set; }
        public String TempUid1 { get; set; }
        public String TempUid2 { get; set; }
        public String TempUid3 { get; set; }
        public String TempUid4 { get; set; }
        public String TempDesc1 { get; set; }
        public String TempDesc2 { get; set; }
        public String TempDesc3 { get; set; }
        public String TempDesc4 { get; set; }
        public int? TempYear { get; set; }
        public virtual Proceeding Proceeding { get; set; }
        public virtual MovableAsset MAsset { get; set; }

        [Key, Column(Order = 0)]
        public Int64 AssetId { get; set; }

        [Key, Column(Order = 1)]
        public Int32 ProceedingId { get; set; }
    }
}
