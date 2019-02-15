
namespace Bska.Client.Domain.Entity.Mapping
{
    using AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class CommodityMap : EntityTypeConfiguration<Commodity>
    {
        public CommodityMap()
        {
            this.HasKey(x => x.AssetId);
            
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.ModeifiedDate).IsRequired();
            this.Property(x => x.Name).IsRequired().HasMaxLength(150);
            this.Property(x => x.Num).IsRequired();
            this.Property(x => x.Cost).IsRequired();
            this.Property(x => x.Description).IsOptional().HasMaxLength(250);
            this.Property(x => x.Country).IsOptional().HasMaxLength(50);
            this.Property(x => x.Company).IsOptional().HasMaxLength(50);
            this.Property(x => x.DateOfBirth).IsOptional();
            this.Property(x => x.ExpirationDate).IsOptional();
            this.Property(x => x.KalaUid).IsRequired();
            this.Property(x => x.KalaNo).HasMaxLength(50).IsRequired();
            this.Property(x => x.UnitId).IsRequired();
            this.Property(x => x.Quality).IsRequired().HasMaxLength(1);
            this.Property(x => x.StoreBillId).IsOptional();
            this.Property(x => x.StoreAddress).HasMaxLength(50).IsOptional();
            this.Property(x => x.BatchNumber).IsOptional();
            this.Property(x => x.IndentId).IsOptional();
            this.ToTable("EmployeeResources.Commodity");

            this.HasOptional(x => x.UnConsumption).WithMany(u => u.Commodities)
                .HasForeignKey(x => x.UnConsuptionId).WillCascadeOnDelete(false);
        }
    }
}
