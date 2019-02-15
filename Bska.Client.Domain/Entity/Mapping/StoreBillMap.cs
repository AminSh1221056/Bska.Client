
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class StoreBillMap : EntityTypeConfiguration<StoreBill>
    {
        public StoreBillMap()
        {
            this.HasKey(x => x.StoreBillId);

            this.Property(x => x.ArrivalDate).IsRequired();
            this.Property(x => x.AcqType).IsRequired();
            this.Property(x => x.StoreId).IsOptional();
            this.Property(x => x.ModifiedDate).IsRequired();
            this.Property(x => x.StoreBillNo).IsRequired().HasMaxLength(20);
            this.Property(x => x.Desc1).HasMaxLength(150);
            this.Property(x => x.Desc2).HasMaxLength(150);
            this.Property(x => x.Desc3).HasMaxLength(50);
            this.Property(x => x.StuffType).IsRequired();

            this.ToTable("EmployeeResources.StoreBill");
            this.Property(x => x.StoreBillId).HasColumnName("StoreBillId");
            this.Property(x => x.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(x => x.AcqType).HasColumnName("AcqType");
            this.Property(x => x.ArrivalDate).HasColumnName("ArrivalDate");

            this.HasOptional(sb => sb.Store).WithMany(s => s.StoreBills)
                .HasForeignKey(sb => sb.StoreId).WillCascadeOnDelete(true);
            this.HasMany(x => x.MAssets).WithOptional(x => x.StoreBill)
                .HasForeignKey(x => x.StoreBillId).WillCascadeOnDelete(true);
            this.HasMany(x => x.Commodities).WithOptional(x => x.StoreBill)
             .HasForeignKey(x => x.StoreBillId).WillCascadeOnDelete(true);
            this.HasOptional(x => x.AccountDocument)
         .WithOptionalPrincipal().WillCascadeOnDelete(false);
        }
    }
}
