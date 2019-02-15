
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class StoreDesignMap : EntityTypeConfiguration<StoreDesign>
    {
        public StoreDesignMap()
        {
            this.HasKey(x => x.StoreDesignId);

            this.Property(x => x.StoreId).IsOptional();
            this.Property(x => x.Name).IsRequired().HasMaxLength(50);

            this.ToTable("Production.StoreDesign");
            this.Property(x => x.StoreDesignId).HasColumnName("StoreDesignId");
            this.Property(x => x.StoreId).HasColumnName("StoreId");
            this.Property(x => x.Name).HasColumnName("Name");

            this.HasOptional(x => x.Store)
                .WithMany(x => x.StoreDesign).HasForeignKey(x => x.StoreId)
                .WillCascadeOnDelete(true);

            this.HasMany(x => x.ChildNode)
               .WithOptional(x => x.ParentNode)
               .Map(m => m.MapKey("NodeId"));
        }
    }
}
