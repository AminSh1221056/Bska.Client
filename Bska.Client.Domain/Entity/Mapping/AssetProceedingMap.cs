
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class AssetProceedingMap : EntityTypeConfiguration<AssetProceeding>
    {
        public AssetProceedingMap()
        {
            this.Property(x => x.LicenseNumber).HasMaxLength(50).IsOptional();
            this.Property(x => x.Price).IsRequired();
            this.Property(x => x.State).IsRequired();
            this.Property(x => x.AccidentDivanNo).HasMaxLength(50).IsOptional();
            this.Property(x => x.IsOrganFault).IsRequired();
            this.Property(x => x.RecipetNo).HasMaxLength(50).IsOptional();
            this.Property(x => x.TempDesc4).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempDesc3).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempDesc2).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempDesc1).IsOptional().HasMaxLength(50);

            this.Property(x => x.TempUid1).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempUid2).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempUid3).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempUid4).IsOptional().HasMaxLength(50);
            this.Property(x => x.TempYear).IsOptional();

            this.ToTable("EmployeeResources.AssetProcceding");
            this.HasRequired(x => x.MAsset).WithMany(x => x.AssetProceedings)
                .HasForeignKey(x => x.AssetId);

            this.HasRequired(x => x.Proceeding).WithMany(x => x.AssetProceedings)
               .HasForeignKey(x => x.ProceedingId);
        }
    }
}
