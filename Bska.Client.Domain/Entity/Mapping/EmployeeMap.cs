
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            this.HasKey(x => x.EmployeeId);

            this.Property(x => x.AddressLine).IsOptional();
            this.Property(x => x.Email).IsOptional().HasMaxLength(80);
            this.Property(x => x.Fax).IsOptional().HasMaxLength(15);
            this.Property(x => x.Name).IsRequired().HasMaxLength(250);
            this.Property(x => x.RegisterationNo).IsRequired().HasMaxLength(25);
            this.Property(x => x.BudgetNo).IsRequired();
            this.Property(x => x.Tell).IsOptional().HasMaxLength(12);
            this.Property(x => x.WebAddress).IsOptional().HasMaxLength(20);
            this.Property(x => x.CreateDate).IsRequired();
            this.Property(x => x.Logo).IsOptional();
            this.Property(x => x.ParentName).IsRequired().HasMaxLength(250);
            this.Property(x => x.Province).IsRequired().HasMaxLength(2);
            this.Property(x => x.TwonShip).IsRequired().HasMaxLength(4);
            this.Property(x => x.Zone).IsRequired().HasMaxLength(6);
            this.Property(x => x.City).IsRequired().HasMaxLength(10);

            this.ToTable("EmployeeResources.Employee");
            this.Property(x => x.AddressLine).HasColumnName("AddressLine");
            this.Property(x => x.Email).HasColumnName("Email");
            this.Property(x => x.EmployeeId).HasColumnName("EmployeeId");
            this.Property(x => x.Fax).HasColumnName("Fax");
            this.Property(x => x.Name).HasColumnName("Name");
            this.Property(x => x.RegisterationNo).HasColumnName("RegisterationNo");
            this.Property(x => x.Tell).HasColumnName("Tell");
            this.Property(x => x.WebAddress).HasColumnName("WebAddress");
            this.Property(x => x.CreateDate).HasColumnName("CreateDate");

            this.HasMany(x => x.Persons)
                .WithOptional(x => x.Employee)
                .HasForeignKey(x => x.EmployeeId)
                .WillCascadeOnDelete(true);
        }
    }
}
