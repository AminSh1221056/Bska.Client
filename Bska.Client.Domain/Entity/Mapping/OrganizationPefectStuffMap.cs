
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class OrganizationPefectStuffMap : EntityTypeConfiguration<OrganizationPefectStuff>
    {
        public OrganizationPefectStuffMap()
        {
            this.ToTable("EmployeeResources.OrganizationPefectStuff");
            this.HasRequired(x => x.Stuff).WithMany(x => x.OrganizationPefectStuffs)
                .HasForeignKey(x => x.KalaNo);

            this.HasRequired(x => x.EmployeeDesign).WithMany(x => x.OrganizationPerfectStuffs)
               .HasForeignKey(x => x.BuidldingDesignId);
        }
    }
}
