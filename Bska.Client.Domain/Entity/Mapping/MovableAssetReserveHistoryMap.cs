
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class MovableAssetReserveHistoryMap : EntityTypeConfiguration<MovableAssetReserveHistory>
    {
        public MovableAssetReserveHistoryMap()
        {
            this.HasKey(sb => sb.Id);

            this.Property(sb => sb.AssetId).IsOptional();
            this.Property(sb => sb.Status).IsRequired();
            this.Property(Sb => Sb.Description).IsOptional().HasMaxLength(500);

            this.ToTable("EmployeeResources.MovableAssetReserveHistory");
            this.HasOptional(sb => sb.MAsset).WithMany(s => s.MovableAssetReserveHistories)
                .HasForeignKey(sb => sb.AssetId).WillCascadeOnDelete(true);
        }
    }
}
