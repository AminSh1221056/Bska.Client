
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    public class StuffMap : EntityTypeConfiguration<Stuff>
    {
        public StuffMap()
        {
            this.HasKey(x => x.KalaNo);
            this.Property(x => x.KalaNo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(x => x.Name).HasMaxLength(300).IsRequired();
            this.Property(x => x.StuffId).IsRequired();
            this.Property(x => x.IsStuff).IsRequired();
            this.Property(x => x.StuffType).IsRequired();
            this.Property(s => s.FloorNew).IsOptional();
            this.Property(s => s.FloorOld).IsOptional();
            this.Property(x => x.KalaNo).IsRequired().HasMaxLength(50);
            this.Property(x => x.GS1).IsOptional().HasMaxLength(16);

            this.ToTable("Production.Stuff");
            this.Property(x => x.IsStuff).HasColumnName("IsStuff");
            this.Property(x => x.Name).HasColumnName("KalaNme");
            this.Property(x => x.GS1).HasColumnName("GS1");
            this.Property(x => x.KalaNo).HasColumnName("KalaNo");
            this.Property(x => x.StuffId).HasColumnName("KalaUID");
            this.Property(x => x.StuffType).HasColumnName("Typ");

            this.HasMany(x => x.Childeren)
                .WithOptional(x => x.Parent)
                .Map(x => x.MapKey("ParentId"));

            this.MapToStoredProcedures(s => s.Insert(st => st.HasName("insert_stuff")));
            this.MapToStoredProcedures(s => s.Update(st => st.HasName("update_stuff")));
        }
    }
}
