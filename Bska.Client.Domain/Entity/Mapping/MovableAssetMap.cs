
namespace Bska.Client.Domain.Entity.Mapping
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using System.Data.Entity.ModelConfiguration;
    public class MovableAssetMap : EntityTypeConfiguration<MovableAsset>
    {
        public MovableAssetMap()
        {
            this.HasKey(x => x.AssetId);
            
            this.Property(x => x.CurState).IsRequired();
            this.Property(x => x.InsertDate).IsRequired();
            this.Property(x => x.ModeifiedDate).IsRequired();
            this.Property(x => x.Name).IsRequired().HasMaxLength(150);
            this.Property(x => x.Num).IsRequired();
            this.Property(x => x.Cost).IsRequired();
            this.Property(x => x.KalaUid).IsRequired();
            this.Property(x => x.KalaNo).HasMaxLength(50).IsRequired();
            this.Property(x => x.Description).IsOptional().HasMaxLength(250);
            this.Property(x => x.UnitId).IsRequired();
            this.Property(x => x.Label).IsOptional();
            this.Property(x => x.Quality).IsRequired().HasMaxLength(1);
            this.Property(x => x.Floor).IsOptional().HasMaxLength(2);
            this.Property(x => x.FloorType).IsOptional();
            this.Property(x => x.OldLabel).IsOptional();
            this.Property(x => x.OrganLabel).IsOptional();
            this.Property(x => x.Desc1).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc2).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc3).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc4).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc5).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc6).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc7).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc8).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc9).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc10).IsOptional().HasMaxLength(50);
            this.Property(x => x.Desc11).IsOptional().HasMaxLength(50);
            this.Property(x => x.Uid1).IsOptional().HasMaxLength(50);
            this.Property(x => x.Uid2).IsOptional().HasMaxLength(50);
            this.Property(x => x.Uid3).IsOptional().HasMaxLength(50);
            this.Property(x => x.Uid4).IsOptional().HasMaxLength(50);
            this.Property(x => x.ISCompietion).IsRequired();
            this.Property(x => x.ISConfirmed).IsRequired();
            this.Property(x => x.StoreBillId).IsOptional();
            this.Property(x => x.IndentId).IsOptional();
            this.ToTable("EmployeeResources.MovableAsset");

            this.Map<UnConsumption>(m => m.Requires("Type").HasValue(11002))
                 .Map<InCommidity>(m => m.Requires("Type").HasValue(11003))
                  .Map<Installable>(m => m.Requires("Type").HasValue(11004))
                 .Map<Belonging>(m=>m.Requires("Type").HasValue(11005));
        }
    }
}
