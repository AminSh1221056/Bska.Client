
using System.Collections.Generic;

namespace Bska.Client.UI.Helper.SendingModels
{
    public class GlobalExchangeModel<T, TM>
    {
        public GlobalExchangeModel(IEnumerable<T> item1, IEnumerable<TM> item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public IEnumerable<T> Item1 { get; set; }
        public IEnumerable<TM> Item2 { get; set; }
    }
}
