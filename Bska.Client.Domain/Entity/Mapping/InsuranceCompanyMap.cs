
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class InsuranceCompanyMap : EntityTypeConfiguration<InsuranceCompany>
    {
        public InsuranceCompanyMap()
        {
            this.HasKey(x => x.InsuranceId);
            this.Property(x => x.InsuranceId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(c => c.Name).IsRequired().HasMaxLength(50);

            this.ToTable("Production.InsuranceCompany");
        }
    }
}
