using Bska.Client.Common;
using System;

namespace Bska.Client.UI.Helper
{
    public class UnitModel
    {
        public Int32 UnitId { get; set; }
        public String Name { get; set; }
        public StuffType StuffType { get; set; }
        public String Desctiption { get; set; }
        public UnitMathType MathType { get; set; }
        public Int32? MathNum { get; set; }
        public int? ParentId { get; set; }
    }
}
