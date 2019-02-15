
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;
    public class OrderHistoryMap : EntityTypeConfiguration<OrderUserHistory>
    {
        public OrderHistoryMap()
        {
            this.HasKey(x => x.Id);

            this.Property(x => x.Description).HasMaxLength(300).IsOptional();
            this.Property(x => x.OrderDetailsId).IsOptional();
            this.Property(x => x.UserDecision).IsRequired();
            this.Property(x => x.UserId).IsRequired();
            this.Property(x => x.IsCurrent).IsRequired();
            this.Property(x => x.Identity).IsOptional().HasMaxLength(50);
            this.ToTable("Person.OrderUserHistory");
        }
    }
}
