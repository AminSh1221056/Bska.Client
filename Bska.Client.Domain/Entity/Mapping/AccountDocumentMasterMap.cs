
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;

    public class AccountDocumentMasterMap : EntityTypeConfiguration<AccountDocumentMaster>
    {
        public AccountDocumentMasterMap()
        {
            this.HasKey(x => x.ID);

            this.Property(x => x.AccountCover).IsRequired().HasMaxLength(50);
            this.Property(x => x.AccountDate).IsRequired();
            this.Property(x => x.EmployeeId).IsOptional();
            this.ToTable("EmployeeResources.AccountDocumentMaster");

            this.HasOptional(x => x.Employee).WithMany(x => x.AccountDocumentMasters)
           .HasForeignKey(x => x.EmployeeId).WillCascadeOnDelete(true);

            this.HasOptional(x => x.StoreBill)
          .WithOptionalPrincipal().WillCascadeOnDelete(false);

            this.HasOptional(x => x.Document)
       .WithOptionalPrincipal().WillCascadeOnDelete(false);
        }
    }
}
