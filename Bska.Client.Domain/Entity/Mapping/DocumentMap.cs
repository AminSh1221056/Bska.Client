
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class DocumentMap : EntityTypeConfiguration<Document>
    {
        public DocumentMap()
        {
            this.HasKey(x => x.DocumentId);

            this.Property(x => x.Desc1).HasMaxLength(100).IsOptional();
            this.Property(x => x.DocumentDate).IsRequired();
            this.Property(x => x.DocumentType).IsRequired();
            this.Property(x => x.StoreId).IsOptional();
            this.Property(x => x.Desc2).HasMaxLength(250).IsOptional();
            this.Property(x => x.Desc3).HasMaxLength(250).IsOptional();
            this.Property(x => x.Desc4).HasMaxLength(100).IsOptional();
            this.Property(x => x.Transferee).HasMaxLength(50).IsOptional();
            this.ToTable("EmployeeResources.Document");

            this.HasOptional(sb => sb.Store).WithMany(s => s.Documents)
               .HasForeignKey(sb => sb.StoreId).WillCascadeOnDelete(false);

            this.HasMany(m => m.MovableAsset)
                .WithMany(d => d.Documetns)
                .Map(md =>
                {
                    md.ToTable("DocumentMAsset", "EmployeeResources");
                    md.MapLeftKey("DocumentId");
                    md.MapRightKey("AssetId");
                });

            this.HasOptional(x => x.AccountDocument)
       .WithOptionalPrincipal().WillCascadeOnDelete(false);
        }
    }
}
