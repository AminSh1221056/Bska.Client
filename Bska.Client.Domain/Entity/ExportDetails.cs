
namespace Bska.Client.Domain.Entity
{
    using AssetEntity;
    using Bska.Client.API.EF6;
    using Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    public class ExportDetails : Entity,IFormattable
    {
        public ExportDetails()
        {
            this.ExportDetailsProceeding = new List<ExportDetailsProceeding>();
            this.ExportDetailsMAsset = new List<ExportDetailsMAsset>();
        }
        public int ID { get; set; }
        public string TbName { get; set; }
        public DateTime InsertDate { get; set; }
        public string FileNo { get; set; }
        public string VertifiedNo { get; set; }
        public ExportState State { get; set; }
        public ExportType SendType { get; set; }
        public Int32? EmployeeId { get; set; }
        public DateTime BillDate { get; set; }
        [NotMapped]
        public string Description { get; set; }
        public string Directory { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<ExportDetailsProceeding> ExportDetailsProceeding { get; private set; }
        public virtual ICollection<ExportDetailsMAsset> ExportDetailsMAsset { get; private set; }
        public override string ToString()
        {
            return string.Format("{0} مربوط به {1}", TbName, ID);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString();
            }
            string formatUpper = format.ToUpper();
            switch (formatUpper)
            {
                case "DESC":
                    return setDescritpionFormat();
                default:
                    return ToString();
            }
        }

        private string setDescritpionFormat()
        {
            string desc = "";
            if (this.ExportDetailsMAsset.Count > 0)
            {
                var firstAsset = this.ExportDetailsMAsset.OrderBy(x => x.MAsset.Label).First();
                var lastAsset = this.ExportDetailsMAsset.OrderByDescending(x => x.MAsset.Label).First();
                desc = $"از تاریخ {firstAsset.MAsset.InsertDate.PersianDateString()} تا {BillDate.PersianDateString()}...از برچسب {firstAsset.MAsset.Label} تا {lastAsset.MAsset.Label}";
            }
            else if (this.ExportDetailsProceeding.Count > 0)
            {
                desc = $"تعداد مال داخل کل صورت جلسه های ارسالی: {ExportDetailsProceeding.Sum(v=>v.Proceeding.AssetProceedings.Count())}";
            }
            else
            {
                desc = "هیچ مالی یافت نشد";
            }
            return desc;
        }
    }
}
