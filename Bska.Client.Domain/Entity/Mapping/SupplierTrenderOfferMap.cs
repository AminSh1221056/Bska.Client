
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;

    public class SupplierTrenderOfferMap : EntityTypeConfiguration<SupplierTrenderOffer>
    {
        public SupplierTrenderOfferMap()
        {
            this.HasKey(sp => sp.Id);

            this.Property(sp => sp.InsertDate).IsRequired();
            this.Property(sp => sp.SupplierId).IsRequired();
            this.Property(sp => sp.ProForma).IsOptional();
            this.Property(sp => sp.SubOrderId).IsOptional();
            this.Property(sp => sp.ISEnableTrender).IsRequired();
            this.ToTable("Production.SupplierTrenderOffer");

            this.HasOptional(s => s.SubOrder).WithMany(s => s.SupplierTrenderOffers)
                .HasForeignKey(s => s.SubOrderId).WillCascadeOnDelete(true);
        }
    }
}
