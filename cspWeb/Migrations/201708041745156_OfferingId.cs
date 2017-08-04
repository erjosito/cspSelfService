namespace cspWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OfferingId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "OfferingId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "OfferingId");
        }
    }
}
