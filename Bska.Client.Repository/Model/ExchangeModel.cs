
namespace Bska.Client.Repository.Model
{
    using Bska.Client.Common;
    using System;
    public class ExchangeModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public DateTime ExchangeTime { get; set; }
        public string EmployeeName { get; set; }
    }
}
