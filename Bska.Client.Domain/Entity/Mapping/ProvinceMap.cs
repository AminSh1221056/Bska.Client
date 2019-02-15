
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class ProvinceMap : EntityTypeConfiguration<Province>
    {
        public ProvinceMap()
        {
            this.HasKey(x => x.ProvinceId);

            this.Property(c => c.ID).IsRequired();
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);

            this.ToTable("Production.Province");
        }
    }
}
