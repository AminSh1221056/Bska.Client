using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bska.Client.UI.Helper
{
    public class AccountDocCodingModel
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public string AccountCode { get; set; }
        public Int32? ParentId { get; set; }
    }
}
