
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;
    public class ExportDetailsMap : EntityTypeConfiguration<ExportDetails>
    {
        public ExportDetailsMap()
        {
            this.HasKey(x => x.ID);
            this.Property(x => x.EmployeeId).IsOptional();
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.State).IsRequired();
            this.Property(x => x.SendType).IsRequired();
            this.Property(x => x.TbName).HasMaxLength(50).IsRequired();
            this.Property(x => x.VertifiedNo).HasMaxLength(50).IsOptional();
            this.Property(x => x.FileNo).HasMaxLength(50).IsRequired();
            this.Property(x => x.Directory).IsOptional().HasMaxLength(150);
            this.ToTable("EmployeeResources.ExportDetails");
            this.HasOptional(x => x.Employee).WithMany(x => x.ExportDetails).HasForeignKey(x => x.EmployeeId).WillCascadeOnDelete(true);

            this.Property(x => x.FileNo).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Employee_ExportDetails_Unique", 1) { IsUnique = true }));

            this.Property(x => x.TbName).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Employee_ExportDetails_Unique", 2) { IsUnique = true }));
        }
    }
}
