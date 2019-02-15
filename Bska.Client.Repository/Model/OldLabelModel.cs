
namespace Bska.Client.Repository.Model
{
    using System;
    using System.Collections.Generic;
    public class OldLabelModel
    {
        public Int32 Label { get; set; }
        public Int32 FloorType { get; set; }
        public String Floor { get; set; }
        public Int32 OldLabel { get; set; }
        public List<string> Floors { get; set; }
    }
}
