namespace TradingPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankBills",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BankName = c.String(),
                    BankNameEng = c.String(),
                    Mfo = c.String(),
                    BillNumber = c.String(),
                    IsPrimary = c.Boolean(nullable: false),
                    IsResident = c.Boolean(nullable: false),
                    Address = c.String(),
                    AddressEng = c.String(),
                    IsCorrespondent = c.Boolean(nullable: false),
                    Iban = c.String(),
                    Swift = c.String(),
                    ContragentId = c.Int(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contragents", t => t.ContragentId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.ContragentId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Contragents",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    LongName = c.String(maxLength: 355),
                    LongNameEng = c.String(),
                    GroupId = c.Int(nullable: false),
                    Code = c.String(maxLength: 10),
                    TaxCode = c.String(maxLength: 128),
                    ContragentEmail = c.String(),
                    ContragentSite = c.String(),
                    ContragentPhone = c.String(),
                    ContragentFax = c.String(),
                    ApprovingComment = c.String(),
                    IsResident = c.Boolean(nullable: false),
                    IsApproved = c.Boolean(nullable: false),
                    HasContractCopy = c.Boolean(nullable: false),
                    HasContractOriginal = c.Boolean(nullable: false),
                    ContractOnSignin = c.Boolean(nullable: false),
                    IsTaxPayer = c.Boolean(nullable: false),
                    IsDutyPayer = c.Boolean(nullable: false),
                    TaxType = c.String(),
                    CreateDate = c.DateTime(nullable: false),
                    UpdateDate = c.DateTime(nullable: false),
                    IsSeller = c.Boolean(nullable: false),
                    IsBuyer = c.Boolean(nullable: false),
                    IsConfidant = c.Boolean(nullable: false),
                    CeoTitle = c.String(maxLength: 128),
                    CeoName = c.String(maxLength: 128),
                    ConfidantName = c.String(maxLength: 128),
                    ConfidantNameEng = c.String(),
                    ConfidantDocument = c.String(maxLength: 128),
                    ConfidantDocumentEng = c.String(),
                    ApprovedByUserId = c.String(maxLength: 128),
                    Address = c.String(),
                    AddressEng = c.String(),
                    PostAddress = c.String(),
                    PostAddressEng = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(nullable: false, maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.ApprovedByUserId)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.GroupId)
                .Index(t => t.ApprovedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    RegisterName = c.String(),
                    Locale = c.String(),
                    LastLoggedIn = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Registered = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    GroupId = c.Int(),
                    IsDebug = c.Boolean(nullable: false),
                    IsAcceptedOffert = c.Boolean(nullable: false),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .Index(t => t.GroupId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.Files",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    FileName = c.String(maxLength: 255),
                    ContentType = c.String(maxLength: 100),
                    Content = c.Binary(),
                    UserId = c.String(maxLength: 128),
                    IsApproved = c.Boolean(nullable: false),
                    Comment = c.String(maxLength: 255),
                    ContragentId = c.Int(nullable: false),
                    ApprovedByUserId = c.String(maxLength: 128),
                    FileTypeId = c.Int(),
                    CreateDate = c.DateTime(),
                    UpdateDate = c.DateTime(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ApprovedByUserId)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.FileTypes", t => t.FileTypeId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Contragents", t => t.ContragentId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.ContragentId)
                .Index(t => t.ApprovedByUserId)
                .Index(t => t.FileTypeId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.FileTypes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 255),
                    Desc = c.String(maxLength: 255),
                    Alias = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.UserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Groups",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    GroupName = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Notifications",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Subject = c.String(),
                    Body = c.String(),
                    Email = c.String(),
                    IsViewed = c.Boolean(nullable: false),
                    CreateDate = c.DateTime(nullable: false),
                    ViewedDate = c.DateTime(nullable: false),
                    FromUserId = c.String(maxLength: 128),
                    ToUserId = c.String(maxLength: 128),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.FromUserId)
                .ForeignKey("dbo.Users", t => t.ToUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.UserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.UserRoles",
                c => new
                {
                    RoleId = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: false)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.UserInfo",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    AdditionalPhone = c.String(maxLength: 20),
                    AdditionalMail = c.String(maxLength: 80),
                    Name = c.String(maxLength: 50),
                    Patronymyc = c.String(maxLength: 50),
                    Surname = c.String(maxLength: 50),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                    User_Id = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId)
                .Index(t => t.User_Id);

            CreateTable(
                "dbo.Bets",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TradeId = c.Int(nullable: false),
                    BuyerId = c.Int(nullable: false),
                    Volume = c.Decimal(precision: 18, scale: 2),
                    Price = c.Decimal(precision: 18, scale: 2),
                    LotsCount = c.Int(nullable: false),
                    IsActual = c.Boolean(nullable: false),
                    IsRedemption = c.Boolean(nullable: false),
                    IsRebetted = c.Boolean(nullable: false),
                    DateBet = c.DateTime(nullable: false),
                    DateUpdate = c.DateTime(nullable: false),
                    AspNetUserId = c.String(maxLength: 128),
                    RebetterId = c.String(maxLength: 128),
                    LotId = c.Int(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AspNetUserId)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Trades", t => t.TradeId, cascadeDelete: false)
                .ForeignKey("dbo.Lots", t => t.LotId)
                .ForeignKey("dbo.Users", t => t.RebetterId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Contragents", t => t.BuyerId, cascadeDelete: false)
                .Index(t => t.TradeId)
                .Index(t => t.BuyerId)
                .Index(t => t.AspNetUserId)
                .Index(t => t.RebetterId)
                .Index(t => t.LotId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Lots",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TradeId = c.Int(nullable: false),
                    LotNumber = c.Int(nullable: false),
                    BuyerId = c.Int(),
                    SellerId = c.Int(nullable: false),
                    Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    MinPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    IsActual = c.Boolean(nullable: false),
                    IsSelled = c.Boolean(nullable: false),
                    ElapsingTime = c.DateTime(nullable: false),
                    OnThinking = c.Boolean(nullable: false),
                    ReSellingCount = c.Int(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contragents", t => t.BuyerId)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Contragents", t => t.SellerId, cascadeDelete: false)
                .ForeignKey("dbo.Trades", t => t.TradeId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.TradeId)
                .Index(t => t.BuyerId)
                .Index(t => t.SellerId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Trades",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    SellerId = c.Int(nullable: false),
                    IsFinallyApproved = c.Boolean(),
                    BankBillId = c.Int(nullable: false),
                    IsPreapproved = c.Boolean(nullable: false),
                    IsFixed = c.Boolean(),
                    Type = c.String(maxLength: 128),
                    DifferencialMin = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DifferencialMax = c.Decimal(nullable: false, precision: 18, scale: 2),
                    DifferencialPriceText = c.String(),
                    DifferencialPriceDateType = c.String(),
                    DifferencialPriceDateTypeDesc = c.String(),
                    DifferencialPriceValueType = c.String(),
                    DifferencialPriceValue = c.Double(nullable: false),
                    TaxForUpTime = c.Int(nullable: false),
                    Currency = c.String(),
                    DaysForUptime = c.Int(nullable: false),
                    IsOpened = c.Boolean(nullable: false),
                    IsClosedByBills = c.Boolean(nullable: false),
                    PriceCurrency = c.String(),
                    IsActual = c.Boolean(),
                    AllowBets = c.Boolean(),
                    IsSuccefullyClosed = c.Boolean(),
                    IsFuture = c.Boolean(),
                    IsPast = c.Boolean(),
                    IsProcessed = c.Boolean(nullable: false),
                    DateBegin = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DateEnd = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DayToPay = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    ProductName = c.String(maxLength: 300),
                    ProductNomenclature = c.String(maxLength: 300),
                    ProductQuality = c.String(maxLength: 300),
                    ProductCountry = c.String(maxLength: 300),
                    ProductManufacturer = c.String(maxLength: 300),
                    TankTherms = c.String(),
                    Contact = c.String(),
                    Incothermns = c.String(),
                    PriceStart = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PriceStep = c.Decimal(precision: 18, scale: 2),
                    MaxPrice = c.Decimal(precision: 18, scale: 2),
                    TotalVolume = c.Decimal(precision: 18, scale: 2),
                    MinVolumeBet = c.Decimal(precision: 18, scale: 2),
                    MinVolumeStep = c.Decimal(precision: 18, scale: 2),
                    LotsCount = c.Int(),
                    LotsCountAvailable = c.Int(),
                    LotVolume = c.Decimal(precision: 18, scale: 2),
                    DeliveryDateFrom = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    DeliveryDateTo = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    AcceptedByUserId = c.String(maxLength: 128),
                    ApprovedByUserId = c.String(maxLength: 128),
                    ApprovedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    AcceptedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    EditedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    TransportType = c.String(maxLength: 300),
                    ShipmentPoint = c.String(maxLength: 300),
                    Unit = c.String(maxLength: 300),
                    RailwayBegin = c.String(maxLength: 300),
                    RailwayEnd = c.String(maxLength: 300),
                    FirstTherms = c.String(),
                    IsAccepted = c.Boolean(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                    ProductPassport_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AcceptedByUserId)
                .ForeignKey("dbo.Users", t => t.ApprovedByUserId)
                .ForeignKey("dbo.BankBills", t => t.BankBillId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Files", t => t.ProductPassport_Id)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Contragents", t => t.SellerId, cascadeDelete: false)
                .Index(t => t.SellerId)
                .Index(t => t.BankBillId)
                .Index(t => t.AcceptedByUserId)
                .Index(t => t.ApprovedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId)
                .Index(t => t.ProductPassport_Id);

            CreateTable(
                "dbo.Contracts",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ContractNumber = c.String(),
                    ShowName = c.String(),
                    TradeId = c.Int(nullable: false),
                    FromContragentId = c.Int(nullable: false),
                    ToContragentId = c.Int(nullable: false),
                    CreateDate = c.DateTime(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Trades", t => t.TradeId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Contragents", t => t.ToContragentId, cascadeDelete: false)
                .ForeignKey("dbo.Contragents", t => t.FromContragentId, cascadeDelete: false)
                .Index(t => t.TradeId)
                .Index(t => t.FromContragentId)
                .Index(t => t.ToContragentId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.TradeBills",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BillName = c.String(maxLength: 128),
                    AddContractName = c.String(maxLength: 128),
                    ContracttId = c.Int(nullable: false),
                    CreateDate = c.DateTime(nullable: false),
                    ToContragentId = c.Int(nullable: false),
                    FromContragentId = c.Int(nullable: false),
                    TradeId = c.Int(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContracttId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Trades", t => t.TradeId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Contragents", t => t.ToContragentId, cascadeDelete: false)
                .ForeignKey("dbo.Contragents", t => t.FromContragentId, cascadeDelete: false)
                .Index(t => t.ContracttId)
                .Index(t => t.ToContragentId)
                .Index(t => t.FromContragentId)
                .Index(t => t.TradeId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Catalogs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Code = c.String(),
                    Uk = c.String(),
                    Ru = c.String(),
                    DescUk = c.String(),
                    DescRu = c.String(),
                    Type = c.String(),
                    ParentId = c.Int(),
                    IsUsable = c.Boolean(nullable: false),
                    UserId = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Catalogs", t => t.ParentId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.ParentId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Configs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Key = c.String(),
                    Value = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Contents",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Lang = c.String(maxLength: 7),
                    Title = c.String(maxLength: 256),
                    KeyWords = c.String(maxLength: 256),
                    Descriptions = c.String(maxLength: 256),
                    Alias = c.String(maxLength: 100),
                    HtmlContent = c.String(),
                    UpdateDate = c.DateTime(),
                    UserId = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Feedbacks",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 150),
                    Email = c.String(maxLength: 150),
                    PhoneNumber = c.String(maxLength: 25),
                    Subject = c.String(maxLength: 350),
                    Text = c.String(),
                    Date = c.DateTime(nullable: false),
                    IsCommited = c.Boolean(nullable: false),
                    UserId = c.String(maxLength: 128),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.HelpGroups",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: false),
                    Hashtag = c.String(nullable: false, maxLength: 50),
                    TitleRu = c.String(),
                    TitleUa = c.String(),
                    TitleEn = c.String(),
                    Ru = c.String(),
                    Ua = c.String(),
                    En = c.String(),
                    UpdateDate = c.DateTime(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Helps",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: false),
                    Hashtag = c.String(nullable: false, maxLength: 50),
                    TitleRu = c.String(),
                    TitleUa = c.String(),
                    TitleEn = c.String(),
                    Ru = c.String(),
                    Ua = c.String(),
                    En = c.String(),
                    UpdateDate = c.DateTime(nullable: false),
                    HelpGroupId = c.Int(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.HelpGroups", t => t.HelpGroupId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.HelpGroupId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Langs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    LanguageCulture = c.String(),
                    UniqueSeoCode = c.String(),
                    FlagImageFileName = c.String(),
                    Rtl = c.Boolean(nullable: false),
                    DefaultCurrencyId = c.Int(nullable: false),
                    Published = c.Boolean(nullable: false),
                    DisplayOrder = c.Int(nullable: false),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Log",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Logged = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Application = c.String(),
                    Level = c.String(maxLength: 50),
                    Message = c.String(),
                    UserName = c.String(maxLength: 256),
                    ServerName = c.String(),
                    Port = c.String(),
                    Url = c.String(),
                    Https = c.Boolean(nullable: false),
                    ServerAddress = c.String(),
                    RemoteAddress = c.String(),
                    Logger = c.String(),
                    Callsite = c.String(),
                    Exception = c.String(),
                    AD = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Pushes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Device = c.String(),
                    DeviceId = c.String(maxLength: 2500, unicode: false),
                    UserId = c.String(maxLength: 128),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.DeviceId)
                .Index(t => t.UserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.Tokens",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Key = c.String(nullable: false, maxLength: 100),
                    Uk = c.String(),
                    Ru = c.String(),
                    PageLink = c.String(),
                    UpdateDate = c.DateTime(),
                    UserId = c.String(),
                    Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    CreatedByUserId = c.String(maxLength: 128),
                    UpdatedByUserId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);

            CreateTable(
                "dbo.UserContragents",
                c => new
                {
                    ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    Contragent_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Contragent_Id })
                .ForeignKey("dbo.Users", t => t.ApplicationUser_Id, cascadeDelete: false)
                .ForeignKey("dbo.Contragents", t => t.Contragent_Id, cascadeDelete: false)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Contragent_Id);

            CreateTable(
                "dbo.BuyerTrades",
                c => new
                {
                    Contragent_Id = c.Int(nullable: false),
                    Trade_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Contragent_Id, t.Trade_Id })
                .ForeignKey("dbo.Contragents", t => t.Contragent_Id, cascadeDelete: false)
                .ForeignKey("dbo.Trades", t => t.Trade_Id, cascadeDelete: false)
                .Index(t => t.Contragent_Id)
                .Index(t => t.Trade_Id);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Tokens", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Pushes", "UserId", "dbo.Users");
            DropForeignKey("dbo.Pushes", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Pushes", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Langs", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Langs", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.HelpGroups", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Helps", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Helps", "HelpGroupId", "dbo.HelpGroups");
            DropForeignKey("dbo.Helps", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.HelpGroups", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Feedbacks", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Contents", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Contents", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Configs", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Configs", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Catalogs", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Catalogs", "ParentId", "dbo.Catalogs");
            DropForeignKey("dbo.Catalogs", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.BankBills", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.BankBills", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Contragents", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Trades", "SellerId", "dbo.Contragents");
            DropForeignKey("dbo.BuyerTrades", "Trade_Id", "dbo.Trades");
            DropForeignKey("dbo.BuyerTrades", "Contragent_Id", "dbo.Contragents");
            DropForeignKey("dbo.Contracts", "FromContragentId", "dbo.Contragents");
            DropForeignKey("dbo.TradeBills", "FromContragentId", "dbo.Contragents");
            DropForeignKey("dbo.TradeBills", "ToContragentId", "dbo.Contragents");
            DropForeignKey("dbo.Files", "ContragentId", "dbo.Contragents");
            DropForeignKey("dbo.Contragents", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Contracts", "ToContragentId", "dbo.Contragents");
            DropForeignKey("dbo.Contracts", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.TradeBills", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.TradeBills", "TradeId", "dbo.Trades");
            DropForeignKey("dbo.TradeBills", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.TradeBills", "ContracttId", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "TradeId", "dbo.Trades");
            DropForeignKey("dbo.Contracts", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Bets", "BuyerId", "dbo.Contragents");
            DropForeignKey("dbo.Bets", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Bets", "RebetterId", "dbo.Users");
            DropForeignKey("dbo.Bets", "LotId", "dbo.Lots");
            DropForeignKey("dbo.Lots", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Trades", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Trades", "ProductPassport_Id", "dbo.Files");
            DropForeignKey("dbo.Lots", "TradeId", "dbo.Trades");
            DropForeignKey("dbo.Trades", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Bets", "TradeId", "dbo.Trades");
            DropForeignKey("dbo.Trades", "BankBillId", "dbo.BankBills");
            DropForeignKey("dbo.Trades", "ApprovedByUserId", "dbo.Users");
            DropForeignKey("dbo.Trades", "AcceptedByUserId", "dbo.Users");
            DropForeignKey("dbo.Lots", "SellerId", "dbo.Contragents");
            DropForeignKey("dbo.Lots", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Lots", "BuyerId", "dbo.Contragents");
            DropForeignKey("dbo.Bets", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Bets", "AspNetUserId", "dbo.Users");
            DropForeignKey("dbo.BankBills", "ContragentId", "dbo.Contragents");
            DropForeignKey("dbo.Contragents", "ApprovedByUserId", "dbo.Users");
            DropForeignKey("dbo.UserInfo", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserInfo", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.UserInfo", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.UserContragents", "Contragent_Id", "dbo.Contragents");
            DropForeignKey("dbo.UserContragents", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.Notifications", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Notifications", "ToUserId", "dbo.Users");
            DropForeignKey("dbo.Notifications", "FromUserId", "dbo.Users");
            DropForeignKey("dbo.Notifications", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Users", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Groups", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Contragents", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.UserClaims", "UserId", "dbo.Users");
            DropForeignKey("dbo.Files", "UserId", "dbo.Users");
            DropForeignKey("dbo.Files", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Files", "FileTypeId", "dbo.FileTypes");
            DropForeignKey("dbo.FileTypes", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.FileTypes", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Files", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Files", "ApprovedByUserId", "dbo.Users");
            DropIndex("dbo.BuyerTrades", new[] { "Trade_Id" });
            DropIndex("dbo.BuyerTrades", new[] { "Contragent_Id" });
            DropIndex("dbo.UserContragents", new[] { "Contragent_Id" });
            DropIndex("dbo.UserContragents", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Tokens", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Tokens", new[] { "CreatedByUserId" });
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropIndex("dbo.Pushes", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Pushes", new[] { "CreatedByUserId" });
            DropIndex("dbo.Pushes", new[] { "UserId" });
            DropIndex("dbo.Pushes", new[] { "DeviceId" });
            DropIndex("dbo.Langs", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Langs", new[] { "CreatedByUserId" });
            DropIndex("dbo.Helps", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Helps", new[] { "CreatedByUserId" });
            DropIndex("dbo.Helps", new[] { "HelpGroupId" });
            DropIndex("dbo.HelpGroups", new[] { "UpdatedByUserId" });
            DropIndex("dbo.HelpGroups", new[] { "CreatedByUserId" });
            DropIndex("dbo.Feedbacks", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Feedbacks", new[] { "CreatedByUserId" });
            DropIndex("dbo.Feedbacks", new[] { "UserId" });
            DropIndex("dbo.Contents", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Contents", new[] { "CreatedByUserId" });
            DropIndex("dbo.Configs", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Configs", new[] { "CreatedByUserId" });
            DropIndex("dbo.Catalogs", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Catalogs", new[] { "CreatedByUserId" });
            DropIndex("dbo.Catalogs", new[] { "ParentId" });
            DropIndex("dbo.TradeBills", new[] { "UpdatedByUserId" });
            DropIndex("dbo.TradeBills", new[] { "CreatedByUserId" });
            DropIndex("dbo.TradeBills", new[] { "TradeId" });
            DropIndex("dbo.TradeBills", new[] { "FromContragentId" });
            DropIndex("dbo.TradeBills", new[] { "ToContragentId" });
            DropIndex("dbo.TradeBills", new[] { "ContracttId" });
            DropIndex("dbo.Contracts", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Contracts", new[] { "CreatedByUserId" });
            DropIndex("dbo.Contracts", new[] { "ToContragentId" });
            DropIndex("dbo.Contracts", new[] { "FromContragentId" });
            DropIndex("dbo.Contracts", new[] { "TradeId" });
            DropIndex("dbo.Trades", new[] { "ProductPassport_Id" });
            DropIndex("dbo.Trades", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Trades", new[] { "CreatedByUserId" });
            DropIndex("dbo.Trades", new[] { "ApprovedByUserId" });
            DropIndex("dbo.Trades", new[] { "AcceptedByUserId" });
            DropIndex("dbo.Trades", new[] { "BankBillId" });
            DropIndex("dbo.Trades", new[] { "SellerId" });
            DropIndex("dbo.Lots", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Lots", new[] { "CreatedByUserId" });
            DropIndex("dbo.Lots", new[] { "SellerId" });
            DropIndex("dbo.Lots", new[] { "BuyerId" });
            DropIndex("dbo.Lots", new[] { "TradeId" });
            DropIndex("dbo.Bets", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Bets", new[] { "CreatedByUserId" });
            DropIndex("dbo.Bets", new[] { "LotId" });
            DropIndex("dbo.Bets", new[] { "RebetterId" });
            DropIndex("dbo.Bets", new[] { "AspNetUserId" });
            DropIndex("dbo.Bets", new[] { "BuyerId" });
            DropIndex("dbo.Bets", new[] { "TradeId" });
            DropIndex("dbo.UserInfo", new[] { "User_Id" });
            DropIndex("dbo.UserInfo", new[] { "UpdatedByUserId" });
            DropIndex("dbo.UserInfo", new[] { "CreatedByUserId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropIndex("dbo.Notifications", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Notifications", new[] { "CreatedByUserId" });
            DropIndex("dbo.Notifications", new[] { "ToUserId" });
            DropIndex("dbo.Notifications", new[] { "FromUserId" });
            DropIndex("dbo.Groups", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Groups", new[] { "CreatedByUserId" });
            DropIndex("dbo.UserClaims", new[] { "UserId" });
            DropIndex("dbo.FileTypes", new[] { "UpdatedByUserId" });
            DropIndex("dbo.FileTypes", new[] { "CreatedByUserId" });
            DropIndex("dbo.Files", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Files", new[] { "CreatedByUserId" });
            DropIndex("dbo.Files", new[] { "FileTypeId" });
            DropIndex("dbo.Files", new[] { "ApprovedByUserId" });
            DropIndex("dbo.Files", new[] { "ContragentId" });
            DropIndex("dbo.Files", new[] { "UserId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.Users", new[] { "GroupId" });
            DropIndex("dbo.Contragents", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Contragents", new[] { "CreatedByUserId" });
            DropIndex("dbo.Contragents", new[] { "ApprovedByUserId" });
            DropIndex("dbo.Contragents", new[] { "GroupId" });
            DropIndex("dbo.BankBills", new[] { "UpdatedByUserId" });
            DropIndex("dbo.BankBills", new[] { "CreatedByUserId" });
            DropIndex("dbo.BankBills", new[] { "ContragentId" });
            DropTable("dbo.BuyerTrades");
            DropTable("dbo.UserContragents");
            DropTable("dbo.Tokens");
            DropTable("dbo.Roles");
            DropTable("dbo.Pushes");
            DropTable("dbo.Log");
            DropTable("dbo.Langs");
            DropTable("dbo.Helps");
            DropTable("dbo.HelpGroups");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Contents");
            DropTable("dbo.Configs");
            DropTable("dbo.Catalogs");
            DropTable("dbo.TradeBills");
            DropTable("dbo.Contracts");
            DropTable("dbo.Trades");
            DropTable("dbo.Lots");
            DropTable("dbo.Bets");
            DropTable("dbo.UserInfo");
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserLogins");
            DropTable("dbo.Notifications");
            DropTable("dbo.Groups");
            DropTable("dbo.UserClaims");
            DropTable("dbo.FileTypes");
            DropTable("dbo.Files");
            DropTable("dbo.Users");
            DropTable("dbo.Contragents");
            DropTable("dbo.BankBills");
        }
    }
}
