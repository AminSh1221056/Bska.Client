
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.HasKey(x => x.OrderId);

            this.Property(x => x.DueDate).IsOptional();
            this.Property(x => x.ModifiedDate).IsRequired();
            this.Property(x => x.OrderDate).IsRequired();
            this.Property(x => x.OrderType).IsRequired();
            this.Property(x => x.PersonId).IsOptional();
            this.Property(x => x.Status).IsRequired();
            this.Property(x => x.OrderProcType).IsOptional();

            this.ToTable("Person.Order");

            this.HasMany(o => o.OrderDetails).WithOptional(o => o.Order)
                .HasForeignKey(o => o.OrderId).WillCascadeOnDelete(true);

            this.HasMany(x => x.MovableAssets)
                .WithMany(x => x.Orders)
                .Map(po =>
                {
                    po.ToTable("OrderMovableAsset", "Person");
                    po.MapLeftKey("OrderId");
                    po.MapRightKey("AssetId");
                });

            this.HasMany(x => x.Commodities)
             .WithMany(x => x.Orders)
             .Map(po =>
             {
                 po.ToTable("OrderCommodity", "Person");
                 po.MapLeftKey("OrderId");
                 po.MapRightKey("AssetId");
             });
        }
    }
}
