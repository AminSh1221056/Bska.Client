
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;
    public class SubOrderMap : EntityTypeConfiguration<SubOrder>
    {
        public SubOrderMap()
        {
            this.HasKey(x => x.SubOrderId);

            this.Property(x => x.Num).IsRequired();
            this.Property(x => x.OrderDetailsId).IsOptional();
            this.Property(x => x.Type).IsRequired();
            this.Property(x => x.State).IsRequired();
            this.Property(x => x.UnitId).IsRequired();
            this.Property(x => x.Remain).IsRequired();
            this.Property(x => x.Identity).HasMaxLength(50).IsOptional();

            this.ToTable("Person.SubOrder");
        }
    }
}
