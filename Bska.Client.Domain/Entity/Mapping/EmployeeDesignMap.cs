
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class EmployeeDesignMap : EntityTypeConfiguration<EmployeeDesign>
    {
        public EmployeeDesignMap()
        {
            this.HasKey(x => x.BuidldingDesignId);

            this.Property(x => x.EmployeeId).IsOptional();
            this.Property(x => x.Name).IsRequired().HasMaxLength(50);
            this.Property(x => x.Code).IsOptional().HasMaxLength(50);

            this.ToTable("Production.EmployeeDesign");
            this.Property(x => x.BuidldingDesignId).HasColumnName("BuidldingDesignId");
            this.Property(x => x.EmployeeId).HasColumnName("EmployeeId");
            this.Property(x => x.Name).HasColumnName("Name");

            this.HasOptional(x => x.Employee)
                .WithMany(x => x.EmployeeDesign)
                .HasForeignKey(x => x.EmployeeId).WillCascadeOnDelete(true);

            this.HasMany(x => x.ChildNode)
                .WithOptional(x => x.ParentNode)
                .Map(m => m.MapKey("NodeId"));

            this.Map<OrganizationDesign>(m => m.Requires("Type").HasValue(1))
                .Map<StrategyDesign>(m => m.Requires("Type").HasValue(2));

        }
    }
}
