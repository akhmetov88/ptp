namespace TradingPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CatalogsFilter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Catalogs", "Filter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Catalogs", "Filter");
        }
    }
}
