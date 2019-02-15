
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.ModelConfiguration;
    public class UserMap : EntityTypeConfiguration<Users>
    {
        public UserMap()
        {
            this.HasKey(t => t.UserId);

            this.Property(t => t.FullName).IsRequired().HasMaxLength(100);
            this.Property(t => t.Password).IsRequired().HasMaxLength(32);
            this.Property(t => t.PermissionType).IsRequired();
            this.Property(t => t.UserName).IsRequired().HasMaxLength(50);
            this.Property(t => t.PersonId).IsOptional();

            this.ToTable("Person.Users");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.PermissionType).HasColumnName("PermissionType");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.FullName).HasColumnName("FullName");

            this.Property(x => x.UserName).HasColumnAnnotation(
               IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("Users_UserName_Unique", 1) { IsUnique = true }));
        }
    }
}
