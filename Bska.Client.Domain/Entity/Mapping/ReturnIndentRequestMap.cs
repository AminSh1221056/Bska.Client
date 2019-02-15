
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.OrderEntity;
    using System.Data.Entity.ModelConfiguration;

    public class ReturnIndentRequestMap : EntityTypeConfiguration<ReturnIndentRequest>
    {
        public ReturnIndentRequestMap()
        {
            this.HasKey(sbr => sbr.Id);

            this.Property(sbr => sbr.Status).IsRequired();
            this.Property(sbr => sbr.Description).HasMaxLength(500).IsOptional();
            this.Property(Sbr => Sbr.EmployeeId).IsOptional();
            this.Property(sbr => sbr.InsertDate).IsRequired();

            this.ToTable("EmployeeResources.ReturnIndentRequest");

            this.HasMany(x => x.SupplierIndents)
             .WithMany(x => x.ReturnIndentRequsts)
             .Map(po =>
             {
                 po.ToTable("SupplierIndentReturnRequest", "EmployeeResources");
                 po.MapLeftKey("ReturnIndentRequstId");
                 po.MapRightKey("SupplierIndentId");
             });

            this.HasOptional(sbr => sbr.Employee).WithMany(e => e.ReturnIndentRequests)
                .HasForeignKey(sbr => sbr.EmployeeId).WillCascadeOnDelete(true);
        }
    }
}
