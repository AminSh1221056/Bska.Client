
namespace Bska.Client.Domain.Entity.Mapping
{
    using AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class ExportDetailsProceedingMap : EntityTypeConfiguration<ExportDetailsProceeding>
    {
        public ExportDetailsProceedingMap()
        {
            this.ToTable("EmployeeResources.ExportDetailsProceeding");

            this.HasRequired(ed => ed.ExportDetail)
                .WithMany(ed => ed.ExportDetailsProceeding).HasForeignKey(ed => ed.ExportID).WillCascadeOnDelete(true);

            this.HasRequired(ed => ed.Proceeding)
                .WithMany(ed => ed.ExportDetailsProceeding).HasForeignKey(ed => ed.ProceedingId).WillCascadeOnDelete(false);
        }
    }
}
