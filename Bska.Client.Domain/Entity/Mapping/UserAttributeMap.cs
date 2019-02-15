
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class UserAttributeMap : EntityTypeConfiguration<UserAttribute>
    {
        public UserAttributeMap()
        {
            this.HasKey(x => x.UserId);

            this.Property(x => x.CanRequestConsum).IsRequired();
            this.Property(x => x.CanRequestUnConsum).IsRequired();
            this.Property(x => x.InternalRequest).IsRequired();
            this.Property(x => x.RecivedRequestPrint).IsRequired();
            this.Property(x => x.RequestPrint).IsRequired();
            this.Property(x => x.SurplusRequest).IsRequired();
            this.Property(x => x.CanChangePassword).IsRequired();
            this.Property(x => x.CanRequestBelonging).IsRequired();
            this.Property(x => x.CanRequestInstallable).IsRequired();
            this.Property(x => x.InternalMovedRequest).IsRequired();
            this.Property(x => x.ProceedingRequest).IsRequired();
            this.Property(x => x.CanEditTrenderRequest).IsRequired();

            this.Property(x => x.Atttribute1).IsRequired();
            this.Property(x => x.Atttribute2).IsRequired();
            this.Property(x => x.Atttribute3).IsRequired();
            this.Property(x => x.Atttribute4).IsRequired();
            this.Property(x => x.Atttribute5).IsRequired();
            this.Property(x => x.Atttribute6).IsRequired();
            this.Property(x => x.Atttribute7).IsRequired();
            this.Property(x => x.Atttribute8).IsRequired();
            this.Property(x => x.Atttribute9).IsRequired();
            this.Property(x => x.Atttribute10).IsRequired();
            this.Property(x => x.Atttribute11).IsRequired();

            this.ToTable("Person.UserAttribute");

            this.HasRequired(x => x.User)
                .WithOptional(x => x.UserAttribute)
                .WillCascadeOnDelete(true);
        }
    }
}
