
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class StoreBillEditMap : EntityTypeConfiguration<StoreBillEdit>
    {
        public StoreBillEditMap()
        {
            this.HasKey(sbe => sbe.Id);

            this.Property(sbe => sbe.State).IsRequired();
            this.Property(sbe => sbe.StoreBillId).IsOptional();
            this.Property(sbe => sbe.Description).HasMaxLength(250).IsOptional();
            this.Property(sbe => sbe.InsertDate).IsRequired();

            this.ToTable("EmployeeResources.StoreBillEdit");
            this.HasOptional(sb => sb.StoreBill).WithMany(sbe => sbe.StoreBillEdits).HasForeignKey(sb => sb.StoreBillId)
                .WillCascadeOnDelete(true);
        }
    }
}
