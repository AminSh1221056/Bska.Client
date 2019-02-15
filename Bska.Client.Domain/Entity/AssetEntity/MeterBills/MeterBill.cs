
namespace Bska.Client.Domain.Entity.AssetEntity.MeterBills
{
    using Bska.Client.API.EF6;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System;
    public abstract class MeterBill : Entity, IEquatable<MeterBill>
    {
        public Int32 MeterBillId { get; set; }
        public Int32? ImAssetId { get; set; }
        public DateTime NowReadDate { get; set; }
        public DateTime AgoReadDate { get; set; }
        public String Year { get; set; }
        public String Mounth { get; set; }
        public Decimal CostEra { get; set; }
        public Decimal TaxCost { get; set; }
        public Decimal DebtorCost { get; set; }
        public Decimal OtehrCost { get; set; }
        public String BillRecognition { get; set; }
        public String PayRecognition { get; set; }
        public DateTime PayDateSpace { get; set; }
        public DateTime PayDate { get; set; }
        public String BankName { get; set; }
        public String PersonAccountnumber { get; set; }
        public String PursuitNum { get; set; }
        public Int32? Num1 { get; set; }
        public Int32? Num2 { get; set; }
        public Int32? Num3 { get; set; }
        public Int32? Num4 { get; set; }
        public Int32? Num5 { get; set; }
        public Int32? Num6 { get; set; }
        public Decimal? DNum1 { get; set; }
        public Decimal? DNum2 { get; set; }
        public Decimal? DNum3 { get; set; }
        public Decimal? DNum4 { get; set; }
        public Decimal? DNum5 { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual Meter Meter { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as MeterBill);
        }

        public override int GetHashCode()
        {
            return this.MeterBillId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}--{1} شناسه قبض", this.MeterBillId, this.BillRecognition);
        }

        public bool Equals(MeterBill other)
        {
            if (other == null)
                return base.Equals(other);
            return this.MeterBillId == other.MeterBillId && this.BillRecognition == other.BillRecognition;
        }
    }
}
