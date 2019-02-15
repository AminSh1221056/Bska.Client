
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System;
    using System.Collections.Generic;
    public class Building : Entity
    {
        public Building()
        {
            this.Meters = new List<Meter>();
        }
        public Int32 BuildingId { get; set; }
        public String Name { get; set; }
        public String Province { get; set; }
        public String Zone { get; set; }
        public String City { get; set; }
        public String TownShip { get; set; }
        public String District { get; set; }
        public String MainStreet { get; set; }
        public String SecondaryStreet { get; set; }
        public String Alley { get; set; }
        public String SecondaryAlley { get; set; }
        public String OldPlaque { get; set; }
        public String NewPlaque { get; set; }
        public String PostalCode { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32? EmployeeId { get; set; }
        public virtual ICollection<Meter> Meters { get; private set; }
        public virtual StrategyDesign StrategyDesign { get; set; }
        public override string ToString()
        {
            return string.Format("{0} {1}", this.Name, this.BuildingId);
        }
    }
}
