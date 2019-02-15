
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;
    public class OrderDetailsMap : EntityTypeConfiguration<OrderDetails>
    {
        public OrderDetailsMap()
        {
            this.HasKey(x => x.OrderDetialsId);
            
            this.Property(x => x.StuffName).HasMaxLength(80).IsRequired();
            this.Property(x => x.KalaUid).IsRequired();
            this.Property(x => x.kalaNo).IsRequired().HasMaxLength(50);
            this.Property(x => x.StuffType).IsRequired();
            this.Property(x => x.IsReject).IsRequired();
            this.Property(x => x.State).IsRequired();
            this.Property(x => x.Num).IsRequired();
            this.Property(x => x.UnitId).IsRequired();
            this.Property(x => x.UsingLocation).HasMaxLength(150).IsOptional();
            this.Property(x => x.EstimatePrice).IsRequired();
            this.Property(x => x.ImportantDegree).IsRequired();
            this.Property(x => x.OfferQuality).HasMaxLength(1).IsOptional();
            this.Property(x => x.OfferSpecification).HasMaxLength(150).IsOptional();
            this.Property(x => x.OrganizId).IsOptional();
            this.Property(x => x.StrategyId).IsOptional();
            this.Property(x => x.StoreId).IsOptional();
            this.Property(x => x.StoreDesignId).IsOptional();
            this.Property(x => x.OrderId).IsOptional();
            this.Property(x => x.Description).IsOptional();
            this.Property(x => x.BelongingParentLable).IsOptional();
            this.ToTable("Person.OrderDetails");

            this.HasMany(o => o.SubOrders).WithOptional(o => o.OrderDetails)
            .HasForeignKey(o => o.OrderDetailsId).WillCascadeOnDelete(true);

            this.HasMany(o => o.OrderUserHistories).WithOptional(o => o.OrderDetails)
                .HasForeignKey(o => o.OrderDetailsId).WillCascadeOnDelete(true);
        }
    }
}
