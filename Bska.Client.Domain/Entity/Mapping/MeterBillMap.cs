
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity.MeterBills;
    using System.Data.Entity.ModelConfiguration;
    public class MeterBillMap : EntityTypeConfiguration<MeterBill>
    {
        public MeterBillMap()
        {
            this.HasKey(mb => mb.MeterBillId);

            this.Property(mb => mb.Num1).IsOptional();
            this.Property(mb => mb.Num2).IsOptional();
            this.Property(mb => mb.Num3).IsOptional();
            this.Property(mb => mb.Num4).IsOptional();
            this.Property(mb => mb.Num5).IsOptional();
            this.Property(mb => mb.Num6).IsOptional();
            this.Property(mb => mb.DNum1).IsOptional();
            this.Property(mb => mb.DNum2).IsOptional();
            this.Property(mb => mb.DNum3).IsOptional();
            this.Property(mb => mb.DNum4).IsOptional();
            this.Property(mb => mb.DNum5).IsOptional();
            this.Property(mb => mb.AgoReadDate).IsRequired();
            this.Property(mb => mb.BankName).HasMaxLength(50).IsOptional();
            this.Property(mb => mb.BillRecognition).HasMaxLength(25).IsRequired();
            this.Property(mb => mb.CostEra).IsRequired();
            this.Property(mb => mb.DebtorCost).IsRequired();
            this.Property(mb => mb.ImAssetId).IsOptional();
            this.Property(mb => mb.Mounth).HasMaxLength(50).IsOptional();
            this.Property(mb => mb.NowReadDate).IsRequired();
            this.Property(mb => mb.OtehrCost).IsRequired();
            this.Property(mb => mb.PayDate).IsRequired();
            this.Property(mb => mb.PayDateSpace).IsRequired();
            this.Property(mb => mb.PayRecognition).HasMaxLength(25).IsRequired();
            this.Property(mb => mb.PersonAccountnumber).HasMaxLength(20).IsOptional();
            this.Property(mb => mb.PursuitNum).HasMaxLength(50).IsOptional();
            this.Property(mb => mb.TaxCost).IsRequired();
            this.Property(mb => mb.Year).HasMaxLength(10).IsOptional();
            this.Property(mb => mb.InsertDate).IsRequired();
            this.Property(mb => mb.ModifiedDate).IsRequired();

            this.ToTable("EmployeeResources.MeterBill");

            this.HasOptional(x => x.Meter)
                .WithMany(x => x.MeterBills)
                .HasForeignKey(x => x.ImAssetId).WillCascadeOnDelete(true);

            this.Map<PowerBill>(m => m.Requires("Type").HasValue(1))
                .Map<WaterBill>(m => m.Requires("Type").HasValue(2))
                .Map<GasBill>(m => m.Requires("Type").HasValue(3))
                .Map<TellBill>(m => m.Requires("Type").HasValue(4))
                .Map<MobileBill>(m => m.Requires("Type").HasValue(5));
        }
    }
}
