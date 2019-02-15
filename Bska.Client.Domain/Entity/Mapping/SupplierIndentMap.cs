
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;
    public class SupplierIndentMap : EntityTypeConfiguration<SupplierIndent>
    {
        public SupplierIndentMap()
        {
            this.HasKey(sp => sp.ID);

            this.Property(sp => sp.Num).IsRequired();
            this.Property(sp => sp.SellerId).IsOptional();
            this.Property(sp => sp.State).IsRequired();
            this.Property(sp => sp.SubOrderId).IsOptional();
            this.Property(sp => sp.SupplierId).IsRequired();
            this.Property(sp => sp.Remain).IsRequired();
            this.Property(sp => sp.UnitId).IsRequired();
            this.ToTable("Person.SupplierIndent");

            this.HasOptional(sp => sp.Seller)
                .WithMany(sr => sr.SupplierIndents)
                .HasForeignKey(sp => sp.SellerId).WillCascadeOnDelete(false);

            this.HasOptional(sp => sp.SubOrder)
                .WithMany(sd => sd.SupplierIndents)
                .HasForeignKey(sp => sp.SubOrderId).WillCascadeOnDelete(true);

            this.HasMany(x => x.StoreBills)
            .WithMany(x => x.SupplierIndents)
            .Map(po =>
            {
                po.ToTable("StoreBillSupplierIndent", "EmployeeResources");
                po.MapLeftKey("SupplierIndentId");
                po.MapRightKey("StoreBillId");
            });
        }
    }
}
