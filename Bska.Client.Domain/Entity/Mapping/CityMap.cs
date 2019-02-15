
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            this.HasKey(x => x.CityId);

            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);

            this.ToTable("Production.City");
        }
    }
}
