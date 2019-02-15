
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class UnitMap : EntityTypeConfiguration<Unit>
    {
        public UnitMap()
        {
            this.HasKey(x => x.UnitId);
            this.Property(x => x.UnitId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(x => x.StuffId).IsRequired();
            this.Property(x => x.Name).IsRequired().HasMaxLength(300);
            this.Property(x => x.MathNum).IsOptional();
            this.Property(x => x.MathType).IsRequired();

            this.ToTable("Production.Unit");
            this.Property(x => x.Name).HasColumnName("Name");
            this.Property(x => x.StuffId).HasColumnName("StuffId");
            this.Property(x => x.UnitId).HasColumnName("UnitId");
            this.Property(x => x.MathType).HasColumnName("MathType");
            this.Property(x => x.MathNum).HasColumnName("MathNum");

            this.HasMany(x => x.Childeren)
               .WithOptional(x => x.Parent)
               .Map(x => x.MapKey("ParentId"));
        }
    }
}
