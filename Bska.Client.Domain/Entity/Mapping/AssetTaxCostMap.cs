
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class AssetTaxCostMap : EntityTypeConfiguration<AssetTaxCost>
    {
        public AssetTaxCostMap()
        {
            this.HasKey(e => e.Id);
            this.Property(e => e.Description).HasMaxLength(500).IsOptional();
            this.Property(e => e.Cost).IsRequired();
            this.Property(e => e.AssetId).IsOptional();
            this.Property(e => e.ModifiedDate).IsRequired();

            this.ToTable("EmployeeResources.AssetTaxCost");

            this.HasOptional(e => e.MAsset).WithMany(m => m.AssetTaxCost).HasForeignKey(e => e.AssetId).WillCascadeOnDelete(true);
        }
    }
}
