
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class OrganizationMap : EntityTypeConfiguration<OrganizationModel>
    {
        public OrganizationMap()
        {
            this.HasKey(x => x.EmployeeId);
            this.Property(x => x.EmployeeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(c => c.Name).IsRequired().HasMaxLength(50);
            this.Property(c => c.BudgetNo).IsRequired();

            this.ToTable("Production.Organization");
        }
    }
}
