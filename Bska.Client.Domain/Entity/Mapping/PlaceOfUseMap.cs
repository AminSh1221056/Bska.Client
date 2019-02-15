
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity.CommodityAsset;
    using System.Data.Entity.ModelConfiguration;

    public class PlaceOfUseMap : EntityTypeConfiguration<PlaceOfUse>
    {
        public PlaceOfUseMap()
        {
            this.HasKey(p => p.Id);

            this.Property(x => x.OrganizId).IsRequired();
            this.Property(x => x.StrategtyId).IsRequired();
            this.Property(x => x.Num).IsRequired();
            this.Property(x => x.UnitId).IsRequired();
            this.Property(x => x.CommodityId).IsOptional();
            this.Property(x => x.DocumentId).IsOptional();
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.PersonId).HasMaxLength(20);

            this.ToTable("EmployeeResources.PlaceOfUse");

            this.HasOptional(x => x.Commodity)
                .WithMany(x => x.PlaceOfUses)
                .HasForeignKey(x => x.CommodityId)
                .WillCascadeOnDelete(true);

            this.HasOptional(x => x.Document)
               .WithMany(x => x.Commodities)
               .HasForeignKey(x => x.DocumentId)
               .WillCascadeOnDelete(false);
        }
    }
}
