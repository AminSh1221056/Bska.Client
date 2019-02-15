
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class LocationMap : EntityTypeConfiguration<Location>
    {
        public LocationMap()
        {
            this.HasKey(x => x.LocationId);
            
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.OrganizId).IsRequired();
            this.Property(x => x.PersonId).IsOptional().HasMaxLength(10);
            this.Property(x => x.AssetId).IsOptional();
            this.Property(x => x.ReturnDate).IsOptional();
            this.Property(x => x.MovedRequestDate).IsOptional();
            this.Property(x => x.Status).IsRequired();
            this.Property(x => x.StrategyId).IsRequired();
            this.Property(x => x.StoreId).IsRequired();
            this.Property(x => x.StoreAddressId).IsRequired();
            this.Property(x => x.AccountDocumentType).IsRequired();
            this.ToTable("EmployeeResources.Location");

            this.HasOptional(x => x.MovableAsset)
                .WithMany(x => x.Locations)
                .HasForeignKey(x => x.AssetId)
                .WillCascadeOnDelete(true);
        }
    }
}
