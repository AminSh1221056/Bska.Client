
namespace Bska.Client.Domain.Concrete
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System.Data.Entity;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.Domain.Entity.Mapping;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using Bska.Client.Domain.Entity.AssetEntity.MeterBills;
    using Entity.AssetEntity.CommodityAsset;
    using Bska.Client.Domain.Entity.HelpEntity;

    public partial class BskaContext : DataContext
    {
        static BskaContext()
        {
            Database.SetInitializer<BskaContext>(new BskaDBInitializer());
        }

        public BskaContext() : base(ConnectionHelper.CreateConnectionString()) { }
        public DbSet<Stuff> Stuffs { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Users> Users { get; private set; }
        public DbSet<RequestPermit> RequestPermit { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserAttribute> UserAttributes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<SubOrder> SubOrders { get; set; }
        public DbSet<OrderUserHistory> OrderUserHistories { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<EmployeeDesign> EmployeeDesigns { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreDesign> StoreDesigns { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<StoreBill> StoreBills { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<MovableAsset> MovableAsset { get; set; }
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<Estate> Estates { get; set; }
        public DbSet<Proceeding> Proceedings { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<MeterBill> MeterBills { get; set; }
        public DbSet<AssetProceeding> AssetProceedings { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<AssetTaxCost> AssetTaxCosts { get; set; }
        public DbSet<ExportDetails> ExportDetails { get; set; }
        public DbSet<ExportDetailsMAsset> ExportDetailsMAssets { get; set; }
        public DbSet<ExportDetailsProceeding> ExportDetailsProceedings { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<SupplierIndent> SupplierIndents { get; set; }
        public DbSet<AccountDocumentCoding> AccountDocumentCodings { get; set; }
        public DbSet<AccountDocumentMaster> AccountDocumentMasters { get; set; }
        public DbSet<AccountDocumentDetails> AccountDocumentDetails { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<PlaceOfUse> PlaceOfUses { get; set; }
        public DbSet<CarDetails> CarDetails { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<TwonShip> TwonShips { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<EstateOrgan> EstateOrgans { get; set; }
        public DbSet<StoreBillEdit> StoreBillEdit { get; set; }
        public DbSet<ReturnIndentRequest> ReturnIndentRequsts { get; set; }
        public DbSet<MovableAssetReserveHistory> MovableAssetReserveHistories { get; set; }
        public DbSet<CommodityAssetReserveHistory> CommodityAssetReserveHistories { get; set; }
        public DbSet<OrganizationPefectStuff> OrganizationPerfectStuffs { get; set; }
        public DbSet<SupplierTrenderOffer> SupplierTrenderOffers { get; set; }
        public DbSet<BskaHelp> BskaHelps { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new StuffMap());
            modelBuilder.Configurations.Add(new UnitMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new PersonMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RequestPermitMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserAttributeMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderDetailsMap());
            modelBuilder.Configurations.Add(new OrderHistoryMap());
            modelBuilder.Configurations.Add(new SubOrderMap());
            modelBuilder.Configurations.Add(new BuildingMap());
            modelBuilder.Configurations.Add(new EmployeeDesignMap());
            modelBuilder.Configurations.Add(new StoreMap());
            modelBuilder.Configurations.Add(new StoreDesignMap());
            modelBuilder.Configurations.Add(new DocumentMap());
            modelBuilder.Configurations.Add(new StoreBillMap());
            modelBuilder.Configurations.Add(new LocationMap());
            modelBuilder.Configurations.Add(new MovableAssetMap());
            modelBuilder.Configurations.Add(new EstateMap());
            modelBuilder.Configurations.Add(new ProceedingMap());
            modelBuilder.Configurations.Add(new MeterMap());
            modelBuilder.Configurations.Add(new MeterBillMap());
            modelBuilder.Configurations.Add(new AssetProceedingMap());
            modelBuilder.Configurations.Add(new EventLogMap());
            modelBuilder.Configurations.Add(new ExportDetailsMap());
            modelBuilder.Configurations.Add(new AssetTaxCostMap());
            modelBuilder.Configurations.Add(new ExportDetailsMAssetMap());
            modelBuilder.Configurations.Add(new ExportDetailsProceedingMap());
            modelBuilder.Configurations.Add(new SellerMap());
            modelBuilder.Configurations.Add(new SupplierIndentMap());
            modelBuilder.Configurations.Add(new AccountDocumentCodingMap());
            modelBuilder.Configurations.Add(new AccountDocumentMasterMap());
            modelBuilder.Configurations.Add(new AccountDocumentDetailsMap());
            modelBuilder.Configurations.Add(new InsuranceMap());
            modelBuilder.Configurations.Add(new CommodityMap());
            modelBuilder.Configurations.Add(new PlaceOfUseMap());
            modelBuilder.Configurations.Add(new CarDetailsMap());
            modelBuilder.Configurations.Add(new CityMap());
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new CountryMap());
            modelBuilder.Configurations.Add(new InsuranceCompanyMap());
            modelBuilder.Configurations.Add(new OrganizationMap());
            modelBuilder.Configurations.Add(new ProvinceMap());
            modelBuilder.Configurations.Add(new TwonShipMap());
            modelBuilder.Configurations.Add(new ZoneMap());
            modelBuilder.Configurations.Add(new EstateOrganMap());
            modelBuilder.Configurations.Add(new StoreBillEditMap());
            modelBuilder.Configurations.Add(new ReturnIndentRequestMap());
            modelBuilder.Configurations.Add(new OrganizationPefectStuffMap());
            modelBuilder.Configurations.Add(new MovableAssetReserveHistoryMap());
            modelBuilder.Configurations.Add(new CommodityAssetReserveHistoryMap());
            modelBuilder.Configurations.Add(new SupplierTrenderOfferMap());
            modelBuilder.Configurations.Add(new BskaHelpMap());
        }
        public void Seed(BskaContext context)
        {
            var newPerson = new Person
            {
                FirstName = "@",
                ContractcType = ContractType.None,
                CreateDate = GlobalClass._Today,
                ModifiedDate = GlobalClass._Today,
                Employee = null,
                LastName = "Administrator",
                NationalId = "0000000000",
                ObjectState = ObjectState.Added,
            };

            newPerson.Users.Add(new Users
            {
                FullName = "@Administrator",
                ObjectState = ObjectState.Added,
                PermissionType = PermissionsType.Manager,
                Password = GlobalClass.GetMd5Hash("Manager"),
                UserName = "Manager",
            });

            var newStore = new Store
            {
                StoreType = StoreType.Mixed,
                Description = "انبار اصلی که می تواند فقط اموال غیرمصرفی را دریافت کند",
                CreateDate = GlobalClass._Today,
                Name = "انبار اصلی",
                ObjectState = ObjectState.Added,
            };

            newStore.StoreDesign.Add(new Domain.Entity.StoreDesign
            {
                Name = newStore.Name,
                ObjectState = ObjectState.Added,
            });

            var newStore1 = new Store
            {
                StoreType = StoreType.Retiring,
                Description = "انبار اسقاط برای کالاهای معیوب",
                CreateDate = GlobalClass._Today,
                Name = "انبار اسقاط",
                ObjectState = ObjectState.Added,
            };
            newStore1.StoreDesign.Add(new Domain.Entity.StoreDesign
            {
                Name = newStore1.Name,
                ObjectState = ObjectState.Added,
            });

            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [Role_Unique_Organiz] ON [Person].[Roles]([OrganizId] ASC)WHERE ([OrganizId] IS NOT NULL)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [Label_Unique_Index] ON [EmployeeResources].[MovableAsset]([Label],[Type] ASC)WHERE ([Label] IS NOT NULL And [Type] In(11002,11004,11005))WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [StoreBill_Unique_Index] ON [EmployeeResources].[StoreBill] ([StoreBillNo],[ArrivalDate],[AcqType] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [LocationStatus_Unique_Index] ON [EmployeeResources].[Location]([AssetId] ASC)WHERE ([Status] In (2,3,4,8,9,10,12,13,14))WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [FloorOldLabel_Unique_Index] ON [EmployeeResources].[MovableAsset] ([Floor],[FloorType],[OldLabel] ASC) WHERE ([Floor] IS NOT NULL AND [FloorType] IS NOT NULL AND [OldLabel] IS NOT NULL) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [MAssetProceeding_Unique_Index] ON [EmployeeResources].[AssetProcceding] ([AssetId],[State] ASC) WHERE ([State] IN (1)) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [AccountDocumentCoding_Unique_Index] ON [Production].[AccountDocumentCoding] ([AccountCode] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [CommodityReserveHistory_Unique_Index] ON [EmployeeResources].[CommodityAssetReserveHistory]([Status],[CommodityId] ASC) WHERE ([Status] In (1,2) AND [CommodityId] IS NOT NULL)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX [MovableAssetReserveHistory_Unique_Index] ON [EmployeeResources].[MovableAssetReserveHistory]([Status],[AssetId] ASC) WHERE ([Status] In (1,2) AND [AssetId] IS NOT NULL)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[StuffType] AS TABLE([KalaUID][int] NOT NULL,[KalaNo][nvarchar](50) Not Null,[GS1][nvarchar](16) Null,[KalaNme][nvarchar](150) NOT NULL,[IsStuff][bit] NOT NULL,[Typ][int] NOT NULL,[FloorOld][int] NULL,[FloorNew][int] NULL,[ParentId][int] NULL)");
            context.Database.ExecuteSqlCommand("Create TYPE [dbo].[UnitType] AS TABLE([UnitId][int] NOT NULL,[Name][nvarchar](150) NOT NULL,[StuffId][int] NOT NULL,[MathNum][int] NULL,[MathType][int] Not NULL,[ParentId][int] NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[DepricationType] AS TABLE([StuffId][int] NOT NULL,[DepricationNum][int] NOT NULL,[Type][int] NOT NULL,[FactorLowest][int] NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[CountryType] AS TABLE([CountryId] [int] NOT NULL,[CountryName][nvarchar](50) NOT NULL,[CarCorporationId][int] NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[CompanyType] AS TABLE([CompanyId] [int] NOT NULL,[Name][nvarchar](50) NOT NULL,[CountryId][int] NULL,[IsCarCompany][bit] Not Null)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[InsuranceCompanyType] AS TABLE([InsuranceId][int] NOT NULL,[Name][nvarchar](50) NOT NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[CarDetailsType] AS TABLE([CarDetailsId][int] NOT NULL,[CarType][int] NOT NULL,[SystemType][nvarchar](50) NULL,[Tipe][nvarchar](50) NULL,[Model][nvarchar](50) NULL,[CompanyId][int] NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[OrganizationModelType] AS TABLE([EmployeeId][int] NOT NULL,[Name][nvarchar](50) NOT NULL,[BudgetNo][int] NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[ProvinceType] AS TABLE([ID][nvarchar](50) NOT NULL,[Name][nvarchar](50) NOT NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[EstateOrganType] AS TABLE([Name][nvarchar](50) NOT NULL,[ProvinceId][nvarchar](50) NOT NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[BskaHelpType] AS TABLE([Id][int] NOT NULL,[Title][nvarchar](100) NOT NULL, [Description][nvarchar](max) NOT NULL,[FileType][nvarchar](100) NULL,[InsertDate][datetime] NOT NULL,[Identity][nvarchar](50) NOT NULL,[Photo][varbinary](max) NULL,[TableItems] [nvarchar](max)  NULL)");
            context.Database.ExecuteSqlCommand("CREATE TYPE [dbo].[AccountCodingType] AS TABLE([Id] [int] NOT NULL,[Name] [nvarchar](250) NOT NULL,[AccountCode] [nvarchar](10) NOT NULL,[EmployeeId] [int] NULL,[ParentId] [int] NULL)");

            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[delete_allStuffs] AS BEGIN Delete [Production].[Stuff] End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[MAssetUpdate_Confirmed] AS BEGIN UPDATE [EmployeeResources].MovableAsset Set ISConfirmed=1 Where (ISConfirmed=0) END");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[MAssetUpdate_Compietion] @Typ [int]  AS BEGIN UPDATE [EmployeeResources].MovableAsset Set ISCompietion=1 Where (Type=@Typ) END");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[MAsset_Delete] @AssetId [bigInt],@AssetType [int],@label [int] = Null AS BEGIN Delete [EmployeeResources].MovableAsset Where AssetId=@AssetId  If(@label is Not Null) Exec [dbo].[MAssetLabel_Update] @label,@AssetType END");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[MAssetLabel_Update] @label [int],@AssetType [int] AS BEGIN UPDATE [EmployeeResources].MovableAsset Set Label=Label-1 where Label>@label AND [Type]=@AssetType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[StuffInsert] (@stuffType StuffType  READONLY) AS begin SET NOCOUNT OFF; MERGE INTO[Production].[Stuff] b USING @stuffType a ON b.KalaUID=a.KalaUID And b.KalaNo=a.KalaUID WHEN MATCHED Then update set b.[KalaUID]= a.KalaUID, b.[KalaNo]= a.KalaNo, b.[GS1]= a.GS1, b.[KalaNme]= a.KalaNme, b.[IsStuff]= a.IsStuff, b.[Typ]= a.Typ,b.[FloorOld]= a.FloorOld, b.[FloorNew]= a.FloorNew, b.[ParentId]= a.ParentId WHEN NOT MATCHED THEN INSERT Values([KalaUID],[KalaNo],[GS1], [KalaNme], [IsStuff], [Typ],[FloorOld], [FloorNew], [ParentId]); end");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[UnitInsert](@unitType UnitType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[Unit]([UnitId], [Name], [StuffId], [MathType], [MathNum], [ParentId]) Select* from @unitType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[DepricationInsert](@depricationType DepricationType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[StuffDeprication]([StuffId], [DepricationNum], [Type], [FactorLowest]) Select* from @depricationType End");
            context.Database.ExecuteSqlCommand("Create PROCEDURE [dbo].[CountryInsert](@CountryType CountryType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[Country]([CountryId],[CountryName], [CarCorporationId]) Select* from @CountryType End");
            context.Database.ExecuteSqlCommand("Create PROCEDURE [dbo].[CompanyInsert](@companyType CompanyType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[Company]([CompanyId],[Name], [CountryId], [IsCarCompany]) Select* from @companyType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[InsuranceCompanyInsert](@InsuranceCompanyType InsuranceCompanyType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[InsuranceCompany]([InsuranceId],[Name]) Select* from @InsuranceCompanyType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[CarDetailsInsert](@CarDetailsType CarDetailsType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[CarDetails]([CarDetailsId],[CarType], [SystemType],[Tipe],[Model],[CompanyId]) Select* from @CarDetailsType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[OrganizationModelInsert](@OrganizationModelType OrganizationModelType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[Organization]([EmployeeId],[Name], [BudgetNo]) Select* from @OrganizationModelType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[ProvinceInsert](@ProvinceType ProvinceType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[Province]([ID], [Name]) Select* from @ProvinceType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[TwonShipInsert](@ProvinceType ProvinceType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[TwonShip]([ID], [Name]) Select* from @ProvinceType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[ZoneInsert](@ProvinceType ProvinceType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[Zone]([ID], [Name]) Select* from @ProvinceType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[CityInsert](@ProvinceType ProvinceType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[City]([ID], [Name]) Select* from @ProvinceType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[EstateOrganInsert](@estateOrganType EstateOrganType  READONLY) AS begin SET NOCOUNT OFF;INSERT [Production].[EstateOrgan]([Name],[ProvinceId]) Select* from @estateOrganType End");
            context.Database.ExecuteSqlCommand("CREATE PROCEDURE [dbo].[DeleteAll_Assets] As BEGIN delete EmployeeResources.ExportDetails delete EmployeeResources.StoreBill delete EmployeeResources.AccountDocumentMaster delete EmployeeResources.Document END");
            context.Database.ExecuteSqlCommand("Create PROCEDURE [dbo].[UpdateStuffOnUpdateFromServer]AS begin SET NOCOUNT OFF; update Production.Stuff set IsStuff = 0 from(SELECT p.KalaNo as kno FROM Production.Stuff p WHERE EXISTS(SELECT 1 FROM Production.Stuff c WHERE c.ParentId = p.KalaNo) And p.IsStuff = 1) as v where KalaNo = v.kno End");
            context.Database.ExecuteSqlCommand("Create PROCEDURE [dbo].[BskaHelpExchange](@BskaHelp BskaHelpType READONLY) AS begin SET NOCOUNT OFF; MERGE INTO Production.BskaHelp b USING @BskaHelp a ON b.Id = a.Id WHEN MATCHED Then update  set b.Title = a.Title, b.Description = a.Description, b.Photo = a.Photo,b.FileType = a.FileType, b.InsertDate = a.InsertDate, b.[Identity] = a.[Identity],b.TableItems=a.TableItems WHEN NOT MATCHED THEN INSERT(Title,Description,FileType,InsertDate,[Identity],Photo,TableItems) Values(a.Title, a.Description, a.FileType, a.InsertDate, a.[Identity], a.Photo,a.TableItems); end ");
            context.Database.ExecuteSqlCommand("Create PROCEDURE [dbo].[AccountCodingInsert](@AccountCodingType AccountCodingType  READONLY) AS begin SET NOCOUNT OFF;INSERT[Production].[AccountDocumentCoding]([ID],[Name], [AccountCode],[EmployeeId],[ParentId]) Select* from @AccountCodingType End");

            context.Persons.Add(newPerson);
            context.Stores.Add(newStore);
            context.Stores.Add(newStore1);
            context.SaveChanges();
        }
    }

    public class BskaDBInitializer : CreateDatabaseIfNotExists<BskaContext>
    {
        protected override void Seed(BskaContext context)
        {
           context.Seed(context);
        }
    }
}
