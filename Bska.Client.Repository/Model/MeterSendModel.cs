using Bska.Client.Domain.Entity.AssetEntity.Meters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bska.Client.Repository.Model
{
    public class MeterSendModel
    {
        public string EmployeeCode { get; set; }
        public int Type { get; set; }

        public IEnumerable<Meter> Items { get; set; }
    }
}
