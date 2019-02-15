namespace Bska.Client.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bskaclientmigration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.AccountDocumentCoding",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        AccountCode = c.String(nullable: false, maxLength: 10),
                        EmployeeId = c.Int(),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("Production.AccountDocumentCoding", t => t.ParentId)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "EmployeeResources.Employee",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        ParentName = c.String(nullable: false, maxLength: 250),
                        Tell = c.String(maxLength: 12),
                        RegisterationNo = c.String(nullable: false, maxLength: 25),
                        BudgetNo = c.Int(nullable: false),
                        WebAddress = c.String(maxLength: 20),
                        Email = c.String(maxLength: 80),
                        Fax = c.String(maxLength: 15),
                        AddressLine = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        Logo = c.Binary(),
                        Province = c.String(nullable: false, maxLength: 2),
                        TwonShip = c.String(nullable: false, maxLength: 4),
                        Zone = c.String(nullable: false, maxLength: 6),
                        City = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "EmployeeResources.AccountDocumentMaster",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AccountDate = c.DateTime(nullable: false),
                        AccountCover = c.String(nullable: false, maxLength: 50),
                        AccountNo = c.String(),
                        Description = c.String(),
                        Creditor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Debtor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EmployeeId = c.Int(),
                        Document_DocumentId = c.Long(),
                        StoreBill_StoreBillId = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("EmployeeResources.Document", t => t.Document_DocumentId)
                .ForeignKey("EmployeeResources.StoreBill", t => t.StoreBill_StoreBillId)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.Document_DocumentId)
                .Index(t => t.StoreBill_StoreBillId);
            
            CreateTable(
                "EmployeeResources.Document",
                c => new
                    {
                        DocumentId = c.Long(nullable: false, identity: true),
                        Desc1 = c.String(maxLength: 100),
                        Desc2 = c.String(maxLength: 250),
                        Desc3 = c.String(maxLength: 250),
                        Desc4 = c.String(maxLength: 100),
                        Transferee = c.String(maxLength: 50),
                        StoreId = c.Int(),
                        DocumentType = c.Int(nullable: false),
                        DocumentDate = c.DateTime(nullable: false),
                        AccountDocumentMaster_ID = c.Int(),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("Production.Store", t => t.StoreId)
                .ForeignKey("EmployeeResources.AccountDocumentMaster", t => t.AccountDocumentMaster_ID)
                .Index(t => t.StoreId)
                .Index(t => t.AccountDocumentMaster_ID);
            
            CreateTable(
                "EmployeeResources.PlaceOfUse",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrganizId = c.Int(nullable: false),
                        StrategtyId = c.Int(nullable: false),
                        PersonId = c.String(maxLength: 20),
                        Num = c.Double(nullable: false),
                        UnitId = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        CommodityId = c.Long(),
                        DocumentId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EmployeeResources.Commodity", t => t.CommodityId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.Document", t => t.DocumentId)
                .Index(t => t.CommodityId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "EmployeeResources.Commodity",
                c => new
                    {
                        AssetId = c.Long(nullable: false, identity: true),
                        Country = c.String(maxLength: 50),
                        Company = c.String(maxLength: 50),
                        StoreAddress = c.String(maxLength: 50),
                        BatchNumber = c.String(),
                        DateOfBirth = c.DateTime(),
                        ExpirationDate = c.DateTime(),
                        UnConsuptionId = c.Long(),
                        StoreBillId = c.Int(),
                        Quality = c.String(nullable: false, maxLength: 1),
                        Num = c.Double(nullable: false),
                        UnitId = c.Int(nullable: false),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 250),
                        KalaUid = c.Int(nullable: false),
                        KalaNo = c.String(nullable: false, maxLength: 50),
                        IndentId = c.Long(),
                        Name = c.String(nullable: false, maxLength: 150),
                        InsertDate = c.DateTime(nullable: false),
                        ModeifiedDate = c.DateTime(nullable: false),
                        SupplierIndent_ID = c.Long(),
                    })
                .PrimaryKey(t => t.AssetId)
                .ForeignKey("EmployeeResources.StoreBill", t => t.StoreBillId, cascadeDelete: true)
                .ForeignKey("Person.SupplierIndent", t => t.SupplierIndent_ID)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.UnConsuptionId)
                .Index(t => t.UnConsuptionId)
                .Index(t => t.StoreBillId)
                .Index(t => t.SupplierIndent_ID);
            
            CreateTable(
                "EmployeeResources.CommodityAssetReserveHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 500),
                        CommodityId = c.Long(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EmployeeResources.Commodity", t => t.CommodityId, cascadeDelete: true)
                .Index(t => t.CommodityId);
            
            CreateTable(
                "Person.Order",
                c => new
                    {
                        OrderId = c.Long(nullable: false, identity: true),
                        PersonId = c.Int(),
                        Status = c.Int(nullable: false),
                        OrderType = c.Int(nullable: false),
                        OrderProcType = c.Int(),
                        OrderDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("Person.Person", t => t.PersonId)
                .Index(t => t.PersonId);
            
            CreateTable(
                "EmployeeResources.MovableAsset",
                c => new
                    {
                        AssetId = c.Long(nullable: false, identity: true),
                        Label = c.Int(),
                        Floor = c.String(maxLength: 2),
                        FloorType = c.Int(),
                        OldLabel = c.Int(),
                        OrganLabel = c.Int(),
                        CurState = c.Int(nullable: false),
                        Uid1 = c.String(maxLength: 50),
                        Uid2 = c.String(maxLength: 50),
                        Uid3 = c.String(maxLength: 50),
                        Uid4 = c.String(maxLength: 50),
                        Desc1 = c.String(maxLength: 50),
                        Desc2 = c.String(maxLength: 50),
                        Desc3 = c.String(maxLength: 50),
                        Desc4 = c.String(maxLength: 50),
                        Desc5 = c.String(maxLength: 50),
                        Desc6 = c.String(maxLength: 50),
                        Desc7 = c.String(maxLength: 50),
                        Desc8 = c.String(maxLength: 50),
                        Desc9 = c.String(maxLength: 50),
                        Desc10 = c.String(maxLength: 50),
                        Desc11 = c.String(maxLength: 50),
                        ISCompietion = c.Int(nullable: false),
                        ISConfirmed = c.Boolean(nullable: false),
                        StoreBillId = c.Int(),
                        Quality = c.String(nullable: false, maxLength: 1),
                        Num = c.Double(nullable: false),
                        UnitId = c.Int(nullable: false),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 250),
                        KalaUid = c.Int(nullable: false),
                        KalaNo = c.String(nullable: false, maxLength: 50),
                        IndentId = c.Long(),
                        Name = c.String(nullable: false, maxLength: 150),
                        InsertDate = c.DateTime(nullable: false),
                        ModeifiedDate = c.DateTime(nullable: false),
                        Image1 = c.Binary(),
                        Image2 = c.Binary(),
                        Image3 = c.Binary(),
                        Image4 = c.Binary(),
                        GuaranteeImage = c.Binary(),
                        ParentMAsset_AssetId = c.Long(),
                        Type = c.Int(),
                    })
                .PrimaryKey(t => t.AssetId)
                .ForeignKey("EmployeeResources.StoreBill", t => t.StoreBillId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.ParentMAsset_AssetId)
                .Index(t => t.StoreBillId)
                .Index(t => t.ParentMAsset_AssetId);
            
            CreateTable(
                "EmployeeResources.AssetProcceding",
                c => new
                    {
                        AssetId = c.Long(nullable: false),
                        ProceedingId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseNumber = c.String(maxLength: 50),
                        AccidentDivanNo = c.String(maxLength: 50),
                        IsOrganFault = c.Boolean(nullable: false),
                        RecipetNo = c.String(maxLength: 50),
                        State = c.Int(nullable: false),
                        TempUid1 = c.String(maxLength: 50),
                        TempUid2 = c.String(maxLength: 50),
                        TempUid3 = c.String(maxLength: 50),
                        TempUid4 = c.String(maxLength: 50),
                        TempDesc1 = c.String(maxLength: 50),
                        TempDesc2 = c.String(maxLength: 50),
                        TempDesc3 = c.String(maxLength: 50),
                        TempDesc4 = c.String(maxLength: 50),
                        TempYear = c.Int(),
                    })
                .PrimaryKey(t => new { t.AssetId, t.ProceedingId })
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.Procceding", t => t.ProceedingId, cascadeDelete: true)
                .Index(t => t.AssetId)
                .Index(t => t.ProceedingId);
            
            CreateTable(
                "EmployeeResources.Procceding",
                c => new
                    {
                        ProceedingId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        ProceedingDate = c.DateTime(nullable: false),
                        ExecutionTime = c.DateTime(),
                        Desc1 = c.String(maxLength: 50),
                        Desc2 = c.String(maxLength: 50),
                        Desc3 = c.String(maxLength: 50),
                        Desc4 = c.String(maxLength: 50),
                        Desc5 = c.String(maxLength: 50),
                        Desc6 = c.String(maxLength: 50),
                        Description = c.String(nullable: false),
                        ProcIdentity = c.Guid(nullable: false),
                        IsSended = c.Boolean(nullable: false),
                        StoreId = c.Int(),
                    })
                .PrimaryKey(t => t.ProceedingId)
                .ForeignKey("Production.Store", t => t.StoreId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "EmployeeResources.ExportDetailsProceeding",
                c => new
                    {
                        ExportID = c.Int(nullable: false),
                        ProceedingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExportID, t.ProceedingId })
                .ForeignKey("EmployeeResources.ExportDetails", t => t.ExportID, cascadeDelete: true)
                .ForeignKey("EmployeeResources.Procceding", t => t.ProceedingId)
                .Index(t => t.ExportID)
                .Index(t => t.ProceedingId);
            
            CreateTable(
                "EmployeeResources.ExportDetails",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TbName = c.String(nullable: false, maxLength: 50),
                        InsertDate = c.DateTime(nullable: false),
                        FileNo = c.String(nullable: false, maxLength: 50),
                        VertifiedNo = c.String(maxLength: 50),
                        State = c.Int(nullable: false),
                        SendType = c.Int(nullable: false),
                        EmployeeId = c.Int(),
                        BillDate = c.DateTime(nullable: false),
                        Directory = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => new { t.FileNo, t.TbName }, unique: true, name: "Employee_ExportDetails_Unique")
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EmployeeResources.ExportDetailsMAsset",
                c => new
                    {
                        ExportID = c.Int(nullable: false),
                        AssetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExportID, t.AssetId })
                .ForeignKey("EmployeeResources.ExportDetails", t => t.ExportID, cascadeDelete: true)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId)
                .Index(t => t.ExportID)
                .Index(t => t.AssetId);
            
            CreateTable(
                "Production.Store",
                c => new
                    {
                        StoreId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 70),
                        StoreType = c.Int(nullable: false),
                        Storage = c.String(maxLength: 150),
                        CreateDate = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 350),
                        StrategyId = c.Int(),
                    })
                .PrimaryKey(t => t.StoreId)
                .ForeignKey("Production.EmployeeDesign", t => t.StrategyId, cascadeDelete: true)
                .Index(t => t.StrategyId);
            
            CreateTable(
                "EmployeeResources.StoreBill",
                c => new
                    {
                        StoreBillId = c.Int(nullable: false, identity: true),
                        StoreBillNo = c.String(nullable: false, maxLength: 20),
                        ArrivalDate = c.DateTime(nullable: false),
                        AcqType = c.Int(nullable: false),
                        Desc1 = c.String(maxLength: 150),
                        Desc2 = c.String(maxLength: 150),
                        Desc3 = c.String(maxLength: 50),
                        StuffType = c.Int(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        StoreId = c.Int(),
                        SellerId = c.Int(),
                        AccountDocumentMaster_ID = c.Int(),
                    })
                .PrimaryKey(t => t.StoreBillId)
                .ForeignKey("Production.Store", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.AccountDocumentMaster", t => t.AccountDocumentMaster_ID)
                .Index(t => t.StoreId)
                .Index(t => t.AccountDocumentMaster_ID);
            
            CreateTable(
                "EmployeeResources.StoreBillEdit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InsertDate = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 250),
                        State = c.Int(nullable: false),
                        StoreBillId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EmployeeResources.StoreBill", t => t.StoreBillId, cascadeDelete: true)
                .Index(t => t.StoreBillId);
            
            CreateTable(
                "Person.SupplierIndent",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Num = c.Double(nullable: false),
                        Remain = c.Double(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        UnitId = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        SellerId = c.Int(),
                        SubOrderId = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("Production.Seller", t => t.SellerId)
                .ForeignKey("Person.SubOrder", t => t.SubOrderId, cascadeDelete: true)
                .Index(t => t.SellerId)
                .Index(t => t.SubOrderId);
            
            CreateTable(
                "EmployeeResources.ReturnIndentRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Description = c.String(maxLength: 500),
                        InsertDate = c.DateTime(nullable: false),
                        EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Production.Seller",
                c => new
                    {
                        SellerId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 70),
                        Lastname = c.String(maxLength: 70),
                        Tell = c.String(maxLength: 20),
                        Coding = c.String(nullable: false, maxLength: 50),
                        Mobile = c.String(maxLength: 12),
                        Province = c.String(nullable: false, maxLength: 100),
                        Zone = c.String(nullable: false, maxLength: 100),
                        City = c.String(nullable: false, maxLength: 100),
                        TownShip = c.String(nullable: false, maxLength: 100),
                        AddressLine = c.String(maxLength: 150),
                        EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.SellerId)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Person.SubOrder",
                c => new
                    {
                        SubOrderId = c.Long(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Num = c.Double(nullable: false),
                        Remain = c.Double(nullable: false),
                        State = c.Int(nullable: false),
                        Identity = c.String(maxLength: 50),
                        UnitId = c.Int(nullable: false),
                        OrderDetailsId = c.Long(),
                    })
                .PrimaryKey(t => t.SubOrderId)
                .ForeignKey("Person.OrderDetails", t => t.OrderDetailsId, cascadeDelete: true)
                .Index(t => t.OrderDetailsId);
            
            CreateTable(
                "Person.OrderDetails",
                c => new
                    {
                        OrderDetialsId = c.Long(nullable: false, identity: true),
                        StuffName = c.String(nullable: false, maxLength: 80),
                        KalaUid = c.Int(nullable: false),
                        kalaNo = c.String(nullable: false, maxLength: 50),
                        StuffType = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Num = c.Double(nullable: false),
                        UnitId = c.Int(nullable: false),
                        UsingLocation = c.String(maxLength: 150),
                        EstimatePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImportantDegree = c.Int(nullable: false),
                        OfferQuality = c.String(maxLength: 1),
                        OfferSpecification = c.String(maxLength: 150),
                        StrategyId = c.Int(),
                        OrganizId = c.Int(),
                        StoreId = c.Int(),
                        StoreDesignId = c.Int(),
                        OrderId = c.Long(),
                        IsReject = c.Boolean(nullable: false),
                        Description = c.String(),
                        BelongingParentLable = c.Long(),
                    })
                .PrimaryKey(t => t.OrderDetialsId)
                .ForeignKey("Person.Order", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "Person.OrderUserHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserDecision = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 300),
                        Identity = c.String(maxLength: 50),
                        IsCurrent = c.Boolean(nullable: false),
                        OrderDetailsId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Person.OrderDetails", t => t.OrderDetailsId, cascadeDelete: true)
                .Index(t => t.OrderDetailsId);
            
            CreateTable(
                "Production.SupplierTrenderOffer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SupplierId = c.Int(nullable: false),
                        ProForma = c.Binary(),
                        ISEnableTrender = c.Boolean(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        SubOrderId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Person.SubOrder", t => t.SubOrderId, cascadeDelete: true)
                .Index(t => t.SubOrderId);
            
            CreateTable(
                "Production.StoreDesign",
                c => new
                    {
                        StoreDesignId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 50),
                        NodeId = c.Int(),
                    })
                .PrimaryKey(t => t.StoreDesignId)
                .ForeignKey("Production.StoreDesign", t => t.NodeId)
                .ForeignKey("Production.Store", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId)
                .Index(t => t.NodeId);
            
            CreateTable(
                "Production.EmployeeDesign",
                c => new
                    {
                        BuidldingDesignId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 50),
                        Code = c.String(maxLength: 50),
                        NodeId = c.Int(),
                        Type = c.Int(),
                    })
                .PrimaryKey(t => t.BuidldingDesignId)
                .ForeignKey("Production.EmployeeDesign", t => t.NodeId)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.NodeId);
            
            CreateTable(
                "Production.Building",
                c => new
                    {
                        BuildingId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Province = c.String(nullable: false, maxLength: 100),
                        Zone = c.String(nullable: false, maxLength: 100),
                        City = c.String(nullable: false, maxLength: 100),
                        TownShip = c.String(nullable: false, maxLength: 100),
                        District = c.String(maxLength: 70),
                        MainStreet = c.String(nullable: false, maxLength: 70),
                        SecondaryStreet = c.String(maxLength: 70),
                        Alley = c.String(maxLength: 70),
                        SecondaryAlley = c.String(maxLength: 70),
                        OldPlaque = c.String(maxLength: 10),
                        NewPlaque = c.String(maxLength: 10),
                        PostalCode = c.String(maxLength: 15),
                        CreateDate = c.DateTime(nullable: false),
                        EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.BuildingId)
                .ForeignKey("Production.EmployeeDesign", t => t.BuildingId, cascadeDelete: true)
                .Index(t => t.BuildingId);
            
            CreateTable(
                "EmployeeResources.Meter",
                c => new
                    {
                        ImAssetId = c.Int(nullable: false, identity: true),
                        SubscriptionNo = c.String(maxLength: 25),
                        AddressLine = c.String(maxLength: 150),
                        PostalCode = c.String(nullable: false, maxLength: 15),
                        Plake = c.String(maxLength: 15),
                        CaseNo = c.String(nullable: false, maxLength: 25),
                        BodyNo = c.String(maxLength: 25),
                        TariffType = c.Int(nullable: false),
                        BuildingId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 50),
                        InsertDate = c.DateTime(nullable: false),
                        ModeifiedDate = c.DateTime(nullable: false),
                        CommonCode = c.String(maxLength: 25),
                        MeterSerialNo = c.String(maxLength: 25),
                        Group = c.String(maxLength: 1),
                        AddressCode = c.String(maxLength: 25),
                        EconomicNo = c.String(maxLength: 25),
                        FamiliesNum = c.Int(),
                        IdentificationNo = c.String(maxLength: 25),
                        EarlyInstallationDate = c.DateTime(),
                        Phase = c.Int(),
                        Amper = c.Double(),
                        Statistic = c.String(maxLength: 2),
                        Factor = c.Double(),
                        WaterSplitDiagonal = c.Double(),
                        WasteSplitDiagonal = c.Double(),
                        MeterStatus = c.String(maxLength: 50),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImAssetId)
                .ForeignKey("Production.Building", t => t.BuildingId, cascadeDelete: true)
                .Index(t => t.BuildingId);
            
            CreateTable(
                "EmployeeResources.MeterBill",
                c => new
                    {
                        MeterBillId = c.Int(nullable: false, identity: true),
                        ImAssetId = c.Int(),
                        NowReadDate = c.DateTime(nullable: false),
                        AgoReadDate = c.DateTime(nullable: false),
                        Year = c.String(maxLength: 10),
                        Mounth = c.String(maxLength: 50),
                        CostEra = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DebtorCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OtehrCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BillRecognition = c.String(nullable: false, maxLength: 25),
                        PayRecognition = c.String(nullable: false, maxLength: 25),
                        PayDateSpace = c.DateTime(nullable: false),
                        PayDate = c.DateTime(nullable: false),
                        BankName = c.String(maxLength: 50),
                        PersonAccountnumber = c.String(maxLength: 20),
                        PursuitNum = c.String(maxLength: 50),
                        Num1 = c.Int(),
                        Num2 = c.Int(),
                        Num3 = c.Int(),
                        Num4 = c.Int(),
                        Num5 = c.Int(),
                        Num6 = c.Int(),
                        DNum1 = c.Decimal(precision: 18, scale: 2),
                        DNum2 = c.Decimal(precision: 18, scale: 2),
                        DNum3 = c.Decimal(precision: 18, scale: 2),
                        DNum4 = c.Decimal(precision: 18, scale: 2),
                        DNum5 = c.Decimal(precision: 18, scale: 2),
                        InsertDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MeterBillId)
                .ForeignKey("EmployeeResources.Meter", t => t.ImAssetId, cascadeDelete: true)
                .Index(t => t.ImAssetId);
            
            CreateTable(
                "EmployeeResources.OrganizationPefectStuff",
                c => new
                    {
                        KalaNo = c.String(nullable: false, maxLength: 50),
                        BuidldingDesignId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.KalaNo, t.BuidldingDesignId })
                .ForeignKey("Production.EmployeeDesign", t => t.BuidldingDesignId, cascadeDelete: true)
                .ForeignKey("Production.Stuff", t => t.KalaNo, cascadeDelete: true)
                .Index(t => t.KalaNo)
                .Index(t => t.BuidldingDesignId);
            
            CreateTable(
                "Production.Stuff",
                c => new
                    {
                        KalaNo = c.String(nullable: false, maxLength: 50),
                        KalaUID = c.Int(nullable: false),
                        GS1 = c.String(maxLength: 16),
                        KalaNme = c.String(nullable: false, maxLength: 300),
                        IsStuff = c.Boolean(nullable: false),
                        Typ = c.Int(nullable: false),
                        FloorOld = c.Int(),
                        FloorNew = c.Int(),
                        ParentId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.KalaNo)
                .ForeignKey("Production.Stuff", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "Production.Unit",
                c => new
                    {
                        UnitId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 300),
                        StuffId = c.Int(nullable: false),
                        MathType = c.Int(nullable: false),
                        MathNum = c.Int(),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.UnitId)
                .ForeignKey("Production.Unit", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "EmployeeResources.AssetTaxCost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaxCostType = c.Int(nullable: false),
                        Description = c.String(maxLength: 500),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ModifiedDate = c.DateTime(nullable: false),
                        AssetId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
            CreateTable(
                "EmployeeResources.Location",
                c => new
                    {
                        LocationId = c.Long(nullable: false, identity: true),
                        AssetId = c.Long(),
                        PersonId = c.String(maxLength: 10),
                        OrganizId = c.Int(nullable: false),
                        StrategyId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        StoreAddressId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        AccountDocumentType = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        MovedRequestDate = c.DateTime(),
                        ReturnDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.LocationId)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
            CreateTable(
                "EmployeeResources.MovableAssetReserveHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 500),
                        AssetId = c.Long(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
            CreateTable(
                "Production.Insurance",
                c => new
                    {
                        InsuranceId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        InsuranceCompany = c.String(nullable: false, maxLength: 50),
                        InsuranceNo = c.String(nullable: false, maxLength: 50),
                        ValidityDate = c.DateTime(nullable: false),
                        Missionary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoDamage = c.String(maxLength: 2),
                        InsurancePolicyImage = c.Binary(),
                        AssetId = c.Long(),
                    })
                .PrimaryKey(t => t.InsuranceId)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.AssetId);
            
            CreateTable(
                "Person.Person",
                c => new
                    {
                        PersonId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 150),
                        FatherName = c.String(maxLength: 50),
                        NationalId = c.String(nullable: false, maxLength: 10),
                        PersonCode = c.String(maxLength: 25),
                        Mobile = c.String(maxLength: 11),
                        Married = c.Boolean(nullable: false),
                        Postalcode = c.String(maxLength: 20),
                        AddressLine = c.String(maxLength: 250),
                        ContractcType = c.Int(nullable: false),
                        BirthDate = c.DateTime(),
                        EmployeeDate = c.DateTime(),
                        Photo = c.Binary(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.PersonId)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.NationalId, unique: true, name: "Person_NationalId_Unique")
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Person.RequestPermit",
                c => new
                    {
                        RequestPermitId = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(),
                        OrganizId = c.Int(nullable: false),
                        StrategyId = c.Int(nullable: false),
                        IsEnable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RequestPermitId)
                .ForeignKey("Person.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => new { t.PersonId, t.OrganizId, t.StrategyId }, unique: true, name: "Person_RequestPermit_Unique");
            
            CreateTable(
                "Person.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        UserName = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 32),
                        PersonId = c.Int(),
                        PermissionType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("Person.Person", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.UserName, unique: true, name: "Users_UserName_Unique")
                .Index(t => t.PersonId);
            
            CreateTable(
                "Person.EventLog",
                c => new
                    {
                        EventLogID = c.Long(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Key = c.String(maxLength: 10),
                        Message = c.String(maxLength: 500),
                        EntryDate = c.DateTime(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.EventLogID)
                .ForeignKey("Person.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "Person.Roles",
                c => new
                    {
                        RolesId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        StoreId = c.Int(),
                        OrganizId = c.Int(),
                        RoleType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RolesId)
                .ForeignKey("Person.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "Person.UserAttribute",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        CanRequestUnConsum = c.Boolean(nullable: false),
                        CanRequestConsum = c.Boolean(nullable: false),
                        CanRequestInConsum = c.Boolean(nullable: false),
                        InternalRequest = c.Boolean(nullable: false),
                        SurplusRequest = c.Boolean(nullable: false),
                        RequestPrint = c.Boolean(nullable: false),
                        RecivedRequestPrint = c.Boolean(nullable: false),
                        ProceedingRequest = c.Boolean(nullable: false),
                        InternalMovedRequest = c.Boolean(nullable: false),
                        CanRequestBelonging = c.Boolean(nullable: false),
                        CanRequestInstallable = c.Boolean(nullable: false),
                        CanChangePassword = c.Boolean(nullable: false),
                        CanEditTrenderRequest = c.Boolean(nullable: false),
                        Atttribute1 = c.Boolean(nullable: false),
                        Atttribute2 = c.Boolean(nullable: false),
                        Atttribute3 = c.Boolean(nullable: false),
                        Atttribute4 = c.Boolean(nullable: false),
                        Atttribute5 = c.Boolean(nullable: false),
                        Atttribute6 = c.Boolean(nullable: false),
                        Atttribute7 = c.Boolean(nullable: false),
                        Atttribute8 = c.Boolean(nullable: false),
                        Atttribute9 = c.Boolean(nullable: false),
                        Atttribute10 = c.Boolean(nullable: false),
                        Atttribute11 = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("Person.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "EmployeeResources.Estate",
                c => new
                    {
                        ImAssetId = c.Int(nullable: false, identity: true),
                        State = c.String(nullable: false, maxLength: 50),
                        RegistryOffice = c.String(nullable: false, maxLength: 50),
                        SectionRecords = c.String(maxLength: 50),
                        AreaRecords = c.String(maxLength: 50),
                        OriginalPlaque = c.String(maxLength: 25),
                        MinorPlaque = c.String(maxLength: 25),
                        Type = c.Int(nullable: false),
                        BookNo = c.String(maxLength: 10),
                        PageNumber = c.String(maxLength: 10),
                        Text = c.String(nullable: false),
                        Address = c.String(nullable: false, maxLength: 250),
                        PostalCode = c.String(maxLength: 10),
                        Area = c.Double(nullable: false),
                        Longitude = c.Single(nullable: false),
                        Latitude = c.Single(nullable: false),
                        EmployeeId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 150),
                        InsertDate = c.DateTime(nullable: false),
                        ModeifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ImAssetId)
                .ForeignKey("EmployeeResources.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Production.BskaHelp",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false),
                        Photo = c.Binary(),
                        FileType = c.String(maxLength: 100),
                        TableItems = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        Identity = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Production.CarDetails",
                c => new
                    {
                        CarDetailsId = c.Int(nullable: false),
                        CarType = c.Int(nullable: false),
                        SystemType = c.String(maxLength: 50),
                        Tipe = c.String(maxLength: 50),
                        Model = c.String(maxLength: 50),
                        CompanyId = c.Int(),
                    })
                .PrimaryKey(t => t.CarDetailsId);
            
            CreateTable(
                "Production.City",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        ID = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "Production.Company",
                c => new
                    {
                        CompanyId = c.Int(nullable: false),
                        CountryId = c.Int(),
                        IsCarCompany = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "Production.Country",
                c => new
                    {
                        CountryId = c.Int(nullable: false),
                        CountryName = c.String(nullable: false, maxLength: 50),
                        CarCorporationId = c.Int(),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "Production.EstateOrgan",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ProvinceId = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Production.InsuranceCompany",
                c => new
                    {
                        InsuranceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.InsuranceId);
            
            CreateTable(
                "Production.Organization",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        BudgetNo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "Production.Province",
                c => new
                    {
                        ProvinceId = c.Int(nullable: false, identity: true),
                        ID = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ProvinceId);
            
            CreateTable(
                "Production.TwonShip",
                c => new
                    {
                        TwonShipId = c.Int(nullable: false, identity: true),
                        ID = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.TwonShipId);
            
            CreateTable(
                "Production.Zone",
                c => new
                    {
                        ZoneId = c.Int(nullable: false, identity: true),
                        ID = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ZoneId);
            
            CreateTable(
                "Person.OrderCommodity",
                c => new
                    {
                        OrderId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderId, t.AssetId })
                .ForeignKey("Person.Order", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.Commodity", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "EmployeeResources.SupplierIndentReturnRequest",
                c => new
                    {
                        ReturnIndentRequstId = c.Int(nullable: false),
                        SupplierIndentId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ReturnIndentRequstId, t.SupplierIndentId })
                .ForeignKey("EmployeeResources.ReturnIndentRequest", t => t.ReturnIndentRequstId, cascadeDelete: true)
                .ForeignKey("Person.SupplierIndent", t => t.SupplierIndentId, cascadeDelete: true)
                .Index(t => t.ReturnIndentRequstId)
                .Index(t => t.SupplierIndentId);
            
            CreateTable(
                "EmployeeResources.StoreBillSupplierIndent",
                c => new
                    {
                        SupplierIndentId = c.Long(nullable: false),
                        StoreBillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SupplierIndentId, t.StoreBillId })
                .ForeignKey("Person.SupplierIndent", t => t.SupplierIndentId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.StoreBill", t => t.StoreBillId, cascadeDelete: true)
                .Index(t => t.SupplierIndentId)
                .Index(t => t.StoreBillId);
            
            CreateTable(
                "Production.StuffUnit",
                c => new
                    {
                        StuffId = c.String(nullable: false, maxLength: 50),
                        UnitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StuffId, t.UnitId })
                .ForeignKey("Production.Stuff", t => t.StuffId, cascadeDelete: true)
                .ForeignKey("Production.Unit", t => t.UnitId, cascadeDelete: true)
                .Index(t => t.StuffId)
                .Index(t => t.UnitId);
            
            CreateTable(
                "Person.OrderMovableAsset",
                c => new
                    {
                        OrderId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderId, t.AssetId })
                .ForeignKey("Person.Order", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.AssetId);
            
            CreateTable(
                "EmployeeResources.DocumentMAsset",
                c => new
                    {
                        DocumentId = c.Long(nullable: false),
                        AssetId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.DocumentId, t.AssetId })
                .ForeignKey("EmployeeResources.Document", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("EmployeeResources.MovableAsset", t => t.AssetId, cascadeDelete: true)
                .Index(t => t.DocumentId)
                .Index(t => t.AssetId);
            
            CreateStoredProcedure(
                "dbo.insert_stuff",
                p => new
                    {
                        KalaNo = p.String(maxLength: 50),
                        KalaUID = p.Int(),
                        GS1 = p.String(maxLength: 16),
                        KalaNme = p.String(maxLength: 300),
                        IsStuff = p.Boolean(),
                        Typ = p.Int(),
                        FloorOld = p.Int(),
                        FloorNew = p.Int(),
                        ParentId = p.String(maxLength: 50),
                    },
                body:
                    @"INSERT [Production].[Stuff]([KalaNo], [KalaUID], [GS1], [KalaNme], [IsStuff], [Typ], [FloorOld], [FloorNew], [ParentId])
                      VALUES (@KalaNo, @KalaUID, @GS1, @KalaNme, @IsStuff, @Typ, @FloorOld, @FloorNew, @ParentId)"
            );
            
            CreateStoredProcedure(
                "dbo.update_stuff",
                p => new
                    {
                        KalaNo = p.String(maxLength: 50),
                        KalaUID = p.Int(),
                        GS1 = p.String(maxLength: 16),
                        KalaNme = p.String(maxLength: 300),
                        IsStuff = p.Boolean(),
                        Typ = p.Int(),
                        FloorOld = p.Int(),
                        FloorNew = p.Int(),
                        ParentId = p.String(maxLength: 50),
                    },
                body:
                    @"UPDATE [Production].[Stuff]
                      SET [KalaUID] = @KalaUID, [GS1] = @GS1, [KalaNme] = @KalaNme, [IsStuff] = @IsStuff, [Typ] = @Typ, [FloorOld] = @FloorOld, [FloorNew] = @FloorNew, [ParentId] = @ParentId
                      WHERE ([KalaNo] = @KalaNo)"
            );
            
            CreateStoredProcedure(
                "dbo.Stuff_Delete",
                p => new
                    {
                        KalaNo = p.String(maxLength: 50),
                        ParentId = p.String(maxLength: 50),
                    },
                body:
                    @"DELETE [Production].[Stuff]
                      WHERE (([KalaNo] = @KalaNo) AND (([ParentId] = @ParentId) OR ([ParentId] IS NULL AND @ParentId IS NULL)))"
            );
            
            CreateStoredProcedure(
                "dbo.Insert_EventLog",
                p => new
                    {
                        Type = p.Int(),
                        Key = p.String(maxLength: 10),
                        Message = p.String(maxLength: 500),
                        EntryDate = p.DateTime(),
                        UserId = p.Int(),
                    },
                body:
                    @"INSERT [Person].[EventLog]([Type], [Key], [Message], [EntryDate], [UserId])
                      VALUES (@Type, @Key, @Message, @EntryDate, @UserId)
                      
                      DECLARE @EventLogID bigint
                      SELECT @EventLogID = [EventLogID]
                      FROM [Person].[EventLog]
                      WHERE @@ROWCOUNT > 0 AND [EventLogID] = scope_identity()
                      
                      SELECT t0.[EventLogID]
                      FROM [Person].[EventLog] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[EventLogID] = @EventLogID"
            );
            
            CreateStoredProcedure(
                "dbo.EventLog_Update",
                p => new
                    {
                        EventLogID = p.Long(),
                        Type = p.Int(),
                        Key = p.String(maxLength: 10),
                        Message = p.String(maxLength: 500),
                        EntryDate = p.DateTime(),
                        UserId = p.Int(),
                    },
                body:
                    @"UPDATE [Person].[EventLog]
                      SET [Type] = @Type, [Key] = @Key, [Message] = @Message, [EntryDate] = @EntryDate, [UserId] = @UserId
                      WHERE ([EventLogID] = @EventLogID)"
            );
            
            CreateStoredProcedure(
                "dbo.EventLog_Delete",
                p => new
                    {
                        EventLogID = p.Long(),
                    },
                body:
                    @"DELETE [Person].[EventLog]
                      WHERE ([EventLogID] = @EventLogID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.EventLog_Delete");
            DropStoredProcedure("dbo.EventLog_Update");
            DropStoredProcedure("dbo.Insert_EventLog");
            DropStoredProcedure("dbo.Stuff_Delete");
            DropStoredProcedure("dbo.update_stuff");
            DropStoredProcedure("dbo.insert_stuff");
            DropForeignKey("Production.AccountDocumentCoding", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("Person.Person", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("EmployeeResources.Estate", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("EmployeeResources.StoreBill", "AccountDocumentMaster_ID", "EmployeeResources.AccountDocumentMaster");
            DropForeignKey("EmployeeResources.AccountDocumentMaster", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("EmployeeResources.Document", "AccountDocumentMaster_ID", "EmployeeResources.AccountDocumentMaster");
            DropForeignKey("EmployeeResources.Document", "StoreId", "Production.Store");
            DropForeignKey("EmployeeResources.DocumentMAsset", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.DocumentMAsset", "DocumentId", "EmployeeResources.Document");
            DropForeignKey("EmployeeResources.PlaceOfUse", "DocumentId", "EmployeeResources.Document");
            DropForeignKey("EmployeeResources.PlaceOfUse", "CommodityId", "EmployeeResources.Commodity");
            DropForeignKey("EmployeeResources.Commodity", "UnConsuptionId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.Commodity", "SupplierIndent_ID", "Person.SupplierIndent");
            DropForeignKey("Person.Users", "PersonId", "Person.Person");
            DropForeignKey("Person.UserAttribute", "UserId", "Person.Users");
            DropForeignKey("Person.Roles", "UserId", "Person.Users");
            DropForeignKey("Person.EventLog", "UserId", "Person.Users");
            DropForeignKey("Person.RequestPermit", "PersonId", "Person.Person");
            DropForeignKey("Person.Order", "PersonId", "Person.Person");
            DropForeignKey("Person.OrderDetails", "OrderId", "Person.Order");
            DropForeignKey("Person.OrderMovableAsset", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("Person.OrderMovableAsset", "OrderId", "Person.Order");
            DropForeignKey("Production.Insurance", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.MovableAsset", "ParentMAsset_AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.MovableAssetReserveHistory", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.Location", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.AssetTaxCost", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.AssetProcceding", "ProceedingId", "EmployeeResources.Procceding");
            DropForeignKey("EmployeeResources.Procceding", "StoreId", "Production.Store");
            DropForeignKey("Production.Store", "StrategyId", "Production.EmployeeDesign");
            DropForeignKey("EmployeeResources.OrganizationPefectStuff", "KalaNo", "Production.Stuff");
            DropForeignKey("Production.StuffUnit", "UnitId", "Production.Unit");
            DropForeignKey("Production.StuffUnit", "StuffId", "Production.Stuff");
            DropForeignKey("Production.Unit", "ParentId", "Production.Unit");
            DropForeignKey("Production.Stuff", "ParentId", "Production.Stuff");
            DropForeignKey("EmployeeResources.OrganizationPefectStuff", "BuidldingDesignId", "Production.EmployeeDesign");
            DropForeignKey("Production.EmployeeDesign", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("Production.EmployeeDesign", "NodeId", "Production.EmployeeDesign");
            DropForeignKey("Production.Building", "BuildingId", "Production.EmployeeDesign");
            DropForeignKey("EmployeeResources.MeterBill", "ImAssetId", "EmployeeResources.Meter");
            DropForeignKey("EmployeeResources.Meter", "BuildingId", "Production.Building");
            DropForeignKey("Production.StoreDesign", "StoreId", "Production.Store");
            DropForeignKey("Production.StoreDesign", "NodeId", "Production.StoreDesign");
            DropForeignKey("Person.SupplierIndent", "SubOrderId", "Person.SubOrder");
            DropForeignKey("Production.SupplierTrenderOffer", "SubOrderId", "Person.SubOrder");
            DropForeignKey("Person.SubOrder", "OrderDetailsId", "Person.OrderDetails");
            DropForeignKey("Person.OrderUserHistory", "OrderDetailsId", "Person.OrderDetails");
            DropForeignKey("EmployeeResources.StoreBillSupplierIndent", "StoreBillId", "EmployeeResources.StoreBill");
            DropForeignKey("EmployeeResources.StoreBillSupplierIndent", "SupplierIndentId", "Person.SupplierIndent");
            DropForeignKey("Person.SupplierIndent", "SellerId", "Production.Seller");
            DropForeignKey("Production.Seller", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("EmployeeResources.SupplierIndentReturnRequest", "SupplierIndentId", "Person.SupplierIndent");
            DropForeignKey("EmployeeResources.SupplierIndentReturnRequest", "ReturnIndentRequstId", "EmployeeResources.ReturnIndentRequest");
            DropForeignKey("EmployeeResources.ReturnIndentRequest", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("EmployeeResources.StoreBillEdit", "StoreBillId", "EmployeeResources.StoreBill");
            DropForeignKey("EmployeeResources.StoreBill", "StoreId", "Production.Store");
            DropForeignKey("EmployeeResources.MovableAsset", "StoreBillId", "EmployeeResources.StoreBill");
            DropForeignKey("EmployeeResources.Commodity", "StoreBillId", "EmployeeResources.StoreBill");
            DropForeignKey("EmployeeResources.AccountDocumentMaster", "StoreBill_StoreBillId", "EmployeeResources.StoreBill");
            DropForeignKey("EmployeeResources.ExportDetailsProceeding", "ProceedingId", "EmployeeResources.Procceding");
            DropForeignKey("EmployeeResources.ExportDetailsProceeding", "ExportID", "EmployeeResources.ExportDetails");
            DropForeignKey("EmployeeResources.ExportDetailsMAsset", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("EmployeeResources.ExportDetailsMAsset", "ExportID", "EmployeeResources.ExportDetails");
            DropForeignKey("EmployeeResources.ExportDetails", "EmployeeId", "EmployeeResources.Employee");
            DropForeignKey("EmployeeResources.AssetProcceding", "AssetId", "EmployeeResources.MovableAsset");
            DropForeignKey("Person.OrderCommodity", "AssetId", "EmployeeResources.Commodity");
            DropForeignKey("Person.OrderCommodity", "OrderId", "Person.Order");
            DropForeignKey("EmployeeResources.CommodityAssetReserveHistory", "CommodityId", "EmployeeResources.Commodity");
            DropForeignKey("EmployeeResources.AccountDocumentMaster", "Document_DocumentId", "EmployeeResources.Document");
            DropForeignKey("Production.AccountDocumentCoding", "ParentId", "Production.AccountDocumentCoding");
            DropIndex("EmployeeResources.DocumentMAsset", new[] { "AssetId" });
            DropIndex("EmployeeResources.DocumentMAsset", new[] { "DocumentId" });
            DropIndex("Person.OrderMovableAsset", new[] { "AssetId" });
            DropIndex("Person.OrderMovableAsset", new[] { "OrderId" });
            DropIndex("Production.StuffUnit", new[] { "UnitId" });
            DropIndex("Production.StuffUnit", new[] { "StuffId" });
            DropIndex("EmployeeResources.StoreBillSupplierIndent", new[] { "StoreBillId" });
            DropIndex("EmployeeResources.StoreBillSupplierIndent", new[] { "SupplierIndentId" });
            DropIndex("EmployeeResources.SupplierIndentReturnRequest", new[] { "SupplierIndentId" });
            DropIndex("EmployeeResources.SupplierIndentReturnRequest", new[] { "ReturnIndentRequstId" });
            DropIndex("Person.OrderCommodity", new[] { "AssetId" });
            DropIndex("Person.OrderCommodity", new[] { "OrderId" });
            DropIndex("EmployeeResources.Estate", new[] { "EmployeeId" });
            DropIndex("Person.UserAttribute", new[] { "UserId" });
            DropIndex("Person.Roles", new[] { "UserId" });
            DropIndex("Person.EventLog", new[] { "UserId" });
            DropIndex("Person.Users", new[] { "PersonId" });
            DropIndex("Person.Users", "Users_UserName_Unique");
            DropIndex("Person.RequestPermit", "Person_RequestPermit_Unique");
            DropIndex("Person.Person", new[] { "EmployeeId" });
            DropIndex("Person.Person", "Person_NationalId_Unique");
            DropIndex("Production.Insurance", new[] { "AssetId" });
            DropIndex("EmployeeResources.MovableAssetReserveHistory", new[] { "AssetId" });
            DropIndex("EmployeeResources.Location", new[] { "AssetId" });
            DropIndex("EmployeeResources.AssetTaxCost", new[] { "AssetId" });
            DropIndex("Production.Unit", new[] { "ParentId" });
            DropIndex("Production.Stuff", new[] { "ParentId" });
            DropIndex("EmployeeResources.OrganizationPefectStuff", new[] { "BuidldingDesignId" });
            DropIndex("EmployeeResources.OrganizationPefectStuff", new[] { "KalaNo" });
            DropIndex("EmployeeResources.MeterBill", new[] { "ImAssetId" });
            DropIndex("EmployeeResources.Meter", new[] { "BuildingId" });
            DropIndex("Production.Building", new[] { "BuildingId" });
            DropIndex("Production.EmployeeDesign", new[] { "NodeId" });
            DropIndex("Production.EmployeeDesign", new[] { "EmployeeId" });
            DropIndex("Production.StoreDesign", new[] { "NodeId" });
            DropIndex("Production.StoreDesign", new[] { "StoreId" });
            DropIndex("Production.SupplierTrenderOffer", new[] { "SubOrderId" });
            DropIndex("Person.OrderUserHistory", new[] { "OrderDetailsId" });
            DropIndex("Person.OrderDetails", new[] { "OrderId" });
            DropIndex("Person.SubOrder", new[] { "OrderDetailsId" });
            DropIndex("Production.Seller", new[] { "EmployeeId" });
            DropIndex("EmployeeResources.ReturnIndentRequest", new[] { "EmployeeId" });
            DropIndex("Person.SupplierIndent", new[] { "SubOrderId" });
            DropIndex("Person.SupplierIndent", new[] { "SellerId" });
            DropIndex("EmployeeResources.StoreBillEdit", new[] { "StoreBillId" });
            DropIndex("EmployeeResources.StoreBill", new[] { "AccountDocumentMaster_ID" });
            DropIndex("EmployeeResources.StoreBill", new[] { "StoreId" });
            DropIndex("Production.Store", new[] { "StrategyId" });
            DropIndex("EmployeeResources.ExportDetailsMAsset", new[] { "AssetId" });
            DropIndex("EmployeeResources.ExportDetailsMAsset", new[] { "ExportID" });
            DropIndex("EmployeeResources.ExportDetails", new[] { "EmployeeId" });
            DropIndex("EmployeeResources.ExportDetails", "Employee_ExportDetails_Unique");
            DropIndex("EmployeeResources.ExportDetailsProceeding", new[] { "ProceedingId" });
            DropIndex("EmployeeResources.ExportDetailsProceeding", new[] { "ExportID" });
            DropIndex("EmployeeResources.Procceding", new[] { "StoreId" });
            DropIndex("EmployeeResources.AssetProcceding", new[] { "ProceedingId" });
            DropIndex("EmployeeResources.AssetProcceding", new[] { "AssetId" });
            DropIndex("EmployeeResources.MovableAsset", new[] { "ParentMAsset_AssetId" });
            DropIndex("EmployeeResources.MovableAsset", new[] { "StoreBillId" });
            DropIndex("Person.Order", new[] { "PersonId" });
            DropIndex("EmployeeResources.CommodityAssetReserveHistory", new[] { "CommodityId" });
            DropIndex("EmployeeResources.Commodity", new[] { "SupplierIndent_ID" });
            DropIndex("EmployeeResources.Commodity", new[] { "StoreBillId" });
            DropIndex("EmployeeResources.Commodity", new[] { "UnConsuptionId" });
            DropIndex("EmployeeResources.PlaceOfUse", new[] { "DocumentId" });
            DropIndex("EmployeeResources.PlaceOfUse", new[] { "CommodityId" });
            DropIndex("EmployeeResources.Document", new[] { "AccountDocumentMaster_ID" });
            DropIndex("EmployeeResources.Document", new[] { "StoreId" });
            DropIndex("EmployeeResources.AccountDocumentMaster", new[] { "StoreBill_StoreBillId" });
            DropIndex("EmployeeResources.AccountDocumentMaster", new[] { "Document_DocumentId" });
            DropIndex("EmployeeResources.AccountDocumentMaster", new[] { "EmployeeId" });
            DropIndex("Production.AccountDocumentCoding", new[] { "ParentId" });
            DropIndex("Production.AccountDocumentCoding", new[] { "EmployeeId" });
            DropTable("EmployeeResources.DocumentMAsset");
            DropTable("Person.OrderMovableAsset");
            DropTable("Production.StuffUnit");
            DropTable("EmployeeResources.StoreBillSupplierIndent");
            DropTable("EmployeeResources.SupplierIndentReturnRequest");
            DropTable("Person.OrderCommodity");
            DropTable("Production.Zone");
            DropTable("Production.TwonShip");
            DropTable("Production.Province");
            DropTable("Production.Organization");
            DropTable("Production.InsuranceCompany");
            DropTable("Production.EstateOrgan");
            DropTable("Production.Country");
            DropTable("Production.Company");
            DropTable("Production.City");
            DropTable("Production.CarDetails");
            DropTable("Production.BskaHelp");
            DropTable("EmployeeResources.Estate");
            DropTable("Person.UserAttribute");
            DropTable("Person.Roles");
            DropTable("Person.EventLog");
            DropTable("Person.Users");
            DropTable("Person.RequestPermit");
            DropTable("Person.Person");
            DropTable("Production.Insurance");
            DropTable("EmployeeResources.MovableAssetReserveHistory");
            DropTable("EmployeeResources.Location");
            DropTable("EmployeeResources.AssetTaxCost");
            DropTable("Production.Unit");
            DropTable("Production.Stuff");
            DropTable("EmployeeResources.OrganizationPefectStuff");
            DropTable("EmployeeResources.MeterBill");
            DropTable("EmployeeResources.Meter");
            DropTable("Production.Building");
            DropTable("Production.EmployeeDesign");
            DropTable("Production.StoreDesign");
            DropTable("Production.SupplierTrenderOffer");
            DropTable("Person.OrderUserHistory");
            DropTable("Person.OrderDetails");
            DropTable("Person.SubOrder");
            DropTable("Production.Seller");
            DropTable("EmployeeResources.ReturnIndentRequest");
            DropTable("Person.SupplierIndent");
            DropTable("EmployeeResources.StoreBillEdit");
            DropTable("EmployeeResources.StoreBill");
            DropTable("Production.Store");
            DropTable("EmployeeResources.ExportDetailsMAsset");
            DropTable("EmployeeResources.ExportDetails");
            DropTable("EmployeeResources.ExportDetailsProceeding");
            DropTable("EmployeeResources.Procceding");
            DropTable("EmployeeResources.AssetProcceding");
            DropTable("EmployeeResources.MovableAsset");
            DropTable("Person.Order");
            DropTable("EmployeeResources.CommodityAssetReserveHistory");
            DropTable("EmployeeResources.Commodity");
            DropTable("EmployeeResources.PlaceOfUse");
            DropTable("EmployeeResources.Document");
            DropTable("EmployeeResources.AccountDocumentMaster");
            DropTable("EmployeeResources.Employee");
            DropTable("Production.AccountDocumentCoding");
        }
    }
}
