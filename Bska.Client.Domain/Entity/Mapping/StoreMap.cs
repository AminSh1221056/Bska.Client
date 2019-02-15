
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            this.HasKey(x => x.StoreId);

            this.Property(x => x.CreateDate).IsRequired();
            this.Property(x => x.Description).IsOptional().HasMaxLength(350);
            this.Property(x => x.Name).IsRequired().HasMaxLength(70);
            this.Property(x => x.Storage).IsOptional().HasMaxLength(150);
            this.Property(x => x.StoreType).IsRequired();
            this.Property(x => x.StrategyId).IsOptional();


            this.HasOptional(x => x.Strategy)
                .WithMany(x => x.Stores)
                .HasForeignKey(x => x.StrategyId)
                .WillCascadeOnDelete(true);


            this.ToTable("Production.Store");
            this.Property(x => x.StoreId).HasColumnName("StoreId");
            this.Property(x => x.StoreType).HasColumnName("StoreType");
            this.Property(x => x.Storage).HasColumnName("Storage");
            this.Property(x => x.Name).HasColumnName("Name");
            this.Property(x => x.Description).HasColumnName("Description");
            this.Property(x => x.CreateDate).HasColumnName("CreateDate");
        }
    }
}
