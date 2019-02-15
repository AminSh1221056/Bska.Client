
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            this.HasKey(c => c.CompanyId);
            this.Property(x => x.CompanyId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(c => c.Name).HasMaxLength(50).IsOptional();
            this.Property(c => c.CountryId).IsOptional();
            this.Property(c => c.IsCarCompany).IsRequired();
            this.ToTable("Production.Company");
        }
    }
}
