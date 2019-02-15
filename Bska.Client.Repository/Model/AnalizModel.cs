
using Bska.Client.Common;

namespace Bska.Client.Repository.Model
{
    public class AnalizModel
    {
        public AnalizModel()
        {
            Identity = AnalizModelIdentity.Stock;
            OrderStatus = OrderStatus.None;
        }
        public string Description { get; set; }
        public double Num { get; set; }
        public string UnitName { get; set; }
        public AnalizModelIdentity Identity { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
