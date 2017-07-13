namespace TradingPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewsAndQutotaions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Body = c.String(),
                        Link = c.String(),
                        TradeId = c.Int(),
                        Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedByUserId = c.String(maxLength: 128),
                        UpdatedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .ForeignKey("dbo.Trades", t => t.TradeId)
                .Index(t => t.TradeId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);
            
            CreateTable(
                "dbo.Quotations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Date = c.DateTime(nullable: false),
                        QuotationTypeId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedByUserId = c.String(maxLength: 128),
                        UpdatedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.QuotationTypes", t => t.QuotationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.QuotationTypeId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);
            
            CreateTable(
                "dbo.QuotationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quotations", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Quotations", "QuotationTypeId", "dbo.QuotationTypes");
            DropForeignKey("dbo.QuotationTypes", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.QuotationTypes", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Quotations", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.News", "TradeId", "dbo.Trades");
            DropForeignKey("dbo.News", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.News", "CreatedByUserId", "dbo.Users");
            DropIndex("dbo.QuotationTypes", new[] { "UpdatedByUserId" });
            DropIndex("dbo.QuotationTypes", new[] { "CreatedByUserId" });
            DropIndex("dbo.Quotations", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Quotations", new[] { "CreatedByUserId" });
            DropIndex("dbo.Quotations", new[] { "QuotationTypeId" });
            DropIndex("dbo.News", new[] { "UpdatedByUserId" });
            DropIndex("dbo.News", new[] { "CreatedByUserId" });
            DropIndex("dbo.News", new[] { "TradeId" });
            DropTable("dbo.QuotationTypes");
            DropTable("dbo.Quotations");
            DropTable("dbo.News");
        }
    }
}
