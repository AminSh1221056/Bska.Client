
namespace Bska.Client.Domain.Entity.Mapping
{
    using AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class ExportDetailsMAssetMap : EntityTypeConfiguration<ExportDetailsMAsset>
    {
        public ExportDetailsMAssetMap()
        {
            this.ToTable("EmployeeResources.ExportDetailsMAsset");

            this.HasRequired(ed => ed.ExportDetail)
                .WithMany(ed => ed.ExportDetailsMAsset).HasForeignKey(ed => ed.ExportID).WillCascadeOnDelete(true);

            this.HasRequired(ed => ed.MAsset)
                .WithMany(ed => ed.ExportDetailsMAsset).HasForeignKey(ed => ed.AssetId).WillCascadeOnDelete(false);
        }
    }
}
