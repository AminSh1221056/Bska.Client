
namespace Bska.Client.Domain.Entity.Mapping
{
    using System;
    using System.Data.Entity.ModelConfiguration;

    public class EstateOrganMap : EntityTypeConfiguration<EstateOrgan>
    {
        public EstateOrganMap()
        {
            this.HasKey(es => es.Id);
            this.Property(es => es.Name).IsRequired().HasMaxLength(50);
            this.Property(es => es.ProvinceId).IsRequired().HasMaxLength(50);

            this.ToTable("Production.EstateOrgan");
        }
    }
}
