
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.Common;
    using System;
    public class CarDetails 
    {
        public Int32 CarDetailsId { get; set; }
        public CarType CarType { get; set; }
        public String SystemType { get; set; }
        public String Tipe { get; set; }
        public String Model { get; set; }
        public Int32? CompanyId { get; set; }
    }
}
