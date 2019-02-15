
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity.CommodityAsset;
    using System.Data.Entity.ModelConfiguration;
    public class CommodityAssetReserveHistoryMap : EntityTypeConfiguration<CommodityAssetReserveHistory>
    {
        public CommodityAssetReserveHistoryMap()
        {
            this.HasKey(sb => sb.Id);

            this.Property(sb => sb.Status).IsRequired();
            this.Property(Sb => Sb.Description).IsOptional().HasMaxLength(500);
            this.Property(sb => sb.CommodityId).IsOptional();

            this.ToTable("EmployeeResources.CommodityAssetReserveHistory");

            this.HasOptional(sb => sb.CommodityAsset).WithMany(s => s.CommodityAssetReserveHistories)
                .HasForeignKey(sb => sb.CommodityId).WillCascadeOnDelete(true);
        }
    }
}
