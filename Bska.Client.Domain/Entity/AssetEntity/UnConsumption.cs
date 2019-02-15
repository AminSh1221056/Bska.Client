
namespace Bska.Client.Domain.Entity.AssetEntity
{
    using System;
    using System.Collections.Generic;

    public class UnConsumption : MovableAsset
    {
        public UnConsumption()
        {
            Belongings = new List<Belonging>();
            Insurances = new List<Insurance>();
            AccountDocumentDetails = new List<AccountDocumentDetails>();
            Commodities = new List<Commodity>();
        }

        public UnConsumption(UnConsumption entity)
        {
            Belongings = new List<Belonging>();
            Insurances = new List<Insurance>();
            AccountDocumentDetails = new List<AccountDocumentDetails>();
            Commodities = new List<Commodity>();

            this.Name = entity.Name;
            this.Description = entity.Description;
            this.UnitId = entity.UnitId;
            this.KalaUid = entity.KalaUid;
            this.KalaNo = entity.KalaNo;
            this.CurState = entity.CurState;
            this.Floor = entity.Floor;
            this.FloorType = entity.FloorType;
            this.Desc1 = entity.Desc1;
            this.Desc2 = entity.Desc2;
            this.Desc3 = entity.Desc3;
            this.Desc4 = entity.Desc4;
            this.Desc5 = entity.Desc5;
            this.Desc6 = entity.Desc6;
            this.Desc7 = entity.Desc7;
            this.Desc8 = entity.Desc8;
            this.Desc9 = entity.Desc9;
            this.Desc10 = entity.Desc10;
            this.Desc11 = entity.Desc11;
            this.OrganLabel = entity.OrganLabel;
            this.Uid1 = entity.Uid1;
            this.Uid2 = entity.Uid2;
            this.Uid3 = entity.Uid3;
            this.Uid4 = entity.Uid4;
            this.Cost = entity.Cost;
            this.Quality = entity.Quality;
            this.Num =entity.Num;
            this.Label = entity.Label;
            this.ObjectState = entity.ObjectState;
            this.InsertDate = entity.InsertDate;
            this.ModeifiedDate = entity.ModeifiedDate;
            this.ISCompietion =entity.ISCompietion;
            this.ISConfirmed = entity.ISConfirmed;
            this.Image1 = entity.Image1;
            this.Image2 = entity.Image2;
            this.Image3 = entity.Image3;
            this.Image4 = entity.Image4;
            this.GuaranteeImage = entity.GuaranteeImage;
        }

        public Byte[] Image1 { get; set; }
        public Byte[] Image2 { get; set; }
        public Byte[] Image3 { get; set; }
        public Byte[] Image4 { get; set; }
        public Byte[] GuaranteeImage { get; set; }
        public virtual ICollection<Belonging> Belongings { get; private set; }
        public virtual ICollection<Insurance> Insurances { get; private set; }
        public virtual ICollection<AccountDocumentDetails> AccountDocumentDetails { get; private set; }
        public virtual ICollection<Commodity> Commodities { get; private set; }
        public InCommidity ToInCommidity()
        {
            var inCommodity = new InCommidity
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
                Desc11 = this.Desc11,
                Uid1 = this.Uid1,
                Uid2 = this.Uid2,
                Uid3 = this.Uid3,
                Uid4 = this.Uid4,
                FloorType=this.FloorType,
                Floor=this.Floor,
                OldLabel=this.OldLabel,
                Quality = this.Quality,
                Num = 1,
                Label = null,
                ObjectState = this.ObjectState,
                InsertDate = this.InsertDate,
                ModeifiedDate =this.ModeifiedDate,
                ISCompietion = this.ISCompietion,
                ISConfirmed = this.ISConfirmed,
                OrganLabel=this.OrganLabel,
            };
            return inCommodity;
        }
    }
}
