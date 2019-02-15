
namespace Bska.Client.Domain.Entity.Mapping
{
    using System.Data.Entity.ModelConfiguration;
    public class EventLogMap : EntityTypeConfiguration<EventLog>
    {
        public EventLogMap()
        {
            this.HasKey(ev => ev.EventLogID);

            this.Property(ev => ev.Type).IsRequired();
            this.Property(ev => ev.Key).IsOptional().HasMaxLength(10);
            this.Property(ev => ev.Message).IsOptional().HasMaxLength(500);
            this.Property(ev => ev.EntryDate).IsRequired();

            this.ToTable("Person.EventLog");
            this.HasOptional(ev => ev.User).WithMany(u => u.EventLogs)
                .HasForeignKey(ev => ev.UserId).WillCascadeOnDelete(true);

            this.MapToStoredProcedures(s => s.Insert(st => st.HasName("Insert_EventLog")));
        }
    }
}
