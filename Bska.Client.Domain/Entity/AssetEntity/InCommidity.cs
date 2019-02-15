
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using System;

    public class InCommidity : MovableAsset
    {
        public UnConsumption ToUnconsumpton()
        {
            var unconsumption = new UnConsumption
            {
                Cost = this.Cost,
                CurState = this.CurState,
                Name = this.Name,
                Description = this.Description,
                UnitId = this.UnitId,
                KalaUid = this.KalaUid,
                KalaNo = this.KalaNo,
                Desc1 = this.Desc1,
                Desc2 = this.Desc2,
                Desc3 = this.Desc3,
                Desc4 = this.Desc4,
                Desc5 = this.Desc5,
                Desc6 = this.Desc6,
                Desc7 = this.Desc7,
                Desc8 = this.Desc8,
                Desc9 = this.Desc9,
                Desc10 = this.Desc10,
                Uid1 = this.Uid1,
                Uid2 = this.Uid2,
                Uid3 = this.Uid3,
                Uid4 = this.Uid4,
                FloorType = this.FloorType,
                Floor = this.Floor,
                OldLabel=this.OldLabel,
                Quality = this.Quality,
                Num = 1,
                Label = this.Label,
                ObjectState = this.ObjectState,
                InsertDate = this.InsertDate,
                ModeifiedDate = this.ModeifiedDate,
                ISCompietion = this.ISCompietion,
                Desc11=this.Desc11,
                ISConfirmed=this.ISConfirmed,
                OrganLabel=this.OrganLabel,
            };
            return unconsumption;
        }
    }
}
