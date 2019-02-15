
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.HelpEntity;
    using System.Data.Entity.ModelConfiguration;

    public class BskaHelpMap : EntityTypeConfiguration<BskaHelp>
    {
        public BskaHelpMap()
        {
            this.HasKey(b => b.Id);
            this.Property(b => b.Title).HasMaxLength(100).IsRequired();
            this.Property(b => b.Description).IsRequired();
            this.Property(b => b.Photo).IsOptional();
            this.Property(b => b.FileType).HasMaxLength(100).IsOptional();
            this.Property(b => b.Identity).HasMaxLength(50).IsRequired();
            this.Property(b => b.InsertDate).IsRequired();
            this.Property(b => b.TableItems).IsOptional();

            this.ToTable("Production.BskaHelp");
        }
    }
}
