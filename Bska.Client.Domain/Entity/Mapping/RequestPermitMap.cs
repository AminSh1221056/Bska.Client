
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;
    public class RequestPermitMap : EntityTypeConfiguration<RequestPermit>
    {
        public RequestPermitMap()
        {
            this.HasKey(x => x.RequestPermitId);

            this.Property(x => x.PersonId).IsOptional();
            this.Property(x => x.OrganizId).IsRequired();
            this.Property(x => x.StrategyId).IsRequired();
            this.Property(x => x.IsEnable).IsRequired();

            this.Ignore(x => x.StragegyName);
            this.Ignore(x => x.OrganizName);

            this.ToTable("Person.RequestPermit");

            this.Property(x => x.IsEnable).HasColumnName("IsEnable");
            this.Property(x => x.OrganizId).HasColumnName("OrganizId");
            this.Property(x => x.PersonId).HasColumnName("PersonId");
            this.Property(x => x.RequestPermitId).HasColumnName("RequestPermitId");
            this.Property(x => x.StrategyId).HasColumnName("StrategyId");

            this.Property(x => x.PersonId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Person_RequestPermit_Unique", 1) { IsUnique = true }));

            this.Property(x => x.OrganizId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Person_RequestPermit_Unique", 2) { IsUnique = true }));

            this.Property(x => x.StrategyId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Person_RequestPermit_Unique", 3) { IsUnique = true }));
        }
    }
}
