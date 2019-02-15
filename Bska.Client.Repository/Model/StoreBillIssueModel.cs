
namespace Bska.Client.Repository.Model
{
    using System;
    public class StoreBillIssueModel
    {
        public int KalaUid { get; set; }
        public string SBNo { get; set; }
        public DateTime Date { get; set; }
        public int Num { get; set; }
        public decimal Price { get; set; }
        public string Transfree { get; set; }
    }
}
