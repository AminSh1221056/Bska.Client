
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class CarDetailsMap : EntityTypeConfiguration<CarDetails>
    {
        public CarDetailsMap()
        {
            this.HasKey(x => x.CarDetailsId);
            this.Property(x => x.CarDetailsId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(c => c.CarType).IsRequired();
            this.Property(c => c.SystemType).IsOptional().HasMaxLength(50);
            this.Property(c => c.Tipe).IsOptional().HasMaxLength(50);
            this.Property(c => c.Model).IsOptional().HasMaxLength(50);
            this.Property(c => c.CompanyId).IsOptional();

            this.ToTable("Production.CarDetails");
        }
    }
}
