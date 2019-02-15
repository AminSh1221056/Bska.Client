
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class InsuranceMap : EntityTypeConfiguration<Insurance>
    {
        public InsuranceMap()
        {
            this.HasKey(x => x.InsuranceId);

            this.Property(x => x.InsurancePolicyImage).IsOptional();
            this.Property(x => x.InsuranceNo).IsRequired().HasMaxLength(50);
            this.Property(x => x.AssetId).IsOptional();
            this.Property(x => x.InsuranceCompany).IsRequired().HasMaxLength(50);
            this.Property(x => x.Missionary).IsRequired();
            this.Property(x => x.ValidityDate).IsRequired();
            this.Property(x => x.NoDamage).IsOptional().HasMaxLength(2);

            this.ToTable("Production.Insurance");
            this.HasOptional(x => x.MAsset).WithMany(x => x.Insurances)
                .HasForeignKey(x => x.AssetId).WillCascadeOnDelete(true);
        }
    }
}
