
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class AccountDocumentCodingMap : EntityTypeConfiguration<AccountDocumentCoding>
    {
        public AccountDocumentCodingMap()
        {
            this.HasKey(x => x.ID);
            this.Property(x => x.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Name).IsRequired().HasMaxLength(50);
            this.Property(x => x.AccountCode).IsRequired().HasMaxLength(10);
            this.Property(x => x.TotalAccountType).IsRequired();
            this.Property(x => x.CertainAccountType).IsRequired();
            this.ToTable("Production.AccountDocumentCoding");
            this.HasMany(x => x.Childeren)
             .WithOptional(x => x.Parent)
             .Map(x => x.MapKey("ParentId"));

            this.HasOptional(x => x.Employee).WithMany(x => x.AccountDocumentCodings)
            .HasForeignKey(x => x.EmployeeId).WillCascadeOnDelete(false);
        }
    }
}
