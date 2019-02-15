
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class CountryMap : EntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            this.HasKey(x => x.CountryId);
            this.Property(x => x.CountryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(c => c.CountryName).IsRequired().HasMaxLength(50);
            this.Property(c => c.CarCorporationId).IsOptional();

            this.ToTable("Production.Country");
        }
    }
}
