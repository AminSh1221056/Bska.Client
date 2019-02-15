
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class RoleMap : EntityTypeConfiguration<Roles>
    {
        public RoleMap()
        {
            this.HasKey(x => x.RolesId);

            this.Property(x => x.UserId).IsOptional();
            this.Property(x => x.StoreId).IsOptional();
            this.Property(x => x.RolesId).IsRequired();
            this.Property(x => x.OrganizId).IsOptional();

            this.ToTable("Person.Roles");

            this.HasOptional(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(true);
        }
    }
}
