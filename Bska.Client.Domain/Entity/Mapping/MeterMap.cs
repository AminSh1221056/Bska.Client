
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class MeterMap : EntityTypeConfiguration<Meter>
    {
        public MeterMap()
        {
            this.HasKey(x => x.ImAssetId);

            this.Property(x => x.AddressLine).IsOptional().HasMaxLength(150);
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.ModeifiedDate).IsRequired();
            this.Property(x => x.BodyNo).IsOptional().HasMaxLength(25);
            this.Property(x => x.Name).IsRequired().HasMaxLength(50);
            this.Property(x => x.Plake).IsOptional().HasMaxLength(15);
            this.Property(x => x.PostalCode).IsRequired().HasMaxLength(15);
            this.Property(x => x.SubscriptionNo).IsOptional().HasMaxLength(25);
            this.Property(x => x.CaseNo).IsRequired().HasMaxLength(25);
            this.Property(x => x.TariffType).IsRequired();
            this.Property(x => x.BuildingId).IsOptional();

            this.ToTable("EmployeeResources.Meter");

            this.HasOptional(x => x.Building)
                .WithMany(x => x.Meters)
                .HasForeignKey(x => x.BuildingId).WillCascadeOnDelete(true);

            this.Map<PowerMeter>(m => m.Requires("Type").HasValue(1))
                .Map<WaterMeter>(m => m.Requires("Type").HasValue(2))
                .Map<GasMeter>(m => m.Requires("Type").HasValue(3))
                .Map<TellMeter>(m => m.Requires("Type").HasValue(4))
                .Map<MobileMeter>(m => m.Requires("Type").HasValue(5));
        }
    }
}
