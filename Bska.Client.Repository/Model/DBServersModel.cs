
namespace Bska.Client.Repository.Model
{
    using System;
    public class DBServersModel
    {
        public string Organization { get; set; }
        public string DatabaseName { get; set; }
        public bool IsCurrent { get; set; }
    }
}
