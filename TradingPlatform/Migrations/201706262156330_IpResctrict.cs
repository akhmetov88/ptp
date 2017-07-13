namespace TradingPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IpResctrict : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IpRestricted", c => c.Boolean(nullable: false, defaultValue:false));
            AddColumn("dbo.Users", "AllowedIp", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AllowedIp");
            DropColumn("dbo.Users", "IpRestricted");
        }
    }
}
