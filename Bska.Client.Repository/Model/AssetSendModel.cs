using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bska.Client.Repository.Model
{
    public class AssetSendModel<T>
    {
        public string EmployeeCode { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
