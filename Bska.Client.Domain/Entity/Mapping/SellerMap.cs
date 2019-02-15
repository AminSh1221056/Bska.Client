
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class SellerMap : EntityTypeConfiguration<Seller>
    {
        public SellerMap()
        {
            this.HasKey(x => x.SellerId);

            this.Property(x => x.Type).IsRequired();
            this.Property(x => x.AddressLine).HasMaxLength(150).IsOptional();
            this.Property(x => x.City).IsRequired().HasMaxLength(100);
            this.Property(x => x.Province).IsRequired().HasMaxLength(100);
            this.Property(x => x.TownShip).IsRequired().HasMaxLength(100);
            this.Property(x => x.Zone).IsRequired().HasMaxLength(100);
            this.Property(x => x.Coding).IsRequired().HasMaxLength(50);
            this.Property(x => x.Tell).IsOptional().HasMaxLength(20);
            this.Property(x => x.Mobile).IsOptional().HasMaxLength(12);
            this.Property(x => x.Name).IsRequired().HasMaxLength(70);
            this.Property(x => x.Lastname).IsOptional().HasMaxLength(70);

            this.ToTable("Production.Seller");
            this.HasOptional(x => x.Employee)
                .WithMany(x => x.Sellers).HasForeignKey(x => x.EmployeeId).WillCascadeOnDelete(true);
        }
    }
}
