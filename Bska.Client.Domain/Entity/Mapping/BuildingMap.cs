
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class BuildingMap : EntityTypeConfiguration<Building>
    {
        public BuildingMap()
        {
            this.HasKey(x => x.BuildingId);
            
            this.Property(x => x.Alley).IsOptional().HasMaxLength(70);
            this.Property(x => x.City).IsRequired().HasMaxLength(100);
            this.Property(x => x.District).IsOptional().HasMaxLength(70);
            this.Property(x => x.MainStreet).IsRequired().HasMaxLength(70);
            this.Property(x => x.Name).IsRequired().HasMaxLength(200);
            this.Property(x => x.NewPlaque).IsOptional().HasMaxLength(10);
            this.Property(x => x.OldPlaque).HasMaxLength(10);
            this.Property(x => x.PostalCode).IsOptional().HasMaxLength(15);
            this.Property(x => x.Province).IsRequired().HasMaxLength(100);
            this.Property(x => x.TownShip).IsRequired().HasMaxLength(100);
            this.Property(x => x.Zone).IsRequired().HasMaxLength(100);
            this.Property(x => x.SecondaryAlley).IsOptional().HasMaxLength(70);
            this.Property(x => x.SecondaryStreet).IsOptional().HasMaxLength(70);
            this.Property(x => x.CreateDate).IsRequired();
            this.Property(x => x.EmployeeId).IsOptional();

            this.HasRequired(x => x.StrategyDesign)
              .WithOptional(x => x.Building)
              .WillCascadeOnDelete(true);

            this.ToTable("Production.Building");
        }
    }
}
