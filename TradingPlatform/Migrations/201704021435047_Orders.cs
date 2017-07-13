namespace TradingPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Orders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TradeId = c.Int(nullable: false),
                        Volume = c.Int(nullable: false),
                        IsAcceptedBySeller = c.Boolean(nullable: false),
                        BuyerId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Updated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedByUserId = c.String(maxLength: 128),
                        UpdatedByUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contragents", t => t.BuyerId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedByUserId)
                .ForeignKey("dbo.Trades", t => t.TradeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UpdatedByUserId)
                .Index(t => t.TradeId)
                .Index(t => t.BuyerId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UpdatedByUserId);
            
            AddColumn("dbo.Users", "AllowPromoEmails", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AllowTradeEmails", c => c.Boolean(nullable: false));
            AddColumn("dbo.Trades", "IsOffer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Trades", "IsOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UpdatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "TradeId", "dbo.Trades");
            DropForeignKey("dbo.Orders", "CreatedByUserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "BuyerId", "dbo.Contragents");
            DropIndex("dbo.Orders", new[] { "UpdatedByUserId" });
            DropIndex("dbo.Orders", new[] { "CreatedByUserId" });
            DropIndex("dbo.Orders", new[] { "BuyerId" });
            DropIndex("dbo.Orders", new[] { "TradeId" });
            DropColumn("dbo.Trades", "IsOrder");
            DropColumn("dbo.Trades", "IsOffer");
            DropColumn("dbo.Users", "AllowTradeEmails");
            DropColumn("dbo.Users", "AllowPromoEmails");
            DropTable("dbo.Orders");
        }
    }
}
