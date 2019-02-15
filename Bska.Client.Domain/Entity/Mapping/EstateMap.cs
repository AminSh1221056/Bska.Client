
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class EstateMap : EntityTypeConfiguration<Estate>
    {
        public EstateMap()
        {
            this.HasKey(x => x.ImAssetId);
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.ModeifiedDate).IsRequired();
            this.Property(x => x.Name).IsRequired().HasMaxLength(150);
            this.Property(x => x.EmployeeId).IsOptional();
            this.Property(x => x.State).HasMaxLength(50).IsRequired();
            this.Property(x => x.RegistryOffice).HasMaxLength(50).IsRequired();
            this.Property(x => x.SectionRecords).HasMaxLength(50).IsOptional();
            this.Property(x => x.AreaRecords).HasMaxLength(50).IsOptional();
            this.Property(x => x.OriginalPlaque).HasMaxLength(25).IsOptional();
            this.Property(x => x.MinorPlaque).HasMaxLength(25).IsOptional();
            this.Property(x => x.Type).IsRequired();
            this.Property(x => x.BookNo).HasMaxLength(10).IsOptional();
            this.Property(x => x.PageNumber).HasMaxLength(10).IsOptional();
            this.Property(x => x.Text).IsRequired();
            this.Property(x => x.Address).HasMaxLength(250).IsRequired();
            this.Property(x => x.PostalCode).HasMaxLength(10).IsOptional();
            this.Property(x => x.Area).IsRequired();
            this.Property(x => x.Latitude).IsRequired();
            this.Property(x => x.Longitude).IsRequired();
            this.ToTable("EmployeeResources.Estate");
            this.HasOptional(x => x.Employee).WithMany(x => x.Estates)
             .HasForeignKey(x => x.EmployeeId).WillCascadeOnDelete(true);
        }
    }
}
