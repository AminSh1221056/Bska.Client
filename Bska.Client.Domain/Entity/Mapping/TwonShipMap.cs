
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class TwonShipMap : EntityTypeConfiguration<TwonShip>
    {
        public TwonShipMap()
        {
            this.HasKey(x => x.TwonShipId);

            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);

            this.ToTable("Production.TwonShip");
        }
    }
}
