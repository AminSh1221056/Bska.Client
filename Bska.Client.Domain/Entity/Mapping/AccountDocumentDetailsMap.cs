
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class AccountDocumentDetailsMap : EntityTypeConfiguration<AccountDocumentDetails>
    {
        public AccountDocumentDetailsMap()
        {
            this.HasKey(x => x.ID);

            this.Property(x => x.AccountNo).IsRequired().HasMaxLength(50);
            this.Property(x => x.Creditor).IsRequired();
            this.Property(x => x.Debtor).IsRequired();
            this.Property(x => x.MasterId).IsOptional();
            this.Property(x => x.Description).IsOptional().HasMaxLength(350);

            this.ToTable("EmployeeResources.AccountDocumentDetails");

            this.HasOptional(x => x.MAsset).WithMany(x => x.AccountDocumentDetails)
                .HasForeignKey(x => x.AssetId).WillCascadeOnDelete(true);

            this.HasOptional(x => x.AccountDocumentMaster).WithMany(x => x.AccountDocumentDetails)
               .HasForeignKey(x => x.MasterId).WillCascadeOnDelete(false);
        }
    }
}
