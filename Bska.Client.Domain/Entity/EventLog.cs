
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Common;
    using System;
    public class EventLog : Entity
    {
        public Int64 EventLogID { get; set; }

        public EventType Type { get; set; }

        public String Key { get; set; }

        public String Message { get; set; }

        public DateTime EntryDate { get; set; }
        public Int32? UserId { get; set; }
        public virtual Users User { get; set; }
    }
}
