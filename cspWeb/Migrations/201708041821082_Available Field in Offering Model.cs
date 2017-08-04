namespace cspWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AvailableFieldinOfferingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offerings", "Available", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Offerings", "Available");
        }
    }
}
