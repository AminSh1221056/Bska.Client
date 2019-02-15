
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class ZoneMap : EntityTypeConfiguration<Zone>
    {
        public ZoneMap()
        {
            this.HasKey(x => x.ZoneId);

            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);

            this.ToTable("Production.Zone");
        }
    }
}
