
namespace Bska.Client.Repository.Model
{
    using System;
    using Common;
    public class AccessUploadAssetModel
    {
        public Boolean IsSelected { get; set; }
        public int kalauid { get; set; }
        public string Name { get; set; }
        public int lable { get; set; }
        public string anbrsdno { get; set; }
        public string anbhvlno { get; set; }
        public int acqtyp { get; set; }
        public int Year { get; set; }
        public int Curstate { get; set; }
        public MAssetCurState CurState { get; set; }
        public string Desc { get; set; }
        public decimal Cost { get; set; }
        public string Uid1 { get; set; }
        public string Uid2 { get; set; }
        public string Uid3 { get; set; }
        public string Uid4 { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }
        public string couid { get; set; }
    }
}
