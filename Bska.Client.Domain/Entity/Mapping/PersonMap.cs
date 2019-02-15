
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;
    public class PersonMap : EntityTypeConfiguration<Person>
    {
        public PersonMap()
        {
            this.HasKey(x => x.PersonId);

            this.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            this.Property(x => x.LastName).IsRequired().HasMaxLength(150);
            this.Property(x => x.Mobile).IsOptional().HasMaxLength(11);
            this.Property(x => x.NationalId).IsRequired().HasMaxLength(10);
            this.Property(x => x.PersonCode).IsOptional().HasMaxLength(25);
            this.Property(x => x.ContractcType).IsRequired();
            this.Property(x => x.CreateDate).IsRequired();
            this.Property(x => x.ModifiedDate).IsRequired();
            this.Property(x => x.EmployeeId).IsOptional();
            this.Property(x => x.AddressLine).IsOptional().HasMaxLength(250);
            this.Property(x => x.FatherName).IsOptional().HasMaxLength(50);
            this.Property(x => x.Married).IsRequired();
            this.Property(x => x.Postalcode).IsOptional().HasMaxLength(20);
            this.Property(x => x.BirthDate).IsOptional();
            this.Property(x => x.EmployeeDate).IsOptional();
            this.Property(x => x.Photo).IsOptional();
            this.ToTable("Person.Person");
            this.Property(x => x.PersonId).HasColumnName("PersonId");
            this.Property(x => x.FirstName).HasColumnName("FirstName");
            this.Property(x => x.LastName).HasColumnName("LastName");
            this.Property(x => x.Mobile).HasColumnName("Mobile");
            this.Property(x => x.NationalId).HasColumnName("NationalId");
            this.Property(x => x.PersonCode).HasColumnName("PersonCode");
            this.Property(x => x.CreateDate).HasColumnName("CreateDate");
            this.Property(x => x.ModifiedDate).HasColumnName("ModifiedDate");

            this.HasMany(u => u.Users).WithOptional(p => p.Person)
                .HasForeignKey(p => p.PersonId).WillCascadeOnDelete(true);

            this.HasMany(r => r.RequestPermit).WithOptional(p => p.Person)
                .HasForeignKey(p => p.PersonId).WillCascadeOnDelete(true);

            this.HasMany(o => o.Orders).WithOptional(p => p.Person)
                .HasForeignKey(p => p.PersonId).WillCascadeOnDelete(false);

            this.Property(x => x.NationalId).HasColumnAnnotation(
                IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Person_NationalId_Unique") { IsUnique = true }));
        }
    }
}
