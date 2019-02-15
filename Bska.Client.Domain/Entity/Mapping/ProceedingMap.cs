
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class ProceedingMap : EntityTypeConfiguration<Proceeding>
    {
        public ProceedingMap()
        {
            this.HasKey(p => p.ProceedingId);

            this.Property(p => p.ExecutionTime).IsOptional();
            this.Property(p => p.Type).IsRequired();
            this.Property(p => p.ProceedingDate).IsRequired();
            this.Property(p => p.State).IsRequired();
            this.Property(p => p.ProcIdentity).IsRequired();
            this.Property(p => p.Description).IsRequired();
            this.Property(p => p.IsSended).IsRequired();
            this.Property(x => x.Desc1).HasMaxLength(50).IsOptional();
            this.Property(x => x.Desc2).HasMaxLength(50).IsOptional();
            this.Property(x => x.Desc3).HasMaxLength(50).IsOptional();
            this.Property(x => x.Desc4).HasMaxLength(50).IsOptional();
            this.Property(x => x.Desc5).HasMaxLength(50).IsOptional();
            this.Property(x => x.Desc6).HasMaxLength(50).IsOptional();

            this.ToTable("EmployeeResources.Procceding");

            this.HasOptional(sp => sp.Store).WithMany(p => p.Proceedings)
              .HasForeignKey(sp => sp.StoreId).WillCascadeOnDelete(false);
        }
    }
}
